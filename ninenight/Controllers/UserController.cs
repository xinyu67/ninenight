using ninenight.Security;
using ninenight.Services;
using ninenight.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ninenight.Controllers
{
    [ApiController]
    [Route("api/[UserController]")]
    public class UserController : Controller
    {
        
        private readonly UserDBService userService = new UserDBService();
        private readonly MailDBService mailService = new MailDBService();

        public ActionResult Index()
        {
            return View();
        }


        #region 註冊
        // 註冊一開始顯示頁面
        public ActionResult Register()
        {
            // 判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User");
            // 已登入則重新導向
            // 否則進入註冊畫面
            return View();
        }
        // 傳入註冊資料的 Action
        [HttpGet]
        [Route("Register")]
        // 設定此 Action 只接受頁面 POST 資料傳入
        public ActionResult Register(UserRegisterViewModels RegisterMember)
        {
            // 判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                // 將頁面資料中的密碼欄位填入
                RegisterMember.newMember.user_pwd = RegisterMember.user_pwd;
                // 取得信箱驗證碼
                string user_authcode = mailService.GetValidateCode();
                // 將信箱驗證碼填入
                RegisterMember.newMember.user_authcode = user_authcode;
                // 呼叫 Serrvice 註冊新會員
                userService.Register(RegisterMember.newMember);
                // 取得寫好的驗證信範本內容
                string TempMail = System.IO.File.ReadAllText(

                Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                // 宣告 Email 驗證用的 Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "User"
                , new { Account = RegisterMember.newMember.user_account, user_authcode = user_authcode })
                };


                // 藉由 Service 將使用者資料填入驗證信範本中
                string MailBody = mailService.GetRegisterMailBody(TempMail,
               RegisterMember.newMember.user_name, ValidateUrl.ToString().Replace("%3F", "?"));
                // 呼叫 Service 寄出驗證信
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.user_email);
                // 用 TempData 儲存註冊訊息
                TempData["RegisterState"] = " 註冊成功，請去收信以驗證 Email";
                // 重新導向頁面
                return RedirectToAction("RegisterResult");
            }
            // 未經驗證清空密碼相關欄位
            RegisterMember.user_pwd = null;
            RegisterMember.PasswordCheck = null;
            // 將資料回填至 View 中
            return View(RegisterMember);
        }
        // 註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }
        // 判斷註冊帳號是否已被註冊過 Action
        public JsonResult AccountCheck(UserRegisterViewModels RegisterMember)
        {
            // 呼叫 Service 來判斷，並回傳結果
            return Json(userService.AccountCheck(RegisterMember.newMember.user_account), JsonRequestBehavior.AllowGet);

        }

        // 接收驗證信連結傳進來的 Action
        public ActionResult EmailValidate(string user_account, string user_authcode)
        {
            // 用 ViewData 儲存，使用 Service 進行信箱驗證後的結果訊息
            ViewData["EmailValidate"] = userService.EmailValidate(user_account,
           user_authcode);
            return View();
        }
        #endregion



        #region 登入
        // 登入一開始載入畫面
        public ActionResult Login()
        {
            // 判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User"); // 已登入則重新導向
            return View();// 否則進入登入畫面
        }
        // 傳入登入資料的 Action
        [HttpPost] // 設定此 Action 只接受頁面 POST 資料傳入
        [Route("Login")]
        public ActionResult Login(UserLoginViewModels LoginMember)
        {
            // 使用 Service 裡的方法來驗證登入的帳號密碼
            string ValidateStr = userService.LoginCheck(LoginMember.user_account,
           LoginMember.user_pwd);
            // 判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrEmpty(ValidateStr))
            {
                // 無錯誤訊息，則登入
                // 先藉由 Service 取得登入者角色資料
                string RoleData = userService.GetRole(LoginMember.user_account);
                // 設定 JWT
                JwtService jwtService = new JwtService();
                // 從 Web.Config 撈出資料
                //Cookie 名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].
                ToString();
                string Token = jwtService.GenerateToken(LoginMember.user_account, RoleData);
                //// 產生一個 Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                // 設定單值
                cookie.Value = Server.UrlEncode(Token);
                // 寫到用戶端
                Response.Cookies.Add(cookie);
                // 設定 Cookie 期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.
                ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                // 重新導向頁面
                return RedirectToAction("Index", "Guestbooks");
            }
            else
            {
                // 有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                // 將資料回填至 View 中
                return View(LoginMember);
            }
        }

        #region 修改密碼
        //修改密碼一開始載入頁面
        [Authorize] //設定此 Action 須登入
        public ActionResult ChangePassword()
        {
            return View();
        }
        //修改密碼傳入資料 Action
        [Authorize] //設定此 Action 須登入
        [HttpPost] //設定此 Action 接受頁面 POST 資料傳入
        [Route("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordViewModels ChangeData)
        {
            //判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = userService.ChangePassword(User.Identity.Name, ChangeData.user_pwd, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion

        //----------------------------------------------------------------------

        #endregion
        #region 登出
        // 登出 Action
        [Authorize] // 設定此 Action 須登入
        public ActionResult Logout()
        {
            // 使用者登出
            //Cookie 名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].
           ToString();
            // 清除 Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            // 重新導向至登入 Action
            return RedirectToAction("Login");
        }
        #endregion


    }
}