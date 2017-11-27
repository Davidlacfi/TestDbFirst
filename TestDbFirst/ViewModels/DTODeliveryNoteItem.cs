using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class DTODeliveryNoteItem
    {
        public int Id { get; set; }
        public int? DeliveryNote_Id { get; set; }
        public int? IngredientMovement_Id { get; set; }
        public int? ProductMovement_Id { get; set; }
        public string IngredientMovementRemark { get; set; }
        public int? Ingredient_Id { get; set; }
        public string IngredientName { get; set; }
        public string ProductMovementRemark { get; set; }
        public int? Product_Id { get; set; }
        public string ProductName { get; set; }
        public int? Warehouse_Id { get; set; }
        public string WarehouseName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? Quantity { get; set; }
    }
}