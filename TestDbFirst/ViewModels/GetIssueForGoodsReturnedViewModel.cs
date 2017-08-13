using System;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class GetIssueForGoodsReturnedViewModel
    {
        [Display(Name = "Szállítólevél száma")]
        public string DeliveryNote_Number { get; set; }
    }
}