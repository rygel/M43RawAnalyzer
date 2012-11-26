using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ExifLib;
using System.Drawing.Imaging;

namespace ExifLibTestApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : Form
	{
        private Label lblFile;
        private TextBox txtFileName;
        private Button btnBrowse;
        private Button btnPopulate;
        private TextBox txtFields;
        private Button btnSpeedTest;
        private PictureBox pictureBoxThumbnail;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnPopulate = new System.Windows.Forms.Button();
            this.txtFields = new System.Windows.Forms.TextBox();
            this.btnSpeedTest = new System.Windows.Forms.Button();
            this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(12, 9);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(26, 13);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "File:";
            // 
            // txtFileName
            // 
            this.txtFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtFileName.Location = new System.Drawing.Point(44, 6);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(192, 20);
            this.txtFileName.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(242, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnPopulate
            // 
            this.btnPopulate.Location = new System.Drawing.Point(323, 4);
            this.btnPopulate.Name = "btnPopulate";
            this.btnPopulate.Size = new System.Drawing.Size(75, 23);
            this.btnPopulate.TabIndex = 2;
            this.btnPopulate.Text = "Get data";
            this.btnPopulate.UseVisualStyleBackColor = true;
            this.btnPopulate.Click += new System.EventHandler(this.btnPopulate_Click);
            // 
            // txtFields
            // 
            this.txtFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFields.Location = new System.Drawing.Point(13, 36);
            this.txtFields.Multiline = true;
            this.txtFields.Name = "txtFields";
            this.txtFields.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFields.Size = new System.Drawing.Size(465, 206);
            this.txtFields.TabIndex = 3;
            // 
            // btnSpeedTest
            // 
            this.btnSpeedTest.Location = new System.Drawing.Point(404, 4);
            this.btnSpeedTest.Name = "btnSpeedTest";
            this.btnSpeedTest.Size = new System.Drawing.Size(75, 23);
            this.btnSpeedTest.TabIndex = 2;
            this.btnSpeedTest.Text = "Speed test";
            this.btnSpeedTest.UseVisualStyleBackColor = true;
            this.btnSpeedTest.Click += new System.EventHandler(this.btnSpeedTest_Click);
            // 
            // pictureBoxThumbnail
            // 
            this.pictureBoxThumbnail.Location = new System.Drawing.Point(15, 248);
            this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
            this.pictureBoxThumbnail.Size = new System.Drawing.Size(95, 77);
            this.pictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxThumbnail.TabIndex = 4;
            this.pictureBoxThumbnail.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(490, 337);
            this.Controls.Add(this.pictureBoxThumbnail);
            this.Controls.Add(this.txtFields);
            this.Controls.Add(this.btnSpeedTest);
            this.Controls.Add(this.btnPopulate);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblFile);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exif Test";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { FileName = txtFileName.Text, Filter = "JPEG Images (*.jpg)|*.jpg" };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = dlg.FileName;
                btnPopulate_Click(sender, e);
            }
        }

        private void btnPopulate_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFileName.Text))
            {
                MessageBox.Show(this, "Please enter a valid filename", "File not found", MessageBoxButtons.OK);
                return;
            }

            ExifReader reader = null;
            try
            {
                reader = new ExifReader(txtFileName.Text);

                // Get the image thumbnail (if present)
                var thumbnailBytes = reader.GetJpegThumbnailBytes();

                if (thumbnailBytes == null)
                    pictureBoxThumbnail.Image = null;
                else
                {
                    using (var stream = new MemoryStream(thumbnailBytes))
                        pictureBoxThumbnail.Image = Image.FromStream(stream);
                }

                // To read a single field, use code like this:
                /*
                DateTime datePictureTaken;
                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out datePictureTaken))
                {
                    MessageBox.Show(this, string.Format("The picture was taken on {0}", datePictureTaken), "Image information", MessageBoxButtons.OK);
                }
                */

                // Parse through all available fields
                string props = "";
                foreach (ushort tagID in Enum.GetValues(typeof(ExifTags)))
                {
                    object val;
                    if (reader.GetTagValue(tagID, out val))
                    {
                        // Arrays don't render well without assistance.
                        string renderedTag;
                        if (val is Array)
                        {
                            renderedTag = "";
                            foreach (object item in (Array)val)
                                renderedTag += item + ",";
                            renderedTag = renderedTag.Substring(0, renderedTag.Length - 1);
                        }
                        else
                            renderedTag = val.ToString();

                        props += string.Format("{0}:{1}\r\n", Enum.GetName(typeof(ExifTags), tagID), renderedTag);
                    }
                }

                // Remove the last carriage return
                props = props.Substring(0, props.Length - 2);

                txtFields.Text = props;
            }
            catch (Exception ex)
            {
                // Something didn't work!
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (reader != null)
                    reader.Dispose();
            }
        }

        private void btnSpeedTest_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFileName.Text))
            {
                MessageBox.Show(this, "Please enter a valid filename", "File not found", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show(this, "This will take 10 seconds to complete.", "Exif library speed test", MessageBoxButtons.OKCancel) !=
                DialogResult.OK)
                return;

            // See how many single parameter reads can be done in 5 seconds with
            // 1. The .NET Image class
            // 2. The ExifReader

            TimeSpan msInterval, exifLibInterval;
            int msCount = 0, exifLibCount = 0;
            DateTime startTime = DateTime.Now;

            try
            {
                // Using the imaging classes
                while (startTime.AddSeconds(5) > DateTime.Now)
                {
                    using (FileStream stream = new FileStream(txtFileName.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (Image image = Image.FromStream(stream, true, false))
                        {
                            PropertyItem propertyItem = image.GetPropertyItem((int)ExifTags.Model);
                            BitConverter.ToString(propertyItem.Value, 0);
                            msCount++;
                        }
                    }
                }
                msInterval = DateTime.Now - startTime;

                startTime = DateTime.Now;

                // Using the ExifLib classes
                while (startTime.AddSeconds(5) > DateTime.Now)
                {
                    using (ExifReader reader = new ExifReader(txtFileName.Text))
                    {
                        string model;
                        reader.GetTagValue(ExifTags.Model, out model);
                        exifLibCount++;
                    }

                }
                exifLibInterval = DateTime.Now.Subtract(startTime);

            }
            catch (Exception ex)
            {
                
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string results = string.Format("System.Drawing: {0:F0} reads per second\r\nExifLib: {1:F0} reads per second.\r\nExifLib speed is {2:P0} of System.Drawing", msCount / msInterval.TotalSeconds, exifLibCount / exifLibInterval.TotalSeconds, (exifLibCount / exifLibInterval.TotalSeconds) / (msCount / msInterval.TotalSeconds));

            MessageBox.Show(this, results, "Test results", MessageBoxButtons.OK);
        }
	}
}