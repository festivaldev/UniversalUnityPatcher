using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UniversalUnityPatcher {
	public partial class MainWindow : Form {
		private string assemblyPath;
		private string patchPath;
		private string backupPath;

		private int toolTipIndex = -1;

		public static MainWindow Instance;

		public MainWindow() {
			Instance = this;
			InitializeComponent();

			Patcher.BackupFiles = CreateBackupCheckbox.Checked;

			WriteToConsole($"{Properties.Resources.ApplicationName} v{typeof(MainWindow).Assembly.GetName().Version}\nAwaiting input...\n");

			if (!string.IsNullOrWhiteSpace(Patcher.AssemblyPath)) {
				assemblyPath = Patcher.AssemblyPath;

				AssemblyPathUpdated();
			}

			if (!string.IsNullOrWhiteSpace(Patcher.PatchPath)) {
				patchPath = Path.GetFullPath(Patcher.PatchPath);

				PatchPathUpdated();
			}

			if (!string.IsNullOrWhiteSpace(Program.CLIOptions.BackupDirectory)) {
				backupPath = Path.GetFullPath(Program.CLIOptions.BackupDirectory);
			}

			BackupPathUpdated();
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
			using (var openFile = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select an Assembly Directory" }) {
				if (openFile.ShowDialog() == CommonFileDialogResult.Ok) {
					assemblyPath = openFile.FileName;
					Patcher.AssemblyPath = assemblyPath;

					AssemblyPathUpdated();
					BackupPathUpdated();
				}
			}
		}

		private void LoadPatchesButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFile = new OpenFileDialog {
				Title = "Select a Patch file",
				Filter = "Patch XML|*.xml"
			};

			if (openFile.ShowDialog() == DialogResult.OK) {
				patchPath = Path.GetFullPath(openFile.FileName);

				PatchPathUpdated();
			}
		}

		private void SetBackupPathButton_Click(object sender, EventArgs e) {
			using (var openFile = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select a Backup Directory" }) {
				if (openFile.ShowDialog() == CommonFileDialogResult.Ok) {
					backupPath = openFile.FileName;
					Patcher.BackupPath = backupPath;

					BackupPathUpdated();
				}
			}
		}

		private void ResetBackupPathButton_Click(object sender, EventArgs e) {
			backupPath = null;
			Patcher.BackupPath = null;

			BackupPathUpdated();
		}

		private void PatchButton_Click(object sender, EventArgs e) {
			try {
				Patcher.ApplyPatches();
			} catch (Exception ex) {
				MessageBox.Show($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//WriteToConsole($"Failed to apply patches:\n{ex.Message}\n\n{ex.StackTrace}");
			}
		}

		private void AvailablePatches_ItemCheck(object sender, ItemCheckEventArgs e) {
			Patcher.LoadedPatches.Patches.ElementAt(e.Index).IsEnabled = (e.NewValue == CheckState.Checked);

			UpdatePatchButtonEnabled();
		}

		private void AvailablePatches_MouseMove(object sender, MouseEventArgs e) {
			if (toolTipIndex != AvailablePatches.IndexFromPoint(e.Location)) {
				toolTipIndex = AvailablePatches.IndexFromPoint(e.Location);

				if (toolTipIndex >= 0) {
					patchToolTip.Active = true;
					patchToolTip.SetToolTip(AvailablePatches, Patcher.LoadedPatches.Patches.ElementAt(toolTipIndex).Description.Trim());
				} else {
					patchToolTip.Hide(AvailablePatches);
					patchToolTip.Active = false;
				}
			}
		}

		private void CreateBackupCheckbox_CheckedChanged(object sender, EventArgs e) {
			Patcher.BackupFiles = CreateBackupCheckbox.Checked;
		}

		#region GUI Updates
		private void AssemblyPathUpdated() {
			AssemblyPathLabel.Text = assemblyPath;

			WriteToConsole($"[INFO] Selected Assembly directory: {assemblyPath}");

			UpdatePatchButtonEnabled();
		}

		private void PatchPathUpdated() {
			PatchesPathLabel.Text = Path.GetFileName(patchPath);

			try {
				int loadedPatchesCount = Patcher.LoadPatches(patchPath);
				WriteToConsole($"[INFO] Loaded {loadedPatchesCount} patches from {patchPath}");
			} catch (Exception ex) {
				WriteToConsole($"[ERROR] Failed to load patches from \"{Path.GetFullPath(patchPath)}\":\n{ex.Message}");
			}

			if (Patcher.HasLoadedPatches) {
				AvailablePatches.Items.Clear();

				foreach (var patch in Patcher.LoadedPatches.Patches) {
					if (!patch.IsEnabled && Program.CLIOptions.EnabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch) + 1)) {
						patch.IsEnabled = true;
					} else if (patch.IsEnabled && Program.CLIOptions.DisabledPatches.Contains(Patcher.LoadedPatches.Patches.IndexOf(patch) + 1)) {
						patch.IsEnabled = false;
					}

					if (patch.ShortDescription != null && patch.ShortDescription.Length > 0) {
						AvailablePatches.Items.Add(patch.ShortDescription);
					} else {
						AvailablePatches.Items.Add(patch.Name);
					}

					AvailablePatches.SetItemChecked(AvailablePatches.Items.Count - 1, patch.IsEnabled);
				}

				AvailablePatches.Enabled = true;
			} else {
				AvailablePatches.Enabled = false;
				AvailablePatches.Items.Clear();
				AvailablePatches.Items.Add("No patches available");
			}

			UpdatePatchButtonEnabled();
		}

		private void BackupPathUpdated() {
			var path = string.IsNullOrWhiteSpace(backupPath) ? Patcher.BackupPath : backupPath;

			WriteToConsole($"[INFO] Selected backup directory: {path}");

			BackupPathLabel.Text = path;
			UpdateResetBackupPathButtonEnabled();
		}

		private void UpdatePatchButtonEnabled() {
			PatchButton.Enabled = (!string.IsNullOrWhiteSpace(assemblyPath) && !string.IsNullOrWhiteSpace(patchPath) && Patcher.EnabledPatches.Count > 0);
		}

		private void UpdateResetBackupPathButtonEnabled() {
			ResetBackupPathButton.Enabled = !string.IsNullOrWhiteSpace(backupPath);
		}
		#endregion
	}
}
