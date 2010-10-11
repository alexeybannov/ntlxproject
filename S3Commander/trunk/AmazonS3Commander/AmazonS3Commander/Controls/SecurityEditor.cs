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
            dataGridViewACL.CurrentCellDirtyStateChanged += CurrentCellDirtyStateChanged;

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

        private void CurrentCellDirtyStateChanged(object sender, EventArgs e)
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
                    accessControlList.AddGrant(grantee, Permission.Default);
                    SetACL(accessControlList);
                    dataGridViewACL.Rows[dataGridViewACL.RowCount - 1].Selected = true;
                    OnACLChanged();
                }
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            var selectedRow = 0 < dataGridViewACL.SelectedRows.Count ? dataGridViewACL.SelectedRows[0] : null;
            if (selectedRow != null && 2 < selectedRow.Index)
            {
                var grantee = (Grantee)selectedRow.Cells[1].Value;
                var question = string.Format("Do you really want to delete {0} from ACL?", grantee);
                if (MessageBox.Show(question, RS.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    accessControlList.RemoveGrant(grantee);
                    SetACL(accessControlList);
                    OnACLChanged();
                }
            }
        }

        private void dataGridViewACL_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var grant = accessControlList.Grants[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 0: e.Value = grant.Grantee.GranteeType == GranteeType.User ? Icons.User : Icons.Group;
                    break;
                case 1: e.Value = grant.Grantee;
                    break;
                case 2: e.Value = (grant.Permission & Permission.FullControl) == Permission.FullControl;
                    break;
                case 3: e.Value = (grant.Permission & Permission.Read) == Permission.Read;
                    break;
                case 4: e.Value = (grant.Permission & Permission.Write) == Permission.Write;
                    break;
                case 5: e.Value = (grant.Permission & Permission.ReadAcp) == Permission.ReadAcp;
                    break;
                case 6: e.Value = (grant.Permission & Permission.WriteAcp) == Permission.WriteAcp;
                    break;
            }
        }

        private void dataGridViewACL_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var grant = accessControlList.Grants[e.RowIndex];
            var permission = Permission.None;
            var set = true.Equals(e.Value);
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

            if (set) grant.AddPermission(permission);
            else
            {
                if (e.ColumnIndex != 2) grant.RemovePermission(Permission.FullControl);
                grant.RemovePermission(permission);
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
