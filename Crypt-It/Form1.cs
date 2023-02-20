using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/////////////////////////////////////////////////////////////
//     Crypt-It by Daryl Krans                             //
//     Started on Feb 8th 2023                             //
//     Latest revision Feb 19th 2023                       //
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
///  (done)  Add ability to process existing file instead of creating a new file (sort of.. Creates new file, encrypts, deletes source)
///  (done)  Add batch file processing
///  (done)  Added drag and drop file processing (Also supports files in subfolders now)
///  (done)  Added update interval timer to prevent large # of small files from slowing down the program with constant updates
///  (done)  Added menu strip for ease of use.  Looks cleaner than individual buttons and adds easier access to adjustable options
///          Possibly add more steps to the encryption process

namespace Crypt_It
{
    public partial class Crypt_It : Form
    {
        readonly string Program = "Crypt-It";
        readonly string Version = "v0.8.3";
        /// These setting for debugging purposes (All are available in the options menu. b_Reverse is available in File menu as "Decrypt")
        bool b_Set_Cores = false; // override automatic core detection for threading
        int i_CoreVal = 4; // set number of cpu cores to use for threading
        bool b_Timer = false; // set to true to show time it took to complete the encrypt/decrypt process 
        bool b_TestFile = false; // set to true to set the output file to C:\testfile.(bin/crypt)
        bool b_Reverse = false; // don't mess with this (just use the "decrypt" button)
        /// ------- Personal Preference --------
        readonly bool b_Hide = false;  // set to true to hide output file(s) during encrypt/decrypt process
        bool b_DelSource = false;    // WARNING!! Set to true if you want the source file(s) deleted after encrypt/decrypt process
        readonly long ut = 250; // sets update interval in miliseconds
        /// -------- program variables ---------
        const int Chunk_Length = (131072 << 3); // change chunk size.  (*8 = 8mb chunks) 
        bool b_LC = false;
        bool b_Working = false;
        bool b_Cancel = false;
        bool b_MultiThread = false;
        bool b_Overwrite = false;
        bool b_Yclick = false;
        bool b_OverwriteChecked = false;
        bool b_msDCHK = false;
        bool b_Thread_Cancel = false;
        string[] s_NewFile = new string[0];
        string[] s_OutFile = new string[0];
        string[] s_DropFiles = new string[0];
        string s_Password;
        string s_PasswdConf;
        byte[][] bt_Output = new byte[0][];
        byte[] by_LKey = new byte[0];
        byte[] by_bKey = new byte[0];
        byte[] by_XorKey = new byte[0];
        int i_B = 0;  // Don't click the "show text" button too many times. (You've been warned!)
        int i_TotalFiles;
        int i_Cores;
        int l;
        readonly int def;
        long[] l_FileSize = new long[0];
        long l_tot;

