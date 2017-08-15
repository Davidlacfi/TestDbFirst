using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class DeliveryNoteItemViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Raktár")]
        public int Warehouse_Id { get; set; }
        [Display(Name = "Recept/Termék")]
        public int Recipe_Id { get; set; }
        [Display(Name = "Alapanyag")]
        public int Ingredient_Id { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mennyiség megadása kötelező!")]
        [Display(Name = "Mennyiség (kg)")]
        public decimal Quantity { get; set; }
    }
}