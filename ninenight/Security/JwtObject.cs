using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ninenight.Security
{
    public class JwtObject
    {    //JWT 內容設計Object
         // 內容隨意設計，但注意，不要將太過於重要的資料放入其中，到期時間一定要記得設定。
        public string user_account { get; set; }
        public string Role { get; set; }
        // 到期時間
        public string Expire { get; set; }

    }



}
