﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ClassLibraryServices.DataContracts
{
    [DataContract]
    public class PermanentPriceServiceType
    {        
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Price { get; set; }

        [DataMember]
        public string Group { get; set; }
        
    }
}
