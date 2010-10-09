using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using AmazonS3Commander.Resources;
using AmazonS3Commander.Utils;

namespace AmazonS3Commander.Controls
{
    partial class HttpHeadersEditor : UserControl
    {
        [Browsable(true)]
        public event EventHandler HttpHeadersChanged;

        public HttpHeadersEditor()
        {
            InitializeComponent();
        }


        public IDictionary<string, string> GetHttpHeaders()
        {
            var headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (ListViewItem item in listViewHeaders.Items)
            {
                headers.Add(item.SubItems[0].Text, item.SubItems[1].Text);
            }
            return headers;
        }

        public void SetHttpHeaders(WebHeaderCollection headers)
        {
            listViewHeaders.BeginUpdate();
            try
            {
                listViewHeaders.Items.Clear();
                foreach (string header in headers)
                {
                    var foreColor = Color.FromKnownColor(HttpHeaderProvider.IsEditable(header) ? KnownColor.WindowText : KnownColor.InactiveCaption);
                    var item = new ListViewItem(new[] { header, headers[header] }, -1, foreColor, listViewHeaders.BackColor, null);
                    listViewHeaders.Items.Add(item);
                }
            }
            finally
            {
                listViewHeaders.EndUpdate();
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
            var headers = GetHttpHeaders();
            using (var form = new HttpHeaderForm(headers.Keys))
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                if (headers.ContainsKey(form.HttpHeader))
                {
                    MessageBox.Show("This http header already exists.", RS.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = new ListViewItem(new[] { form.HttpHeader, form.HttpHeaderValue });
                listViewHeaders.Items.Add(item);
                OnHttpHeadersChanged();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listViewHeaders.SelectedItems.Count == 0) return;

            var item = listViewHeaders.SelectedItems[0];
            var header = item.SubItems[0].Text;
            var oldValue = item.SubItems[1].Text;

            using (var form = new HttpHeaderForm(header, oldValue))
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                var newValue = form.HttpHeaderValue;
                if (oldValue != newValue)
                {
                    item.SubItems[1].Text = newValue;
                    OnHttpHeadersChanged();
                }
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listViewHeaders.SelectedItems.Count == 0) return;

            var item = listViewHeaders.SelectedItems[0];

            if (MessageBox.Show("Do you really want to delete the selected http header " + item.Text, RS.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                listViewHeaders.Items.Remove(item);
            }
        }


        private void OnHttpHeadersChanged()
        {
            if (HttpHeadersChanged != null) HttpHeadersChanged(this, EventArgs.Empty);
        }
    }
}
