using System;
using System.ComponentModel;
using System.Windows.Forms;
using AmazonS3Commander.S3;
using AmazonS3Commander.Resources;

namespace AmazonS3Commander.Buckets
{
    partial class NewBucketForm : Form
    {
        [Browsable(false)]
        public string BucketName
        {
            get { return textBoxBucketName.Text; }
        }

        [Browsable(false)]
        public S3BucketLocation BucketLocation
        {
            get { return comboBoxLocation.SelectedItem as S3BucketLocation; }
        }


        public NewBucketForm(string bucketName)
        {
            InitializeComponent();

            Icon = Icons.NewAccount;
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
