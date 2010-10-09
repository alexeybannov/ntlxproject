using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AmazonS3Commander.Resources;
using LitS3;

namespace AmazonS3Commander.Controls
{
    partial class SecurityEditor : UserControl
    {
        private AccessControlList accessControlList;


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

            SetACL(new AccessControlList(new Owner("amazon", "aaaa")));
        }

        public void SetACL(AccessControlList accessControlList)
        {
            if (accessControlList == null) throw new ArgumentNullException("accessControlList");

            this.accessControlList = accessControlList;
            dataGridViewACL.RowCount = accessControlList.Grants.Count;
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

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new SecurityAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var grantee = new Grantee(form.UserEmailOrId.Contains('@') ? GranteeType.Email : GranteeType.User, form.UserEmailOrId);
                    foreach (DataGridViewRow row in dataGridViewACL.Rows)
                    {
                        if (grantee.Equals(row.Cells[1].Value))
                        {
                            row.Selected = true;
                            return;
                        }
                    }
                    accessControlList.SetGrant(grantee, Permission.None);
                    SetACL(accessControlList);
                    dataGridViewACL.Rows[dataGridViewACL.RowCount - 1].Selected = true;
                    OnACLChanged();
                }
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridViewACL.CurrentRow != null || dataGridViewACL.CurrentRow.Index <= 2)
            {
                var question = string.Format("Do you really want to delete {0} from ACL?", dataGridViewACL.CurrentRow.Cells[1].Value);
                if (MessageBox.Show(question, RS.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    accessControlList.Grants.Remove((Grantee)dataGridViewACL.CurrentRow.Cells[1].Value);
                    SetACL(accessControlList);
                    OnACLChanged();
                }
            }
        }

        private void dataGridViewACL_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var grant = accessControlList.Grants.ElementAtOrDefault(e.RowIndex);
            var grantee = grant.Key;
            var permission = grant.Value;
            if (grantee == null) return;

            switch (e.ColumnIndex)
            {
                case 0: e.Value = grantee.GranteeType == GranteeType.User ? Icons.User : Icons.Group;
                    break;
                case 1: e.Value = grantee;
                    break;
                case 2: e.Value = (permission & Permission.FullControl) == Permission.FullControl;
                    break;
                case 3: e.Value = (permission & Permission.Read) == Permission.Read;
                    break;
                case 4: e.Value = (permission & Permission.Write) == Permission.Write;
                    break;
                case 5: e.Value = (permission & Permission.ReadAcp) == Permission.ReadAcp;
                    break;
                case 6: e.Value = (permission & Permission.WriteAcp) == Permission.WriteAcp;
                    break;
            }
        }

        private void dataGridViewACL_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var grant = accessControlList.Grants.ElementAtOrDefault(e.RowIndex);
            var grantee = grant.Key;
            if (e.ColumnIndex < 2 || grantee == null) return;

            var set = true.Equals(e.Value);
            var permission = Permission.None;
            switch (e.ColumnIndex)
            {
                case 2: permission = Permission.FullControl;
                    if (set) permission |= Permission.Read | Permission.Write | Permission.ReadAcp | Permission.WriteAcp;
                    break;
                case 3: permission = Permission.Read;
                    break;
                case 4: permission = Permission.Write;
                    break;
                case 5: permission = Permission.ReadAcp;
                    break;
                case 6: permission = Permission.WriteAcp;
                    break;
            }

            if (set) accessControlList.AddGrant(grantee, permission);
            else
            {
                if (e.ColumnIndex != 2) accessControlList.RemoveGrant(grantee, Permission.FullControl);
                accessControlList.RemoveGrant(grantee, permission);
            }
            dataGridViewACL.Refresh();
        }

        private void dataGridViewACL_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewACL.SelectedRows.Count == 0) buttonRemove.Enabled = false;

            var systemRowSelected = false;
            foreach (DataGridViewRow row in dataGridViewACL.SelectedRows)
            {
                if (row.Index <= 2)
                {
                    systemRowSelected = true;
                    break;
                }
            }

            buttonRemove.Enabled = !systemRowSelected;
        }
    }
}
