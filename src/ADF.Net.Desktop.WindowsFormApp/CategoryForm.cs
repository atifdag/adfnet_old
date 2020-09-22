using System.Windows.Forms;
using ADF.Net.Service;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public partial class CategoryForm : Form
    {

        public CategoryForm(ICategoryService serviceCategory)
        {
            InitializeComponent(serviceCategory);
        }
    }
}
