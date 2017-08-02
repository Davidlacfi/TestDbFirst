using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class IngredientMetadata
    {
        [Display(Name = "Alapanyag Név")]
        [Required(ErrorMessage = "Alapanyag név megadása kötelező!")]
        public string Name { get; set; }
        [Display(Name = "Aktív")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(IngredientMetadata))]
    public partial class Ingredient
    {
    }
}