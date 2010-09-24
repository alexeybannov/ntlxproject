using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using AmazonS3Commander.Resources;
using AmazonS3Commander.Utils;

namespace AmazonS3Commander.Controls
{
    partial class SecurityEditor : UserControl
    {
        [Browsable(true)]
        public event EventHandler HttpHeadersChanged;

        public SecurityEditor()
        {
            InitializeComponent();

            dataGridViewACL.Rows.Add(RS.GroupsIcon, "All Users");
            dataGridViewACL.Rows.Add(RS.UserIcon, "amazon (Owner)");
        }


        private void OnHttpHeadersChanged()
        {
            if (HttpHeadersChanged != null) HttpHeadersChanged(this, EventArgs.Empty);
        }

        private void dataGridViewACL_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            if (e.ColumnIndex == 2)
            {
                var setted = (bool)dataGridViewACL.Rows[e.RowIndex].Cells[2].Value;
                if (setted)
                {
                    dataGridViewACL.Rows[e.RowIndex].Cells[3].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[4].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[5].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[6].Value = true;
                }
            }
        }
    }
}
