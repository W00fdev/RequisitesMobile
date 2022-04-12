using System;
using System.Collections;
using System.Collections.Generic;

using static Assets.Scripts.Core.DataIfns;

namespace Assets.Scripts.Core
{
    public class ParserSite : Parser
    {
        public override Dictionary<string, string> ParseOktmmf(string input)
        {
            var data = new Dictionary<string, string>();
            
            string anchor = ":{\"";
            int startIndex = input.IndexOf(anchor, 0) + anchor.Length;
            int endIndex = input.IndexOf("\"}}", startIndex);

            string sourceData = input.Substring(startIndex, endIndex - startIndex);
            for (int i = 0; i < sourceData.Length; i++)
            {
                string key = sourceData.Substring(i, _oktmmfCodeSize);
                // (":").Length == 3
                i += _oktmmfCodeSize + 3;
                int nextIndex = sourceData.IndexOf('\"', i);
                string value = "";
                if (nextIndex == -1)
                {
                    value = sourceData.Substring(i, sourceData.Length - i);
                    data.Add(key, value);
                    break;
                }

                value = sourceData.Substring(i, nextIndex - i);
                data.Add(key, value);
                i = nextIndex + 2;
            }

            return data;
        }
        public override List<RepublicData> ParseIfns(string input)
        {
            var data = new List<RepublicData>();

            int bracersLevel = 0;
            var currentRepublicData = new RepublicData
            {
                RepublicName = "None",
                IfnsData = new Dictionary<string, string>()
            };

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '[')
                {
                    bracersLevel++;

                    // New republic (List) section [Level == 3]
                    if (bracersLevel == 3)
                    {
                        // "12[","]republic name"...
                        string anchor = "\",\"";
                        int startIndex = input.IndexOf(anchor, i) + anchor.Length;
                        int endIndex = input.IndexOf('\"', startIndex);
                        //currentRepublicData.RepublicName = input[startIndex..endIndex];
                        currentRepublicData.RepublicName = input.Substring(startIndex, endIndex - startIndex);
                    }
                    else if (bracersLevel == 5)
                    {
                        // ["1234","District..."]
                        i += 2;
                        // 1234","District..."]

                        //string ifnsCode = input[i..(i + 4)];
                        string ifnsCode = input.Substring(i, _ifnsCodeSize);
                        i += _ifnsCodeSize;
                        // ","District..."]
                        i += 3;
                        // District..."]
                        int endOfDesctiptionIndex = input.IndexOf('\"', i);
                        //string ifnsDescription = input[i..endOfDesctiptionIndex];
                        string ifnsDescription = input.Substring(i, endOfDesctiptionIndex - i);

                        currentRepublicData.IfnsData.Add(ifnsCode, ifnsDescription);

                        i = endOfDesctiptionIndex;
                    }
                }
                else if (input[i] == ']')
                {
                    bracersLevel--;

                    if (bracersLevel == 2)
                    {
                        data.Add(currentRepublicData);

                        currentRepublicData = new RepublicData
                        {
                            RepublicName = "None",
                            IfnsData = new Dictionary<string, string>()
                        };
                    }

                }
            }

            return data;
        }
    }
}