using System.Net;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class PostRequest : Request
    {
        public string Referer { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }

        public PostRequest(string adress, System.Action<string> onResponseGet) : base(adress, onResponseGet) { }

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

            _request.BeginGetResponse(new System.AsyncCallback(FinishRun), _request);
        }

        protected override void FinishRun(System.IAsyncResult result)
        {
            try
            {
                HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    Response = new StreamReader(stream).ReadToEnd();
                    UnityMainThread.wkr.AddJob(OnResponseGet, Response);
                    
                    // Not thread-safe for Unity
                    // OnResponseGet?.Invoke(Response);
                }
            }
            catch( System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

    }
}
