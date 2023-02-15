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
    public partial class Form1 : Form
    {
        /// These setting for debugging purposes
        readonly bool set_cores = false; // override automatic core detection for threading
        readonly int core_val = 4; // set number of cpu cores to use for threading
        readonly bool timer = false; // set to true to show time it took to complete the encrypt/decrypt process 
        readonly bool TestFile = false; // set to true to set the output file to C:\testfile.(bin/crypt)
        bool Reverse = false; // don't mess with this (just use the "decrypt" button)
        /// ------- Personal Preference --------
        readonly bool hide = false;  // set to true to hide output file(s) during encrypt/decrypt process
        readonly bool del_source = false;    // WARNING!! Set to true if you want the source file(s) deleted after encrypt/decrypt process
        /// -------- program variables ---------
        bool lc = false;
        bool working = false;
        bool cancel = false;
        bool Multi_Thread = false;
        bool all = false;
        bool overwrite = false;
        bool nclick = false;
        bool yclick = false;
        string[] NewFile = new string[0];
        string[] OutFile = new string[0];
        string Password;
        string paswdc;
        byte[][] output = new byte[0][];
        int b = 0;  // Don't click the "show text" button too many times. (You've been warned!)
        int TotalFiles;
        long[] FileSize = new long[0];
        long tot;

        public Form1()
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        // ------------------- Program start here -----------------------------
        {
            InitializeComponent();
            Options();
        }
        //🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊🦊
        void Start_Working(bool a)
        {
            OpenFile.Enabled = Pass.Enabled = PassC.Enabled = Start.Enabled = Dec.Visible = !a;
            PBar.Visible = working = Cancel.Visible = a;
        }
        private static string Trunc(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : Path.GetPathRoot(value) + "..." + value.Substring(value.Length - (maxChars), maxChars);
        }

        void Clear_Info()
        {
            NewFile = new string[0];
            FileSize = new long[0];
            Password = paswdc = Pass.Text = PassC.Text = "";
            Reverse = cancel = Dec.Checked = all = overwrite = nclick = yclick = false;
        }
        async void Process_File()
        {
            var def = this.Text;
            long TotalLength = 0;
            // ---- Make encryption keys from password ------------------------------------------------- //
            byte[] pwd = Encoding.ASCII.GetBytes(Password);
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
                if (i < 8)
                {
                    bKey[i] = (byte)rand.Next(1, 8);
                }
                xorKey[i] = (byte)rand.Next(255);   // generates 1024-bit (kLen = 128) XOR key (128 bytes long)
            }
            // ------ END make encryption keys ------------- START  break file into segments ----------- //
            PBar.Value = 0; // set progress bar to 0%
            for (int FileNum = 0; FileNum < TotalFiles; FileNum++)
            {
                Start_Working(true);
                FileStream Stream = new FileStream(NewFile[FileNum], FileMode.Open, FileAccess.Read);
                bool Do_Work = overwrite = true;
                if (Path.GetExtension(NewFile[FileNum]) == ".crypt")
                {
                    Reverse = true;
                }
                else Reverse = false;
                var Chunk_Length = 131072 << 3; // change chunk size.  (*8 = 8mb chunks) 
                var uLongs = FileSize[FileNum] >> 3; // FileSize / 8;
                long[] ByteSegment = new long[3]
                {
                    uLongs / Chunk_Length, ((FileSize[FileNum] - (((uLongs / Chunk_Length) * Chunk_Length) << 3)) >> 3) << 3, FileSize[FileNum] - (uLongs << 3)
                };
                var length = Chunk_Length << 3;
                // ----- Condition Checks ---------------
                if (TestFile)
                {
                    if (Reverse)
                    {
                        OutFile[FileNum] = @"c:\testfile" + FileNum + ".bin";
                    }
                    else OutFile[FileNum] = @"c:\testfile" + FileNum + ".crypt";

                }
                Overwrite_Prompt(OutFile[FileNum]);
                if (FileNum == TotalFiles - 1)
                {
                    Do_Work = false;
                }
                // ------ end condition checks ------------
                if (overwrite && !cancel)
                {
                    if (FileNum == (TotalFiles - 1) && !overwrite)
                    {
                        goto Endall;
                    }
                    FileStream Dest = new FileStream(OutFile[FileNum], FileMode.Append);
                    if (hide)
                    {
                        Set_File_Hidden(OutFile[FileNum], "h");
                    }
                    /// ---------- start Asynchronous task -----------------------------------
                    await Task.Run(delegate
                    {
                        this.Invoke(new Action(() => OpenFile.Update()));
                        this.Invoke(new Action(() => Pass.Update()));
                        this.Invoke(new Action(() => PassC.Update()));
                        this.Invoke(new Action(() => Cancel.Update()));
                        this.Invoke(new Action(() => Start.Update()));
                        long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        var Cores = Environment.ProcessorCount - 1;
                        // -- Check to see how many threads to use -------------------------------------
                        if (set_cores) Cores = core_val - 1; // Manually set # of cores (-1) for testing
                        if (!set_cores && Cores > 5) Cores /= 2;
                        if (Cores < 0) Cores = 0; // this line shouldn't be needed, but just in case
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
                                if (cancel) goto Loopend;
                                switch (step)
                                {
                                    case 0: offset = (long)(j * length); break;
                                    case 1: offset = (long)ByteSegment[0] * (Chunk_Length << 3); break;
                                    case 2: offset = (long)ByteSegment[0] * (Chunk_Length << 3) + (long)ByteSegment[1]; break;
                                }
                                if (i - j >= enc.Length && Cores > 0) Multi_Thread = true;
                                else Multi_Thread = false; 
                                if (i - j < enc.Length && Cores > 0)
                                {
                                    var c = Cores;
                                    for (int x = 0; x < c; x++)
                                    {
                                        Cores -= 1;
                                        if (i - j > Cores) { Multi_Thread = true; break; }
                                    }
                                }
                                byte[][] chunk = output = new byte[Cores + 1][];
                                for (int x = 0; x <= Cores; x++)
                                {
                                    chunk[x] = Read_Data(offset + (x * length), length);
                                    TotalLength += length;
                                }
                                j += Cores;
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
                                for (int x = 0; x <= Cores; x++) Dest.Write(output[x], 0, output[x].Length);
                                // -------------------------------------------------------------------
                                double prog = 100.0 * TotalLength / tot;
                                this.Invoke(new Action(() => PBar.Value = (int)prog));
                                this.Invoke(new Action(() => this.Text = def + " (" + prog.ToString("f1") + "%)"));
                                if (TotalFiles > 1)
                                {
                                    this.Invoke(new Action(() => Fname.Text = "(File " + (NewFile.Length - FileNum).ToString()
                                    + ")  " + Trunc(NewFile[FileNum], 42)));
                                }
                                this.Invoke(new Action(() => PBar.Update()));
                                this.Invoke(new Action(() => Fsize.Text = ((tot - TotalLength) / 1024).ToString("N0") + " kb remaining."));
                                if (Multi_Thread)
                                {
                                    this.Invoke(new Action(() => thd.Text = "(Multi-Threaded x" + (Cores + 1).ToString() + ")"));
                                }
                                else this.Invoke(new Action(() => thd.Text = null));
                            }
                        }
                    Loopend:
                        Stream.Close();
                        Dest.Close();
                        if (cancel)
                        {
                            try
                            {
                                if (hide)
                                {
                                    Set_File_Hidden(OutFile[FileNum], "u");
                                }
                                if (File.Exists(OutFile[FileNum]))
                                {
                                    File.Delete(OutFile[FileNum]);
                                }
                            }
                            catch { }
                        }
                        this.Invoke(new Action(() => thd.Text = ""));
                        if (timer)
                        {
                            long msf = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            this.Invoke(new Action(() => this.Text = (msf - ms).ToString()));
                        }
                        FileSize[FileNum] = 0;
                        if (!cancel && hide)
                        {
                            Set_File_Hidden(OutFile[FileNum], "u");
                        }
                        if (!cancel && del_source && overwrite)
                        {
                            try
                            {
                                if (File.Exists(NewFile[FileNum])) File.Delete(NewFile[FileNum]);
                            }
                            catch { }
                        }
                        if (FileNum == TotalFiles - 1)
                        {
                            this.Invoke(new Action(() => Start_Working(false)));
                            this.Invoke(new Action(() => Clear_Info()));
                            Do_Work = false;
                        }
                        if (cancel)
                        {
                            Do_Work = false; FileNum = TotalFiles; this.Invoke(new Action(() => this.Text = def));
                        }
                    });
                    /// ----------------- end Asynchronous task --------------------------------
                    if (cancel) goto Endall;
                }
            Endall:
                if (!Do_Work)
                {
                    this.Text = def;
                    Stream?.Close();
                    Clear_Info();
                    Do_Work = true;
                }

                Start_Working(false);
                Options();

                void Overwrite_Prompt(string f)
                {
                    if (!all)
                    {
                        string mes, mes2; var e = 0;
                        if (TotalFiles > 1) { mes = "s"; mes2 = null; }
                        else { mes = null; mes2 = "s"; }
                        for (int w = 0; w < OutFile.Length; w++)
                        {
                            if (File.Exists(OutFile[w])) e++;
                        }
                        if (e > 0)
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                            DialogResult result = MessageBox.Show("overwrite file" + mes + "?", (e.ToString() + " Destination file" + mes +
                                " already exist" + mes2 + "!"), buttons, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes)
                            {
                                {
                                    yclick = true;
                                }
                            }
                            if (result == DialogResult.No)
                            {
                                nclick = true;
                            }
                            if (result == DialogResult.Cancel)
                            {
                                Stream?.Close();
                                overwrite = false;
                                all = cancel = Do_Work = true;
                                this.Text = def;
                            }
                        }
                        all = true;
                    }
                    if (all && nclick)
                    {
                        if (File.Exists(OutFile[FileNum]))
                        {
                            Stream?.Close();
                            if (FileNum == TotalFiles - 1) Do_Work = false;
                            overwrite = false;
                            tot += FileSize[FileNum];
                        }
                        else overwrite = true;
                    }
                    if (all && yclick)
                    {
                        try { File.Delete(f); } catch { }
                        if (!File.Exists(f))
                        overwrite = true;
                    }
                }
                //----------------------- end of main file process void -- sub voids follow --------------------------
                void Set_File_Hidden(string f, string a)
                {
                    if (a == "h") try { File.SetAttributes(f, File.GetAttributes(f) | FileAttributes.Hidden); } catch { } // hide file
                    if (a == "u") try { File.SetAttributes(f, File.GetAttributes(f) & ~FileAttributes.Hidden); } catch { } // unhide file
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
        void Encrypt(byte[] DataIn, byte[] skey, byte[] rkey, byte[] xor, long length, long r1, int o)
        {
            output[o] = XOR(Shift(XOR(DataIn, xor), r1), xor);
            byte[] Shift(byte[] dat, long step)
            {
                int r = 0; if (Reverse) r = 1;
                int h = 0;
                ulong g;
                var buffer = new MemoryStream();
                var write = new BinaryWriter(buffer);
                if (step < 2)
                {
                    for (int x = 0; x < (length >> 3); x++) // code to bitshift unsigned long's (64-bit)
                    {
                        if (x % 2 == r)
                        {
                            g = RotateRight(BitConverter.ToUInt64(dat, x << 3), skey[h]); // << 3 = * 8
                        }
                        else
                        {
                            g = RotateLeft(BitConverter.ToUInt64(dat, x << 3), skey[h]);
                        }
                        if (h < skey.Length - 1)
                        {
                            h++;
                        }
                        else h = 0;

                        write.Write(BitConverter.GetBytes(g));
                    }
                }
                else
                {
                    for (int x = 0; x < dat.Length; x++) // code to bitshift a single unsigned byte (8-bit)
                    {
                        if (x % 2 == r)
                        {
                            g = ((uint)dat[x] >> rkey[x]) | ((uint)dat[x] << (8 - rkey[x])); // bit rotate byte to the right
                        }
                        else g = ((uint)dat[x] << rkey[x]) | ((uint)dat[x] >> (8 - rkey[x])); // bit rotate byte to the left
                        write.Write((byte)(g));
                    }
                }
                buffer.Close();
                write.Close();
                return (buffer.ToArray());
            }
            byte[] XOR(byte[] Data, byte[] XorKey)
            {
                for (int t = 0; t < Data.Length; t++)
                {
                    Data[t] = (byte)(Data[t] ^ (XorKey[t % XorKey.Length])); // XOR data with xor key
                }
                return Data;
            }
            ulong RotateLeft(ulong a, int b)
            {
                return (a << b) | (a >> (64 - b));  // bitrotate Ulong to the left
            }
            ulong RotateRight(ulong a, int b)
            {
                return (a >> b) | (a << (64 - b)); // bitrotate Ulong to the right
            }
        }
        void LockProgram() // This was just for fun.  A little hidden joke.
        {
            Start.Enabled = Cancel.Enabled = OpenFile.Enabled = Pass.Enabled =
                PassC.Enabled = CheckBox1.Enabled = Dec.Visible = this.ControlBox = false;
            if (lc)
            {
                Sorry.Visible = false;
                this.Text = "Program LOCKED. You're done!";
                while (1 == 1)
                {
                    Console.Beep(4000, 1000);
                }
            }
            else
            {
                this.Text = "Do you deserver another chance?";
                Sorry.Visible = true;
            }
        }
        void Options()
        {
            if (working)
            {
                Cancel.Visible = true; Dec.Enabled = false;
            }
            else
            {
                Cancel.Visible = false; Dec.Enabled = true;
            }
            Match.Visible = Start.Enabled = Sorry.Visible = false;
            Password = Pass.Text;
            if (Reverse)
            {
                PassC.Visible = label2.Visible = false; paswdc = Password;
            }
            else
            {
                PassC.Visible = label2.Visible = true;
            }
            if (!Reverse)
            {
                paswdc = PassC.Text;
            }
            bool m = paswdc.Equals(Password);
            Show_PWD();
            Disable_Passwd();
            if (NewFile.Length == 0)
            {
                FN.Visible = SZ.Visible = PBar.Visible = Fname.Visible = Fsize.Visible = false;
            }
            else
            {
                FN.Visible = SZ.Visible = Fsize.Visible = true;
                if (FileSize.Length > 0 && FileSize[TotalFiles - 1] > 0)
                {
                    Enable_Passwd();
                }
            }
            if (paswdc.Length > 0)
            {
                if (!Reverse) Match.Visible = true;
                if (m)
                {
                    if (FileSize.Length > 0 && FileSize[TotalFiles - 1] > 0) Start.Enabled = true;
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
            if (!CheckBox1.Checked)
            {
                Pass.PasswordChar = PassC.PasswordChar = '\u25CF'; CheckBox1.Text = "Show Text";
            }
            else
            {
                Pass.PasswordChar = PassC.PasswordChar = '\0'; CheckBox1.Text = "Hide Text";
            }
        }
        private void OpenFile_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = true;
            if ((Dec.Checked == true) && (Reverse == true))
            {
                this.openFileDialog1.Filter = "Crypt-It Files (*.crypt)|*.crypt|All files (*.*)|*.*";
            }
            else this.openFileDialog1.Filter = "All files (*.*)|*.*|Crypt-It Files (*.crypt)|*.crypt";
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open file to encrypt";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0) Sort();
            else Clear_Info();
            if (NewFile.Length == 0) Clear_Info();
            Options();
            this.ActiveControl = Pass;
            void Sort()
            {
                var l = openFileDialog1.FileNames.Length;
                int x = 0; tot = 0;
                string[] temp = new string[l];
                long[] temp2 = new long[l];
                bool crypt_file = true;
                for (int i = 0; i < l; i++)
                {
                    var s = new System.IO.FileInfo(openFileDialog1.FileNames[i]).Length;
                    if (s > 0)
                    {
                        temp[x] = openFileDialog1.FileNames[i]; temp2[x] = s; x++;
                    };
                }
                if (x > 0)
                {
                    FileSize = new long[x];
                    NewFile = new string[x];
                    OutFile = new string[x];
                    TotalFiles = x;
                    for (int i = 0; i < x; i++)
                    {
                        NewFile[i] = temp[i];
                        tot += temp2[i];
                        FileSize[i] = temp2[i];
                        if (Path.GetExtension(NewFile[i]) != ".crypt")
                        {
                            OutFile[i] = NewFile[i] + ".crypt"; crypt_file = false;
                        }
                        else OutFile[i] = Path.GetDirectoryName(NewFile[i]) + @"\" + Path.GetFileNameWithoutExtension(NewFile[i]);
                    }
                    if (NewFile.Length == 1) { Fname.Text = Trunc(NewFile[0], 52); Fname.Visible = true; }
                    if (NewFile.Length > 1) { Fname.Text = "Batch file process (" + NewFile.Length.ToString() + ") files."; Fname.Visible = true; }
                    if (NewFile.Length >= 1) { Fsize.Text = (tot / 1024).ToString("N0") + " kb"; Fsize.ForeColor = Color.Silver; }
                    else { Fname.Visible = Fsize.Visible = false; }
                    if (NewFile.Length == 1 && Path.GetExtension(NewFile[0]) == ".crypt") Reverse = Dec.Checked = true;
                    else { Reverse = Dec.Checked = false; }
                }
                else
                {
                    FileSize = new long[x];
                    NewFile = new string[x];
                    OutFile = new string[x];
                    TotalFiles = x;
                }
                if (crypt_file) Reverse = Dec.Checked = true;
                if (OutFile.Length < 1) Reverse = Dec.Checked = false;
            }
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!working)
            {
                Options();
            }
            else Show_PWD();
            b++;
            if (!lc)
            {
                switch (b)
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
                if (b == 30) this.Text = "Don't blow your last chance.";
                else if (b == 50) LockProgram();
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
            cancel = true;
        }
        private void Sorry_Click(object sender, EventArgs e)
        {
            this.ControlBox = Start.Enabled = Cancel.Enabled = OpenFile.Enabled =
                Pass.Enabled = PassC.Enabled = CheckBox1.Enabled = Dec.Visible = lc = true;
            Sorry.Visible = false;
            b = 0;
            this.Text = "Crypt-It  (Let's behave now)";
            Options();
        }
        private void Dec_CheckedChanged(object sender, EventArgs e)
        {
            Reverse = !Reverse;
            Options();
        }
    }
}
