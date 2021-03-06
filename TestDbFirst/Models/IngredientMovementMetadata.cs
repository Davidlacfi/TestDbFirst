﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestDbFirst
{
    public class IngredientMovementMetadata
    {
        [Display(Name = "Mozgás típusa")]
        public int MovementType_Id { get; set; }
        [Display(Name = "Gyártás azonosító")]
        public Nullable<int> Production_Id { get; set; }
        [Display(Name = "Alapanyag")]
        public int Ingredient_Id { get; set; }
        [Display(Name = "Raktár")]
        public int Warehouse_Id { get; set; }
        [Display(Name = "Mennyiség (kg)")]
        public decimal Quantity { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Megjegyzés")]
        public string Remark { get; set; }
        [Display(Name = "Létrehozta")]
        public Nullable<int> CreatedBy { get; set; }
        [Display(Name = "Mozgás időpontja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
    [MetadataType(typeof(IngredientMovementMetadata))]
    public partial class IngredientMovement
    {
    }
}