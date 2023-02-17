using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/// THINGS TO DO!!
///
///  (done)  Fix the password generation to ensure no two different passwords yield the same seed
///  (done)  Fix decrypt button checks
///  (done)  Add threading to the encryption process for extra speed
///  (done)  Add ability to process existing file instead of creating a new file (sort of.. Creates new file, encrypts, deletes source)
///  (done)  Add batch file processing
///          Add an options menu to toggle single/batch mode, keep/delete source when done and maybe some other options
//           // single/batch mode currently is automatic.  Might just leave it that way
///          Possibly add more steps to the encryption process
///          Try to make decryption process the same as encryption without reversing it

/// 🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊 Fancy if/else statement 🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
//            //  if c = 7 then fox = 6, c = 9 then fox = 9 else fox = 8
//            fox = c == 7? c == 9 ? 6 : 9 : 8;

namespace Crypt_It
{
    public partial class Crypt_It : Form
    {
        readonly string Program = "Crypt-It";
        readonly string Version = "v0.6.1";
        /// These setting for debugging purposes
        readonly bool b_Set_Cores = false; // override automatic core detection for threading
        readonly int i_CoreVal = 4; // set number of cpu cores to use for threading
        readonly bool b_Timer = false; // set to true to show time it took to complete the encrypt/decrypt process 
        readonly bool b_TestFile = false; // set to true to set the output file to C:\testfile.(bin/crypt)
        bool b_Reverse = false; // don't mess with this (just use the "decrypt" button)
        /// ------- Personal Preference --------
        readonly bool b_Hide = false;  // set to true to hide output file(s) during encrypt/decrypt process
        readonly bool b_DelSource = false;    // WARNING!! Set to true if you want the source file(s) deleted after encrypt/decrypt process
        /// -------- program variables ---------
        bool b_LC = false;
        bool b_Working = false;
        bool b_Cancel = false;
        bool b_MultiThread = false;
        bool b_Overwrite = false;
        bool b_Nclick = false;
        bool b_Yclick = false;
        bool b_OverwriteChecked = false;
        string[] s_NewFile = new string[0];
        string[] s_OutFile = new string[0];
        string s_Password;
        string s_PasswdConf;
        byte[][] bt_Output = new byte[0][];
        int i_B = 0;  // Don't click the "show text" button too many times. (You've been warned!)
        int i_TotalFiles;
        long[] l_FileSize = new long[0];
        long l_tot;

        public Crypt_It()
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        // ------------------- Program start here -----------------------------
        {
            InitializeComponent();
            this.Text = $"{Program} {Version}";
            Options();
        }
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        void Start_Working(bool a)
        {
            OpenFile.Enabled = Pass.Enabled = PassC.Enabled = Start.Enabled = Dec.Visible = !a;
            PBar.Visible = b_Working = Cancel.Visible = a;
        }
        private static string Trunc(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : $"{Path.GetPathRoot(value)}...{value.Substring(value.Length - (maxChars), maxChars)}";
        }

