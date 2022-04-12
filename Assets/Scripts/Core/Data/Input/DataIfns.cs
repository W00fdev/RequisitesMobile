using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core
{
    public class DataIfns
    {
        public struct RepublicData
        {
            public string RepublicName;
            public Dictionary<string, string> IfnsData;
        }

        public List<RepublicData> Data { get; set; }

        public DataIfns(List<RepublicData> data) => Data = data; 

        public bool CheckIfnsExistence(string ifnsToCheck)
        {
            if (ifnsToCheck.Length != 4)
                return false;

            int ifnsFull = 0;
            int ifns = 0;
            try
            {
                ifnsFull = Convert.ToInt32(ifnsToCheck);
                ifns = ifnsFull / 100;
                // Check first two digits:
                if (ifns < 1 || ifns > 99)
                    return false;
            }
            catch (Exception)
            {
                // Can't convert to int value
                return false;
            }

            var nonExistentPrefixes = new List<string>
            {
                "00", "80", "81", "82",
                "84", "85", "88", "90",
                "93", "94", "95", "96",
                "97", "98",
            };

            if (nonExistentPrefixes.Contains(ifnsToCheck.Substring(0, 2)))
                return false;

            // 01 .. 79 all indexes are the same
            if (ifns < 80)
            {
                return Data[ifns - 1].IfnsData.ContainsKey(ifnsToCheck);
            }
            else
            {
                for (int i = 80; i < Data.Count; i++)
                {
                    string ifnsPrefix = Data[i].IfnsData.Keys.First().Substring(0, 2);
                    if (ifnsPrefix == ifnsToCheck.Substring(0, 2))
                    {
                        return Data[i].IfnsData.Keys.Contains(ifnsToCheck);
                    }
                }
            }


            return false;
        }

        public void Debug()
        {
            foreach(var republic in Data)
            {
                UnityEngine.Debug.Log(republic.RepublicName + ": {");
                foreach(var ifnsVar in republic.IfnsData)
                {
                    UnityEngine.Debug.Log(" \" " + ifnsVar.Key + ": \"" + ifnsVar.Value + " \", ");
                }
                UnityEngine.Debug.Log("}");
            }
        }
    }
}
