using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithLanguage : FilterModel
    {
        public IdCodeName Language { get; set; }
    }
}