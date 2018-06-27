using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 通用文件提取器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string RootPath { get => textBox1.Text; }
        public string SavePath { get => textBox3.Text; }
        public string Suffix { get => textBox2.Text; }
        public List<string> lst;
        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
                lst = new List<string>();
                GetFilesRecursion(RootPath, ref lst);
                richTextBox1.Text += $"共计{lst.Count()}个文件";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = dialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Extract(lst);
        }

        void Extract(IEnumerable<string> lst)
        {
            foreach (var n in lst)
            {
                var fi = new FileInfo(n);
                var savepath = $"{SavePath}/{fi.Name}";
                var t = Task.Run(() =>
                {
                    var f = fi;
                    f.CopyTo(savepath, false);
                });
                Task.Run(() =>
                {
                    Task.WaitAll(t);
                    this.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.Text += $"{fi.FullName}已提取到{savepath}\r\n";
                    });
                });
            }
        }
        void GetFilesRecursion(string path,ref List<string> lst)
        {
            var files = Directory.GetFiles(path).WhereIf(n=>n.ToUpper().EndsWith(Suffix.ToUpper()),!string.IsNullOrEmpty(Suffix));
            foreach (string file in files)
            {
                lst.Add(file);
                richTextBox1.Text += $"找到文件:{file}\r\n";
            }
            var dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                GetFilesRecursion(dir, ref lst);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
            richTextBox1.Focus();
        }
    }
  
}
