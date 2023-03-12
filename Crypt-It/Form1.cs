using Crypt_It.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

/////////////////////////////////////////////////////////////
//     Crypt-It by Daryl Krans                             //
//     Started on Feb 8th 2023                             //
//     Latest revision Mar 11th 2023                       //
/////////////////////////////////////////////////////////////

///
/// Crypt-It is a light-weight high performance file encryption program. It uses seed based encryption keys. The seed is generated
/// using the password inputted by the user.  That unique seed is used to generate encryption keys for the XOR function and the
/// Bit-Rotate function.  
/// 
/// The data is read from the source file, XORed, Rotated, and XORed again before being written to a .crypt file
/// If a .crypt file is selected as the source, the Bit-Rotate funciton is reversed to reconstruct the original data. Both XOR
/// operations do not need to be reversed since the key used is the same for both XOR operations
/// 
///
/// THINGS TO DO!!
///
///  (done)  Fix the password generation to ensure no two different passwords yield the same seed
///  (done)  Fix decrypt button checks
///  (done)  Add threading to the encryption process for extra speed
///  (done)  Add ability to process existing file instead of creating a new file (sort of.. Creates new file, encrypts, deletes source if option is checked)
///  (done)  Add batch file processing
///  (done)  Added drag and drop file processing (Also supports files in subfolders)
///  (done)  Added update interval timer to prevent large # of small files from slowing down the program with constant updates
///  (done)  Added menu strip for ease of use. Looks cleaner than individual buttons and adds easier access to adjustable options
///  (done)  Added message box for the timer function instead of displaying it in the title bar
///  (done)  Added remaining time to menu strip and fixed the time format output for both time remaining and time elapsed.
///  (done)  Added ability to drag multiple folders into the program for processing
///  (done)  Fixed bug where Open File and Batch File button didn't do anything and returned to main screen
///  (done)  Added progress indicator to taskbark
///  (done)  Added NuGet package Costura.Fody to embed external DLL files into the executable on compile
///  (done)  Added status bar to the bottom that displays selected options and other potentially useful information
///  (done)  Added ability grant access to protected files and folders.  Highway to the <DANGER DANGER ZONE </DANGER> (read access only, no write)
///  (done)  Added manual garbage collection to manage memory usage better
///  (done)  Added ability to detect physical core count instead of guessing by dividing threads by 2
///          Possibly add more steps to the encryption process

//          -- Must add reference to assembly to make this work!! --
//                    Set Taskbar progress indicators
//      TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error, Handle);
//      TaskbarManager.Instance.SetProgressValue((int)prog, 100, Handle);

namespace Crypt_It
{

    public partial class Crypt_It : Form
    {
        readonly string Program = "Crypt-It";
        readonly string Version = "v0.9.63";
        /// These settings are available in the options menu. b_Reverse is available in File menu as "Decrypt"
        int CoreVal = 4; // set number of cpu cores to use for threading
        string[] s_OpenFiles = new string[0];
        string s_Password;
        string s_PasswdConf;
        byte[][] bt_Output = new byte[0][];
        byte[] by_LKey = new byte[0];
        byte[] by_bKey = new byte[0];
        byte[] by_XorKey = new byte[0];
        int i_B = 0;  // Don't click the "show text" button too many times. (You've been warned!)
        int i_Cores;
        int i_ActualCores = 0;
        int l;
        readonly int Chunk_Length = 131072 * 6; //(131072 << 3); // change chunk size.  (*8 = 8mb chunks) 
        readonly int def;
        readonly string dummypath = $@"{(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))}\Crypt-It";
        readonly string testpath = $@"{(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))}\Crypt-It TestFile";
        readonly string dummyfile = @"\dummyfil.000";
        readonly CustomProgressBar PBar = new CustomProgressBar { DisplayStyle = ProgressBarDisplayText.Percentage, };
        public Crypt_It()
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        // ------------------- Program start here -----------------------------
        {
            InitializeComponent();
            def = CoreVal;
            Set_Options_Start();
            Options();
            Clear_Info();
        }
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        public static string Get_Time(long ms, bool s)
        {
            string hr; string mn; string se;
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            if (t.Hours < 1) hr = ""; else hr = $"{t.Hours.ToString().PadLeft(1, '0')}:";
            if (t.Hours > 9) hr = $"{(t.Hours.ToString().PadLeft(2, '0'))}:";
            if (t.Hours < 1 && t.Minutes < 1 && s) mn = ""; else mn = "0:";
            if (t.Hours > 0 && mn != "") mn = $"{(t.Minutes.ToString().PadLeft(2, '0'))}:";
            if (t.Hours < 1 && mn != "") { mn = $"{t.Minutes.ToString().PadLeft(1, '0')}:"; }
            se = $"{t.Seconds.ToString().PadLeft(2, '0')}";
            if (s && t.Minutes < 1) se = $"{t.Seconds.ToString().PadLeft(1, '0')}";
            if (s) return $"{hr}{mn}{se}.{t.Milliseconds}"; else return $"({hr}{mn}{se})";
        }
        /// <summary>
        /// IEnumerable string used to set IO file permissions to allow read of protected folders/files
        /// </summary>
        public IEnumerable<string> Get(string path)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();
            IEnumerable<string> directories = Enumerable.Empty<string>();
            try
            {
                var permission = new FileIOPermission(FileIOPermissionAccess.Read, path);
                permission.Demand();
                files = Directory.GetFiles(path);
                directories = Directory.GetDirectories(path);
            }
            catch
            {
                path = null;
            }

