using System.Collections.Generic;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Core.GenericCrudModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithMultiParent : FilterModel
    {
        public List<IdCodeName> Parents { get; set; }
    }
}