using System.Net;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GetRequest : Request
    {
        public GetRequest(string adress, System.Action<string> onResponseGet) : base(adress, onResponseGet) { }

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
                //cookie.Add(response.Cookies);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
