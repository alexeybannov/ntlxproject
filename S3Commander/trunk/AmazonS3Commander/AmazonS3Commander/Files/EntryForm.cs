using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace AmazonS3Commander.Files
{
    partial class EntryForm : Form
    {
        private readonly Entry entry;

        private bool folder;


        public EntryForm(Entry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            this.entry = entry;

            InitializeComponent();

            Text = entry.Name;

            propertyGridFile.SelectedObject = new EntryInfo();

            ThreadPool.QueueUserWorkItem(IsEntryFolderAsync, new WorkItemParam(entry, OnAsyncComplete));
        }


        private void IsEntryFolderAsync(object state)
        {
            var param = (WorkItemParam)state;
            try
            {
                var entry = (Entry)param.State;
                var folder = false;
                if (entry.S3Service.ObjectExists(entry.BucketName, entry.Key))
                {
                    folder = false;
                }
                else
                {
                    if (entry.S3Service.ObjectExists(entry.BucketName, entry.FolderKey))
                    {
                        folder = true;
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found.", entry.Key);
                    }
                }
                param.OnComplete(folder);
            }
            catch (Exception error)
            {
                param.OnComplete(error);
            }
        }

        private void GetEntryInfoAsync(object state)
        {
            var param = (WorkItemParam)state;
            try
            {
                var entry = (Entry)param.State;
                var headers = entry.S3Service.HeadObject(entry.BucketName, folder ? entry.FolderKey : entry.Key);
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
                if (state is bool)
                {
                    folder = (bool)state;
                    checkBoxSubfolders.Visible = checkBoxSubfolders.Checked = folder;
                    ThreadPool.QueueUserWorkItem(GetEntryInfoAsync, new WorkItemParam(entry, OnAsyncComplete));
                    ThreadPool.QueueUserWorkItem(GetACLAsync, new WorkItemParam(entry, OnAsyncComplete));
                }
                else if (state is WebHeaderCollection)
                {
                    var headers = (WebHeaderCollection)state;
                    propertyGridFile.SelectedObject = new EntryInfo(entry.BucketName, entry.Name, entry.Key, headers);
                    httpHeadersEditor.SetHttpHeaders(headers);
                }
                else if (state is Exception)
                {
                    propertyGridFile.SelectedObject = new DataGridError((Exception)state);
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

        private class DataGridError
        {
            [DisplayName("Error")]
            public string Message
            {
                get;
                private set;
            }

            public DataGridError(Exception error)
            {
                Message = error.Message;
            }
        }
    }
}
