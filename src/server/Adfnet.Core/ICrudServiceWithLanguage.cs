using System;
using Adfnet.Core.GenericCrudModels;

namespace Adfnet.Core
{

    public interface ICrudServiceWithLanguage<T> : ICrudService<T> where T : class, IServiceModel, new()
    {

        ListModel<T> List(FilterModel filterModel, Guid languageId);
        DetailModel<T> Detail(Guid id, Guid languageId);
        UpdateModel<T> Update(Guid id, Guid languageId);
        void Delete(Guid id, Guid languageId);

    }
}
