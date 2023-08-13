using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using UniversalUnityPatcher.Instructions;

namespace UniversalUnityPatcher {
	class Patcher {
		private static DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
		private static PatchDefinition loadedPatches;

		private static string assemblyPath;
		public static string AssemblyPath {
			get {
				return assemblyPath;
			}

			set {
				assemblyPath = Path.GetFullPath(value);

				if (Directory.Exists(assemblyPath))
					assemblyResolver.AddSearchDirectory(assemblyPath);
			}
		}

		private static string patchPath;
		public static string PatchPath {
			get {
				return patchPath;
			}

			set {
				patchPath = Path.GetFullPath(value);

				if (File.Exists(patchPath))
					LoadPatches();
			}
		}

		public static bool BackupFiles;

		private static string backupPath;
		public static string BackupPath {
			get {
				if (string.IsNullOrWhiteSpace(backupPath)) {
					return Path.Combine(assemblyPath, "backup");
				}

				return backupPath;
			}

			set {
				backupPath = string.IsNullOrWhiteSpace(value) ? null : Path.GetFullPath(value);
			}
		}


		public static bool HasLoadedPatches {
			get {
				return loadedPatches != null && loadedPatches.Patches.Count() > 0;
			}
		}

		public static PatchDefinition LoadedPatches {
			get {
				return loadedPatches;
			}
		}

		public static List<Patch> EnabledPatches {
			get {
				return loadedPatches.Patches.Where(_ => _.IsEnabled).ToList();
			}
		}

		public static List<Patch> DisabledPatches {
			get {
				return loadedPatches.Patches.Where(_ => !_.IsEnabled).ToList();
			}
		}

