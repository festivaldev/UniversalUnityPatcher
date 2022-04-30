using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			set {
				assemblyPath = value;

				if (Directory.Exists(assemblyPath))
					assemblyResolver.AddSearchDirectory(assemblyPath);
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

		public static void ApplyPatches() {
			if (loadedPatches.Patches.Count() == 0)
				return;

			MainWindow.Instance.ClearConsole();
			MainWindow.Instance.WriteToConsole("[Info] Applying patches...");

			foreach (Patch patch in loadedPatches.Patches) {
				assemblyResolver.Dispose();

				if (!patch.IsEnabled || patch.Assemblies.AppliesTo == null)
					continue;

				using (AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(Path.Combine(Path.GetFullPath(assemblyPath), patch.Assemblies.AppliesTo), new ReaderParameters { AssemblyResolver = assemblyResolver, ReadWrite = true })) {
					foreach (PatchAssemblyReference reference in patch.Assemblies.References) {
						assemblyResolver.Resolve(reference.AssemblyName);
					}

					foreach (PatchType type in patch.Types) {
						List<TypeDefinition> typeList = assemblyDef.MainModule.Types.Where(_ => type.Match != null ? Regex.IsMatch(_.Name, type.Match) : _.Name == type.Name).ToList();

						if (typeList.Count == 0) {
							MainWindow.Instance.WriteToConsole($"Error] Couldn't find type/class matching \"{(type.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!");
							//MessageBox.Show($"Couldn't find type/class matching \"{(type.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
							continue;
						}

						foreach (TypeDefinition typeDef in typeList) {
							if (type.Methods.Count() > 0) {
								foreach (PatchMethod methodOp in type.Methods) {
									List<MethodDefinition> methodList = typeDef.Methods.Where(_ => methodOp.Match != null ? Regex.IsMatch(_.Name, methodOp.Match) : _.Name == methodOp.Name).ToList();

									if (methodList.Count == 0) {
										MainWindow.Instance.WriteToConsole($"Error] Couldn't find method(s) matching \"{(methodOp.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!");
										//MessageBox.Show($"Couldn't find method(s) matching \"{(methodOp.Name)}\" in assembly \"{assemblyDef.Name.Name}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
										continue;
									}

									foreach (MethodDefinition methodDef in methodList) {
										MethodBody methodBody = methodDef.Body;
										methodBody.SimplifyMacros();

										ILProcessor processor = methodBody.GetILProcessor();

										if (methodOp.Instructions.Count() == 0)
											continue;

										MainWindow.Instance.WriteToConsole($"[Info] Attempting to patch method \"{methodDef.Name}\" in type/class \"{typeDef.Name}\"... ");

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
														MainWindow.Instance.WriteToConsole($"[Error] Couldn't find OpCode named \"{patchInstruction.OpCode}\"!");
														//MessageBox.Show($"Couldn't find OpCode named \"{patchInstruction.OpCode}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
														continue;
													}

													InstructionBase opCode = (InstructionBase)Activator.CreateInstance(t);
													Instruction instr = opCode.ParseInstruction(processor, assemblyDef, typeDef, methodDef, patchInstruction);

													if (instr == null) {
														MainWindow.Instance.WriteToConsole($"[Error] Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!");
														//MessageBox.Show($"Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
														continue;
													}

													if (patchInstruction.Index >= 0) {
														processor.InsertBefore(methodBody.Instructions.ElementAt(patchInstruction.Index), instr);
													} else {
														if (beginInstr != null) {
															processor.InsertBefore(beginInstr, instr);
														} else if (prevInstr != null) {
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

														if(t == null) {
															MainWindow.Instance.WriteToConsole($"[Error] Couldn't find OpCode named \"{patchInstruction.OpCode}\"!");
															//MessageBox.Show($"Couldn't find OpCode named \"{patchInstruction.OpCode}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
															continue;
														}

														InstructionBase opCode = (InstructionBase)Activator.CreateInstance(t);
														Instruction instr = opCode.ParseInstruction(processor, assemblyDef, typeDef, methodDef, patchInstruction);

														if (instr == null) {
															MainWindow.Instance.WriteToConsole($"[Error] Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!");
															//MessageBox.Show($"Invalid instruction for OpCode \"{patchInstruction.OpCode}\"!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
															continue;
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
															//if (textToReplace.Contains(textReplaceOp.TextToReplace)) {
															if (textToReplace.Contains(textReplaceOp.TextToReplace) || Regex.Match(textToReplace, textReplaceOp.TextToReplace) != null) {
																//instruction.Operand = textToReplace.Replace(textReplaceOp.TextToReplace, textReplaceOp.ReplacementText);
																instruction.Operand = new Regex(textReplaceOp.TextToReplace).Replace(textToReplace, textReplaceOp.ReplacementText);
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
												MainWindow.Instance.WriteToConsole("Error: Invalid ExceptionHandler definition");
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

											MainWindow.Instance.WriteToConsole($"[Info] Replacing string \"{textReplaceOp.TextToReplace}\" with \"{textReplaceOp.ReplacementText}\" in type/class \"{typeDef.Name}\"");

											foreach (MethodDefinition methodDef in typeDef.Methods) {
												MethodBody methodBody = methodDef.Body;
												ILProcessor processor = methodBody.GetILProcessor();

												methodBody.SimplifyMacros();

												foreach (Instruction instruction in methodBody.Instructions) {
													if (instruction.OpCode == OpCodes.Ldstr) {
														var textToReplace = (string)instruction.Operand;

														//if (textToReplace.Contains(textReplaceOp.TextToReplace)) {
														if (textToReplace.Contains(textReplaceOp.TextToReplace) || Regex.Match(textToReplace, textReplaceOp.TextToReplace) != null) {
															//instruction.Operand = textToReplace.Replace(textReplaceOp.TextToReplace, textReplaceOp.ReplacementText);
															instruction.Operand = new Regex(textReplaceOp.TextToReplace).Replace(textToReplace, textReplaceOp.ReplacementText);
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

					assemblyDef.Write(Path.Combine(assemblyPath, $".{assemblyDef.Name.Name}.dll"));
					assemblyResolver.Dispose();
					assemblyDef.Dispose();

					if (File.Exists(Path.Combine(assemblyPath, $"{assemblyDef.Name.Name}.dll"))) {
						File.Delete(Path.Combine(assemblyPath, $"{assemblyDef.Name.Name}.dll"));
					}

					File.Move(Path.Combine(assemblyPath, $".{assemblyDef.Name.Name}.dll"), Path.Combine(assemblyPath, $"{assemblyDef.Name.Name}.dll"));

					MainWindow.Instance.WriteToConsole($"[Info] Writing file {Path.Combine(assemblyPath, $"{assemblyDef.Name.Name}.dll")}...");
				}
			}

			MainWindow.Instance.WriteToConsole("Done!");
		}

		public static int LoadPatches(string path) {
			XmlSerializer ser = new XmlSerializer(typeof(PatchDefinition));
			using (XmlReader reader = XmlReader.Create(path)) {
				try {
					loadedPatches = (PatchDefinition)ser.Deserialize(reader);

					return loadedPatches.Patches.Count();
				} catch (Exception e) {
					MessageBox.Show($"Failed to load patches from \"{Path.GetFullPath(path)}\":\n{e.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			return 0;
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
	}
}
