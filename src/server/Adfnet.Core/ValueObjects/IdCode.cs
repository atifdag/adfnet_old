using System;

namespace Adfnet.Core.ValueObjects
{
    public struct IdCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public IdCode(Guid id, string code)
        {
            Id = id;
            Code = code;
        }
    }
}
