using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class DTOReceipt
    {
        public DTOReceipt()
        {
            DeliveryNote = new DTODeliveryNote();
        }
        public int Id { get; set; } //DeliveryNoteId-ja!
        public DTODeliveryNote DeliveryNote { get; set; }
        public string MovementType_Id { get; set; }
        public string MovementTypeName { get; set; }

    }
}