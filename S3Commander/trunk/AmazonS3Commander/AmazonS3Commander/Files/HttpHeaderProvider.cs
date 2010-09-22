using System;
using System.Collections.Generic;
using System.Linq;

namespace AmazonS3Commander.Files
{
    static class HttpHeaderProvider
    {
        //header - editable
        private static readonly Dictionary<string, bool> headers = new Dictionary<string, bool>(55, StringComparer.InvariantCultureIgnoreCase)
        {
            { "Accept", true },
            { "Accept-Charset", true },
            { "Accept-Encoding", true },
            { "Accept-Language", true },
            { "Accept-Ranges", true },
            { "Age", true },
            { "Allow", true },
            { "Authorization", false },
            { "Cache-Control", true },
            { "Connection", false },
            { "Content-Disposition", true },
            { "Content-Encoding", true },
            { "Content-Language", true },
            { "Content-Length", false },
            { "Content-Location", false },
            { "Content-MD5", false },
            { "Content-Range", false },
            { "Content-Type", true },
            { "Cookie", false },
            { "Date", false },
            { "ETag", false },
            { "Expect", true },
            { "Expires", true },
            { "From", true },
            { "Host", false },
            { "If-Match", false },
            { "If-Modified-Since", false },
            { "If-None-Match", false },
            { "If-Range", false },
            { "If-Unmodified-Since", false },
            { "Keep-Alive", false },
            { "Last-Modified", false },
            { "Location", false },
            { "Max-Forwards", false },
            { "Pragma", false },
            { "Proxy-Authenticate", false },
            { "Proxy-Authorization", false },
            { "Range", false },
            { "Referer", false },
            { "Retry-After", false },
            { "Server", false },
            { "Set-Cookie", false },
            { "TE", false },
            { "Trailer", false },
            { "Transfer-Encoding", false },
            { "Translate", false },
            { "Upgrade", false },
            { "User-Agent", false },
            { "Vary", false },
            { "Via", false },
            { "Warning", false },
            { "WWW-Authenticate", false },
        };

        public static string[] GetHeaders()
        {
            return headers.Keys.ToArray();
        }

        public static bool Editable(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                return true;
            }
            if (headers.ContainsKey(header))
            {
                return headers[header];
            }
            return
                !header.StartsWith("x-amz-", StringComparison.InvariantCultureIgnoreCase) ||
                header.StartsWith("x-amz-meta-", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
