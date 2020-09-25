using System;
using ADF.Net.Core.GenericCrudModels;

namespace ADF.Net.Core
{
    public interface ICrudService<T> : ITransientDependency where T : class, IServiceModel, new()
    {
        ListModel<T> List(FilterModel filterModel);

        DetailModel<T> Detail(Guid id);

        AddModel<T> Add(AddModel<T> addModel);

        UpdateModel<T> Update(UpdateModel<T> updateModel);

        void Delete(Guid id);
    }
}
