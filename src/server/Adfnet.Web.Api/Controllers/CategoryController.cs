using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class CategoryController : BaseCrudWithLanguageApiController<CategoryModel>
    {
        public CategoryController(ICategoryService serviceCategory) : base(serviceCategory)
        {
        }
    }
}
