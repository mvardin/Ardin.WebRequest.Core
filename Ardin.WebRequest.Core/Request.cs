using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ardin
{
    public class Request
    {
        public string Url { get; set; }
        public string Body { get; set; }
        public string Method { get; set; }
        public WebProxy Proxy { get; set; }
        public CookieCollection Cookie { get; set; }
        public WebHeaderCollection Header { get; set; }
    }
}
