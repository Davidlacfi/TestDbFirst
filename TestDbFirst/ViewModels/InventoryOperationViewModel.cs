using System;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class InventoryOperationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Raktár")]
        public int Warehouse_Id { get; set; }
        [Display(Name = "Recept/Termék")]
        public int Recipe_Id { get; set; }
        [Display(Name = "Alapanyag")]
        public int Ingredient_Id { get; set; }
        [Display(Name = "Partner")]
        public int Customer_Id { get; set; }
        [Display(Name = "Szállítólevél")]
        public int DeliveryNote_Id { get; set; }
        [Display(Name = "Szállítólevél száma")]
        public string DeliveryNote_Number { get; set; }
        [Display(Name = "Szállítólevél megjegyzés")]
        [DataType(DataType.MultilineText)]
        public string DeliveryNote_Remark { get; set; }
        [Required(ErrorMessage = "Mennyiség megadása kötelező!")]
        [Display(Name = "Mennyiség (kg)")]
        public decimal Quantity { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Bevételezés megjegyzése")]
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
        public virtual Ingredient Ingredient { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual DeliveryNote DeliveryNote { get; set; }
    }
}