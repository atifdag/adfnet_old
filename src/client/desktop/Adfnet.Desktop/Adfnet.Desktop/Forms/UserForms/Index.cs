using System;
using System.Windows.Forms;

namespace Adfnet.Desktop.Forms.UserForms
{
    public partial class Index : Form
    {
        public Index(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            
        }


    }
}
