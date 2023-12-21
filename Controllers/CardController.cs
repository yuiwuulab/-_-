using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBMVCFinal.Models;
using DBMVCFinal.ViewModels;

namespace DBMVCFinal.Controllers
{
    public class CardController : Controller
    {
        DBFinal7Entities db = new DBFinal7Entities();
        MOCModel moc = new MOCModel();


        // 這個是取登入的使用者的id

        public ActionResult UserOwnCard(int ? mId)
        {

            //mId = mId = Convert.ToInt32(User.Identity.Name); 
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if (ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);

            }
            else if(mId == null)
            {
                mId = Convert.ToInt32(User.Identity.Name);
            }
            ViewBag.MemberId = mId;
            var Name = db.Member.Find(mId);
            ViewBag.MemberName = Name.name;

            //if (mId == null )
            //{
            //    mId = Convert.ToInt32(User.Identity.Name);
               
            //}
            //else
            //{
            //    mId = Convert.ToInt32(Session["DetailMId"]);
            //    ViewBag.MemberId = mId;
            //    var Name = db.Member.Find(mId);
            //    ViewBag.MemberName = Name.name;
            //}
            

            moc.CardData = db.Card.ToList();
            moc.OwnData = db.Owns.ToList();

            //開始查詢
            var OwnResult = db.Owns.Where(m => m.mId == mId);
          

            if (OwnResult != null)
            {
                moc.OwnData = OwnResult;
                var card1 = db.Owns.Where(m => m.mId == mId).FirstOrDefault();
                var CResult = moc.CardData.Where(m => m.cId == card1.cId );

                if (CResult != null)
                {
                    moc.CardData = CResult;
                    return View(moc);
                }
                
            }

            return View();

        }






        //創建
        public ActionResult UserOwnCreate()
        {
            int mId = Convert.ToInt32(User.Identity.Name); 
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if (ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);

            }
            ViewBag.mid = mId;
            moc.CardData = db.Card.ToList();
            return View(moc);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserOwnCreate(int OwncId, int OwnNum)
        {
            //取得mId
            int mId = Convert.ToInt32(User.Identity.Name);
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if (ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);

            }



            if (ModelState.IsValid)
            {

                //先找出有沒有已經擁有的卡片
                var Qresult = db.Owns.Where(m => m.cId == OwncId&&m.mId == mId).FirstOrDefault();

                if (Qresult == null) //如果沒有的話，新增一張卡片
                {
                    Owns own = new Owns();
                    own.mId = mId;
                    own.cId = OwncId;
                    own.num = OwnNum;
                    db.Owns.Add(own);
                    db.SaveChanges();

                    var OwnResult = db.Owns.Where(m => m.mId == mId);
                    //塞進去viewmodel中的owndata
                    moc.OwnData = OwnResult;
                    return RedirectToAction("UserOwnCard");
                }
                else //如果有，在現有的基礎上家數字
                {

                    var result = db.Owns.Find(mId,OwncId);
                    result.num += OwnNum;
                    db.SaveChanges();
                    var oresult = db.Owns.Where(m => m.mId == mId);
                    moc.OwnData = oresult;
                    return RedirectToAction("UserOwnCard");
                }
          
            }
            return View(moc);
        }


        // GET: Card
        public ActionResult Index()
        {
            

            return View();
        }



        //查詢
        public ActionResult Search()
        {
            var Qcard = db.Card.ToList();
            return View(Qcard);
        }

        [HttpPost]
        public ActionResult SearchResult(String CName)
        {
            //檢查是否有員工Id的判斷
            if (CName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ////var Qresult = db.Card.Where(m => m.cName.ToString() == CName);
            ////var Qresult = db.Owns;
            //foreach (var a in db.Card.Where(m => m.cName.ToString() == CName))
            //{
            //    var Ocard = db.Owns.Where(m => m.cId == a.cId);
            //    foreach (var b in db.Owns.Where(m => m.cId == a.cId))
            //    {
            //        //Qresult = db.Member.Where(m => m.mId == b.cId);
            //    }
            //}

            var Qa = from o in db.Owns
                     from c in db.Card
                     from m in db.Member
                     where c.cName == CName && c.cId == o.cId && o.mId == m.mId
                     select m;


            return View(Qa.ToList());
        }

    }
}