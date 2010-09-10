using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace AmazonS3Commander.Files
{
    partial class EntryForm : Form
    {
        private readonly Entry entry;


        public EntryForm(Entry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            this.entry = entry;

            InitializeComponent();

            var key = entry.Key ?? string.Empty;
            var index = key.TrimEnd('/').LastIndexOf('/');
            Text = 0 < index ? key.Substring(index + 1) : key;

            ThreadPool.QueueUserWorkItem(GetEntryInfoAsync, new WorkItemParam(entry, OnAsyncComplete));
            ThreadPool.QueueUserWorkItem(GetACLAsync, new WorkItemParam(entry, OnAsyncComplete));
        }


        private void GetEntryInfoAsync(object state)
        {
            var param = (WorkItemParam)state;
            try
            {
                var entry = (Entry)param.State;
                var headers = entry.S3Service.HeadObject(entry.BucketName, entry.Key);
                param.OnComplete(headers);
            }
            catch (Exception error)
            {
                param.OnComplete(error);
            }
        }

        private void GetACLAsync(object state)
        {
            var param = (WorkItemParam)state;
            try
            {
                var entry = (Entry)param.State;
                //get acl
                param.OnComplete(null);
            }
            catch (Exception error)
            {
                param.OnComplete(error);
            }
        }

        private void OnAsyncComplete(object state)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(OnAsyncComplete), state);
            }
            else
            {
                if (state is WebHeaderCollection)
                {
                    var headers = (WebHeaderCollection)state;
                    propertyGridFile.SelectedObject = new EntryInfo(entry.BucketName, entry.Key, headers);
                    foreach (string header in headers)
                    {
                        var item = new ListViewItem(new[] { header, headers[header] });
                        listViewHeaders.Items.Add(item);
                    }
                }
                else if (state is Exception)
                {
                    throw (Exception)state;
                }
            }
        }


        private class WorkItemParam
        {
            public object State
            {
                get;
                private set;
            }

            public WaitCallback OnComplete
            {
                get;
                private set;
            }

            public WorkItemParam(object state, WaitCallback onComplete)
            {
                State = state;
                OnComplete = onComplete;
            }
        }
    }
}
