using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class DTODeliveryNote
    {
        public DTODeliveryNote()
        {
            DeliveryNoteItems = new List<DTODeliveryNoteItem>();
            Customer = new List<Customer>();
        }
        public int Id { get; set; }
        public List<DTODeliveryNoteItem> DeliveryNoteItems { get; set; }
        public DateTime? DeliveryNoteDate { get; set; }
        public int? Customer_Id { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ChangedBy { get; set; }
        public string ChangedByName { get; set; }
        public DateTime? ChangedDate { get; set; }
        public List<Customer> Customer { get; set; }

    }
}