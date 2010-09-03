using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitS3.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new S3Service()
            {
                AccessKeyID = "AKIAJ6F26BFDPKHVPHIA",
                SecretAccessKey = "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",
            };

            var buckets = service.GetAllBuckets();
            var objects = service
                .ListAllObjects("static.teamlab.com", "")
                .ToList();
        }
    }
}
