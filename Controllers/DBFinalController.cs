using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBMVCFinal.Models;
using System.Net;
using System.Data.Entity;
using System.Web.Security;
using DBMVCFinal.ViewModels;

namespace DBMVCFinal.Controllers
{
    public class DBFinalController : Controller
    {
        DBFinal7Entities db = new DBFinal7Entities();
        CTCModel ctc = new CTCModel();
        MTC mtc = new MTC();
        
        // GET: dbEmp

        //個人交易紀錄 
        public ActionResult OwnTransactionDetails(int? tId)
        {

            int mId = mId = Convert.ToInt32(User.Identity.Name); ;
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if(ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);
             
            }
           

            ViewBag.mId = mId;
            var Name = db.Member.Where(m => m.mId == mId).FirstOrDefault();
            ViewBag.Name = Name.name;
            var transactions = db.Transaction.Where(m => m.mId == mId);
            return View(transactions);



        }


        public ActionResult MoreDetails(int ? tId)
        {


            var tResult = db.Transaction.Where(m => m.tId == tId).FirstOrDefault();
            ctc.TransactionData = tResult;

            var containResult = db.Contains.Where(m => m.tId == tId);
            ctc.ContainData = containResult;

            ctc.CardData = db.Card.ToList();


            return View(ctc);
        }


        public ActionResult CreateDetail()
        {

            int mId = mId = Convert.ToInt32(User.Identity.Name); ;
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if (ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);

            }

            ViewBag.mid = mId;

            int count = db.Transaction
             .OrderByDescending(m => m.tId)
             .Select(m => m.tId)
             .First();
            ViewBag.TID = count + 1;

            var Oresult = db.Owns.Where(m => m.mId == mId);
            mtc.owndata = Oresult;
            mtc.Carddata = db.Card.ToList();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//"mId//,tId,payment,tDate,cId,num,price"
        public ActionResult CreateDetail(MTC mTC)
        {

            int mId = mId = Convert.ToInt32(User.Identity.Name); ;
            bool ismanager = Convert.ToBoolean(Session["isManager"]);
            if (ismanager == true)
            {
                mId = Convert.ToInt32(Session["DetailMId"]);

            }


            //取得交易編號
            int count = db.Transaction
               .OrderByDescending(m => m.tId)
               .Select(m => m.tId)
               .First();
            ViewBag.TID = count + 1;

            mTC.Transactiondata.mId = mId;
            DateTime da = mTC.Transactiondata.tDate;

            //用ModelState.IsValid判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                mTC.Transactiondata.tId = count + 1;
                mTC.Containdata.tId = count + 1;
                //db.Transaction.Add(mTC.Transactiondata);
                //db.SaveChanges();
                //int T = count + 1;
                var QQ = mTC.Transactiondata;
                QQ.tDate = da;
                db.Transaction.Add(QQ);
                db.SaveChanges();
                db.Contains.Add(mTC.Containdata);
                db.SaveChanges();


                return RedirectToAction("OwnTransactionDetails");

            }

           

            return View(mtc);
   
        }





    }
}