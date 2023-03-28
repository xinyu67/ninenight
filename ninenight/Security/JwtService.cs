using Jose;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ninenight.Security
{
    public class JwtService
    {
        #region 製作 Token
        public string GenerateToken(string user_account, string Role)
        {
            JwtObject jwtObject = new JwtObject
            {
                user_account = user_account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32
           (WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };
            // 從 Web.Config 取得密鑰
            string SecretKey = WebConfigurationManager.AppSettings
           ["SecretKey"].ToString();
            //JWT 的內容
            var payload = jwtObject;
            // 將資料加密為 Token
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes
           (SecretKey), JwsAlgorithm.HS512);
            return token;
        }
        #endregion
    }
}