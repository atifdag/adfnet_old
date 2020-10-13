using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithLanguageAndParent : FilterModelWithLanguage
    {
        public IdCodeName Parent { get; set; }
    }
}