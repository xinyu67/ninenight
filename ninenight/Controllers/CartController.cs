using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ninenight.Controllers
{
    [ApiController]
    [Route("api/[CartController]")]
    public class CartController : Controller
    {
        // 宣告 Cart 相關的 Service 物件
        private readonly CartService cartService = new CartService();

        [Authorize] // 設定此 Action 須登入
        // GET: Cart
        [HttpGet]
        [Route("Index")]
        public ActionResult Index()
        {
            // 宣告一個新頁面模型
            CartBuyViewModel Data = new CartBuyViewModel();
            // 取得 Session 內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null)
                ? HttpContext.Session["Cart"].ToString() : null;
            // 藉由 Service 並根據 Session 內儲存購物車編號
            // 取得以放入購物車的商品資料陣列
            Data.DataList = cartService.GetItemFromCart(Cart);
            // 藉由 Service 來確認購物車是否已保存
            Data.isCartsave = cartService.CheckCartSave(User.Identity.Name, Cart);
            return View();
        }

        //----------------------------------------------------------------------

        #region 保存購物車
        // 保存使用者購物車資料 Action
        [Authorize] // 設定此 Action 須登入
        [HttpPost]
        [Route("CartSave")]
        public ActionResult CartSave(){
            // 宣告接收購物車 Session 資料物件
            string Cart;
            // 判斷 Session 內是否有值
            if (HttpContext.Session["Cart"] != null){
                // 設定購物車值
                Cart = HttpContext.Session["Cart"].ToString();
            }else{
                // 重新定義購物車值
                Cart = DateTime.Now.ToString() + User.Identity.Name;
                // 填入 Session 中
                HttpContext.Session["Cart"] = Cart;
            }
            // 藉由 Service 來儲存購物車資料
            cartService.SaveCart(User.Identity.Name, Cart);
            // 重新導向頁面至開始頁面
            return RedirectToAction("Index");
        }
        #endregion

        //----------------------------------------------------------------------

        #region 取消保存購物車
        // 取消保存使用者購物車資料 Action
        [Authorize] // 設定此 Action 須登入
        [HttpDelete]
        [Route("CartSaveRemove")]
        public ActionResult CartSaveRemove(){
            // 藉由 Service 取消儲存購物車資料
            cartService.SaveCartRemove(User.Identity.Name);
            // 重新導向頁面至開始頁面
            return RedirectToAction("Index");
        }
        #endregion

        //----------------------------------------------------------------------

        #region 從購物車中取出
        // 將商品從購物車取出 Action
        [Authorize] // 設定此 Action 須登入
        [HttpGet]
        [Route("Pop")]
        public ActionResult Pop(int Id, string toPage){
            // 取得 Session 內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null)
            ? HttpContext.Session["Cart"].ToString() : null;
            // 藉由 Service 來將商品從購物車取出
            cartService.RemoveForCart(Cart, Id);
            if (toPage == "Item"){// 判斷傳入的 toPage 來決定導向
                return RedirectToAction("Item", "Item", new { Id = Id });
            }else if (toPage == "ItemBlock"){
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }else{
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion

        //----------------------------------------------------------------------

        #region 放入購物車中
        // 將商品放入購物車中 Action
        [Authorize] // 設定此 Action 須登入
        [HttpPut]
        [Route("Put")]
        public ActionResult Put(int Id, string toPage){
            // 若 Session 中無購物車資料，以使用者名稱與時間，新增一購物車資料
            if (HttpContext.Session["Cart"] == null){
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }
            // 藉由 Service 來將商品放入購物車中
            cartService.AddtoCart(HttpContext.Session["Cart"].ToString(), Id);
            // 判斷傳入的 toPage 來決定導向
            if (toPage == "Item"){
                return RedirectToAction("Item", "Item", new { Id = Id });
            }else if (toPage == "ItemBlock"){
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }else{
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}