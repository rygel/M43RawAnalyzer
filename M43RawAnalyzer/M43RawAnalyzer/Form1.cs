using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace M43RawAnalyzer
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw = new BackgroundWorker();
        private string[] files;
        private string folder;
        private int currentFile;
        public Form1()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folder = Properties.Settings.Default.DirPath;
            labelDirectory.Text = "Directory: " + Properties.Settings.Default.DirPath;
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            string startupPath = Application.StartupPath;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!Properties.Settings.Default.DirPath.Equals(""))
                {
                    dialog.SelectedPath = (string)Properties.Settings.Default.DirPath;
                }
                
                dialog.Description = "Open a folder which contains the xml output";
                dialog.ShowNewFolderButton = true;
                //dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.DirPath = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    folder = dialog.SelectedPath;
                    labelDirectory.Text = "Directory: " + folder;
                }
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            if (buttonAnalyze.Text.Equals("Cancel"))
            {
                bw.CancelAsync();
            }
            else
            {
                files = Directory.GetFiles(folder, "*.RW2", SearchOption.TopDirectoryOnly);

                if (files.Length == 0) return;

                currentFile = 0;
                labelFile.Text = "Current File: " + files[currentFile];
                labelProcessing.Text = String.Format("Processing file {0} of {1}.", currentFile + 1, files.Length);
                buttonAnalyze.Text = "Cancel";
                bw.RunWorkerAsync(files);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            BackgroundWorker worker = sender as BackgroundWorker;

            RW2Parser rw2Parser = new RW2Parser();

            string[] localFiles = (string[])e.Argument;
            double currentFile = 0;

            foreach (string filename in localFiles)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    currentFile++;
                    // Perform a time consuming operation and report progress.
                    rw2Parser.Parse2(filename);
                    double prog = (currentFile / localFiles.Length) * 100;
                    int progress = (int)Math.Truncate(prog);
                    worker.ReportProgress(progress);
                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.labelProcessing.Text = "Canceled!";
            }
            else if (!(e.Error == null))
            {
                this.labelProcessing.Text = ("Error: " + e.Error.Message);
            }
            else
            {
                this.labelProcessing.Text = "Done!";
            }
            buttonAnalyze.Text = "Analyze";
            progressBar.Value = 0;
            labelFile.Text = "File: ";
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressBar.Text = (e.ProgressPercentage.ToString() + "%");
            currentFile++;
            if (currentFile < files.Length)
            {
                labelFile.Text = "Current File: " + files[currentFile];
                labelProcessing.Text = String.Format("Processing file {0} of {1}.", currentFile + 1, files.Length);
            }
            else
            {
                labelFile.Text = "Current File: ";
                labelProcessing.Text = String.Format("Finished processing.");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            bw.CancelAsync();
        }
    }
}
