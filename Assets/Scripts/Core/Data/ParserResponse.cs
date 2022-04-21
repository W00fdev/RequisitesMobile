using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public class ParserResponse
    {
        public enum ResponseType { UNKNOWNERROR = 0, NOTFOUND = 1, EMPTY = 2, OK = 3 }

        public ResponseType Type { get; private set; }
        public string Response { get; private set; }

        public ParserResponse(ResponseType responseType, string responseString = "")
        {
            Type = responseType;
            Response = responseString;
        }

        public string MakeHint(string code)
        {
            switch (Type)
            {
                case ResponseType.UNKNOWNERROR:
                    return $"{code} - Ошибка.";
                case ResponseType.NOTFOUND:
                    return $"{code} - Не найден";
                case ResponseType.EMPTY:
                    return "";
                case ResponseType.OK:
                    return Response;
                default:
                    break;
            }

            return "";
        }

        public bool IsError() => Type == ResponseType.UNKNOWNERROR;
        public bool IsNotFound() => Type == ResponseType.NOTFOUND;
        public bool IsEmpty() => Type == ResponseType.EMPTY;
        public bool IsOk() => Type == ResponseType.OK;
    }
}