using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ninenight.ViewModels
{
    public class UserLoginViewModels
    {
        [DisplayName("帳號:")]
        [Required(ErrorMessage = "請輸入會員帳號")]
        public string user_account { get; set; }

        [DisplayName("密碼:")]
        [Required(ErrorMessage = "請輸入會員密碼")]
        public string user_pwd { get; set; }

    }
}