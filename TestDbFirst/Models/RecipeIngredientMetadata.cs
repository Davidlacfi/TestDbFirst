using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class RecipeIngredientMetadata
    {
        [Display(Name = "Recept")]
        public int Recipe_Id { get; set; }
        [Display(Name = "Alapanyag")]
        public int Ingredient_Id { get; set; }
        [Display(Name = "Mennyiség (kg)")]
        [Required(ErrorMessage = "Mennyiség megadása kötelező!")]
        public decimal Ammount { get; set; }
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
        [Display(Name = "Aktív")]
        public bool IsActive { get; set; }
        [Display(Name = "Létrehozta")]
        public Nullable<int> CreatedBy { get; set; }
        [Display(Name = "Létrehozás dátuma")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Display(Name = "Utoljára módosította")]
        public Nullable<int> ChangedBy { get; set; }
        [Display(Name = "Utolsó módosítás dátuma")]
        public Nullable<System.DateTime> ChangedDate { get; set; }

    }
    [MetadataType(typeof(RecipeIngredientMetadata))]
    public partial class RecipeIngredient
    {
    }
}