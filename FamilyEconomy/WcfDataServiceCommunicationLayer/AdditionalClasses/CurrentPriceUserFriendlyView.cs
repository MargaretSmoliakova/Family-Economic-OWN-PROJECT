using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyEconomy.WcfDataServiceCommunicationLayer.AdditionalClasses
{
    public class CurrentPriceUserFriendlyView
    {
        public string Name { get; set; }

        public string Price { get; set; }

        public int? Check_box { get; set; }

        public int? Consideration { get; set; }
    }
}
