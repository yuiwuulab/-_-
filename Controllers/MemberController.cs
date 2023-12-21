using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBMVCFinal.Models;
using PagedList;

namespace DBMVCFinal.Controllers
{
    
    public class MemberController : Controller
    {
        DBFinal7Entities db = new DBFinal7Entities();
        // GET: Member

        //如果未登入點選title會顯示這個
        public ActionResult Warning()
        {
            return View();
        }

        int pageSize = 5;
        //所有的會員資訊 (只有管理者看的到)
        public ActionResult MemberIndex(int page = 1)
        {

            int currentPage = page < 1 ? 1 : page;

            ViewBag.Man = Convert.ToBoolean(Session["isManager"]);
            var members = db.Member.ToList();
            var result = members.ToPagedList(currentPage, pageSize);


            return View(result);
        }

        //編輯，如果是會員只能編輯自己的(不包含使用者權限)，管理員可以更該使用者權限
        public ActionResult Edit(int? Id)
        {
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            ViewBag.Manager = ismanager;

            //檢查是否有員工編號
            if (Id == null)
            {
                return Content("查無此資料，請提供員工編號");
            }
            //以Id尋找員工編號
            Member member = db.Member.Find(Id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member member)
        {
           
            bool ismanager =Convert.ToBoolean(Session["isManager"]) ;
           

            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                if (ismanager == true)
                {
                    return RedirectToAction("MemberIndex");
                }
                else
                { 
                    return RedirectToAction("UserOwnCard" , "Card");
                }
            }
            return View(member);
        }



        //會員詳細資料 (資料列旁邊的按鈕)
        public ActionResult MemberDetails(int ? mId)
        {

            Session["DetailMId"] = Convert.ToInt32(mId);
            //以ID尋找員工資料
            Member member = db.Member.Find(mId);  

            //如果沒有找到回傳HttpNotFound
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        //會員登入後導覽列可以看到的資料
        public ActionResult NavBarMemberDetails()
        {


            int mId = Convert.ToInt32(User.Identity.Name);
            //int mId = Convert.ToInt32(Session["mId"]);
            //int signInMid = Convert.ToInt32(Session["SignInMId"]);
            //以ID尋找員工資料
            Member member = db.Member.Find(mId);

            //如果沒有找到回傳HttpNotFound
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        //會員列旁邊的刪除
        public ActionResult Delete(int? Id)
        {

            if (Id == null)
            {
                return Content("查無資料，請提供員工編號");
            }

            Member member = db.Member.Find(Id);
            if (member == null)
            {
                return HttpNotFound();
    
            }
        
            
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id)
        {
           Member member = db.Member.Find(Id);
            db.Member.Remove(member);
            db.SaveChanges();
            return RedirectToAction("MemberIndex");
        }

        //註冊功能，也就是創建會員
        public ActionResult SignIn()
        {

            int count = db.Member
             .OrderByDescending(m => m.mId)
             .Select(m => m.mId)
             .First();
            ViewBag.Count = count + 1;

            if (User.Identity.IsAuthenticated)
            {
                int mId = Convert.ToInt32(User.Identity.Name);
                //判斷是否為管理者，是管理者的話就create完成會跳回成員列表
                var mresult = db.Member.Where(m => m.mId == mId).FirstOrDefault();
                bool manager = mresult.isManager;
                ViewBag.Manager = manager;

            }
          
      

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult SignIn(Member member)
        {


            int count = db.Member
              .OrderByDescending(m => m.mId)
              .Select(m => m.mId)
              .First();


            if (ModelState.IsValid )
            {
                bool ismanager = Convert.ToBoolean(Session["isManager"]);
                if (ismanager == true) //如果是管理者按下的
                {
                    //int mId = Convert.ToInt32(User.Identity.Name);
                    member.mId = count + 1;
                    //將資料庫異動儲存到資料庫
                    db.Member.Add(member);
                    db.SaveChanges();
                    return RedirectToAction("MemberIndex");
                }
                else
                {
                    member.mId = count + 1;
                    //將資料庫異動儲存到資料庫
                    db.Member.Add(member);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }

            }

            return View(member);
        }


       
        //登入功能
        [AllowAnonymous]
        public ActionResult Login()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Member member)
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            
            if (member.name == null && member.password == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Qresult = db.Member.Where(m => m.name == member.name && m.password == member.password).FirstOrDefault();
            
            
            if (Qresult != null && Qresult.isManager ==true)//管理者
            {

                FormsAuthentication.RedirectFromLoginPage(Qresult.mId.ToString(), true);
                Session["isManager"] = Qresult.isManager;
                return RedirectToAction("MemberIndex");
            }
            else if(Qresult != null && Qresult.isManager == false)//使用者
            {
                FormsAuthentication.RedirectFromLoginPage(Qresult.mId.ToString(), true);
                return RedirectToAction("UserOwnCard", "Card");
            }
            else
            {
                ViewBag.Err = "帳密錯誤";
            }

           
            return View();

        }

        //登出功能
        public ActionResult LogOut()
        {

            FormsAuthentication.SignOut();
            Session.Clear();

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            Response.Redirect("Login");

            return RedirectToAction("Login");
        }



    }
}