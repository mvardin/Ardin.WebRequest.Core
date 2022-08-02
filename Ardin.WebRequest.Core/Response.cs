using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ardin
{
    public class Response
    {
        public WebHeaderCollection Header { get; set; }
        public string Body { get; set; }
        public CookieCollection Cookie { get; set; }
        public HttpStatusCode Code { get; set; }
        public string ExtraMessage { get; set; }
    }
}
