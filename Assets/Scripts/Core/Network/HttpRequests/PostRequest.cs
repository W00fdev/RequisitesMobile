using System.Net;
using System.IO;
using System.Text;

namespace Assets.Scripts.Core
{
    public class PostRequest : Request
    {
        public string Referer { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }

        public PostRequest(string adress) : base(adress) { }

        public override void Run(ref CookieContainer cookie)
        {
            _request = (HttpWebRequest)WebRequest.Create(_adress);
            _request.Method = "POST";
            _request.CookieContainer = cookie;
            _request.Accept = Accept;
            _request.Host = Host;
            _request.ContentType = ContentType;
            _request.UserAgent = UserAgent;
            _request.Referer = Referer;

            byte[] sentData = Encoding.UTF8.GetBytes(Data);
            _request.ContentLength = sentData.Length;
            Stream sendStream = _request.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

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
            }
            catch { }
        }

    }
}
