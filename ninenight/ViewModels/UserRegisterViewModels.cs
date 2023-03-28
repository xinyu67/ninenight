using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ninenight.Models;
namespace ninenight.ViewModels
{
    public class UserRegisterViewModels
    {
        public User newMember { get; set; }

        [DisplayName("密碼:")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string user_pwd { get; set; }

        [DisplayName("確認密碼")]
        [Compare("user-pwd", ErrorMessage = "兩次密碼輸入不一致")]
        [Required(ErrorMessage = "請輸入確認密碼")]
        public string PasswordCheck { get; set; }
    }
}