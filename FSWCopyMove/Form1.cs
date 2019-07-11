using FSWCopyMove.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSWCopyMove
{
    public partial class Form1 : Form
    {
        public ApplicationDbContext AppContext = null;
        public Form1()
        {
            AppContext = new ApplicationDbContext();
            InitializeComponent();
        }

        private void InitCheckListBox()
        {
            // Initialise % database.
            foreach (var item in AppContext.PrmVal.ToList())
            {
                // 
                //Console.WriteLine(item);
                _AddDirectory(item.Val);

                CheckListBoxButton clbb = new CheckListBoxButton();
                clbb.checkBox1.Text = "Test";
                listBox1.Items.Add(clbb);
            }
        }

        private void _AddDirectory(String path)
        {
            // Ajout dans l'interface
            this.checkedListBox1.Items.Add(path, true);
            FSWExtended fsw = new FSWExtended();
            fsw.Path = path;
            fsw.WriteMsg += WriteLog;
            fileSystemWatchers.Add(fsw);
            fsw.EnableRaisingEvents = true;
        }

        private void AddDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _AddDirectory(fbd.SelectedPath);
                }
            }
        }

        List<FSWExtended> fileSystemWatchers = new List<FSWExtended>();
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                WriteLog(e.Index.ToString() + " " +e.CurrentValue + " => " + e.NewValue);
            }));
        }

        const int TRUE = 0;
        const int FALSE = 1;
        static int _firstTime = TRUE;
        private static Boolean FirstTime
        {
            get
            {
                return Interlocked.CompareExchange(ref _firstTime, FALSE, TRUE) == TRUE;
            }
        }
        private void WriteLog(string pFmt, params object[] args)
        {
            
            textBox1.Invoke((MethodInvoker)delegate ()
            {
                textBox1.AppendText((FirstTime ? "" : Environment.NewLine) + String.Format(pFmt, args));
                textBox1.ScrollToCaret();
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitCheckListBox();
        }

        private void textBox1_SizeChanged(object sender, EventArgs e)
        {
        }
    }
}
