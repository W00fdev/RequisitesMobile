using System.Net;
using System.IO;

namespace Assets.Scripts.Core
{
    public class GetRequest : Request
    {
        public GetRequest(string adress) : base(adress) { }

        public override void Run(ref CookieContainer cookie) 
        {
            _request = (HttpWebRequest)WebRequest.Create(_adress);
            _request.Method = "GET";
            _request.CookieContainer = cookie;
            _request.Accept = Accept;
            _request.Host = Host;
            _request.UserAgent = UserAgent;

            foreach (var pair in Headers)
            {
                _request.Headers.Add(pair.Key, pair.Value);
            }

            try
            {
                using HttpWebResponse responce = (HttpWebResponse)_request.GetResponse();
                Stream stream = responce.GetResponseStream();
                if (stream != null)
                    Response = new StreamReader(stream).ReadToEnd();

                cookie.Add(responce.Cookies);
            }
            catch { }
        }
    }
}