            if (path != null)
            {
                yield return path;
            }

            foreach (var file in files)
            {
                yield return file;
            }
            var subdirectoryItems = directories.SelectMany(Get);
            foreach (var result in subdirectoryItems)
            {
                yield return result;
            }
        }
        protected virtual bool Locked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
        void Set_Cores()
        {
            foreach (var item in new System.Management.ManagementObjectSearcher("Select NumberOfCores from Win32_Processor").Get())
            {
                int coreCount = int.Parse(item["NumberOfCores"].ToString());
                i_ActualCores += coreCount;
            }
            i_ActualCores -= 1;
            if (CoreVal < 1) CoreVal = 1;
        }
        void Set_Selected_Options()
        {
            string d = ""; string dr = ""; string tf = ""; string mt; string ti = ""; string dc = "";
            if (B.DelSource) d = " (Delete source) ";
            if (B.DryRun) dr = " (Dry run) ";
            if (B.TestFile) tf = " (Test file) ";
            if (B.Timer) ti = " (Timer) ";
            if (msDec.Checked) dc = " (Decrypt mode) ";
            if (msThreadMan.Checked) mt = $" (Max threads {CoreVal}) "; else mt = " (Max threads Auto) ";
            if (File.NewFile.Length > 0) SelectedOptions.Text = $"{d}{dr}{tf}{mt}{ti}{dc}";
            else SelectedOptions.Text = "Open or Drag Files.";
            if (SelectedOptions.Text.Length <= 4) SelectedOptions.Visible = false; else SelectedOptions.Visible = true;
        }
        void Set_Options_Start()
        {
            pathLabel.Text = "";
            PBar.Location = new System.Drawing.Point(15, 82);
            PBar.Size = new System.Drawing.Size(320, 15);
            PBar.ForeColor = Color.White;
            Controls.Add(PBar);
            var a = 128;
            Color bcolor = Color.FromArgb(64, a, a, a);
            thd.ForeColor = SelectedOptions.ForeColor = Color.FromArgb(144, 255, 144);
            AllowDrop = true;
            DragEnter += new DragEventHandler(Crypt_It_DragEnter);
            DragDrop += new DragEventHandler(Crypt_It_DragDrop);
            Menu.BackColor = t_remain.BackColor = bcolor;
            Menu.Renderer = new MyRender();
            panel1.BackColor = bcolor;
            this.Text = $"{Program} {Version}";
            Set_Cores();
            Set_Selected_Options();
        }
        void Start_Working(bool a)
        {
            Pass.Enabled = PassC.Enabled = !a;
            PBar.Visible = B.Working = a;
            fileToolStripMenuItem.Enabled = optionsToolStripMenuItem.Enabled = msClear.Visible = msClear.Enabled = !a;
            if (a) { msStart.Text = "Cancel"; msStart.Enabled = a; }
            else msStart.Text = "Start";
        }
        void Clear_Info()
        {
            if (pathLabel.Text.Contains("inaccessible") || pathLabel.Text.Contains("No files") && File.NewFile.Length == 0)
            {
                pathLabel.ForeColor = Color.Yellow;
                pathLabel.Visible = true;
            }
            else
            {
                pathLabel.Visible = false;
                pathLabel.Visible = false;
                pathLabel.Text = "";
                pathLabel.ForeColor = Color.Silver;
            }
            Array.Clear(File.NewFile, 0, File.NewFile.Length);
            Array.Clear(File.OutFile, 0, File.OutFile.Length);
            Array.Clear(File.FileSize, 0, File.FileSize.Length);
            Array.Clear(s_OpenFiles, 0, s_OpenFiles.Length);
            TaskbarManager.Instance.SetProgressValue(0, 100, Handle);
            File.NewFile = new string[0];
            File.OutFile = new string[0];
            File.FileSize = new long[0];
            File.l_tot = File.i_TotalFiles = 0;
            s_OpenFiles = new string[0];
            thd.Text = t_remain.Text = "";
            PBar.Value = 0;
            s_Password = s_PasswdConf = Pass.Text = PassC.Text = "";
            if (!B.msDCHK) B.Reverse = msDec.Checked = msClear.Visible = false;
            B.Overwrite = B.Overwrite_Checked = B.YesClick = msStart.Enabled = B.msDCHK =
                t_remain.Visible = B.Drop = B.Cancel = false;
            GC.Collect();
            Set_Selected_Options();
        }
        async void Process_File_List()
        {
            (by_LKey, by_bKey, by_XorKey) = Make_Keys();
            if (B.DryRun) { B.DelSource = msDelFile.Checked = false; }
            var def = this.Text;
            long TotalLength = 0;
            Start_Working(true);
            int tfil = File.i_TotalFiles;
            long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            DateTime start = DateTime.Now;
            Thread Check = new Thread(new ThreadStart(Threaded_Update)); Check.Start();
            for (int FileNum = 0; FileNum < File.i_TotalFiles; FileNum++)
            {
                if (FileNum == File.i_TotalFiles | B.Cancel) Check.Abort();
                File.FileNum = FileNum;
                FileStream Stream = new FileStream(File.NewFile[FileNum], FileMode.Open, FileAccess.Read);
                bool b_DoWork = B.Overwrite = true;
                B.Reverse = (System.IO.Path.GetExtension(File.NewFile[FileNum]) == ".crypt");
                var uLongs = File.FileSize[FileNum] >> 3; // FileSize / 8;
                long[] ByteSegment = new long[3]
                {
                    uLongs / Chunk_Length, ((File.FileSize[FileNum] - (((uLongs / Chunk_Length) *
                    Chunk_Length) << 3)) >> 3) << 3, File.FileSize[FileNum] - (uLongs << 3)
                };
                var length = Chunk_Length << 3;
                // ----- Condition Checks ---------------
                if (B.TestFile)
                {
                    if (B.Reverse) File.OutFile[FileNum] =  $@"{testpath}\testfile{FileNum}.bin";
                    else File.OutFile[FileNum] = $@"{testpath}\testfile{FileNum}.crypt";
                    if (!Directory.Exists(testpath)) Directory.CreateDirectory(testpath);
                }
                Overwrite_Prompt(File.OutFile[FileNum]);
                if (B.DryRun)
                {
                    bool exists = System.IO.Directory.Exists(dummypath);
                    if (!exists) System.IO.Directory.CreateDirectory(dummypath);
                    File.OutFile[FileNum] = dummypath + dummyfile;
                }
                b_DoWork = (FileNum != (File.i_TotalFiles - 1));
                // ------ end condition checks ------------
                if (B.Overwrite && !B.Cancel)
                {
                    if (FileNum == (File.i_TotalFiles - 1) && !B.Overwrite) goto Endall;
                    FileStream Dest = new FileStream(File.OutFile[FileNum], FileMode.Append);
                    if (B.Hide_Files | B.DryRun) Set_File_Hidden(File.OutFile[FileNum], "h");
                    /// ---------- start Asynchronous task -----------------------------------
                    await Task.Run(delegate
                    {
                        if (B.Set_Cores) { if (CoreVal > 0) i_Cores = CoreVal - 1; else i_Cores = 0; } else i_Cores = i_ActualCores;
                        // -----------------------------------------------------------------------------
                        for (long step = 0; step < 3; step++)
                        {
                            var i = ByteSegment[step];
                            if (step > 0)
                            {
                                length = (int)ByteSegment[step]; i = 1; i_Cores = 0;
                            }
                            long offset = 0;
                            Thread[] enc = new Thread[i_Cores + 1];
                            for (long j = 0; j < i; j++)
                            {
                                if (B.Cancel) goto Loopend;
                                switch (step)
                                {
                                    case 0: offset = (long)(j * length); break;
                                    case 1: offset = (long)ByteSegment[0] * (Chunk_Length << 3); break;
                                    case 2: offset = (long)ByteSegment[0] * (Chunk_Length << 3) + (long)ByteSegment[1]; break;
                                }
                                B.MultiThread = (i - j >= enc.Length && i_Cores > 0);
                                if (i - j < enc.Length && i_Cores > 0) { i_Cores = (int)(i - j) - 1; B.MultiThread = true; }
                                byte[][] chunk = bt_Output = new byte[i_Cores + 1][];
                                for (int x = 0; x <= i_Cores; x++) chunk[x] = Read_Data(offset + (x * length), length);
                                j += i_Cores; TotalLength += length * (i_Cores + 1);
                                // --------- Create and start new worker threads ----------------------
                                for (int x = 0; x <= i_Cores; x++)
                                {
                                    var xx = x;
                                    enc[x] = new Thread(() => { Encrypt(chunk[xx], by_LKey, by_bKey, by_XorKey, length, step, xx); });
                                    enc[x].Start();
                                }
                                for (int x = 0; x <= i_Cores; x++) enc[x]?.Join();
                                // -------------------------------------------------------------------
                                if (!B.DryRun) for (int x = 0; x <= i_Cores; x++) Dest.Write(bt_Output[x], 0, bt_Output[x].Length);
                                GC.Collect();
                            }
                        }
                    Loopend:
                        Stream?.Close();
                        Dest.Close();
                        if (B.Cancel)
                        {
                            try
                            {
                                if (B.Hide_Files) Set_File_Hidden(File.OutFile[FileNum], "u");
                                if (System.IO.File.Exists(File.OutFile[FileNum])) System.IO.File.Delete(File.OutFile[FileNum]);
                            }
                            catch { }
                        }
                        if (i_Cores == 0) this.Invoke(new Action(() => thd.Text = ""));
                        if (B.Timer && FileNum == File.i_TotalFiles - 1)
                        {
                            long msf = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            string e_time = Get_Time(msf - ms, true);
                            string s = "";
                            if (tfil > 1) s = "s";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result = System.Windows.Forms.MessageBox.Show($"time {e_time} seconds", ($"{tfil} file{s} processed.")
                                , buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        }
                        if (File.FileSize.Length >= FileNum && File.FileSize.Length > 0) File.FileSize[FileNum] = 0;
                        if (!B.Cancel && B.Hide_Files) Set_File_Hidden(File.OutFile[FileNum], "u");
                        if (!B.Cancel && B.DelSource && B.Overwrite)
                        {
                            try
                            {
                                if (System.IO.File.Exists(File.NewFile[FileNum])) System.IO.File.Delete(File.NewFile[FileNum]);
                            }
                            catch { }
                        }
                        if (FileNum == File.i_TotalFiles)
                        {
                            this.Invoke(new Action(() => Start_Working(false)));
                            this.Invoke(new Action(() => B.msDCHK = false));
                            this.Invoke(new Action(() => Clear_Info()));
                            b_DoWork = false;
                        }
                        if (B.Cancel)
                        {
                            Check.Abort();
                            b_DoWork = false; FileNum = File.i_TotalFiles - 1; this.Invoke(new Action(() => this.Text = def));
                        }
                    });
                    /// ----------------- end Asynchronous task --------------------------------
                    if (B.Cancel) goto Endall;
                }

            Endall:
                if (!b_DoWork)
                {
                    Check.Abort();
                    this.Text = def;
                    Stream?.Close();
                    b_DoWork = true;
                    Start_Working(false);
                    B.msDCHK = false;
                    GC.Collect();
                    Clear_Info();
                    Options();
                }

                void Overwrite_Prompt(string f)
                {
                    if (!B.Overwrite_Checked)
                    {
                        string mes, mes2; var e = 0;
                        if (File.i_TotalFiles > 1) { mes = "s"; mes2 = null; }
                        else { mes = null; mes2 = "s"; }
                        for (int w = 0; w < File.OutFile.Length; w++)
                        {
                            if (System.IO.File.Exists(File.OutFile[w])) e++;
                        }
                        B.Overwrite_Checked = true;
                        if (e > 0)
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                            DialogResult result = System.Windows.Forms.MessageBox.Show($"overwrite file{mes}?", ($"{e} Destination file{mes} already exist{mes2}!")
                                , buttons, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes) B.YesClick = true;
                            if (result == DialogResult.No) B.YesClick = false;
                            if (result == DialogResult.Cancel)
                            {
                                Stream?.Close();
                                B.Overwrite = b_DoWork = false;
                                B.Cancel = true;
                                this.Text = def;
                                FileNum = File.i_TotalFiles - 1;
                            }
                        }
                    }
                    if (!B.YesClick)
                    {
                        if (System.IO.File.Exists(File.OutFile[FileNum]))
                        {
                            Stream?.Close();
                            b_DoWork = (FileNum != File.i_TotalFiles - 1);
                            B.Overwrite = false;
                            File.l_tot += File.FileSize[FileNum];
                        }
                        else B.Overwrite = true;
                    }
                    if (B.YesClick && !B.DryRun)
                    {
                        try { System.IO.File.Delete(f); } catch { }
                        B.Overwrite = (!System.IO.File.Exists(f));
                    }
                }
                //----------------------- end of main file process void -- sub voids follow --------------------------
                void Set_File_Hidden(string f, string a)
                {
                    if (a == "h") try { System.IO.File.SetAttributes(f, System.IO.File.GetAttributes(f) | FileAttributes.Hidden); } catch { } // hide file
                    if (a == "u") try { System.IO.File.SetAttributes(f, System.IO.File.GetAttributes(f) & ~FileAttributes.Hidden); } catch { } // unhide file
                }
                byte[] Read_Data(long offset, int len)
                {
                    var dat = new byte[len];
                    Stream.Seek(offset, SeekOrigin.Begin);
                    Stream.Read(dat, 0, len);
                    return dat;
                }
            }
            void Threaded_Update()
            {
                while (B.Working | !B.Cancel)
                {
                    this.Invoke(new Action(() => Progress_Update()));
                    Thread.Sleep(75);
                }
            }
            void Progress_Update()
            {
                TimeSpan spent = (DateTime.Now - start);
                int tms = (int)spent.TotalMilliseconds + 2;
                if (tms < 1) tms = 1;
                long Elapsed = 0;
                if (TotalLength > tms) Elapsed = (File.l_tot - TotalLength) / (TotalLength / tms);
                double prog = 100.0 * TotalLength / File.l_tot;
                PBar.Value = (int)prog;
                TaskbarManager.Instance.SetProgressValue(PBar.Value, 100, Handle);
                if (!t_remain.Visible) t_remain.Visible = true;
                t_remain.Text = Get_Time(Elapsed, false);
                if (File.i_TotalFiles > 1)
                {
                    Fname.Text = $"{Trunc(true, File.NewFile[File.FileNum], 48)}";
                }
                long left = 0;
                string kb;
                var p = (File.l_tot - TotalLength);
                if (((p) >> 10) > 1000000) { left = (((p) >> 10) >> 10); kb = "mb"; }
                else { left = ((p) >> 10); kb = "kb"; }
                Fsize.Text = $"{left:N0} {kb} / Files ({(File.NewFile.Length - File.FileNum):N0}) remaining.";
                if (B.MultiThread) thd.Text = $"Threads (x{i_Cores + 1})"; else thd.Text = null;
            }
        }
        /// ------------------------------------ Actual code for Encrytion process --------------------------------------- ///
        void Encrypt(byte[] DataIn, byte[] skey, byte[] rkey, byte[] xor, long length, long r1, int o)
        {
            bt_Output[o] = XOR(Shift(XOR(DataIn, xor), r1), xor);

            byte[] Shift(byte[] dat, long step)
            {
                int r = 0; if (B.Reverse) r = 1;
                ulong g;
                var buffer = new MemoryStream();
                var write = new BinaryWriter(buffer);
                if (step < 2)
                {
                    for (int x = 0; x < (length >> 3); x++) // code to bitshift unsigned long's (64-bit)
                    {
                        g = (x % 2 == r) ? RotateRight(BitConverter.ToUInt64(dat, x << 3), skey[x % skey.Length]) :
                            RotateLeft(BitConverter.ToUInt64(dat, x << 3), skey[x % skey.Length]);
                        write.Write(BitConverter.GetBytes(g));
                    }
                }
                else
                {
                    for (int x = 0; x < dat.Length; x++) // code to bitshift a single unsigned byte (8-bit)
                    {
                        g = (x % 2 == r) ? ((uint)dat[x] >> rkey[x]) | ((uint)dat[x] << (8 - rkey[x])) :
                            ((uint)dat[x] << rkey[x]) | ((uint)dat[x] >> (8 - rkey[x]));
                        write.Write((byte)(g));
                    }
                }
                buffer.Close();
                write.Close();
                return (buffer.ToArray());
            }
            byte[] XOR(byte[] Data, byte[] XorKey)
            {
                for (int t = 0; t < Data.Length; t++) Data[t] = (byte)(Data[t] ^ (XorKey[t % XorKey.Length]));
                return Data;
            }
            ulong RotateLeft(ulong a, int b)
            { return (a << b) | (a >> (64 - b)); }  // bitrotate Ulong to the left
            ulong RotateRight(ulong a, int b)
            { return (a >> b) | (a << (64 - b)); }// bitrotate Ulong to the right
        }
        /// --------------------------------------- End encrytion process ------------------------------------------ ///
        void LockProgram() // This was just for fun.  A little hidden joke.
        {
            msStart.Enabled = Pass.Enabled = PassC.Enabled = CheckBox1.Enabled = msClear.Enabled =
                ControlBox = optionsToolStripMenuItem.Enabled = fileToolStripMenuItem.Enabled = false;
            if (B.LastChance)
            {
                msSorry.Visible = false;
                this.Text = "Program LOCKED. You're done!";
                while (1 == 1) Console.Beep(4000, 1000);
            }
            else
            {
                this.Text = "Do you deserver another chance?";
                msSorry.Visible = true;
            }
        }
        void Options()
        {
            Match.Visible = msSorry.Visible = false;
            s_Password = Pass.Text;
            if (B.Reverse)
            { PassC.Visible = label2.Visible = false; s_PasswdConf = s_Password; }
            else PassC.Visible = label2.Visible = true;
            if (!B.Reverse) s_PasswdConf = PassC.Text;
            bool m = s_PasswdConf.Equals(s_Password);
            Show_PWD();
            Disable_Passwd();
            if (File.NewFile.Length == 0)
                FN.Visible = SZ.Visible = PBar.Visible = Fname.Visible = Fsize.Visible = false;
            else
            {
                FN.Visible = SZ.Visible = Fsize.Visible = msClear.Visible = true;
                if (File.FileSize.Length > 0 && File.FileSize[File.i_TotalFiles - 1] > 0) Enable_Passwd();
            }
            if (!B.Working)
            {
                if (s_PasswdConf.Length > 0)
                {
                    Match.Visible = (!B.Reverse);
                    if (m)
                    {
                        if (File.FileSize.Length > 0 && File.FileSize[File.i_TotalFiles - 1] > 0) msStart.Enabled = true;
                        Match.Text = "Match";
                        Match.ForeColor = Color.Silver;
                    }
                    else
                    {
                        Match.ForeColor = Color.Red;
                        Match.Text = "No Match";
                        msStart.Enabled = false;
                    }
                }
            }
            else Pass.Enabled = PassC.Enabled = false;
            void Enable_Passwd()
            {
                PassC.Enabled = Pass.Enabled = true;
                PassC.BackColor = Pass.BackColor = Color.White;
            }
            void Disable_Passwd()
            {
                PassC.Enabled = Pass.Enabled = false;
                PassC.BackColor = Pass.BackColor = Color.Gray;
            }
        }
        void Show_PWD()
        {
            if (!CheckBox1.Checked) { Pass.PasswordChar = PassC.PasswordChar = '\u25CF'; CheckBox1.Text = "Show Text"; }
            else { Pass.PasswordChar = PassC.PasswordChar = '\0'; CheckBox1.Text = "Hide Text"; }
        }
        void Filter_Files()
        {
            if (!B.Drop)
            {
                var s = "";
                if (B.Reverse && msDec.Checked) this.openFileDialog1.Filter = "Crypt-It Files (*.crypt)|*.crypt|All files (*.*)|*.*";
                else this.openFileDialog1.Filter = "All files (*.*)|*.*|Crypt-It Files (*.crypt)|*.crypt";
                openFileDialog1.FileName = "";
                if (openFileDialog1.Multiselect) s = "s"; else s = "";
                openFileDialog1.Title = $"Open file{s} to encrypt";
                openFileDialog1.ShowDialog();
                if (openFileDialog1.FileName.Length > 0)
                {
                    s_OpenFiles = openFileDialog1.FileNames;
                    Sort();
                }
                else Clear_Info();
            }
            if (B.Drop) if (File.NewFile.Length > 0) Sort();
                else { Clear_Info(); Options(); }
            this.ActiveControl = Pass;
            async void Sort()
            {
                this.Text = "Checking files.";
                File.cryptfile = true;
                await Task.Run(delegate
                {
                    if (!B.Drop)
                    {
                        B.Working = true;
                        if (s_OpenFiles.Length > 0) l = s_OpenFiles.Length;
                        else l = 0;
                        int x = 0; File.l_tot = 0;
                        string[] temp = new string[l];
                        long[] temp2 = new long[l];
                        for (int i = 0; i < l; i++)
                        {
                            if (!Locked(new FileInfo(s_OpenFiles[i])))  //(!u)
                            {
                                try
                                {
                                    var s = new System.IO.FileInfo(s_OpenFiles[i]).Length;
                                    if (s > 0) { temp[x] = s_OpenFiles[i]; temp2[x] = s; x++; }
                                }
                                catch (Exception) { }
                            }
                        }
                        if (x > 0)
                        {
                            File.FileSize = new long[x];
                            File.NewFile = new string[x];
                            File.OutFile = new string[x];
                            File.i_TotalFiles = x;
                            for (int i = 0; i < x; i++)
                            {
                                File.NewFile[i] = temp[i];
                                File.l_tot += temp2[i];
                                File.FileSize[i] = temp2[i];
                                if (System.IO.Path.GetExtension(File.NewFile[i]) != ".crypt") { File.OutFile[i] = $"{File.NewFile[i]}.crypt"; File.cryptfile = false; }
                                else File.OutFile[i] = $@"{System.IO.Path.GetDirectoryName(File.NewFile[i])}\{System.IO.Path.GetFileNameWithoutExtension(File.NewFile[i])}";
                            }
                        }
                        Array.Clear(temp, 0, temp.Length);
                        Array.Clear(temp2, 0, temp2.Length);
                    }
                });

                if (File.NewFile.Length == 1) { Fname.Text = Trunc(true, File.NewFile[0], 52); Fname.Visible = true; }
                if (File.NewFile.Length > 1) { Fname.Text = $"Batch file process ({File.NewFile.Length:N0}) files."; Fname.Visible = true; }
                if (File.NewFile.Length >= 1)
                {
                    long left = 0;
                    string kb;
                    if (((File.l_tot) >> 10) > 1000000) { left = (((File.l_tot) >> 10) >> 10); kb = "mb"; }
                    else { left = ((File.l_tot) >> 10); kb = "kb"; }
                    Fsize.Text = $"{left:N0} {kb}";
                    Fsize.ForeColor = Color.Silver;
                }
                else { Fname.Visible = Fsize.Visible = false; }
                if (File.cryptfile && File.l_tot > 0) B.Reverse = msDec.Checked = true;
                else B.Reverse = msDec.Checked = false;
                B.Working = B.Drop = false;
                B.Reverse = msDec.Checked = (File.NewFile.Length == 1 && System.IO.Path.GetExtension(File.NewFile[0]) == ".crypt");
                this.Text = $"{Program} {Version}";
                Options();
                Set_Selected_Options();
            }
        }
        private (byte[], byte[], byte[]) Make_Keys()
        {
            byte[] pwd = Encoding.ASCII.GetBytes(s_Password);
            Random pass = new Random(pwd[0]);
            int seed = 1;
            for (int i = 0; i < pwd.Length; i++)
            {
                var o = pass.Next(255);
                var x = pass.Next(1, 255);
                var d = pass.Next(0, 4);
                switch (d)
                {
                    case 0: seed += seed + (pwd[i] ^ o) * x; break;
                    case 1: seed += seed / (pwd[i] - o) ^ x; break;
                    case 2: seed += seed ^ (pwd[i] * o) / x; break;
                    case 3: seed += seed * (pwd[i] + o) - x; break;
                }
            }
            seed = Math.Abs(seed);
            /// WARNING!! /// Decrypting files that were encrypted with a different kLen value will result in corrupted data!
            var KeyLen = 512;             // change encryption key length (multiples of 8 recommended)
            var LKey = new byte[KeyLen];
            var bKey = new byte[8];  // vaule MUST be at least 8, maybe 7.  Higher than 8 is pointless
            var XorKey = new byte[KeyLen];
            for (int i = 0; i < pwd.Length; i++)
            {
                seed += (pwd[i]);
            }
            Random rand = new Random(seed);
            for (int i = 0; i < KeyLen; i++)
            {
                LKey[i] = (byte)rand.Next(1, 64); // generates 128 (kLen = 128) variables for bit-shifting Ulong's (64-bit integers)
                if (i < 8) bKey[i] = (byte)rand.Next(1, 8);
                XorKey[i] = (byte)rand.Next(255);   // generates 1024-bit (kLen = 128) XOR key (128 bytes long)
            }
            return (LKey, bKey, XorKey);
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!B.Working) Options();
            else Show_PWD();
            i_B++;
            if (!B.LastChance)
            {
                switch (i_B)
                {
                    case 100: this.Text = "Are you having fun?"; break;
                    case 150: this.Text = "Quit playing around!"; break;
                    case 200: this.Text = "Warning #1!"; break;
                    case 220: this.Text = "Warning #2!!"; break;
                    case 240: this.Text = "Final WARNING!!!"; break;
                    case 250: LockProgram(); break;
                }
            }
            else
            {
                if (i_B == 30) this.Text = "Don't blow your last chance.";
                else if (i_B == 50) LockProgram();
            }
        }
        private void PassC_TextChanged(object sender, EventArgs e)
        {
            Options();
            this.ActiveControl = PassC;
        }
        private void Pass_TextChanged(object sender, EventArgs e)
        {
            Options();
            this.ActiveControl = Pass;
        }
        private void Start_Click(object sender, EventArgs e)
        {
            Process_File_List();
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            B.Cancel = true;
        }
        private void Dec_CheckedChanged(object sender, EventArgs e)
        {
            B.Reverse = !B.Reverse;
            Options();
        }
        private static string Trunc(bool r, string value, int maxChars)
        {
            if (r) return value.Length <= maxChars ? value : $"{System.IO.Path.GetPathRoot(value)}..{value.Substring(value.Length - (maxChars), maxChars)}";
            else return value.Length <= maxChars ? value : $"{value.Substring(0, maxChars)}..";
        }
        void Crypt_It_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        async void Crypt_It_DragDrop(object sender, DragEventArgs e)
        {
            if (!B.Working)
            {
                Clear_Info();
                Options();
                B.Drop = true;
                long len = 0;
                var f = 0;
                pathLabel.Visible = false;
                pathLabel.Text = "";
                pathLabel.ForeColor = Color.Silver;
                SelectedOptions.Text = "Retrieving File List.";
                string[] File_List = (string[])e.Data.GetData(DataFormats.FileDrop);
                var files = new List<string>();
                var size = new List<long>();
                var outfile = new List<string>();
                await Task.Run(delegate
                {
                    B.Working = true;
                    File.cryptfile = true;
                    Thread Check = new Thread(new ThreadStart(P_Update));
                    Check.Start();
                    for (int r = 0; r < File_List.Length; r++)
                    {
                        try
                        {
                            if (!Directory.Exists(File_List[r]))
                            {
                                len = CheckFile(File_List[r]);
                                if (len > 0)
                                {

                                    files.Add(File_List[r]);
                                    size.Add(len);
                                    outfile.Add(Ext(File_List[r]));
                                }
                            }
                            else
                            {
                                var Folder_files = Get(File_List[r]).ToArray();
                                for (int s = 0; s < Folder_files.Length; s++)
                                {
                                    if (!Directory.Exists(Folder_files[s])) len = CheckFile(Folder_files[s]);
                                    if (len > 0)
                                    {
                                        files.Add(Folder_files[s]);
                                        size.Add(len);
                                        outfile.Add(Ext(Folder_files[s]));
                                    }
                                }
                            }
                            string Ext(string inf)
                            {
                                if (System.IO.Path.GetExtension(inf) != ".crypt") { File.cryptfile = false; return ($"{inf}.crypt"); }
                                else return ($@"{System.IO.Path.GetDirectoryName(inf)}\{System.IO.Path.GetFileNameWithoutExtension(inf)}");
                            }
                        }
                        catch (Exception) { f++; }
                    }
                    B.Working = false;
                    Check.Abort();
                });
                t_remain.Visible = false;
                File.NewFile = files.ToArray();
                File.OutFile = outfile.ToArray();
                File.FileSize = size.ToArray();
                files.Clear(); size.Clear(); outfile.Clear();
                File.Enumerate();
                Array.Clear(File_List, 0, File_List.Length);
                GC.Collect();
                if (f > 0 && File.NewFile.Length == 0) pathLabel.Text = $"({f}) Files or folders are inaccessible!";
                if (f == 0 && File.NewFile.Length == 0) pathLabel.Text = "No files in path or files contain no data!";
                if (!pathLabel.Text.Contains("inaccessible") || !pathLabel.Text.Contains("No files")) pathLabel.Visible = false;
                Filter_Files();

                long CheckFile(string file)
                {
                    if (!Locked(new FileInfo(file)))
                    {
                        try
                        {
                            return new FileInfo(file).Length;
                        }
                        catch (Exception) { f++; }
                    }
                    return 0;
                }
                void P_Update()
                {
                    Thread.Sleep(1500);
                    this.Invoke(new Action(() => pathLabel.Visible = t_remain.Visible = true));
                    while (1 == 1)
                    {
                        try
                        {
                            if (files.Count > 0 && files[files.Count - 1].Length > 0) this.Invoke(new Action(() =>
                            { pathLabel.Text = $"{Trunc(false, files[files.Count - 1], 47)}"; SelectedOptions.Text = $"Retrieving file list ({files.Count:N0})"; }));
                            Thread.Sleep(25);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (B.Working) B.Cancel = true;
            else Process_File_List();
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                Clear_Info();
                this.openFileDialog1.Multiselect = false;
                Filter_Files();
            }
        }
        private void BatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                Clear_Info();
                this.openFileDialog1.Multiselect = true;
                Filter_Files();
            }
        }
        private void DeleteSouceFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                if (B.DelSource) B.DelSource = msDelFile.Checked = false;
                else { B.DelSource = msDelFile.Checked = true; msDryRun.Checked = B.DryRun = msTestFile.Checked = B.TestFile = false; }
                Set_Selected_Options();
            }
        }
        private void UseTestfileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                if (B.TestFile) B.TestFile = msTestFile.Checked = false;
                else { B.TestFile = msTestFile.Checked = true; msDelFile.Checked = B.DelSource = msDryRun.Checked = B.DryRun = false; }
                Set_Selected_Options();
            }
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                if (B.Timer) B.Timer = msTimer.Checked = false;
                else B.Timer = msTimer.Checked = true;
                Set_Selected_Options();
            }
        }
        private void MsDec_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                if (B.Reverse) { B.Reverse = msDec.Checked = B.msDCHK = false; msDec.Text = "Decrypt"; msDec.Image = Resources.decrypt; }
                else { B.Reverse = msDec.Checked = B.msDCHK = true; msDec.Text = "Encrypt"; msDec.Image = Resources.enc; }
                Set_Selected_Options();
                Options();
            }
        }
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear_Info();
            Options();
        }
        private void SorryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                this.ControlBox = fileToolStripMenuItem.Enabled = optionsToolStripMenuItem.Enabled =
                    Pass.Enabled = PassC.Enabled = msClear.Enabled = CheckBox1.Enabled = B.LastChance = true;
                msSorry.Visible = false;
                i_B = 0;
                this.Text = $"Annoyed-{Program} {Version}";
                Options();
            }
        }
        private void MsThreadAuto_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                msThreadAuto.Checked = true;
                B.Set_Cores = msThreadMan.Checked = false;
                Set_Selected_Options();
            }
        }
        private void SetManuallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                msThreadAuto.Checked = false;
                B.Set_Cores = msThreadMan.Checked = true;
                Set_Cores();
                void Set_Cores()
                {
                    (CoreVal, B.Thread_Cancel) = Prompt.ShowDialog("(1-255)", "Set Thread Count", CoreVal);
                    if (B.Thread_Cancel) { CoreVal = def; msThreadAuto.Checked = true; B.Thread_Cancel = msThreadMan.Checked = B.Set_Cores = false; }
                }
                Set_Selected_Options();
            }
        }
        private async void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (B.Working)
            {
                B.Cancel = true;
                while (B.Cancel == true) await Task.Delay(25);
            }
            this.Close();
        }
        private void DryRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!B.Working)
            {
                if (msDryRun.Checked) B.DryRun = msDryRun.Checked = false;
                else { msDryRun.Checked = B.DryRun = true; B.DelSource = msDelFile.Checked = msTestFile.Checked = B.TestFile = false; }
                Set_Selected_Options();
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                this.Text = "Closing..";
                Application.Exit();
                Environment.Exit(0);
            }
            catch { }
            this.Close();
        }
    }
}
