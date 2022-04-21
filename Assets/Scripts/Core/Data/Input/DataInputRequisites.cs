using System.Linq;

namespace Assets.Scripts.Core
{
    public static class DataInputRequisites
    {
        public static DataIfns DataIfns { get; set; } = null;
        public static DataOktmmf DataOktmmf { get; set; } = null;

        public static string IfnsComplete 
        {
            get => _ifnsComplete;
            set
            {
                _ifnsComplete = string.Join("", value.Where(c => char.IsDigit(c)));
            }
        }
        
        public static string OktmmfComplete 
        { 
            get => _oktmmfComplete;
            set
            {
                _oktmmfComplete = string.Join("", value.Where(c => char.IsDigit(c)));
            }
        }

        private static string _ifnsComplete;
        private static string _oktmmfComplete;
    }
}
