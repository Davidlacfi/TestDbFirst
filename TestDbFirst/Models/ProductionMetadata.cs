using System;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst
{
    public class ProductionMetadata
    {
        [Display(Name = "Recept")]
        [Required(ErrorMessage = "Recept megadása kötelező!")]
        public int Recipe_Id { get; set; }
        [Display(Name = "Partner")]
        [Required(ErrorMessage = "Partner megadása kötelező!")]
        public int Customer_Id { get; set; }
        [Display(Name = "Raktár")]
        [Required(ErrorMessage = "Raktár megadása kötelező!")]
        public int Destination_Warehouse_Id { get; set; }
        [Display(Name = "Gyártás dátuma")]
        [Required(ErrorMessage = "Gyártás dátumának megadása kötelező!")]
        public System.DateTime ProductionDate { get; set; }
        [Display(Name = "Mennyiség (t)")]
        [Required(ErrorMessage = "Mennyiség megadása kötelező!")]
        public decimal Quantity { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
        [Display(Name = "Aktív")]
        public bool IsActive { get; set; }
        [Display(Name = "Létrehozta")]
        public Nullable<int> CreatedBy { get; set; }
        [Display(Name = "Létrehozás időpontja")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Display(Name = "Utoljára Módosította")]
        public Nullable<int> ChangedBy { get; set; }
        [Display(Name = "Utolsó módosítás időpontja")]
        public Nullable<System.DateTime> ChangedDate { get; set; }
    }
    [MetadataType(typeof(ProductionMetadata))]
    public partial class Production
    {
    }
}