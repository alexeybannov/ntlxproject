using System;
using System.Net;
using System.Windows.Forms;
using AmazonS3Commander.Files;

namespace AmazonS3Commander.Controls
{
    partial class HttpHeadersEditor : UserControl
    {
        public HttpHeadersEditor()
        {
            InitializeComponent();
        }


        public void SetHttpHeaders(WebHeaderCollection headers)
        {
            listViewHeaders.Items.Clear();
            foreach (string header in headers)
            {
                var item = new ListViewItem(new[] { header, headers[header] });
                listViewHeaders.Items.Add(item);
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            var listView = (ListView)sender;
            if (0 < listView.SelectedItems.Count)
            {
                var header = listView.SelectedItems[0].Text;
                buttonEdit.Enabled = buttonRemove.Enabled = HttpHeaderProvider.IsEditable(header);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var headerForm = new HttpHeaderForm())
            {
                if (headerForm.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listViewHeaders.SelectedItems.Count == 0) return;

            var item = listViewHeaders.SelectedItems[0];
            using (var headerForm = new HttpHeaderForm(item.Text, item.SubItems[1].Text))
            {
                if (headerForm.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
    }
}
