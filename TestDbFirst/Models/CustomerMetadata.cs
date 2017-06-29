using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class CustomerMetadata
    {


        [Display(Name = "Név")]
        public string Name { get; set; }
        [Display(Name = "Irányítószám")]
        public string ZipCode { get; set; }
        [Display(Name = "Település")]
        public string City { get; set; }
        [Display(Name = "Cím")]
        public string StreetAddress { get; set; }
        [Display(Name = "Kontakt személy 1")]
        public string ContactPerson1 { get; set; }
        [Display(Name = "Telefon 1")]
        public string Telephone1 { get; set; }
        [Display(Name = "Kontakt személy 2")]
        public string ContactPerson2 { get; set; }
        [Display(Name = "Telefon2")]
        public string Telephone2 { get; set; }
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