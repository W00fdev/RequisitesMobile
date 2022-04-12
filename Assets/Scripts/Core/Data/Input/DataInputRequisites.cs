using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core
{
    public class DataInputRequisites
    {
        public DataIfns DataIfns { get; set; }
        public DataOktmmf DataOktmmf { get; set; }
        public DataInputRequisites(DataIfns dataIfns = null, DataOktmmf dataOktmmf = null)
        {
            DataIfns = dataIfns;
            DataOktmmf = dataOktmmf;
        }
    }
}
