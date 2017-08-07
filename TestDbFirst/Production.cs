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
    
    public partial class Production
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Production()
        {
            this.IngredientMovements = new HashSet<IngredientMovement>();
            this.ProductMovements = new HashSet<ProductMovement>();
        }
    
        public int Id { get; set; }
        public int Recipe_Id { get; set; }
        public int Customer_Id { get; set; }
        public int Destination_Warehouse_Id { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ChangedBy { get; set; }
        public Nullable<System.DateTime> ChangedDate { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IngredientMovement> IngredientMovements { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMovement> ProductMovements { get; set; }
    }
}
