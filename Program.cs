using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UniversalUnityPatcher {
	public class Options {
		//[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
		//public bool Verbose { get; set; }

		[Option("no-gui", HelpText = "Disables the graphical user interface.")]
		public bool DisableGUI { get; private set; }

		[Option('s', "silent", Required = false, HelpText = "Disables any output from the application. (CLI mode only)")]
		public bool Silent { get; private set; }

		[Option('i', "assembly-dir", HelpText = "Specifies the assembly directory.")]
		public string AssemblyDirectory { get; private set; }

		[Option('o', "output-dir", HelpText = "Specifies the output directory. If unset, patched assemblies are saved in the assembly directory (-i).")]
		public string OutputDirectory { get; private set; }

		[Option('p', "patch", HelpText = "Specifies the path for the patch XML.")]
		public string PatchPath { get; private set; }

		[Option('b', "backup", HelpText = "Enables backup of files to be patched. (CLI mode only)")]
		public bool BackupFiles { get; private set; }

		[Option("backup-dir", HelpText = "Specifies the path where files a backed up to before patching.")]
		public string BackupDirectory { get; private set; }

		[Option("ignore-duplicate-patch", HelpText = "Ignore warnings for duplicate patches and patch anyways.")]
		public bool IgnoreDuplicatePatch { get; private set; }

		[Option("enabled-patches", HelpText = "Force specific patch indices to be enabled, starting at 1.")]
		public IEnumerable<int> EnabledPatches { get; private set; }

		[Option("disabled-patches", HelpText = "Force patch indices to be disabled, starting at 1.")]
		public IEnumerable<int> DisabledPatches { get; private set; }

		public override string ToString() {
			return $"no-gui:{DisableGUI} silent:{Silent} i:{AssemblyDirectory} o:{OutputDirectory} p:{PatchPath} ignore...:{IgnoreDuplicatePatch}";
		}
	}

	static class Program {
		public static Options CLIOptions { get; private set; }
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o => {
					CLIOptions = o;

					if (!o.DisableGUI) {
						if (!string.IsNullOrWhiteSpace(o.AssemblyDirectory)) {
							if (File.GetAttributes(o.AssemblyDirectory).HasFlag(FileAttributes.Directory)) {
								Patcher.AssemblyPath = Path.GetFullPath(o.AssemblyDirectory);
							} else {
								Patcher.AssemblyPath = Path.GetFullPath(Path.GetDirectoryName(o.AssemblyDirectory));
							}
						}

						if (!string.IsNullOrWhiteSpace(o.PatchPath)) {
							try {
								Patcher.PatchPath = o.PatchPath;
							} catch (Exception ex) {
								MessageBox.Show($"Failed to load patches from \"{Path.GetFullPath(o.PatchPath)}\":\n{ex.Message}", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}

						if (!string.IsNullOrWhiteSpace(o.BackupDirectory)) {
							if (File.GetAttributes(o.BackupDirectory).HasFlag(FileAttributes.Directory)) {
								Patcher.BackupPath = Path.GetFullPath(o.BackupDirectory);
							} else {
								Patcher.BackupPath = Path.GetFullPath(Path.GetDirectoryName(o.BackupDirectory));
							}
						}

						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						Application.Run(new MainWindow());
					} else {
						if (string.IsNullOrWhiteSpace(o.AssemblyDirectory)) {
							if (!o.Silent) {
								MessageBox.Show("Assembly directory (-i) cannot be empty!", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}

							Environment.Exit(1);
						} else {
							if (File.GetAttributes(o.AssemblyDirectory).HasFlag(FileAttributes.Directory)) {
								Patcher.AssemblyPath = Path.GetFullPath(o.AssemblyDirectory);
							} else {
								Patcher.AssemblyPath = Path.GetFullPath(Path.GetDirectoryName(o.AssemblyDirectory));
							}
						}

						Patcher.BackupFiles = o.BackupFiles;

						if (!string.IsNullOrWhiteSpace(o.BackupDirectory)) {
							if (File.GetAttributes(o.BackupDirectory).HasFlag(FileAttributes.Directory)) {
								Patcher.BackupPath = Path.GetFullPath(o.BackupDirectory);
							} else {
								Patcher.BackupPath = Path.GetFullPath(Path.GetDirectoryName(o.BackupDirectory));
							}
						}

						if (string.IsNullOrWhiteSpace(o.PatchPath)) {
							if (!o.Silent) {
								MessageBox.Show("Path to patch XML (-p) cannot be empty!", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}

							Environment.Exit(1);
						} else {
							try {
								Patcher.PatchPath = o.PatchPath;
							} catch (Exception ex) {
								if (!o.Silent) {
									MessageBox.Show($"Failed to load patches from \"{Path.GetFullPath(o.PatchPath)}\":\n{ex.Message}", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}

								Environment.Exit(1);
							}

							if (Patcher.HasLoadedPatches) {
								foreach (var patch in Patcher.LoadedPatches.Patches) {
									if (!patch.IsEnabled && o.EnabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch) + 1)) {
										patch.IsEnabled = true;
									} else if (patch.IsEnabled && o.DisabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch) + 1)) {
										patch.IsEnabled = false;
									}
								}
							}

							if (Patcher.EnabledPatches.Count == 0) {
								if (!o.Silent) {
									MessageBox.Show($"Nothing to patch!\r\nEnabled: {Patcher.EnabledPatches.Count} patches\r\nDisabled: {Patcher.DisabledPatches.Count} patches", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								}

								Environment.Exit(1);
							}

							try {
								Patcher.ApplyPatches(string.IsNullOrWhiteSpace(o.OutputDirectory) ? o.AssemblyDirectory : o.OutputDirectory);
							} catch (Exception ex) {
								if (!o.Silent) {
									MessageBox.Show($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}

								Environment.Exit(1);
							}
						}
					}
				});
		}
	}
}
