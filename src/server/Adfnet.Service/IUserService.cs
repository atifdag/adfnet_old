using Adfnet.Core;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IUserService : ICrudService<UserModel>
    {
        ListModel<UserModel> List(FilterModelWithMultiParent filterModel);
        MyProfileModel MyProfile();
        void UpdateMyPassword(UpdatePasswordModel model);
        void UpdateMyInformation(UpdateInformationModel model);
    }
}
