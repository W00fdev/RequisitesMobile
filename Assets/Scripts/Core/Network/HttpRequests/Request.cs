using System.Net;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public abstract class Request
    {
        protected HttpWebRequest _request;
        protected string _adress;

        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public string Response { get; set; }
        public string Accept { get; set; }
        public string Host { get; set; }
        public string UserAgent { get; set; }

        public Request(string adress) => _adress = adress;

        public abstract void Run(ref CookieContainer cookie);
    }
}
