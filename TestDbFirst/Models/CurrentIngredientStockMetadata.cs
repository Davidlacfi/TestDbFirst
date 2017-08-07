using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class CurrentIngredientStockMetadata
    {
        [Display(Name = "Mennyiség (kg)")]
        public decimal Quantity { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
    }
    [MetadataType(typeof(CurrentIngredientStockMetadata))]
    public partial class CurrentIngredientStock
    {
    }
}