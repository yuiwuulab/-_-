using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBMVCFinal.Models;

namespace DBMVCFinal.ViewModels
{
    public class MTC
    {
        public Transaction Transactiondata
        {
            get;
            set;
        }


        public Contains Containdata
        {
            get;
            set;
        }
        public  IEnumerable<Card >  Carddata 
        {
            get;
            set;
        }

        public IEnumerable<Owns> owndata
        {
            get;
            set;
        }
    }
}