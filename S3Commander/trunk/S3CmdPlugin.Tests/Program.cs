using System;
using System.Collections.Generic;
using System.Text;
using Amazon.S3;
using System.Net;

namespace S3CmdPlugin.Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			AmazonS3Config cfg = new AmazonS3Config();
			var re = WebRequest.Create("http://s3.amazon.com");
			var proxy = WebRequest.GetSystemWebProxy();
		}
	}
}
