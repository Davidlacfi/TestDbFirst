using System;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst
{
    public class CustomerMetadata
    {


        [Display(Name = "Partner Név")]
        [Required(ErrorMessage = "Partner név megadása kötelező!")]
        public string Name { get; set; }
        [Display(Name = "Irányítószám")]
        [Required(ErrorMessage = "Irányítószám megadása kötelező!")]
        public string ZipCode { get; set; }
        [Display(Name = "Település")]
        [Required(ErrorMessage = "Település megadása kötelező!")]
        public string City { get; set; }
        [Display(Name = "Cím")]
        [Required(ErrorMessage = "Cím megadása kötelező!")]
        public string StreetAddress { get; set; }
        [Display(Name = "Kontakt személy 1")]
        [Required(ErrorMessage = "Legalább egy kontakt személy megadása kötelező!")]
        public string ContactPerson1 { get; set; }
        [Display(Name = "Telefon 1")]
        [Required(ErrorMessage = "Legalább egy telefonszám megadása kötelező!")]
        public string Telephone1 { get; set; }
        [Display(Name = "Kontakt személy 2")]
        public string ContactPerson2 { get; set; }
        [Display(Name = "Telefon2")]
        public string Telephone2 { get; set; }
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

    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
    }
}