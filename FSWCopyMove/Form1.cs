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
                //Console.WriteLine(item);
                _AddDirectory(item.Val);
            }
        }

        private void _AddDirectory(String path)
        {
            // Ajout dans l'interface
            bool directoryExist = Directory.Exists(path);
            //this.checkedListBox1.Items.Add(path, true);

            //DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
            dataGridView1.Rows.Add();
            int index = this.dataGridView1.Rows.Count-1;

            dataGridView1.Rows[index].Cells[0].Value = directoryExist;
            dataGridView1.Rows[index].Cells[1].Value = path;
            dataGridView1.Rows[index].Cells[2].Value = "Suppression";

            if (directoryExist)
            {
                FSWExtended fsw = new FSWExtended();
                fsw.Path = path;
                fsw.WriteMsg += WriteLog;
                fileSystemWatchers.Add(fsw);
                fsw.EnableRaisingEvents = true;
            }
            else
            {
                dataGridView1.Rows[index].ReadOnly = true;
                dataGridView1.Rows[index].DefaultCellStyle.ForeColor = Color.Red;

            }
        }

        private void AddDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Prm rep = AppContext.Prm.Single(p => p.Nom == "REPERTOIRE");
                    PrmVal val = AppContext.PrmVal.Where(p => p.Prm_Idt == rep.Idt).OrderByDescending(p => p.Seq_Num).First();
                    AppContext.PrmVal.Add(new Data.PrmVal { Prm_Idt = rep.Idt, Val = fbd.SelectedPath, Seq_Num = val.Seq_Num + 1 });
                    AppContext.SaveChanges();
                    _AddDirectory(fbd.SelectedPath);
                }
            }
        }

        List<FSWExtended> fileSystemWatchers = new List<FSWExtended>();
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
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
                try
                {
                    textBox1.AppendText((FirstTime ? "" : Environment.NewLine) + String.Format(pFmt, args));
                }
                catch(Exception exc)
                {
                    textBox1.AppendText((FirstTime ? "" : Environment.NewLine) + exc.Message + " : " + pFmt);
                }
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Columns[e.ColumnIndex].Name == "Column1")
            //if (e.ColumnIndex == 0)
            {
                bool isChecked = (bool)dataGridView1[e.ColumnIndex, e.RowIndex].EditedFormattedValue;
                String txt = "Checked to Unchecked";
                if (isChecked == true)
                {
                    txt = "Unchecked to Checked";
                }
                this.BeginInvoke(new Action(() =>
                    {
                        WriteLog(txt);
                    }));
                dataGridView1.EndEdit();
            }
        }
    }
}
