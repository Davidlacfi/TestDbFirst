using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst.Models
{
    public class StockOperationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Raktár")]
        public int Warehouse_Id { get; set; }
        [Display(Name = "Recept/Termék")]
        public int Recipe_Id { get; set; }
        [Required(ErrorMessage = "Mennyiség megadása kötelező!")]
        [Display(Name = "Mennyiség (t)")]
        public decimal Quantity { get; set; }
        [Required(ErrorMessage = "Alapanyag korrekció esetén megjegyzés megadása kötelező!")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
        [Display(Name = "Aktív")]
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ChangedBy { get; set; }
        public Nullable<System.DateTime> ChangedDate { get; set; }
        [Display(Name = "Mozgás típusa")]
        public int MovementType_Id { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual MovementType MovementType { get; set; }

    }
}