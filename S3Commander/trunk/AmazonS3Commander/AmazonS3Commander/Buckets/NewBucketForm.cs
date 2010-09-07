using System;
using System.ComponentModel;
using System.Windows.Forms;
using AmazonS3Commander.S3;

namespace AmazonS3Commander.Buckets
{
    public partial class NewBucketForm : Form
    {
        [Browsable(false)]
        public string BucketName
        {
            get { return textBoxBucketName.Text; }
        }


        public NewBucketForm(string bucketName)
        {
            InitializeComponent();
            textBoxBucketName.Text = bucketName;
            comboBoxLocation.DataSource = S3BucketLocation.GetAvailableLocations();
            comboBoxLocation.SelectedItem = S3BucketLocation.Default;
        }

        private void NewBucketForm_Shown(object sender, EventArgs e)
        {
            textBoxBucketName.SelectAll();
        }
    }
}