        void Clear_Info()
        {
            s_NewFile = new string[0];
            l_FileSize = new long[0];
            s_Password = s_PasswdConf = Pass.Text = PassC.Text = "";
            b_Reverse = b_Cancel = Dec.Checked = b_Overwrite = b_OverwriteChecked = b_Nclick = b_Yclick = false;
        }
        async void Process_File()
        {
            var def = this.Text;
            long TotalLength = 0;
            // ---- Make encryption keys from password ------------------------------------------------- //
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
            byte[] lKey = new byte[KeyLen];
            byte[] bKey = new byte[8];  // vaule MUST be at least 8, maybe 7.  Higher than 8 is pointless
            byte[] xorKey = new byte[KeyLen];
            for (int i = 0; i < pwd.Length; i++)
            {
                seed += (pwd[i]);
            }
            Random rand = new Random(seed);
            for (int i = 0; i < KeyLen; i++)
            {
                lKey[i] = (byte)rand.Next(1, 64); // generates 128 (kLen = 128) variables for bit-shifting Ulong's (64-bit integers)
                if (i < 8) bKey[i] = (byte)rand.Next(1, 8);
                xorKey[i] = (byte)rand.Next(255);   // generates 1024-bit (kLen = 128) XOR key (128 bytes long)
            }
            // ------ END make encryption keys ------------- START  break file into segments ----------- //
            PBar.Value = 0; // set progress bar to 0%
            Start_Working(true);
            for (int FileNum = 0; FileNum < i_TotalFiles; FileNum++)
            {
                FileStream Stream = new FileStream(s_NewFile[FileNum], FileMode.Open, FileAccess.Read);
                bool b_DoWork = true;
                b_Overwrite = (!b_Nclick | !b_Yclick);
                b_Reverse = (Path.GetExtension(s_NewFile[FileNum]) == ".crypt");
                var Chunk_Length = 131072 << 3; // change chunk size.  (*8 = 8mb chunks) 
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
                        this.Invoke(new Action(() => W_Update()));
                        long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        var Cores = Environment.ProcessorCount - 1;
                        Cores = (b_Set_Cores && i_CoreVal > 0) ? (!b_Set_Cores && Cores > 5) ? 0 : i_CoreVal - 1 : Cores /= 2;
                        // -----------------------------------------------------------------------------
                        for (long step = 0; step < 3; step++)
                        {
                            var i = ByteSegment[step];
                            if (step > 0)
                            {
                                length = (int)ByteSegment[step]; i = 1; Cores = 0;
                            }
                            long offset = 0;
                            Thread[] enc = new Thread[Cores + 1];
                            for (long j = 0; j < i; j++)
                            {
                                if (b_Cancel) goto Loopend;
                                switch (step)
                                {
                                    case 0: offset = (long)(j * length); break;
                                    case 1: offset = (long)ByteSegment[0] * (Chunk_Length << 3); break;
                                    case 2: offset = (long)ByteSegment[0] * (Chunk_Length << 3) + (long)ByteSegment[1]; break;
                                }
                                b_MultiThread = (i - j >= enc.Length && Cores > 0);
                                if (i - j < enc.Length && Cores > 0) { Cores = (int)(i - j) - 1; b_MultiThread = true; }
                                byte[][] chunk = bt_Output = new byte[Cores + 1][];
                                for (int x = 0; x <= Cores; x++) chunk[x] = Read_Data(offset + (x * length), length);
                                j += Cores; TotalLength += length * (Cores + 1);
                                // --------- Create and start new worker threads ----------------------
                                for (int x = 0; x <= Cores; x++)
                                {
                                    var xx = x;
                                    enc[x] = new Thread(() => { Encrypt(chunk[xx], lKey, bKey, xorKey, length, step, xx); });
                                    enc[x].Start();
                                }
                                // --- wait for worker threads to complete before continuing --------
                                for (int x = 0; x <= Cores; x++) enc[x].Join();
                                // --- write finished workload to file -------------------------------
                                for (int x = 0; x <= Cores; x++) Dest.Write(bt_Output[x], 0, bt_Output[x].Length);
                                // -------------------------------------------------------------------
                                this.Invoke(new Action(() => Progress_Update(Cores)));
                            }
                        }
                    Loopend:
                        Stream.Close();
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
                        if (Cores == 0) this.Invoke(new Action(() => thd.Text = ""));
                        if (b_Timer)
                        {
                            long msf = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            this.Invoke(new Action(() => this.Text = (msf - ms).ToString()));
                        }
                        l_FileSize[FileNum] = 0;
                        if (!b_Cancel && b_Hide) Set_File_Hidden(s_OutFile[FileNum], "u");
                        if (!b_Cancel && b_DelSource && b_Overwrite)
                        {
                            try
                            {
                                if (File.Exists(s_NewFile[FileNum])) File.Delete(s_NewFile[FileNum]);
                            }
                            catch { }
                        }
                        if (FileNum == i_TotalFiles - 1)
                        {
                            this.Invoke(new Action(() => Start_Working(false)));
                            this.Invoke(new Action(() => Clear_Info()));
                            b_DoWork = false;
                        }
                        if (b_Cancel)
                        {
                            b_DoWork = false; FileNum = i_TotalFiles; this.Invoke(new Action(() => this.Text = def));
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
                    Clear_Info();
                    b_DoWork = true;
                    Start_Working(false);
                }
                Options();

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
                            b_Yclick = (result == DialogResult.Yes);
                            b_Nclick = (result == DialogResult.No);
                            if (result == DialogResult.Cancel)
                            {
                                Stream?.Close();
                                b_Overwrite = false;
                                b_Cancel = b_DoWork = true;
                                this.Text = def;
                                FileNum = i_TotalFiles - 1;
                            }
                        }
                    }
                    if (b_Nclick)
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
                void Progress_Update(int C)
                {
                    double prog = 100.0 * TotalLength / l_tot;
                    PBar.Value = (int)prog;
                    this.Text = def + $" ({prog:f1}%)";
                    if (i_TotalFiles > 1)
                    {
                        Fname.Text = $"(File {s_NewFile.Length - FileNum}) {Trunc(s_NewFile[FileNum], 42)}";
                    }
                    PBar.Update();
                    Fsize.Text = $"{(l_tot - TotalLength) >> 10:N0} kb remaining.";
                    if (b_MultiThread) thd.Text = $"(Multi-Threaded x{C + 1})"; else thd.Text = null;
                }
                void W_Update()
                {
                    OpenFile.Update();
                    Pass.Update();
                    PassC.Update();
                    Cancel.Update();
                    Start.Update();
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
        /// ------------------------------------ Actual code for Encrytion process --------------------------------------- ///
        void Encrypt(byte[] DataIn, byte[] skey, byte[] rkey, byte[] xor, long length, long r1, int o)
        {
            bt_Output[o] = XOR(Shift(XOR(DataIn, xor), r1), xor);
            byte[] Shift(byte[] dat, long step)
            {
                int r = 0; if (b_Reverse) r = 1;
                int h = 0;
                ulong g;
                var buffer = new MemoryStream();
                var write = new BinaryWriter(buffer);
                if (step < 2)
                {
                    for (int x = 0; x < (length >> 3); x++) // code to bitshift unsigned long's (64-bit)
                    {
                        g = (x % 2 == r) ? RotateRight(BitConverter.ToUInt64(dat, x << 3), skey[h]) :
                            RotateLeft(BitConverter.ToUInt64(dat, x << 3), skey[h]);
                        h = (h < skey.Length - 1) ? h + 1 : 0;
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
            Start.Enabled = Cancel.Enabled = OpenFile.Enabled = Pass.Enabled =
                PassC.Enabled = CheckBox1.Enabled = Dec.Visible = this.ControlBox = false;
            if (b_LC)
            {
                Sorry.Visible = false;
                this.Text = "Program LOCKED. You're done!";
                while (1 == 1) Console.Beep(4000, 1000);
            }
            else
            {
                this.Text = "Do you deserver another chance?";
                Sorry.Visible = true;
            }
        }
        void Options()
        {
            if (b_Working)
            { Cancel.Visible = true; Dec.Enabled = false; }
            else { Cancel.Visible = false; Dec.Enabled = true; }
            Match.Visible = Start.Enabled = Sorry.Visible = false;
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
                FN.Visible = SZ.Visible = Fsize.Visible = true;
                if (l_FileSize.Length > 0 && l_FileSize[i_TotalFiles - 1] > 0) Enable_Passwd();
            }
            if (s_PasswdConf.Length > 0)
            {
                Match.Visible = (!b_Reverse);
                if (m)
                {
                    if (l_FileSize.Length > 0 && l_FileSize[i_TotalFiles - 1] > 0) Start.Enabled = true;
                    Match.Text = "Match";
                    Match.ForeColor = Color.Silver;
                }
                else
                {
                    Match.ForeColor = Color.Red;
                    Match.Text = "No Match";
                    Start.Enabled = false;
                }
            }
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
        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (Dec.Checked) this.openFileDialog1.Filter = "Crypt-It Files (*.crypt)|*.crypt|All files (*.*)|*.*";
            else this.openFileDialog1.Filter = "All files (*.*)|*.*|Crypt-It Files (*.crypt)|*.crypt";
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open file to encrypt";
            this.openFileDialog1.Multiselect = true;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0) Sort(); else Clear_Info();
            if (s_NewFile.Length == 0) Clear_Info();
            Options();
            this.ActiveControl = Pass;
            void Sort()
            {
                var l = openFileDialog1.FileNames.Length;
                int x = 0; l_tot = 0;
                string[] temp = new string[l];
                long[] temp2 = new long[l];
                bool b_CryptFile = true;
                for (int i = 0; i < l; i++)
                {
                    var s = new System.IO.FileInfo(openFileDialog1.FileNames[i]).Length;
                    if (s > 0) { temp[x] = openFileDialog1.FileNames[i]; temp2[x] = s; x++; }
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
                    b_Reverse = Dec.Checked = (s_NewFile.Length == 1 && Path.GetExtension(s_NewFile[0]) == ".crypt");
                }
                else
                {
                    l_FileSize = new long[x];
                    s_NewFile = new string[x];
                    s_OutFile = new string[x];
                    i_TotalFiles = x;
                }
                b_Reverse = Dec.Checked = (b_CryptFile);
            }
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
            Process_File();
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            b_Cancel = true;
        }
        private void Sorry_Click(object sender, EventArgs e)
        {
            this.ControlBox = Start.Enabled = Cancel.Enabled = OpenFile.Enabled =
                Pass.Enabled = PassC.Enabled = CheckBox1.Enabled = Dec.Visible = b_LC = true;
            Sorry.Visible = false;
            i_B = 0;
            this.Text = $"Annoyed-{Program} {Version} (Last Chance)";
            Options();
        }
        private void Dec_CheckedChanged(object sender, EventArgs e)
        {
            b_Reverse = !b_Reverse;
            Options();
        }
    }
}
