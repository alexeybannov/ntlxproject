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
        public event EventHandler ACLChanged;

        public SecurityEditor()
        {
            InitializeComponent();
            dataGridViewACL.CurrentCellDirtyStateChanged += dataGridViewACL_CurrentCellDirtyStateChanged;

            Column2.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Column3.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Column4.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Column5.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Column6.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewACL.Rows.Add(RS.GroupsIcon, "All Users");
            dataGridViewACL.Rows.Add(RS.UserIcon, "amazon (Owner)");
        }


        private void OnACLChanged()
        {
            if (ACLChanged != null) ACLChanged(this, EventArgs.Empty);
        }

        private void dataGridViewACL_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewACL.IsCurrentCellDirty)
                {
                    dataGridViewACL.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch { }
        }

        private void dataGridViewACL_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

                if (e.ColumnIndex == 2 && true.Equals(dataGridViewACL.Rows[e.RowIndex].Cells[e.ColumnIndex].Value))
                {
                    dataGridViewACL.Rows[e.RowIndex].Cells[3].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[4].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[5].Value = true;
                    dataGridViewACL.Rows[e.RowIndex].Cells[6].Value = true;
                }
                if (2 < e.ColumnIndex && false.Equals(dataGridViewACL.Rows[e.RowIndex].Cells[e.ColumnIndex].Value))
                {
                    dataGridViewACL.Rows[e.RowIndex].Cells[2].Value = false;
                }
            }
            catch { }
            finally
            {
                OnACLChanged();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new SecurityAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var row = dataGridViewACL.Rows.Add(RS.UserIcon, form.UserEmailOrId);
                    dataGridViewACL.Rows[row].Selected = true;
                    OnACLChanged();
                }
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridViewACL.CurrentRow != null)
            {
                var question = string.Format("Do you really want to delete {0} from ACL?", dataGridViewACL.CurrentRow.Cells[1].Value);
                if (MessageBox.Show(question, RS.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataGridViewACL.Rows.Remove(dataGridViewACL.CurrentRow);
                    OnACLChanged();
                }
            }
        }
    }
}
