using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBMVCFinal.Models;

namespace DBMVCFinal.ViewModels
{
    public class CTCModel
    {
        //管理transaction detail的model
     
        public IEnumerable<Card>  CardData
        {
            get;
            set;
        }

        public Transaction TransactionData
        {
            get;
            set;
        }

        public IEnumerable<Contains> ContainData
        {
            get;
            set;
        }
    }
}