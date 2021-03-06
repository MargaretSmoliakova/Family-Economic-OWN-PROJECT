﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ClassLibraryServices.DataContracts
{
    [DataContract]
    public class CurrentPriceServiceType
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? Id_monthly_prices { get; set; }

        [DataMember]
        public int? Id_permanent_prices { get; set; }

        [DataMember]
        public string Price { get; set; }

        [DataMember]
        public int? Check_box { get; set; }        
    }
}
