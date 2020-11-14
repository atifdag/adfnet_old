using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Adfnet.Core.Security;
using Adfnet.Desktop.WindowsForms.Forms.HomeForms;
using Adfnet.Service;
using Adfnet.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Desktop.WindowsForms.Forms.AuthenticationForms
{
    public partial class LoginForm : Form
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _serviceAuthentication;
        private readonly IIdentityService _serviceIdentity;
        public LoginForm(IConfiguration configuration, IAuthenticationService serviceAuthentication, IIdentityService serviceIdentity, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceAuthentication = serviceAuthentication;
            _serviceIdentity = serviceIdentity;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            _serviceProvider.GetRequiredService<RegisterForm>().Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {

            try
            {
                var model = new LoginModel
                {
                    Username = TxbUsername.Text,
                    Password = TxbPassword.Text,
                    Key = _configuration.GetSection("JwtSecurityKey").Value
                };
                _serviceAuthentication.Login(model);
                var identity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                _serviceIdentity.Set(identity, DateTime.Now.AddMinutes(20), false);
                Close();
                _serviceProvider.GetRequiredService<HomeForm>().Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
