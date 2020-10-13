using System;

namespace Adfnet.Core.ValueObjects
{
    public struct TwoKeyValueSelected
    {
        public Guid Key1 { get; set; }
        public string Value1 { get; set; }
        public Guid Key2 { get; set; }
        public string Value2 { get; set; }
        public bool Selected { get; set; }

        public TwoKeyValueSelected(Guid key1, string value1, Guid key2, string value2, bool selected)
        {
            Key1 = key1;
            Value1 = value1;
            Key2 = key2;
            Value2 = value2;
            Selected = selected;
        }
    }
}
