//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestDbFirst
{
    using System;
    using System.Collections.Generic;
    
    public partial class CurrentIngredientStock
    {
        public int Id { get; set; }
        public int Warehouse_Id { get; set; }
        public int Ingredient_Id { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ChangedBy { get; set; }
        public Nullable<System.DateTime> ChangedDate { get; set; }
    
        public virtual Ingredient Ingredient { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