        public Crypt_It()
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        // ------------------- Program start here -----------------------------
        {
            InitializeComponent();
            this.Text = $"{Program} {Version}";
            if (i_CoreVal < 1) i_CoreVal = 1;
            def = i_CoreVal;
            Options();
            Clear_Info();
            AllowDrop = true;
            DragEnter += new DragEventHandler(Crypt_It_DragEnter);
            DragDrop += new DragEventHandler(Crypt_It_DragDrop);
        }
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        public class Prompt // Prompt window configuration
        {

            public static (int, bool) ShowDialog(string text, string caption, int cv)
            {
                bool t = false;
                Form prompt = new Form
                {
                    Width = 200,
                    Height = 120,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen,
                    ControlBox = false
                };
                Label textLabel = new Label()
                {
                    Left = 35,
                    Top = 18,
                    Text = text
                };
                NumericUpDown textBox = new NumericUpDown
                {
                    Left = 75,
                    Top = 15,
                    Width = 50,
                    Value = cv,
                    Maximum = 255,
                    Minimum = 1
                };
                Button confirmation = new Button()
                {
                    Text = "Ok",
                    Left = 30,
                    Width = 50,
                    Top = 40,
                    DialogResult = DialogResult.OK
                };
                Button cancel = new Button()
                {
                    Text = "Cancel",
                    Left = 80,
                    Width = 65,
                    Top = 40,
                    DialogResult = DialogResult.Cancel
                };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;
                var c = 0;
                var dr = (prompt.ShowDialog());
                if (dr == DialogResult.OK) c = (int)textBox.Value; else c = cv;
                if (dr == DialogResult.Cancel) t = true;
                return (c, t);
            }

        }
        void Start_Working(bool a)
        {
            Pass.Enabled = PassC.Enabled = !a;
            PBar.Visible = b_Working = a;
            fileToolStripMenuItem.Enabled = optionsToolStripMenuItem.Enabled = msClear.Visible = msClear.Enabled = !a;
            if (a) { msStart.Text = "Cancel"; msStart.Enabled = a; }
            else msStart.Text = "Start";
        }
        void Clear_Info()
        {
            s_NewFile = new string[0];
            s_DropFiles = new string[1];
            s_DropFiles[0] = "";
            thd.Text = "";
            l_FileSize = new long[0];
            PBar.Value = 0;
            s_Password = s_PasswdConf = Pass.Text = PassC.Text = "";
            if (!b_msDCHK) b_Reverse = msDec.Checked = msClear.Visible = false;
            b_Overwrite = b_OverwriteChecked = b_Yclick = msStart.Enabled = b_msDCHK = b_Cancel = false;
        }
        async void Process_File_List()
        {
            (by_LKey, by_bKey, by_XorKey) = Make_Keys();
            var def = this.Text;
            long TotalLength = 0;
            Start_Working(true);
            long Cur_Time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long Update_Interval = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + ut;
            long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            for (int FileNum = 0; FileNum < i_TotalFiles; FileNum++)
            {
                FileStream Stream = new FileStream(s_NewFile[FileNum], FileMode.Open, FileAccess.Read);
                bool b_DoWork = b_Overwrite = true;
                b_Reverse = (Path.GetExtension(s_NewFile[FileNum]) == ".crypt");
                //var Chunk_Length = 131072 << 3; // change chunk size.  (*8 = 8mb chunks) 
                var uLongs = l_FileSize[FileNum] >> 3; // FileSize / 8;
                long[] ByteSegment = new long[3]
                {
                    uLongs / Chunk_Length, ((l_FileSize[FileNum] - (((uLongs / Chunk_Length) *
                    Chunk_Length) << 3)) >> 3) << 3, l_FileSize[FileNum] - (uLongs << 3)
                };
                var length = Chunk_Length << 3;
                // ----- Condition Checks ---------------
                if (b_TestFile)
                {
                    if (b_Reverse) s_OutFile[FileNum] = $@"c:\testfile{FileNum}.bin";
                    else s_OutFile[FileNum] = $@"c:\testfile{FileNum}.crypt";
                }
                Overwrite_Prompt(s_OutFile[FileNum]);
                b_DoWork = (FileNum != (i_TotalFiles - 1));
                // ------ end condition checks ------------
                if (b_Overwrite && !b_Cancel)
                {
                    if (FileNum == (i_TotalFiles - 1) && !b_Overwrite) goto Endall;
                    FileStream Dest = new FileStream(s_OutFile[FileNum], FileMode.Append);
                    if (b_Hide) Set_File_Hidden(s_OutFile[FileNum], "h");
                    /// ---------- start Asynchronous task -----------------------------------
                    await Task.Run(delegate
                    {
                        if (Update_Interval >= Cur_Time) this.Invoke(new Action(() => W_Update()));
                        i_Cores = Environment.ProcessorCount - 1;
                        if (b_Set_Cores) { if (i_CoreVal > 0) i_Cores = i_CoreVal - 1; else i_Cores = 0; }
                        else if (i_Cores > 5) i_Cores /= 2;
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
                                if (b_Cancel) goto Loopend;
                                switch (step)
                                {
                                    case 0: offset = (long)(j * length); break;
                                    case 1: offset = (long)ByteSegment[0] * (Chunk_Length << 3); break;
                                    case 2: offset = (long)ByteSegment[0] * (Chunk_Length << 3) + (long)ByteSegment[1]; break;
                                }
                                b_MultiThread = (i - j >= enc.Length && i_Cores > 0);
                                if (i - j < enc.Length && i_Cores > 0) { i_Cores = (int)(i - j) - 1; b_MultiThread = true; }
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
                                for (int x = 0; x <= i_Cores; x++) Dest.Write(bt_Output[x], 0, bt_Output[x].Length);
                                Cur_Time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                                if (Cur_Time >= Update_Interval | TotalLength == l_tot)
                                {
                                    Update_Interval += ut;
                                    this.Invoke(new Action(() => Progress_Update()));
                                }
                            }
                        }
                    Loopend:
                        Stream?.Close();
                        Dest.Close();
                        if (b_Cancel)
                        {
                            try
                            {
                                if (b_Hide) Set_File_Hidden(s_OutFile[FileNum], "u");
                                if (File.Exists(s_OutFile[FileNum])) File.Delete(s_OutFile[FileNum]);
                            }
                            catch { }
                        }
                        if (i_Cores == 0) this.Invoke(new Action(() => thd.Text = ""));
                        if (b_Timer && FileNum == i_TotalFiles - 1)
                        {
                            long msf = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            this.Invoke(new Action(() => this.Text = $"completed in {decimal.Divide((msf - ms), 1000):f2} seconds"));
                            Thread.Sleep(4000);
                        }
                        if (l_FileSize.Length >= FileNum && l_FileSize.Length > 0) l_FileSize[FileNum] = 0;
                        if (!b_Cancel && b_Hide) Set_File_Hidden(s_OutFile[FileNum], "u");
                        if (!b_Cancel && b_DelSource && b_Overwrite)
                        {
                            try
                            {
                                if (File.Exists(s_NewFile[FileNum])) File.Delete(s_NewFile[FileNum]);
                            }
                            catch { }
                        }
                        if (FileNum == i_TotalFiles)
                        {
                            this.Invoke(new Action(() => Start_Working(false)));
                            this.Invoke(new Action(() => b_msDCHK = false));
                            this.Invoke(new Action(() => Clear_Info()));
                            b_DoWork = false;
                        }
                        if (b_Cancel)
                        {
                            b_DoWork = false; FileNum = i_TotalFiles - 1; this.Invoke(new Action(() => this.Text = def));
                        }
                    });
                    /// ----------------- end Asynchronous task --------------------------------
                    if (b_Cancel) goto Endall;
                }
            Endall:
                if (!b_DoWork)
                {
                    this.Text = def;
                    Stream?.Close();
                    b_DoWork = true;
                    Start_Working(false);
                    b_msDCHK = false;
                    Clear_Info();
                    Options();
                }

