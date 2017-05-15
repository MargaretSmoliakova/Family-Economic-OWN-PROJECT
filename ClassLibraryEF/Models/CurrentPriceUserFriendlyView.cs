namespace ClassLibraryEF.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CurrentPriceUserFriendlyView
    {
        public int Id { get; set; }

        public string Name { get; set; }        
        
        public string Price { get; set; }

        public int? Check_box { get; set; }        
    }
}
