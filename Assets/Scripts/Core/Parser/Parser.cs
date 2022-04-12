using System.Collections.Generic;

using static Assets.Scripts.Core.DataIfns;

namespace Assets.Scripts.Core
{
    public abstract class Parser
    {
        protected static readonly int _ifnsCodeSize = 4;
        protected static readonly int _oktmmfCodeSize = 8;

        public abstract Dictionary<string, string> ParseOktmmf(string input);
        public abstract List<RepublicData> ParseIfns(string input);
    }
}
