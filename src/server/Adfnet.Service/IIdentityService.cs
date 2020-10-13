using System;
using Adfnet.Core.Security;

namespace Adfnet.Service
{
    public interface IIdentityService
    {
        void Set(CustomIdentity identity, DateTime expires, bool rememberMe);
        CustomIdentity Get();
        void Remove();
    }
}
