using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ninenight.Models;
using System.Diagnostics;

namespace ninenight.Services
{

    public class UUID
    {


        public class UserDBService
        {

            private readonly static string cnstr = ConfigurationManager.
      ConnectionStrings["ASP.NET MVC"].ConnectionString;

            private readonly SqlConnection conn = new SqlConnection(cnstr);

            #region
            public void Register(User newMember)
            {//將密碼Hash過

                newMember.user_pwd = HashPassword(newMember.user_pwd);
                string sql = $@"Insert Into user (user_id,user_account,user_pwd,user_name,user_gender,user_birthday,user_email,
            user_authcode,user_phone,user_address,user_level,create_id,create_time)VALUES('{newMember.user_id}','{newMember.user_account}','{newMember.user_pwd}',
            '{newMember.user_name}','{newMember.user_gender}','{DateTime.Now.ToString("yyyy-MM-dd")}','{newMember.user_email}','{newMember.user_authcode}','{newMember.user_phone}','{newMember.user_address}','1,'{newMember.create_id}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }

            }
            #endregion
            //Hash密碼
            //用的方法
            public string HashPassword(string user_pwd)
            {
                //宣告Hash時所添加的無意義亂數值
                string satltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
                string saltAndPassword = String.Concat(user_pwd, satltkey);
                SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
                byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
                byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
                string Hashresult = Convert.ToBase64String(HashDate);
                return Hashresult;

            }
            #region
            //查詢一筆資料
            //藉由帳號取得單筆資料的方法
            private User GetDataByAccount(string user_account)
            {
                User Data = new User();
                string sql = $@"select * from user where user_account ='{user_account}'";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    Data.user_id = dr["user_id"].ToString();
                    Data.user_account = dr["user_account"].ToString();
                    Data.user_name = dr["user_name"].ToString();
                    Data.user_gender = Convert.ToBoolean(dr["user_gender"]);
                    Data.user_birthday = Convert.ToDateTime(dr["CreateTime"]);
                    Data.user_email = dr["user_gender"].ToString();
                    Data.user_authcode = dr["user_gender"].ToString();
                    Data.user_phone = dr["user_gender"].ToString();
                    Data.user_address = dr["user_gender"].ToString();
                    Data.user_level = Convert.ToBoolean(dr["user_level"]);
                    Data.create_id = dr["create_id"].ToString();
                    Data.create_time = Convert.ToDateTime(dr["create_time"]);

                }
                catch (Exception e)
                {
                    Data = null;
                }
                finally
                {
                    conn.Close();
                }

                return Data;


            }


            #endregion
            #region 帳號註冊重複確認
            // 確認要註冊帳號是否有被註冊過的方法
            public bool AccountCheck(string user_account)
            {
                // 藉由傳入帳號取得會員資料
                User Data = GetDataByAccount(user_account);
                // 判斷是否有查詢到會員
                bool result = (Data == null);
                // 回傳結果
                return result;
            }
            #endregion
            #region 信箱驗證
            // 信箱驗證碼驗證方法
            public string EmailValidate(string user_account, string user_authcode)
            {
                // 取得傳入帳號的會員資料
                User ValidateMember = GetDataByAccount(user_account);
                // 宣告驗證後訊息字串
                string ValidateStr = string.Empty;
                if (ValidateMember != null)
                {
                    // 判斷傳入驗證碼與資料庫中是否相同
                    if (ValidateMember.user_authcode == user_authcode)
                    {
                        // 將資料庫中的驗證碼設為空
                        //sql 更新語法
                        string sql = $@" update user set user_authcode = '{string.
                       Empty}' where Account = '{user_authcode}' ";
                        try
                        {
                            // 開啟資料庫連線
                            conn.Open();
                            // 執行 Sql 指令
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            // 丟出錯誤
                            throw new Exception(e.Message.ToString());
                        }
                        finally
                        {
                            // 關閉資料庫連線
                            conn.Close();
                        }
                        ValidateStr = " 帳號信箱驗證成功，現在可以登入了 ";
                    }
                    else
                    {
                        ValidateStr = " 驗證碼錯誤，請重新確認或再註冊 ";
                    }
                }
                else
                {
                    ValidateStr = " 傳送資料錯誤，請重新確認或再註冊 ";
                }
                // 回傳驗證訊息
                return ValidateStr;
            }

            #endregion
            #region 登入確認
            // 登入帳密確認方法，並回傳驗證後訊息
            public string LoginCheck(string user_authcode, string user_pwd)
            {
                // 取得傳入帳號的會員資料
                User LoginUser = GetDataByAccount(user_authcode);
                // 判斷是否有此會員
                if (LoginUser != null)
                {
                    // 判斷是否有經過信箱驗證，有經驗證驗證碼欄位會被清空
                    if (String.IsNullOrWhiteSpace(LoginUser.user_authcode))
                    {
                        // 進行帳號密碼確認
                        if (PasswordCheck(LoginUser, user_pwd))
                        {
                            return "";
                        }
                        else
                        {
                            return " 密碼輸入錯誤 ";
                        }
                    }
                    else
                    {
                        return " 此帳號尚未經過 Email 驗證，請去收信 ";
                    }
                }
                else
                {
                    return " 無此會員帳號，請去註冊 ";
                }
            }
            #endregion
            #region 密碼確認
            // 進行密碼確認方法
            public bool PasswordCheck(User CheckMember, string user_pwd)
            {
                // 判斷資料庫裡的密碼資料與傳入密碼資料 Hash 後是否一樣
                bool result = CheckMember.user_pwd.Equals(HashPassword(user_pwd));
                // 回傳結果
                return result;
            }
            #endregion
            #region 取得角色
            // 取得會員的權限角色資料
            public string GetRole(string user_account)
            {
                // 宣告初始角色字串
                string Role = "User";
                // 取得傳入帳號的會員資料
                User LoginUser = GetDataByAccount(user_account);
                // 判斷資料庫欄位，用以確認是否為 Admon
                if (LoginUser.user_level)
                {
                    Role += ",Admin"; // 添加 Admin
                }
                // 回傳最後結果
                return Role;
            }
            #endregion
            #region 變更密碼
            // 變更會員密碼方法，並回傳最後訊息
            public string ChangePassword(string user_account, string user_pwd, string newPassword)

            {
                // 取得傳入帳號的會員資料
                User LoginMember = GetDataByAccount(user_account);
                // 確認舊密碼正確性
                if (PasswordCheck(LoginMember, user_pwd))
                {
                    // 將新密碼 Hash 後寫入資料庫中
                    LoginMember.user_pwd = HashPassword(newPassword);
                    //sql 修改語法
                    string sql = $@" update Members set Password = '{LoginMember.
                   user_pwd}' where Account = '{user_account}' ";
                    try
                    {
                        // 開啟資料庫連線
                        conn.Open();
                        // 執行 Sql 指令
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        // 丟出錯誤
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        // 關閉資料庫連線
                        conn.Close();
                    }
                    return " 密碼修改成功 ";
                }
                else
                {
                    return " 舊密碼輸入錯誤 ";
                }
            }
            #endregion
        }
    }
}
