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
    
    public partial class DeliveryNoteItem
    {
        public int Id { get; set; }
        public Nullable<int> IngredientMovement_Id { get; set; }
        public Nullable<int> ProductMovement_Id { get; set; }
    
        public virtual ProductMovement ProductMovement { get; set; }
        public virtual IngredientMovement IngredientMovement { get; set; }
    }
}
