using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class WarehouseMetadata
    {
        [Display(Name = "Raktár Név")]
        [Required(ErrorMessage = "Raktár név megadása kötelező!")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
        [Display(Name = "Aktív")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(WarehouseMetadata))]
    public partial class Warehouse
    {
    }
}