using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithTwoParent : FilterModel
    {
        public IdCodeName Parent1 { get; set; }
        public IdCodeName Parent2 { get; set; }
    }
}