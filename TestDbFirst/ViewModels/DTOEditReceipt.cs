using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class DTOEditReceipt
    {
        public DTOEditReceipt()
        {
            DeliveryNote = new DTODeliveryNote();
            MovementType = new List<MovementType>();
        }
        public int Id { get; set; } //DeliveryNoteId-ja!
        public DTODeliveryNote DeliveryNote { get; set; }
        public string MovementType_Id { get; set; }
        public string MovementTypeName { get; set; }
        public List<MovementType> MovementType { get; set; }
        public int RowId { get; set; }
    }
}