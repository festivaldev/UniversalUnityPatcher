
namespace UniversalUnityPatcher {
	partial class MainWindow {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ConsoleWindow = new System.Windows.Forms.RichTextBox();
			this.AvailablePatches = new System.Windows.Forms.CheckedListBox();
			this.PatchButton = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.LoadAssemblyButton = new System.Windows.Forms.Button();
			this.AssemblyPathLabel = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.PatchesPathLabel = new System.Windows.Forms.Label();
			this.LoadPatchesButton = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.CreateBackupCheckbox = new System.Windows.Forms.CheckBox();
			this.BackupPathLabel = new System.Windows.Forms.Label();
			this.SetBackupPathButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.ConsoleWindow);
			this.groupBox2.Location = new System.Drawing.Point(294, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(554, 321);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Output";
			// 
			// ConsoleWindow
			// 
			this.ConsoleWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ConsoleWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
			this.ConsoleWindow.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ConsoleWindow.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConsoleWindow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.ConsoleWindow.Location = new System.Drawing.Point(6, 19);
			this.ConsoleWindow.Name = "ConsoleWindow";
			this.ConsoleWindow.ReadOnly = true;
			this.ConsoleWindow.Size = new System.Drawing.Size(542, 296);
			this.ConsoleWindow.TabIndex = 0;
			this.ConsoleWindow.Text = "";
			// 
			// AvailablePatches
			// 
			this.AvailablePatches.CheckOnClick = true;
			this.AvailablePatches.Enabled = false;
			this.AvailablePatches.FormattingEnabled = true;
			this.AvailablePatches.Items.AddRange(new object[] {
            "No patches loaded"});
			this.AvailablePatches.Location = new System.Drawing.Point(6, 19);
			this.AvailablePatches.Name = "AvailablePatches";
			this.AvailablePatches.Size = new System.Drawing.Size(264, 109);
			this.AvailablePatches.TabIndex = 0;
			this.AvailablePatches.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.AvailablePatches_ItemCheck);
			// 
			// PatchButton
			// 
			this.PatchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PatchButton.Enabled = false;
			this.PatchButton.Location = new System.Drawing.Point(773, 339);
			this.PatchButton.Name = "PatchButton";
			this.PatchButton.Size = new System.Drawing.Size(75, 23);
			this.PatchButton.TabIndex = 1;
			this.PatchButton.Text = "Patch!";
			this.PatchButton.UseVisualStyleBackColor = true;
			this.PatchButton.Click += new System.EventHandler(this.PatchButton_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.AssemblyPathLabel);
			this.groupBox3.Controls.Add(this.LoadAssemblyButton);
			this.groupBox3.Location = new System.Drawing.Point(12, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(276, 48);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Assembly Directory";
			// 
			// LoadAssemblyButton
			// 
			this.LoadAssemblyButton.AutoSize = true;
			this.LoadAssemblyButton.Location = new System.Drawing.Point(244, 19);
			this.LoadAssemblyButton.Name = "LoadAssemblyButton";
			this.LoadAssemblyButton.Size = new System.Drawing.Size(26, 23);
			this.LoadAssemblyButton.TabIndex = 0;
			this.LoadAssemblyButton.Text = "...";
			this.LoadAssemblyButton.UseVisualStyleBackColor = true;
			this.LoadAssemblyButton.Click += new System.EventHandler(this.LoadAssemblyButton_Click);
			// 
			// AssemblyPathLabel
			// 
			this.AssemblyPathLabel.AutoEllipsis = true;
			this.AssemblyPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssemblyPathLabel.Location = new System.Drawing.Point(6, 24);
			this.AssemblyPathLabel.Name = "AssemblyPathLabel";
			this.AssemblyPathLabel.Size = new System.Drawing.Size(232, 13);
			this.AssemblyPathLabel.TabIndex = 1;
			this.AssemblyPathLabel.Text = "None";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.PatchesPathLabel);
			this.groupBox4.Controls.Add(this.LoadPatchesButton);
			this.groupBox4.Location = new System.Drawing.Point(12, 66);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(276, 48);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Patch File";
			// 
			// PatchesPathLabel
			// 
			this.PatchesPathLabel.AutoEllipsis = true;
			this.PatchesPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PatchesPathLabel.Location = new System.Drawing.Point(6, 24);
			this.PatchesPathLabel.Name = "PatchesPathLabel";
			this.PatchesPathLabel.Size = new System.Drawing.Size(232, 13);
			this.PatchesPathLabel.TabIndex = 1;
			this.PatchesPathLabel.Text = "None";
			// 
			// LoadPatchesButton
			// 
			this.LoadPatchesButton.AutoSize = true;
			this.LoadPatchesButton.Location = new System.Drawing.Point(244, 19);
			this.LoadPatchesButton.Name = "LoadPatchesButton";
			this.LoadPatchesButton.Size = new System.Drawing.Size(26, 23);
			this.LoadPatchesButton.TabIndex = 0;
			this.LoadPatchesButton.Text = "...";
			this.LoadPatchesButton.UseVisualStyleBackColor = true;
			this.LoadPatchesButton.Click += new System.EventHandler(this.LoadPatchesButton_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.AvailablePatches);
			this.groupBox5.Location = new System.Drawing.Point(12, 120);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(276, 135);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Available Patches";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.BackupPathLabel);
			this.groupBox6.Controls.Add(this.SetBackupPathButton);
			this.groupBox6.Controls.Add(this.CreateBackupCheckbox);
			this.groupBox6.Enabled = false;
			this.groupBox6.Location = new System.Drawing.Point(12, 261);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(276, 72);
			this.groupBox6.TabIndex = 5;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Backup";
			// 
			// CreateBackupCheckbox
			// 
			this.CreateBackupCheckbox.AutoSize = true;
			this.CreateBackupCheckbox.Checked = true;
			this.CreateBackupCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CreateBackupCheckbox.Location = new System.Drawing.Point(7, 20);
			this.CreateBackupCheckbox.Name = "CreateBackupCheckbox";
			this.CreateBackupCheckbox.Size = new System.Drawing.Size(97, 17);
			this.CreateBackupCheckbox.TabIndex = 0;
			this.CreateBackupCheckbox.Text = "Create Backup";
			this.CreateBackupCheckbox.UseVisualStyleBackColor = true;
			// 
			// BackupPathLabel
			// 
			this.BackupPathLabel.AutoEllipsis = true;
			this.BackupPathLabel.Location = new System.Drawing.Point(6, 48);
			this.BackupPathLabel.Name = "BackupPathLabel";
			this.BackupPathLabel.Size = new System.Drawing.Size(232, 13);
			this.BackupPathLabel.TabIndex = 3;
			this.BackupPathLabel.Text = ".\\backup\\";
			// 
			// SetBackupPathButton
			// 
			this.SetBackupPathButton.AutoSize = true;
			this.SetBackupPathButton.Location = new System.Drawing.Point(244, 43);
			this.SetBackupPathButton.Name = "SetBackupPathButton";
			this.SetBackupPathButton.Size = new System.Drawing.Size(26, 23);
			this.SetBackupPathButton.TabIndex = 2;
			this.SetBackupPathButton.Text = "...";
			this.SetBackupPathButton.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label2.Location = new System.Drawing.Point(12, 349);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(129, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "(c) 2022 Team FESTIVAL";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(860, 374);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.PatchButton);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.MinimumSize = new System.Drawing.Size(876, 413);
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Universal Unity Patcher";
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RichTextBox ConsoleWindow;
		private System.Windows.Forms.CheckedListBox AvailablePatches;
		private System.Windows.Forms.Button PatchButton;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label AssemblyPathLabel;
		private System.Windows.Forms.Button LoadAssemblyButton;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label PatchesPathLabel;
		private System.Windows.Forms.Button LoadPatchesButton;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.CheckBox CreateBackupCheckbox;
		private System.Windows.Forms.Label BackupPathLabel;
		private System.Windows.Forms.Button SetBackupPathButton;
		private System.Windows.Forms.Label label2;
	}
}

