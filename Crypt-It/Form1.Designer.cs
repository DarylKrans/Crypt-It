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
            this.OpenFile = new System.Windows.Forms.Button();
            this.FN = new System.Windows.Forms.Label();
            this.SZ = new System.Windows.Forms.Label();
            this.Fname = new System.Windows.Forms.Label();
            this.Fsize = new System.Windows.Forms.Label();
            this.PBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Pass = new System.Windows.Forms.TextBox();
            this.PassC = new System.Windows.Forms.TextBox();
            this.Start = new System.Windows.Forms.Button();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Match = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.Sorry = new System.Windows.Forms.Button();
            this.Dec = new System.Windows.Forms.CheckBox();
            this.thd = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // OpenFile
            // 
            this.OpenFile.BackColor = System.Drawing.Color.Silver;
            this.OpenFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.OpenFile.Location = new System.Drawing.Point(15, 6);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(75, 23);
            this.OpenFile.TabIndex = 0;
            this.OpenFile.Text = "Open";
            this.OpenFile.UseVisualStyleBackColor = false;
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // FN
            // 
            this.FN.AutoSize = true;
            this.FN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FN.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.FN.Location = new System.Drawing.Point(12, 99);
            this.FN.Name = "FN";
            this.FN.Size = new System.Drawing.Size(41, 13);
            this.FN.TabIndex = 1;
            this.FN.Text = "Name :";
            // 
            // SZ
            // 
            this.SZ.AutoSize = true;
            this.SZ.ForeColor = System.Drawing.SystemColors.AppWorkspace;
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
            this.Fname.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.Fname.Location = new System.Drawing.Point(49, 99);
            this.Fname.Name = "Fname";
            this.Fname.Size = new System.Drawing.Size(35, 13);
            this.Fname.TabIndex = 3;
            this.Fname.Text = "label3";
            // 
            // Fsize
            // 
            this.Fsize.AutoSize = true;
            this.Fsize.ForeColor = System.Drawing.SystemColors.AppWorkspace;
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
            // Start
            // 
            this.Start.BackColor = System.Drawing.Color.Silver;
            this.Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Start.Location = new System.Drawing.Point(96, 6);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 10;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = false;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.CheckBox1.Location = new System.Drawing.Point(252, 37);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(77, 17);
            this.CheckBox1.TabIndex = 11;
            this.CheckBox1.Text = "Show Text";
            this.CheckBox1.UseVisualStyleBackColor = true;
            this.CheckBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // Match
            // 
            this.Match.AutoSize = true;
            this.Match.ForeColor = System.Drawing.Color.Red;
            this.Match.Location = new System.Drawing.Point(252, 60);
            this.Match.Name = "Match";
            this.Match.Size = new System.Drawing.Size(54, 13);
            this.Match.TabIndex = 12;
            this.Match.Text = "No Match";
            // 
            // Cancel
            // 
            this.Cancel.BackColor = System.Drawing.Color.Silver;
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel.Location = new System.Drawing.Point(260, 6);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 13;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = false;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Sorry
            // 
            this.Sorry.BackColor = System.Drawing.Color.Silver;
            this.Sorry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Sorry.Location = new System.Drawing.Point(177, 6);
            this.Sorry.Name = "Sorry";
            this.Sorry.Size = new System.Drawing.Size(75, 23);
            this.Sorry.TabIndex = 14;
            this.Sorry.Text = "I\'m Sorry";
            this.Sorry.UseVisualStyleBackColor = false;
            this.Sorry.Click += new System.EventHandler(this.Sorry_Click);
            // 
            // Dec
            // 
            this.Dec.AutoSize = true;
            this.Dec.ForeColor = System.Drawing.Color.Silver;
            this.Dec.Location = new System.Drawing.Point(183, 10);
            this.Dec.Name = "Dec";
            this.Dec.Size = new System.Drawing.Size(63, 17);
            this.Dec.TabIndex = 15;
            this.Dec.Text = "Decrypt";
            this.Dec.UseVisualStyleBackColor = true;
            this.Dec.CheckedChanged += new System.EventHandler(this.Dec_CheckedChanged);
            // 
            // thd
            // 
            this.thd.AutoSize = true;
            this.thd.ForeColor = System.Drawing.Color.Silver;
            this.thd.Location = new System.Drawing.Point(223, 112);
            this.thd.Name = "thd";
            this.thd.Size = new System.Drawing.Size(0, 13);
            this.thd.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(344, 132);
            this.Controls.Add(this.thd);
            this.Controls.Add(this.Dec);
            this.Controls.Add(this.Sorry);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Match);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.PassC);
            this.Controls.Add(this.Pass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PBar);
            this.Controls.Add(this.Fsize);
            this.Controls.Add(this.Fname);
            this.Controls.Add(this.SZ);
            this.Controls.Add(this.FN);
            this.Controls.Add(this.OpenFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crypt-It";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button OpenFile;
        private System.Windows.Forms.Label FN;
        private System.Windows.Forms.Label SZ;
        private System.Windows.Forms.Label Fname;
        private System.Windows.Forms.Label Fsize;
        private System.Windows.Forms.ProgressBar PBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Pass;
        private System.Windows.Forms.TextBox PassC;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.CheckBox CheckBox1;
        private System.Windows.Forms.Label Match;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Sorry;
        private System.Windows.Forms.CheckBox Dec;
        private System.Windows.Forms.Label thd;
    }
}

