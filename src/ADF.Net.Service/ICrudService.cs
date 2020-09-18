using System;
using ADF.Net.Core;
using ADF.Net.Service.GenericCrudModels;

namespace ADF.Net.Service
{
    public interface ICrudService<T> : ITransientDependency where T : class, IServiceModel, new()
    {
        ListModel<T> List(FilterModel filterModel);

        DetailModel<T> Detail(Guid id);

        AddModel<T> Add();

        AddModel<T> Add(AddModel<T> addModel);

        UpdateModel<T> Update(Guid id);

        UpdateModel<T> Update(UpdateModel<T> updateModel);

        void Delete(Guid id);
    }
}
