using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBMVCFinal.Models;

namespace DBMVCFinal.ViewModels
{
    
    public class MOCModel
    {
    
        //管理OWNCard的model們

        public IEnumerable<Owns> OwnData
        {
            get;
            set;
        }


        public IEnumerable<Card> CardData
        {
            get;
            set;
        }

    }
}