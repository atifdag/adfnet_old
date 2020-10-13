namespace Adfnet.Service.Models
{
    public class UpdatePasswordModel
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
