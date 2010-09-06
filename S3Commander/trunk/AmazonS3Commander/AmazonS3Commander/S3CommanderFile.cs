using System;
using System.Collections.Generic;
using System.Drawing;
using AmazonS3Commander.S3;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    class S3CommanderFile
    {
        public static readonly S3CommanderFile Empty = new S3CommanderFile();

        protected static readonly IEnumerator<FindData> EmptyFindDataEnumerator = new List<FindData>().GetEnumerator();


        private S3CommanderContext context;

        private IS3Service s3Service;


        protected S3CommanderContext Context
        {
            get
            {
                if (context == null) throw new InvalidOperationException("Context not initialized.");
                return context;
            }
        }

        protected IS3Service S3Service
        {
            get
            {
                if (s3Service == null) throw new InvalidOperationException("S3Service not initialized.");
                return s3Service;
            }
        }


        protected S3CommanderFile()
        {
        }

        public S3CommanderFile Initialize(S3CommanderContext context)
        {
            return Initialize(context, null);
        }

        public S3CommanderFile Initialize(S3CommanderContext context, IS3Service s3Service)
        {
            if (context == null) throw new ArgumentNullException("context");
            
            this.context = context;
            this.s3Service = s3Service;
            return this;
        }


        public virtual IEnumerator<FindData> GetFiles()
        {
            return null;
        }


        public virtual ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }


        public virtual FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult Upload(string localName, CopyFlags copyFlags)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult CopyTo(S3CommanderFile dest, bool overwrite, bool move, RemoteInfo info)
        {
            return FileOperationResult.Default;
        }

        public virtual bool DeleteFile()
        {
            return false;
        }

        public virtual bool CreateFolder()
        {
            return false;
        }

        public virtual bool DeleteFolder()
        {
            return false;
        }


        public virtual Icon GetIcon()
        {
            return null;
        }
    }
}
