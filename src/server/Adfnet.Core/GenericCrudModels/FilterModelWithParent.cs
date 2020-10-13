using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithParent : FilterModel
    {
        public IdCodeName Parent { get; set; }
    }
}