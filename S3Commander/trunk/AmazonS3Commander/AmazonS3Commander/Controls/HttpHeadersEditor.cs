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
                buttonEdit.Enabled = buttonRemove.Enabled = HttpHeaderProvider.Editable(header);
            }
        }
    }
}
