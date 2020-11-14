using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Index = Adfnet.Desktop.Forms.UserForms.Index;

namespace Adfnet.Desktop.Forms
{
    public partial class MainForm : Form
    {
        public MainForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            Hide();
            _serviceProvider.GetRequiredService<Index>().Show();
        }
    }
}
