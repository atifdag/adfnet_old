using Adfnet.Core.Enums;
using Adfnet.Service.Models;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IAuthenticationService
    {
        void Login(LoginModel model);
        void SignOut(SignOutOption signOutOption);
        void Register(RegisterModel model);
        void ForgotPassword(string username);
     

    }
}
