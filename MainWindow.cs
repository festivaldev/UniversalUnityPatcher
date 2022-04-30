using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalUnityPatcher {
	public partial class MainWindow : Form {
		private string assemblyPath;
		private string patchPath;

		public static MainWindow Instance;

		public MainWindow() {
			Instance = this;
			InitializeComponent();

			WriteToConsole($"{Properties.Resources.ApplicationName} v{typeof(MainWindow).Assembly.GetName().Version}\nAwaiting input...\n");
		}

		public void ClearConsole() {
			ConsoleWindow.Clear();
		}

		public void WriteToConsole(string text, bool newLine = true) {
			if (newLine) {
				ConsoleWindow.Text += text + "\n";
			} else {
				ConsoleWindow.Text += text;
			}

			ConsoleWindow.SelectionStart = ConsoleWindow.Text.Length;
			ConsoleWindow.ScrollToCaret();
		}

		private void LoadAssemblyButton_Click(object sender, EventArgs e) {
			FolderBrowserDialog openFolder = new FolderBrowserDialog {
				Description = "Select a directory containing Unity Assemblies to patch."
			};

			if (openFolder.ShowDialog() == DialogResult.OK) {
				assemblyPath = openFolder.SelectedPath;
				AssemblyPathLabel.Text = assemblyPath;

				Patcher.AssemblyPath = assemblyPath;

				WriteToConsole($"[Info] Selected Assembly directory: {assemblyPath}");
			}

			if (assemblyPath != null && patchPath != null) {
				if (patchPath.Length > 0 && patchPath.Length > 0) {
					PatchButton.Enabled = true;
				}
			}
		}

		private void LoadPatchesButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFile = new OpenFileDialog {
				Title = "Select a Patch file",
				Filter = "Patch XML|*.xml"
			};

			if (openFile.ShowDialog() == DialogResult.OK) {
				patchPath = openFile.FileName;
				PatchesPathLabel.Text = Path.GetFileName(patchPath);

				int loadedPatchesCount = Patcher.LoadPatches(patchPath);
				WriteToConsole($"[Info] Loaded {loadedPatchesCount} patches from {patchPath}");

				if (Patcher.HasLoadedPatches) {
					AvailablePatches.Items.Clear();

					foreach (var patch in Patcher.LoadedPatches.Patches) {
						if (patch.ShortDescription != null && patch.ShortDescription.Length > 0) {
							AvailablePatches.Items.Add(patch.ShortDescription);
						} else {
							AvailablePatches.Items.Add(patch.Name);
						}

						AvailablePatches.SetItemChecked(AvailablePatches.Items.Count - 1, patch.IsEnabled);
					}

					AvailablePatches.Enabled = true;
					//	this.Text = Patcher.PatchName != null ? string.Join(": ", (new string[] { strings.ApplicationName, Patcher.PatchName })) : strings.ApplicationName;
				} else {
					AvailablePatches.Enabled = false;
					AvailablePatches.Items.Clear();
					AvailablePatches.Items.Add("No patches available");
				}
			}

			if (assemblyPath != null && patchPath != null) {
				if (patchPath.Length > 0 && patchPath.Length > 0) {
					PatchButton.Enabled = true;
				}
			}
		}

		private void PatchButton_Click(object sender, EventArgs e) {
			try {
				Patcher.ApplyPatches();
			} catch (Exception ex) {
				//MessageBox.Show($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				WriteToConsole($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}");
			}
		}

		private void AvailablePatches_ItemCheck(object sender, ItemCheckEventArgs e) {
			Console.WriteLine(e.Index);
			Console.WriteLine(e.NewValue == CheckState.Checked);

			Patcher.LoadedPatches.Patches.ElementAt(e.Index).IsEnabled = (e.NewValue == CheckState.Checked);
		}
	}
}
