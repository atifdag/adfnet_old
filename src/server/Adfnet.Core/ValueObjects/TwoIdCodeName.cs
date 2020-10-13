using System;

namespace Adfnet.Core.ValueObjects
{
    public struct TwoIdCodeName
    {
        public Guid Id1 { get; set; }
        public string Code1 { get; set; }
        public string Name1 { get; set; }
        public Guid Id2 { get; set; }
        public string Code2 { get; set; }
        public string Name2 { get; set; }

        public TwoIdCodeName(Guid id1, string code1, string name1, Guid id2, string code2, string name2)
        {
            Id1 = id1;
            Code1 = code1;
            Name1 = name1;
            Id2 = id2;
            Code2 = code2;
            Name2 = name2;
        }
    }
}