		private static void ThrowError(string message) {
			WriteToConsole($"[ERROR] {message}");
			
			if (!Program.CLIOptions.DisableGUI || !Program.CLIOptions.Silent) {
				MessageBox.Show(message, "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static int LoadPatches(string path) {
			patchPath = path;

			return LoadPatches();
		}

		private static int LoadPatches() {
			XmlSerializer ser = new XmlSerializer(typeof(PatchDefinition));
			using (XmlReader reader = XmlReader.Create(patchPath, new XmlReaderSettings { IgnoreComments = true })) {
				loadedPatches = (PatchDefinition)ser.Deserialize(reader);

				return loadedPatches.Patches.Count();
			}
		}

		public static void ApplyPatches(string outputPath = null) {
			if (loadedPatches.Patches.Count() == 0)
				return;

			ClearConsole();
			WriteToConsole("[INFO] Applying patches...");

			var watch = System.Diagnostics.Stopwatch.StartNew();

			var ignoreAlreadyPatched = Program.CLIOptions.IgnoreDuplicatePatch;

			foreach (Patch patch in loadedPatches.Patches) {
				assemblyResolver.Dispose();

				if (!patch.IsEnabled || patch.Assemblies.AppliesTo == null)
					continue;

				if (patch.FileHashes?.Count > 0) {
					List<string> hashsumMismatches = new List<string>();

					foreach (var fileHash in patch.FileHashes) {
						if (!File.Exists(Path.Combine(Path.GetFullPath(assemblyPath), fileHash.Name))) continue;
						if (fileHash.ValidHashes.Count == 0) continue;

						using (FileStream fileStream = File.OpenRead(Path.Combine(Path.GetFullPath(assemblyPath), fileHash.Name))) {
							using (var sha256 = SHA256.Create()) {
								var sha256sum = BitConverter.ToString(sha256.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();

								if (fileHash.ValidHashes.Find(_ => string.Equals(sha256sum, _, StringComparison.OrdinalIgnoreCase)) == null) {
									hashsumMismatches.Add(fileHash.Name);
								}
							}
						}
					}

					if (!ignoreAlreadyPatched && hashsumMismatches.Count > 0) {
						var mismatchList = string.Join("\r\n", hashsumMismatches.Select(_ => $"• {_}"));
						if (MessageBox.Show($"The following files appear to have been patched already or otherwise modified:\r\n\r\n{mismatchList}\r\n\r\nContinuing can break things horribly. Do you wish to continue patching?", "Hash Sum Mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
							WriteToConsole("Patching cancelled by user.");
							return;
						} else {
							ignoreAlreadyPatched = true;
						}
					}
				}

				using (AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(Path.Combine(Path.GetFullPath(assemblyPath), patch.Assemblies.AppliesTo), new ReaderParameters { AssemblyResolver = assemblyResolver, ReadWrite = true })) {
					foreach (PatchAssemblyReference reference in patch.Assemblies.References) {
						assemblyResolver.Resolve(reference.AssemblyName);
					}

					foreach (PatchType type in patch.Types) {
						List<TypeDefinition> typeList = assemblyDef.MainModule.Types.Where(_ => type.Match != null ? Regex.IsMatch(_.Name, type.Match) : _.Name == type.Name).ToList();

						if (typeList.Count == 0) {
							ThrowError($"Couldn't find type/class matching \"{(type.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!");

							continue;
						}

						foreach (TypeDefinition typeDef in typeList) {
							if (type.Methods.Count() > 0) {
								foreach (PatchMethod methodOp in type.Methods) {
									List<MethodDefinition> methodList = typeDef.Methods.Where(_ => methodOp.Match != null ? Regex.IsMatch(_.Name, methodOp.Match) : _.Name == methodOp.Name).ToList();

									if (methodList.Count == 0) {
										ThrowError($"Couldn't find method(s) matching \"{(methodOp.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!");

										continue;
									}

									foreach (MethodDefinition methodDef in methodList) {
										MethodBody methodBody = methodDef.Body;
										methodBody.SimplifyMacros();

										ILProcessor processor = methodBody.GetILProcessor();

										if (methodOp.Instructions.Count() == 0)
											continue;

										WriteToConsole($"[INFO] Attempting to patch method \"{methodDef.Name}\" in type/class \"{typeDef.Name}\"... ");

										Instruction beginInstr = null, prevInstr = null;
										int startIndex = (methodBody.Instructions.Count - 1);

										if (methodOp.BeginAt >= 0) {
											startIndex = Math.Min(
												Math.Max(
													methodOp.BeginAt, 0
												),
												methodBody.Instructions.Count - 1
											);

											beginInstr = methodBody.Instructions.ElementAt(startIndex);
										} else if (methodOp.Offset > 0) {
											startIndex = Math.Min(
												Math.Max(
													startIndex + methodOp.Offset, 0
												),
												methodBody.Instructions.Count - 1
											);
										}

										if (beginInstr == null) {
											prevInstr = methodBody.Instructions.ElementAt(startIndex);
										}

										foreach (object patchOp in methodOp.Instructions) {
											switch (patchOp.GetType().Name) {
												case "PatchInstruction": {
													PatchInstruction patchInstruction = (PatchInstruction)patchOp;
													Type t = Type.GetType($"UniversalUnityPatcher.Instructions.{patchInstruction.OpCode}");

													if (t == null) {
														ThrowError($"Couldn't find OpCode named \"{patchInstruction.OpCode}\"!");

														continue;
													}

													InstructionBase opCode = (InstructionBase)Activator.CreateInstance(t);
													Instruction instr = opCode.ParseInstruction(processor, assemblyDef, typeDef, methodDef, patchInstruction);

													if (instr == null) {
														ThrowError($"Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!");

														continue;
													}

													if (patchInstruction.Index >= 0) {
														var _instr = methodBody.Instructions.ElementAt(patchInstruction.Index);

														if (!ignoreAlreadyPatched && opCode.CompareInstruction(_instr, instr)) {
															ThrowError("Assembly appears to have been patched already. Patching will not continue.");
															return;
														}

														processor.InsertBefore(methodBody.Instructions.ElementAt(patchInstruction.Index), instr);
													} else {
														if (beginInstr != null) {
															if (!ignoreAlreadyPatched && opCode.CompareInstruction(beginInstr, instr)) {
																ThrowError("Assembly appears to have been patched already. Patching will not continue.");
																return;
															}

															processor.InsertBefore(beginInstr, instr);
														} else if (prevInstr != null) {
															if (!ignoreAlreadyPatched && opCode.CompareInstruction(prevInstr, instr)) {
																ThrowError("Assembly appears to have been patched already. Patching will not continue.");
																return;
															}

															processor.InsertAfter(prevInstr, instr);
														}
													}

													prevInstr = instr;

													break;
												}
												case "ReplaceOperation": {
													ReplaceOperation replaceOp = (ReplaceOperation)patchOp;

													prevInstr = methodBody.Instructions.ElementAt(replaceOp.Index);

													foreach (PatchInstruction patchInstruction in replaceOp.Replacements) {
														Type t = Type.GetType($"UniversalUnityPatcher.Instructions.{patchInstruction.OpCode}");

														if (t == null) {
															ThrowError($"Couldn't find OpCode named \"{patchInstruction.OpCode}\"!");

															continue;
														}

														InstructionBase opCode = (InstructionBase)Activator.CreateInstance(t);
														Instruction instr = opCode.ParseInstruction(processor, assemblyDef, typeDef, methodDef, patchInstruction);

														if (instr == null) {
															ThrowError($"Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!");

															continue;
														}

														if (!ignoreAlreadyPatched && opCode.CompareInstruction(prevInstr, instr)) {
															ThrowError("Assembly appears to have been patched already. Patching will not continue.");
															return;
														}

														processor.InsertAfter(prevInstr, instr);
														prevInstr = instr;
													}

													methodBody.Instructions.RemoveAt(replaceOp.Index);

													break;
												}
												case "RemoveOperation": {
													RemoveOperation removeOp = (RemoveOperation)patchOp;

													for (int i = 0; i < removeOp.Count; i++) {
														if (methodBody.Instructions.Count == 0)
															break;

														if (removeOp.Index >= 0) {
															if (methodBody.Instructions.ElementAt(removeOp.Index) == null)
																break;

															processor.Remove(methodBody.Instructions.ElementAt(removeOp.Index));
														} else if (prevInstr != null) {
															if (prevInstr.Next == null)
																break;

															processor.Remove(prevInstr.Next);
														} else {
															if (methodBody.Instructions.ElementAt(startIndex) == null)
																break;

															processor.Remove(methodBody.Instructions.ElementAt(startIndex));
														}
													}
													break;
												}
												case "TextReplaceOperation": {
													TextReplaceOperation textReplaceOp = (TextReplaceOperation)patchOp;

													foreach (Instruction instruction in methodBody.Instructions) {
														if (instruction.OpCode == OpCodes.Ldstr) {
															var textToReplace = (string)instruction.Operand;
															if (textToReplace.Contains(textReplaceOp.TextToReplace) || Regex.Match(textToReplace, textReplaceOp.TextToReplace) != null) {
																var replacedText = new Regex(textReplaceOp.TextToReplace).Replace(textToReplace, textReplaceOp.ReplacementText);

																if (!ignoreAlreadyPatched && instruction.Operand.Equals(replacedText)) {
																	ThrowError("Assembly appears to have been patched already. Patching will not continue.");
																	return;
																}

																instruction.Operand = replacedText;
															}
														}
													}
													break;
												}
												default:
													break;
											}
										}

										if (methodOp.ExceptionHandler != null) {
											if (methodOp.ExceptionHandler.HandlerStart < 0 ||
												methodOp.ExceptionHandler.HandlerEnd < 0 ||
												methodOp.ExceptionHandler.TryStart < 0 ||
												methodOp.ExceptionHandler.TryEnd < 0) {
												ThrowError("Invalid ExceptionHandler definition.");
												continue;
											}

											methodBody.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch) {
												CatchType = assemblyDef.MainModule.ImportReference(typeof(Exception)),
												HandlerType = ExceptionHandlerType.Catch,
												HandlerStart = methodBody.Instructions.ElementAt(methodOp.ExceptionHandler.HandlerStart),
												HandlerEnd = methodBody.Instructions.ElementAt(methodOp.ExceptionHandler.HandlerEnd),
												TryStart = methodBody.Instructions.ElementAt(methodOp.ExceptionHandler.TryStart),
												TryEnd = methodBody.Instructions.ElementAt(methodOp.ExceptionHandler.TryEnd)
											});
										}

										methodBody.OptimizeMacros();
									}
								}
							}

							if (type.Instructions.Count() > 0) {
								foreach (object patchOp in type.Instructions) {
									switch (patchOp.GetType().Name) {
										case "TextReplaceOperation": {
											TextReplaceOperation textReplaceOp = (TextReplaceOperation)patchOp;

											WriteToConsole($"[INFO] Replacing string \"{textReplaceOp.TextToReplace}\" with \"{textReplaceOp.ReplacementText}\" in type/class \"{typeDef.Name}\"");

											foreach (MethodDefinition methodDef in typeDef.Methods) {
												MethodBody methodBody = methodDef.Body;
												ILProcessor processor = methodBody.GetILProcessor();

												methodBody.SimplifyMacros();

												foreach (Instruction instruction in methodBody.Instructions) {
													if (instruction.OpCode == OpCodes.Ldstr) {
														var textToReplace = (string)instruction.Operand;

														if (textToReplace.Contains(textReplaceOp.TextToReplace) || Regex.Match(textToReplace, textReplaceOp.TextToReplace) != null) {
															var replacedText = new Regex(textReplaceOp.TextToReplace).Replace(textToReplace, textReplaceOp.ReplacementText);

															if (!ignoreAlreadyPatched && instruction.Operand.Equals(replacedText)) {
																ThrowError("Assembly appears to have been patched already. Patching will not continue.");
																return;
															}

															instruction.Operand = replacedText;
														}
													}
												}

												methodBody.OptimizeMacros();
											}
											break;
										}
										default:
											break;
									}
								}
							}
						}
					}

					#region Backup Assembly
					if (BackupFiles) {
						if (!Directory.Exists(BackupPath)) {
							Directory.CreateDirectory(BackupPath);
						}

						if (File.Exists(Path.Combine(BackupPath, patch.Assemblies.AppliesTo))) {
							WriteToConsole($"[INFO] {assemblyDef.Name.Name}.dll: Backup exists");
						} else {
							WriteToConsole($"[INFO] Copying {assemblyDef.Name.Name}.dll to {BackupPath}...");
							File.Copy(Path.Combine(Path.GetFullPath(assemblyPath), patch.Assemblies.AppliesTo), Path.Combine(BackupPath, patch.Assemblies.AppliesTo));
						}
					}
					#endregion

					#region Write Assembly
					if (string.IsNullOrWhiteSpace(outputPath)) {
						outputPath = assemblyPath;
					}

					WriteToConsole($"[INFO] Writing file {Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll")}...");

					if (!Directory.Exists(outputPath)) {
						Directory.CreateDirectory(outputPath);
					}

					assemblyDef.Write(Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll.patched"));

					assemblyResolver.Dispose();
					assemblyDef.Dispose();

					if (File.Exists(Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll"))) {
						File.Delete(Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll"));
					}

					File.Move(Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll.patched"), Path.Combine(outputPath, $"{assemblyDef.Name.Name}.dll"));
					#endregion
				}
			}

			watch.Stop();

			WriteToConsole($"Done! ({watch.ElapsedMilliseconds}ms)");
		}

		public static AssemblyDefinition ResolveAssembly(string name, string version = "0.0.0.0") {
			return assemblyResolver.Resolve(new AssemblyNameReference(name, new Version(version)));
		}

		public static MethodReference MakeHostInstanceGeneric(
								  MethodReference self,
								  params TypeReference[] args) {
			var reference = new MethodReference(
				self.Name,
				self.ReturnType,
				self.DeclaringType.MakeGenericInstanceType(args)) {
				HasThis = self.HasThis,
				ExplicitThis = self.ExplicitThis,
				CallingConvention = self.CallingConvention
			};

			foreach (var parameter in self.Parameters) {
				reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
			}

			foreach (var genericParam in self.GenericParameters) {
				reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
			}

			return reference;
		}

		public static void ClearConsole() {
			MainWindow.Instance?.ClearConsole();
		}

		public static void WriteToConsole(string text, bool newLine = true) {
			MainWindow.Instance?.WriteToConsole(text, newLine);

			if (newLine) {
				Console.WriteLine(text);
			} else {
				Console.Write(text);
			}
		}
	}
}
