using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class CurrentProductStockMetadata
    {
        [Display(Name = "Mennyiség (t)")]
        public decimal Quantity { get; set; }
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
    }
    [MetadataType(typeof(CurrentProductStockMetadata))]
    public partial class CurrentProductStock
    {
    }
}