                void Overwrite_Prompt(string f)
                {
                    if (!b_OverwriteChecked)
                    {
                        string mes, mes2; var e = 0;
                        if (i_TotalFiles > 1) { mes = "s"; mes2 = null; }
                        else { mes = null; mes2 = "s"; }
                        for (int w = 0; w < s_OutFile.Length; w++)
                        {
                            if (File.Exists(s_OutFile[w])) e++;
                        }
                        b_OverwriteChecked = true;
                        if (e > 0)
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                            DialogResult result = MessageBox.Show($"overwrite file{mes}?", ($"{e} Destination file{mes} already exist{mes2}!")
                                , buttons, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes) b_Yclick = true;
                            if (result == DialogResult.No) b_Yclick = false;
                            if (result == DialogResult.Cancel)
                            {
                                Stream?.Close();
                                b_Overwrite = b_DoWork = false;
                                b_Cancel = true;
                                this.Text = def;
                                FileNum = i_TotalFiles - 1;
                            }
                        }
                    }
                    if (!b_Yclick)
                    {
                        if (File.Exists(s_OutFile[FileNum]))
                        {
                            Stream?.Close();
                            b_DoWork = (FileNum != i_TotalFiles - 1);
                            b_Overwrite = false;
                            l_tot += l_FileSize[FileNum];
                        }
                        else b_Overwrite = true;
                    }
                    if (b_Yclick)
                    {
                        try { File.Delete(f); } catch { }
                        b_Overwrite = (!File.Exists(f));
                    }
                }
                //----------------------- end of main file process void -- sub voids follow --------------------------
                void Set_File_Hidden(string f, string a)
                {
                    if (a == "h") try { File.SetAttributes(f, File.GetAttributes(f) | FileAttributes.Hidden); } catch { } // hide file
                    if (a == "u") try { File.SetAttributes(f, File.GetAttributes(f) & ~FileAttributes.Hidden); } catch { } // unhide file
                }
                void Progress_Update()
                {
                    double prog = 100.0 * TotalLength / l_tot;
                    PBar.Value = (int)prog;
                    PBar.Update();
                    this.Text = def + $" ({prog:f1}%)";
                    if (i_TotalFiles > 1)
                    {
                        var c = CountDigits(i_TotalFiles - FileNum);
                        Fname.Text = $"(File {s_NewFile.Length - FileNum}) {Trunc(s_NewFile[FileNum], 42 - c)}";
                    }
                    Fsize.Text = $"{(l_tot - TotalLength) >> 10:N0} kb remaining.";
                    if (b_MultiThread) thd.Text = $"(Multi-Threaded x{i_Cores + 1})"; else thd.Text = null;
                }
                void W_Update()
                {
                    Pass.Update();
                    PassC.Update();
                }
                byte[] Read_Data(long offset, int len)
                {
                    var dat = new byte[len];
                    Stream.Seek(offset, SeekOrigin.Begin);
                    Stream.Read(dat, 0, len);
                    return dat;
                }
            }
        }
        static int CountDigits(long MyNum)
        {
            int count = 0;
            while (MyNum != 0)
            {
                MyNum /= 10;
                count++;
            }
            return count;
        }
        /// ------------------------------------ Actual code for Encrytion process --------------------------------------- ///
        void Encrypt(byte[] DataIn, byte[] skey, byte[] rkey, byte[] xor, long length, long r1, int o)
        {
            bt_Output[o] = XOR(Shift(XOR(DataIn, xor), r1), xor);

            byte[] Shift(byte[] dat, long step)
            {
                int r = 0; if (b_Reverse) r = 1;
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
            if (b_LC)
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
            if (b_Reverse)
            { PassC.Visible = label2.Visible = false; s_PasswdConf = s_Password; }
            else PassC.Visible = label2.Visible = true;
            if (!b_Reverse) s_PasswdConf = PassC.Text;
            bool m = s_PasswdConf.Equals(s_Password);
            Show_PWD();
            Disable_Passwd();
            if (s_NewFile.Length == 0)
                FN.Visible = SZ.Visible = PBar.Visible = Fname.Visible = Fsize.Visible = false;
            else
            {
                FN.Visible = SZ.Visible = Fsize.Visible = msClear.Visible = true;
                if (l_FileSize.Length > 0 && l_FileSize[i_TotalFiles - 1] > 0) Enable_Passwd();
            }
            if (!b_Working)
            {
                if (s_PasswdConf.Length > 0)
                {
                    Match.Visible = (!b_Reverse);
                    if (m)
                    {
                        if (l_FileSize.Length > 0 && l_FileSize[i_TotalFiles - 1] > 0) msStart.Enabled = true;
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
            if (s_DropFiles[0] == "")
            {
                if (b_Reverse && msDec.Checked) this.openFileDialog1.Filter = "Crypt-It Files (*.crypt)|*.crypt|All files (*.*)|*.*";
                else this.openFileDialog1.Filter = "All files (*.*)|*.*|Crypt-It Files (*.crypt)|*.crypt";
                openFileDialog1.FileName = "";
                openFileDialog1.Title = "Open file to encrypt";
                openFileDialog1.ShowDialog();
                if (openFileDialog1.FileName.Length > 0)
                {
                    s_DropFiles = openFileDialog1.FileNames;
                    Sort();
                }
                else Clear_Info();
                if (s_NewFile.Length == 0) Clear_Info();
            }
            if (s_DropFiles.Length >= 1 && s_DropFiles[0] != "no files") Sort();
            else Clear_Info();
            Options();
            this.ActiveControl = Pass;
            void Sort()
            {
                if (s_DropFiles[0] != "" && s_DropFiles[0] != "no files") l = s_DropFiles.Length;
                else l = 0;
                int x = 0; l_tot = 0;
                string[] temp = new string[l];
                long[] temp2 = new long[l];
                bool b_CryptFile = true;
                for (int i = 0; i < l; i++)
                {
                    var s = new System.IO.FileInfo(s_DropFiles[i]).Length;
                    if (s > 0) { temp[x] = s_DropFiles[i]; temp2[x] = s; x++; }
                }
                if (x > 0)
                {
                    l_FileSize = new long[x];
                    s_NewFile = new string[x];
                    s_OutFile = new string[x];
                    i_TotalFiles = x;
                    for (int i = 0; i < x; i++)
                    {
                        s_NewFile[i] = temp[i];
                        l_tot += temp2[i];
                        l_FileSize[i] = temp2[i];
                        if (Path.GetExtension(s_NewFile[i]) != ".crypt") { s_OutFile[i] = $"{s_NewFile[i]}.crypt"; b_CryptFile = false; }
                        else s_OutFile[i] = $@"{Path.GetDirectoryName(s_NewFile[i])}\{Path.GetFileNameWithoutExtension(s_NewFile[i])}";
                    }
                    if (s_NewFile.Length == 1) { Fname.Text = Trunc(s_NewFile[0], 52); Fname.Visible = true; }
                    if (s_NewFile.Length > 1) { Fname.Text = $"Batch file process ({s_NewFile.Length}) files."; Fname.Visible = true; }
                    if (s_NewFile.Length >= 1) { Fsize.Text = $"{l_tot >> 10:N0}  kb"; Fsize.ForeColor = Color.Silver; }
                    else { Fname.Visible = Fsize.Visible = false; }
                    b_Reverse = msDec.Checked = (s_NewFile.Length == 1 && Path.GetExtension(s_NewFile[0]) == ".crypt");
                }
                else
                {
                    l_FileSize = new long[x];
                    s_NewFile = new string[x];
                    s_OutFile = new string[x];
                    i_TotalFiles = x;
                }
                if (b_CryptFile && l_tot > 0) b_Reverse = msDec.Checked = true;
                else b_Reverse = msDec.Checked = false;
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
            if (!b_Working) Options();
            else Show_PWD();
            i_B++;
            if (!b_LC)
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
            b_Cancel = true;
        }
        private void Dec_CheckedChanged(object sender, EventArgs e)
        {
            b_Reverse = !b_Reverse;
            Options();
        }
        private static string Trunc(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : $"{Path.GetPathRoot(value)}...{value.Substring(value.Length - (maxChars), maxChars)}";
        }
        void Crypt_It_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Crypt_It_DragDrop(object sender, DragEventArgs e)
        {
            if (!b_Working)
            {
                int x = 0;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] Ffiles = new string[0];
                var DFiles = new string[files.Length];
                foreach (string file in files)
                {
                    if (!Directory.Exists(file))
                    {
                        DFiles[x] = file;
                        x++;
                    }
                    else Ffiles = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
                    var FolderFiles = new string[Ffiles.Length];
                    s_DropFiles = new string[x + Ffiles.Length];
                    for (int i = 0; i < x; i++)
                    {
                        s_DropFiles[i] = DFiles[i];
                    }
                    for (int i = 0; i < Ffiles.Length; i++)
                    {
                        s_DropFiles[i + x] = Ffiles[i];
                    }
                }
                if (s_DropFiles.Length == 0)
                {
                    s_DropFiles = new string[1];
                    s_DropFiles[0] = "no files";
                }
                Filter_Files();
            }
        }
        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (b_Working) b_Cancel = true;
            else Process_File_List();
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                Clear_Info();
                this.openFileDialog1.Multiselect = false;
                Filter_Files();
            }
        }
        private void BatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                Clear_Info();
                this.openFileDialog1.Multiselect = true;
                Filter_Files();
            }
        }
        private void DeleteSouceFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                if (b_DelSource) b_DelSource = msDelFile.Checked = false;
                else b_DelSource = msDelFile.Checked = true;
            }
        }
        private void UseTestfileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                if (b_TestFile) b_TestFile = msTestFile.Checked = false;
                else b_TestFile = msTestFile.Checked = true;
            }
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                if (b_Timer) b_Timer = msTimer.Checked = false;
                else b_Timer = msTimer.Checked = true;
            }
        }
        private void MsDec_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                if (b_Reverse) b_Reverse = msDec.Checked = b_msDCHK = false;
                else b_Reverse = msDec.Checked = b_msDCHK = true;
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
            this.ControlBox = fileToolStripMenuItem.Enabled = optionsToolStripMenuItem.Enabled =
                Pass.Enabled = PassC.Enabled = msClear.Enabled = CheckBox1.Enabled = b_LC = true;
            msSorry.Visible = false;
            i_B = 0;
            this.Text = $"Annoyed-{Program} {Version}";
            Options();
        }
        private void MsThreadAuto_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                msThreadAuto.Checked = true;
                b_Set_Cores = msThreadMan.Checked = false;
            }
        }

        private void SetManuallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!b_Working)
            {
                msThreadAuto.Checked = false;
                b_Set_Cores = msThreadMan.Checked = true;
                Set_Cores();
                void Set_Cores()
                {
                    (i_CoreVal, b_Thread_Cancel) = Prompt.ShowDialog("(1-255)", "Set Thread Count", i_CoreVal);
                    if (b_Thread_Cancel) { i_CoreVal = def; msThreadAuto.Checked = true; b_Thread_Cancel = msThreadMan.Checked = b_Set_Cores = false; }
                }
            }
        }
        private async void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (b_Working)
            {
                b_Cancel = true;
                while (b_Cancel == true) await Task.Delay(25);
            }
            this.Close();

        }
        private void Crypt_It_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }
        private void Crypt_It_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.J)
            {
                openToolStripMenuItem.PerformClick();
            }
        }

    }
}
