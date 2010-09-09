using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AmazonS3Commander.Files
{
    partial class EntryForm : Form
    {
        public EntryForm(Entry entry)
        {
            InitializeComponent();

            var key = entry.Key ?? string.Empty;
            var index = key.TrimEnd('/').LastIndexOf('/');
            Text = 0 < index ? key.Substring(index + 1) : key;
        }
    }
}
