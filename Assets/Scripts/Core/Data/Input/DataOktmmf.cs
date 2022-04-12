using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class DataOktmmf
    {
        public Dictionary<string, string> Data { get; set; }

        // Parser can text or site.
        // Maybe change to Dictionary param?
        public DataOktmmf(Dictionary<string, string> data) => Data = data;

        // delegate??
    }
}
