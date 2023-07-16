using CommandLine.Text;
using CommandLine;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniversalUnityPatcher {
	public class Options {
		//[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
		//public bool Verbose { get; set; }

		[Option("no-gui")]
		public bool DisableGUI { get; private set; }

		[Option("silent", Required = false, HelpText = "Disables any output from the application.")]
		public bool Silent { get; private set; }

		[Option('i')]
		public string AssemblyPath { get; private set; }

		[Option('o')]
		public string OutputDirectory { get; private set; }

		[Option('p')]
		public string PatchPath { get; private set; }

		[Option("ignore-duplicate-patch")]
		public bool IgnoreDuplicatePatch { get; private set; }

		[Option("enabled-patches")]
		public IEnumerable<int> EnabledPatches { get; private set; }

		[Option("disabled-patches")]
		public IEnumerable<int> DisabledPatches { get; private set; }

		public override string ToString() {
			return $"no-gui:{DisableGUI} silent:{Silent} i:{AssemblyPath} o:{OutputDirectory} p:{PatchPath} ignore...:{IgnoreDuplicatePatch}";
		}
	}

	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o => {
					if (!o.DisableGUI) {
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						Application.Run(new MainWindow());
					} else {
						if (string.IsNullOrWhiteSpace(o.AssemblyPath)) {
							MessageBox.Show("Assembly directory (-i) cannot be empty!", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
						} else {
							Patcher.AssemblyPath = o.AssemblyPath;
						}

						if (string.IsNullOrWhiteSpace(o.PatchPath)) {
							MessageBox.Show("Path to patch XML (-p) cannot be empty!", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
						} else {
							try {
								Patcher.LoadPatches(o.PatchPath);
							} catch (Exception ex) {
								MessageBox.Show($"Failed to load patches from \"{Path.GetFullPath(o.PatchPath)}\":\n{ex.Message}", "Universal Unity Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
								return;
							}

							if (Patcher.HasLoadedPatches) {
								foreach (var patch in Patcher.LoadedPatches.Patches) {
									if (o.EnabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch))) {
										patch.IsEnabled = true;
									} else if (o.DisabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch))) {
										patch.IsEnabled = false;
									}
								}
							}

							try {
								Patcher.ApplyPatches(o.OutputDirectory);
							} catch (Exception ex) {
								MessageBox.Show($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
				});
		}
	}
}
