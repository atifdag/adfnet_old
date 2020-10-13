using System.Collections.Generic;

namespace Adfnet.Core.ValueObjects
{
    public struct EmailRow
    {
        private List<EmailKey> _emailKeys;
        public List<EmailKey> EmailKeys
        {
            get
            {
                if (_emailKeys != null)
                {
                    return _emailKeys;
                }
                _emailKeys = new List<EmailKey>();
                return _emailKeys;
            }
            set => _emailKeys = value;
        }
    }
}