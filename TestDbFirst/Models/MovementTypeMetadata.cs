using System.ComponentModel.DataAnnotations;

namespace TestDbFirst
{
    public class MovementTypeMetadata
    {
        [Display(Name = "Mozgás Típus")]
        public string Name { get; set; }
    }
    [MetadataType(typeof(MovementTypeMetadata))]
    public partial class MovementType
    {
    }
}