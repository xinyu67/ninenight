using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ninenight.Models
{
    public class User
    {
        //編號
        public string user_id { get; set; }

        //會員帳號
        [DisplayName("帳號:")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "帳號長度需介於8-20字元")]
        [Remote("AccountCheck", "User", ErrorMessage = "此帳號已被註冊過")]
        public string user_account { get; set; }


        // 密碼
        public string user_pwd { get; set; }


        // 姓名
        [DisplayName("姓名:")]
        [StringLength(15, ErrorMessage = "不可超過15字元")]
        public string user_name { get; set; }

        //性別
        public bool user_gender { get; set; }
        //生日
        public DateTime user_birthday { get; set; }

        // 電子信箱
        [DisplayName("Eamil:")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(50, ErrorMessage = "Eamil長度最多50字元")]
        [EmailAddress(ErrorMessage = "這不是Eamil格式")]
        public string user_email { get; set; }

        // 信箱驗證碼

        public string user_authcode { get; set; }


        [DisplayName("手機號碼:")]
        [Required(ErrorMessage = "請輸入手機號碼")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "號碼不可超過10字元")]
        public string user_phone { get; set; }
        public string user_address { get; set; }
        // 管理者
        public bool user_level { get; set; }
        public bool isdel { get; set; }

        [DisplayName("新增的人:")]
        public string create_id { get; set; }

        [DisplayName("新增時間:")]
        public DateTime create_time { get; set; }


        public string update_id { get; set; }
        public DateTime? update_time { get; set; }

    }
}