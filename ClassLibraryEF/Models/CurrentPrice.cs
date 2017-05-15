namespace ClassLibraryEF.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CurrentPrice
    {
        public int Id { get; set; }

        public int? Id_monthly_prices { get; set; }

        public int? Id_permanent_prices { get; set; }

        [StringLength(20)]
        public string Price { get; set; }

        public int? Check_box { get; set; }

        public int? Consideration { get; set; }

        public virtual MonthlyPrice MonthlyPrice { get; set; }

        public virtual PermanentPrice PermanentPrice { get; set; }
    }
}
