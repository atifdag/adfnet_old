using System;
using Adfnet.Core.GenericCrudModels;

namespace Adfnet.Core
{

    /// <summary>
    /// CRUD işlemleri yapan sınıflar için arayüz
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrudService<T> : ITransientDependency where T : class, IServiceModel, new()
    {

        /// <summary>
        /// Filtreleme yaparak birden çok satır içeren liste modelini döndürür.
        /// </summary>
        /// <param name="filterModel">Filtreleme İçin Sınıf</param>
        /// <returns>T türünden liste modeli</returns>
        ListModel<T> List(FilterModel filterModel);

        /// <summary>
        /// ID parametresi alarak tek satır içeren detay modelini döndürür.
        /// </summary>
        /// <param name="id">ID parametresi</param>
        /// <returns></returns>
        DetailModel<T> Detail(Guid id);

        /// <summary>
        /// Ekleme işlemi için gerekli modeli hazırlar.
        /// </summary>
        /// <returns>Ekleme işlemi için gerekli model</returns>
        AddModel<T> Add();

        /// <summary>
        /// Ekleme işlemini yaparak sonucu modelle döndürür.
        /// </summary>
        /// <param name="addModel"></param>
        /// <returns>Ekleme işlemi sonucu oluşan model</returns>
        AddModel<T> Add(AddModel<T> addModel);

        /// <summary>
        /// ID parametresi alarak güncellemesi yapılacak gerekli modeli hazırlar.
        /// </summary>
        /// <param name="id">ID parametresi</param>
        /// <returns>Güncelleme işlemi için gerekli model</returns>
        UpdateModel<T> Update(Guid id);

        /// <summary>
        /// Güncelleme işlemini yaparak sonucu modelle döndürür.
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns>Güncelleme işlemi sonucu oluşan model</returns>
        UpdateModel<T> Update(UpdateModel<T> updateModel);

        /// <summary>
        /// ID parametresi alarak silme işlemini yapar.
        /// </summary>
        /// <param name="id">ID parametresi</param>
        void Delete(Guid id);

    }
}
