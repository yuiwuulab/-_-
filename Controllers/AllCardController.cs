using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBMVCFinal.Models;

namespace DBMVCFinal.Controllers
{
    public class AllCardController : Controller
    {
        DBFinal7Entities db = new DBFinal7Entities();
        // GET: AllCard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CardIndex()
        {
            var cards = db.Card.ToList();
            return View(cards);
        }

        public ActionResult CardEdit(int? Id)
        {
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            ViewBag.Manager = ismanager;

            //檢查是否有員工編號
            if (Id == null)
            {
                return Content("查無此資料");
            }
            //以Id尋找員工編號
            Card card = db.Card.Find(Id);
            if (card == null)
            {
                return HttpNotFound();
            }
            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CardEdit(Card card)
        {

            bool ismanager = Convert.ToBoolean(Session["isManager"]);

            
            if (ModelState.IsValid)
            {
                db.Entry(card).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CardIndex");
            }
            return View(card);
        }

        public ActionResult CardCreate()
        {
            //int count = db.Member
            // .OrderByDescending(m => m.mId)
            // .Select(m => m.mId)
            // .First();
            //ViewBag.Count = count + 1;

            //if (User.Identity.IsAuthenticated)
            //{
            //    int mId = Convert.ToInt32(User.Identity.Name);
            //    //判斷是否為管理者，是管理者的話就create完成會跳回成員列表
            //    var mresult = db.Member.Where(m => m.mId == mId).FirstOrDefault();
            //    bool manager = mresult.isManager;
            //    ViewBag.Manager = manager;

            //}
            int count = db.Card
             .OrderByDescending(m => m.cId)
             .Select(m => m.cId)
             .First();
            ViewBag.Count = count + 1;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CardCreate(Card card)
        {


            int count = db.Card
              .OrderByDescending(m => m.cId)
              .Select(m => m.cId)
              .First();

            card.cId = count + 1;
            //用ModelState.IsValid判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                //通過驗證，將資料異動儲存到資料庫
                db.Card.Add(card);
                db.SaveChanges();
                //儲存完成後，導向Index動作方法
                return RedirectToAction("CardIndex");
            }

            return View(card);
        }







        //刪除卡片
        public ActionResult CardDelete(int? id)
        {

            if (id == null)
            {
                return Content("查無資料，請提供卡片編號");
            }

            Card card = db.Card.Find(id);
            if (card == null)
            {
                return HttpNotFound();

            }


            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CardDelete(int id)
        {
            Card card = db.Card.Find(id);
            db.Card.Remove(card);
            db.SaveChanges();
            return RedirectToAction("CardIndex");
        }


    }
}