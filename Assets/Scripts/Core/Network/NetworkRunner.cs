using Assets.Scripts.Core;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace Assets.Scripts.Core
{
    // There is deprecated Network class in UnityEngine.
    public static class NetworkRunner
    {
        private static readonly string _siteBaseURI = "https://service.nalog.ru";
        private static readonly string _siteRequsitesURI = "https://service.nalog.ru/addrno-proc.json";

        private static CookieContainer cookieContainer = new CookieContainer();
        public static void Initialize()
        {
            
        }

        public static string GetIfns()
        {
            GetRequest request = new GetRequest(_siteBaseURI + 
                "/static/tree2.html?inp=ifns&tree=SOUN_ADDRNO_FL&treeKind=LINKED&aver=3.42.6&sver=4.39.2&pageStyle=GM2");

            request.Accept = "*/*";
            request.Host = "service.nalog.ru";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0";

            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("DNT", "1");

            request.Headers.Add("Sec-Fetch-Dest", "iframe");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            request.Run(ref cookieContainer);
            return request.Response;
        }

        public static string GetOktmmf(string ifnsCode)
        {
            PostRequest request = new PostRequest(_siteRequsitesURI);
            request.Data = $"c=getOktmmf&ifns={ifnsCode}&okatom=";

            request.Accept = "*/*";
            request.Host = "service.nalog.ru";
            request.Referer = "https://service.nalog.ru/addrno.do";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            request.Headers.Add("DNT", "1");

            request.Headers.Add("Origin", "https://service.nalog.ru");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Sec-GPC", "1");

            request.Run(ref cookieContainer);
            return request.Response;
        }

        public static PayeeDetails GetPayeeRequisites(string ifnsCode, string oktmnfCode)
        {
            PostRequest request = new PostRequest(_siteRequsitesURI);

            request.Data = $"c=next&step=1&npKind=fl&objectAddr=&objectAddr_zip=&objectAddr_ifns=&objectAddr_okatom=&ifns={ifnsCode}&oktmmf={oktmnfCode}&PreventChromeAutocomplete=";

            request.Accept = "*/*";
            request.Host = "service.nalog.ru";
            request.Referer = "https://service.nalog.ru/addrno.do";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            request.Headers.Add("DNT", "1");

            request.Headers.Add("Origin", "https://service.nalog.ru");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");

            request.Run(ref cookieContainer);
            Debug.Log(request.Response);

            if (request.Response != null)
            {
                PayeeDetails dataRequisites = new PayeeDetails();
                dataRequisites = JsonUtility.FromJson<PayeeDetails>(request.Response);

                //Debug.Log(dataRequisites.payeeDetails.payeeAcc);

                return dataRequisites;
            }

            return null;
        }
    }
}

