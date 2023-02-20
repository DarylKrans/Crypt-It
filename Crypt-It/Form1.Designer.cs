using System.Windows.Forms;

namespace Crypt_It
{
    partial class Crypt_It
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Crypt_It));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FN = new System.Windows.Forms.Label();
            this.SZ = new System.Windows.Forms.Label();
            this.Fname = new System.Windows.Forms.Label();
            this.Fsize = new System.Windows.Forms.Label();
            this.PBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Pass = new System.Windows.Forms.TextBox();
            this.PassC = new System.Windows.Forms.TextBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Match = new System.Windows.Forms.Label();
            this.thd = new System.Windows.Forms.Label();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.msDec = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMaxThreadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msThreadAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.msThreadMan = new System.Windows.Forms.ToolStripMenuItem();
            this.msDelFile = new System.Windows.Forms.ToolStripMenuItem();
            this.msTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.msTestFile = new System.Windows.Forms.ToolStripMenuItem();
            this.msStart = new System.Windows.Forms.ToolStripMenuItem();
            this.msClear = new System.Windows.Forms.ToolStripMenuItem();
            this.msSorry = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FN
            // 
            this.FN.AutoSize = true;
            this.FN.BackColor = System.Drawing.Color.Transparent;
            this.FN.ForeColor = System.Drawing.Color.Silver;
            this.FN.Location = new System.Drawing.Point(12, 99);
            this.FN.Name = "FN";
            this.FN.Size = new System.Drawing.Size(41, 13);
            this.FN.TabIndex = 1;
            this.FN.Text = "Name :";
            // 
            // SZ
            // 
            this.SZ.AutoSize = true;
            this.SZ.BackColor = System.Drawing.Color.Transparent;
            this.SZ.ForeColor = System.Drawing.Color.Silver;
            this.SZ.Location = new System.Drawing.Point(17, 112);
            this.SZ.Name = "SZ";
            this.SZ.Size = new System.Drawing.Size(36, 13);
            this.SZ.TabIndex = 2;
            this.SZ.Text = "Size  :";
            this.SZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Fname
            // 
            this.Fname.AutoSize = true;
            this.Fname.BackColor = System.Drawing.Color.Transparent;
            this.Fname.ForeColor = System.Drawing.Color.Silver;
            this.Fname.Location = new System.Drawing.Point(49, 99);
            this.Fname.Name = "Fname";
            this.Fname.Size = new System.Drawing.Size(35, 13);
            this.Fname.TabIndex = 3;
            this.Fname.Text = "label3";
            // 
            // Fsize
            // 
            this.Fsize.AutoSize = true;
            this.Fsize.BackColor = System.Drawing.Color.Transparent;
            this.Fsize.ForeColor = System.Drawing.Color.Silver;
            this.Fsize.Location = new System.Drawing.Point(50, 112);
            this.Fsize.Name = "Fsize";
            this.Fsize.Size = new System.Drawing.Size(35, 13);
            this.Fsize.TabIndex = 4;
            this.Fsize.Text = "label4";
            // 
            // PBar
            // 
            this.PBar.Location = new System.Drawing.Point(15, 83);
            this.PBar.Name = "PBar";
            this.PBar.Size = new System.Drawing.Size(320, 13);
            this.PBar.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label1.Location = new System.Drawing.Point(50, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Confirm Password";
            // 
            // Pass
            // 
            this.Pass.Location = new System.Drawing.Point(109, 35);
            this.Pass.Name = "Pass";
            this.Pass.Size = new System.Drawing.Size(137, 20);
            this.Pass.TabIndex = 8;
            this.Pass.TextChanged += new System.EventHandler(this.Pass_TextChanged);
            // 
            // PassC
            // 
            this.PassC.Location = new System.Drawing.Point(109, 57);
            this.PassC.Name = "PassC";
            this.PassC.Size = new System.Drawing.Size(137, 20);
            this.PassC.TabIndex = 9;
            this.PassC.TextChanged += new System.EventHandler(this.PassC_TextChanged);
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox1.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.CheckBox1.Location = new System.Drawing.Point(252, 37);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(77, 17);
            this.CheckBox1.TabIndex = 11;
            this.CheckBox1.Text = "Show Text";
            this.CheckBox1.UseVisualStyleBackColor = false;
            this.CheckBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // Match
            // 
            this.Match.AutoSize = true;
            this.Match.BackColor = System.Drawing.Color.Transparent;
            this.Match.ForeColor = System.Drawing.Color.Red;
            this.Match.Location = new System.Drawing.Point(252, 60);
            this.Match.Name = "Match";
            this.Match.Size = new System.Drawing.Size(54, 13);
            this.Match.TabIndex = 12;
            this.Match.Text = "No Match";
            // 
            // thd
            // 
            this.thd.AutoSize = true;
            this.thd.BackColor = System.Drawing.Color.Transparent;
            this.thd.ForeColor = System.Drawing.Color.Silver;
            this.thd.Location = new System.Drawing.Point(223, 112);
            this.thd.Name = "thd";
            this.thd.Size = new System.Drawing.Size(0, 13);
            this.thd.TabIndex = 16;
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.msStart,
            this.msClear,
            this.msSorry});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(344, 24);
            this.Menu.TabIndex = 17;
            this.Menu.Text = "Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.batchToolStripMenuItem,
            this.toolStripSeparator1,
            this.msDec,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // batchToolStripMenuItem
            // 
            this.batchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("batchToolStripMenuItem.Image")));
            this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            this.batchToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.batchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.batchToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.batchToolStripMenuItem.Text = "Batch";
            this.batchToolStripMenuItem.Click += new System.EventHandler(this.BatchToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // msDec
            // 
            this.msDec.Image = global::Crypt_It.Properties.Resources.decrypt;
            this.msDec.Name = "msDec";
            this.msDec.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.msDec.Size = new System.Drawing.Size(180, 22);
            this.msDec.Text = "&Decrypt";
            this.msDec.Click += new System.EventHandler(this.MsDec_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setMaxThreadsToolStripMenuItem,
            this.msDelFile,
            this.msTimer,
            this.msTestFile});
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // setMaxThreadsToolStripMenuItem
            // 
            this.setMaxThreadsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msThreadAuto,
            this.msThreadMan});
            this.setMaxThreadsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("setMaxThreadsToolStripMenuItem.Image")));
            this.setMaxThreadsToolStripMenuItem.Name = "setMaxThreadsToolStripMenuItem";
            this.setMaxThreadsToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.setMaxThreadsToolStripMenuItem.Text = "Set max threads";
            // 
            // msThreadAuto
            // 
            this.msThreadAuto.Checked = true;
            this.msThreadAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.msThreadAuto.Name = "msThreadAuto";
            this.msThreadAuto.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.msThreadAuto.Size = new System.Drawing.Size(182, 22);
            this.msThreadAuto.Text = "Automatic";
            this.msThreadAuto.Click += new System.EventHandler(this.MsThreadAuto_Click);
            // 
            // msThreadMan
            // 
            this.msThreadMan.Name = "msThreadMan";
            this.msThreadMan.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.msThreadMan.Size = new System.Drawing.Size(182, 22);
            this.msThreadMan.Text = "Set Manually";
            this.msThreadMan.Click += new System.EventHandler(this.SetManuallyToolStripMenuItem_Click);
            // 
            // msDelFile
            // 
            this.msDelFile.CheckOnClick = true;
            this.msDelFile.Image = ((System.Drawing.Image)(resources.GetObject("msDelFile.Image")));
            this.msDelFile.Name = "msDelFile";
            this.msDelFile.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.msDelFile.Size = new System.Drawing.Size(239, 22);
            this.msDelFile.Text = "Delete souce files";
            this.msDelFile.Click += new System.EventHandler(this.DeleteSouceFilesToolStripMenuItem_Click);
            // 
            // msTimer
            // 
            this.msTimer.CheckOnClick = true;
            this.msTimer.Image = ((System.Drawing.Image)(resources.GetObject("msTimer.Image")));
            this.msTimer.Name = "msTimer";
            this.msTimer.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.M)));
            this.msTimer.Size = new System.Drawing.Size(239, 22);
            this.msTimer.Text = "Timer";
            this.msTimer.Click += new System.EventHandler(this.Timer_Click);
            // 
            // msTestFile
            // 
            this.msTestFile.CheckOnClick = true;
            this.msTestFile.Image = ((System.Drawing.Image)(resources.GetObject("msTestFile.Image")));
            this.msTestFile.Name = "msTestFile";
            this.msTestFile.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
            this.msTestFile.Size = new System.Drawing.Size(239, 22);
            this.msTestFile.Text = "Use Testfile";
            this.msTestFile.Click += new System.EventHandler(this.UseTestfileToolStripMenuItem1_Click);
            // 
            // msStart
            // 
            this.msStart.ForeColor = System.Drawing.Color.Silver;
            this.msStart.Name = "msStart";
            this.msStart.Size = new System.Drawing.Size(43, 20);
            this.msStart.Text = "&Start";
            this.msStart.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
            // 
            // msClear
            // 
            this.msClear.ForeColor = System.Drawing.Color.Silver;
            this.msClear.Name = "msClear";
            this.msClear.Size = new System.Drawing.Size(46, 20);
            this.msClear.Text = "&Clear";
            this.msClear.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // msSorry
            // 
            this.msSorry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.msSorry.ForeColor = System.Drawing.Color.Silver;
            this.msSorry.Name = "msSorry";
            this.msSorry.Size = new System.Drawing.Size(46, 20);
            this.msSorry.Text = "Sorry";
            this.msSorry.Click += new System.EventHandler(this.SorryToolStripMenuItem_Click);
            // 
            // Crypt_It
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(344, 132);
            this.Controls.Add(this.Menu);
            this.Controls.Add(this.thd);
            this.Controls.Add(this.Match);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.PassC);
            this.Controls.Add(this.Pass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PBar);
            this.Controls.Add(this.Fsize);
            this.Controls.Add(this.Fname);
            this.Controls.Add(this.SZ);
            this.Controls.Add(this.FN);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.Menu;
            this.MaximizeBox = false;
            this.Name = "Crypt_It";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crypt-It";
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label FN;
        private System.Windows.Forms.Label SZ;
        private System.Windows.Forms.Label Fname;
        private System.Windows.Forms.Label Fsize;
        private System.Windows.Forms.ProgressBar PBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Pass;
        private System.Windows.Forms.TextBox PassC;
        private System.Windows.Forms.CheckBox CheckBox1;
        private System.Windows.Forms.Label Match;
        private System.Windows.Forms.Label thd;
        private new System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem msDelFile;
        private System.Windows.Forms.ToolStripMenuItem setMaxThreadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem msTimer;
        private System.Windows.Forms.ToolStripMenuItem msTestFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem msStart;
        private System.Windows.Forms.ToolStripMenuItem msDec;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem msClear;
        private System.Windows.Forms.ToolStripMenuItem msSorry;
        private System.Windows.Forms.ToolStripMenuItem msThreadAuto;
        private System.Windows.Forms.ToolStripMenuItem msThreadMan;
    }
}

