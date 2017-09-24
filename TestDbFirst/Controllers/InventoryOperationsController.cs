using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TestDbFirst.Models;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class InventoryOperationsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();
        // GET: AddReceipt
        public ActionResult AddReceipt()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "receipt"), "Id", "Name");
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients.OrderBy(i=>i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.Where(i => i.IsSupplier == true).OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

        // POST: AddReceipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceipt([Bind(Include = "Id,deliveryNoteItem,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,DeliveryNote_Date,MovementType_Id,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryReceipt)
        {   
            if (ModelState.IsValid)
            {
                
                var identity = (ClaimsIdentity) User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                var deliveryNoteCreted = false;

                for (int x=0; x< inventoryReceipt.deliveryNoteItem.Count; x++)
                {
                    //INGREDIENTMOVEMENT ELKÉSZÍTÉSE
                    var currentIndex = x;
                    var currentIngredientId = inventoryReceipt.deliveryNoteItem[x].Ingredient_Id;
                    var currentIngredientQuantity = inventoryReceipt.deliveryNoteItem[currentIndex].Quantity;
                    var ingredientMovement = new IngredientMovement
                    {
                        Ingredient_Id = inventoryReceipt.deliveryNoteItem[currentIndex].Ingredient_Id,
                        MovementType_Id = inventoryReceipt.MovementType_Id,
                        Warehouse_Id = inventoryReceipt.deliveryNoteItem[currentIndex].Warehouse_Id,
                        Quantity = inventoryReceipt.deliveryNoteItem[currentIndex].Quantity,
                        Remark = inventoryReceipt.Remark,
                        IsActive = inventoryReceipt.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid)
                    };
                    db.IngredientMovements.Add(ingredientMovement);


                    //CURRENTINGREDIENTSTOCK MÓDOSÍTÁSA
                    var ingredientexists = db.CurrentIngredientStocks.Where(i => i.Ingredient_Id == currentIngredientId);
                    //HA NINCS AZ ALAPANYAGBÓL, HOZZÁADJUK
                    if (!ingredientexists.Any())
                    {
                        db.CurrentIngredientStocks.Add(new CurrentIngredientStock
                        {
                            CreatedDate = DateTime.Now,
                            CreatedBy = Convert.ToInt32(sid),
                            IsActive = true,
                            Warehouse_Id = inventoryReceipt.deliveryNoteItem[currentIndex].Warehouse_Id,
                            Ingredient_Id = inventoryReceipt.deliveryNoteItem[currentIndex].Ingredient_Id,
                            Quantity = inventoryReceipt.deliveryNoteItem[currentIndex].Quantity

                        });
                    }
                    //HA VAN AZ ALAPANYAGBÓL, UPDATE
                    else
                    {
                        var ingredienttoupdate = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == currentIngredientId);
                        var originalingredientquantity = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == currentIngredientId).Quantity;
                        ingredienttoupdate.Quantity = originalingredientquantity + currentIngredientQuantity;
                        ingredienttoupdate.ChangedDate = DateTime.Now;
                        ingredienttoupdate.ChangedBy = Convert.ToInt32(sid);
                        db.Entry(ingredienttoupdate).State = EntityState.Modified;
                    }

                    //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
                    var existingDeliveryNoteId = 0;
                    if (deliveryNoteCreted == false)
                    {
                        var deliveryNote = new DeliveryNote
                        {
                            Customer_Id = inventoryReceipt.Customer_Id,
                            Type = "receipt",
                            Number = inventoryReceipt.DeliveryNote_Number,
                            Remark = inventoryReceipt.DeliveryNote_Remark,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedBy = Convert.ToInt32(sid),
                            DeliveryNoteDate = inventoryReceipt.DeliveryNote_Date
                        };
                        db.DeliveryNotes.Add(deliveryNote);
                        deliveryNoteCreted = true;
                        existingDeliveryNoteId = deliveryNote.Id;
                        //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
                        var deliveryNoteItem = new DeliveryNoteItem
                        {
                            DeliveryNote = deliveryNote,
                            IngredientMovement = ingredientMovement
                        };
                        db.DeliveryNoteItems.Add(deliveryNoteItem);
                    }
                    else
                    {
                        //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
                        var deliveryNoteItem = new DeliveryNoteItem
                        {
                            DeliveryNote_Id = existingDeliveryNoteId,
                            IngredientMovement = ingredientMovement
                        };
                        db.DeliveryNoteItems.Add(deliveryNoteItem);
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    TempData["Operation"] = "success";
                }
                else
                {
                    TempData["Operation"] = "danger";
                }
                return RedirectToAction("Index", "CurrentIngredientStocks");

            }
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "receipt"), "Id", "Name");
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.Where(i => i.IsSupplier == true).OrderBy(i => i.Name), "Id", "Name");
            return View(inventoryReceipt);
        }

        // GET: AddIssue
        public ActionResult AddIssue()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

        // POST: AddIssue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIssue([Bind(Include = "Id,deliveryNoteItem,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,DeliveryNote_Date,MovementType_Id,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryIssue)
        {
            if (ModelState.IsValid)
            {

                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                var deliveryNoteCreted = false;

                for (int x = 0; x < inventoryIssue.deliveryNoteItem.Count; x++)
                {
                    //PRODUCTMOVEMENT ELKÉSZÍTÉSE
                    var currentIndex = x;
                    var currentProductId = inventoryIssue.deliveryNoteItem[currentIndex].Recipe_Id;
                    var currentProductQuantity = inventoryIssue.deliveryNoteItem[currentIndex].Quantity;
                    var productMovement = new ProductMovement
                    {
                        Recipe_Id = inventoryIssue.deliveryNoteItem[currentIndex].Recipe_Id,
                        MovementType_Id = inventoryIssue.MovementType_Id,
                        Warehouse_Id = inventoryIssue.deliveryNoteItem[currentIndex].Warehouse_Id,
                        Quantity = inventoryIssue.deliveryNoteItem[currentIndex].Quantity,
                        Remark = inventoryIssue.Remark,
                        IsActive = inventoryIssue.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid)
                    };
                    db.ProductMovements.Add(productMovement);


                    //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
                    var productexists = db.CurrentProductStocks.Where(i => i.Recipe_Id == currentProductId);

                    //HA NINCS A TERMÉKBŐL, VAGY KEVESEBB, MINT AMENNYIT KI AKAR ADNI, HIBA
                    if (!productexists.Any())
                    {
                        TempData["Operation"] = "danger";
                        TempData["OperationMessage"] = string.Format("A következő termék nincs raktáron: {0}",
                        db.Recipes.First(i=>i.Id == currentProductId).Name);
                        return RedirectToAction("AddIssue", "InventoryOperations");
                    }
                    if (currentProductQuantity > productexists.First().Quantity)
                    {
                        TempData["Operation"] = "danger";
                        TempData["OperationMessage"] = string.Format("Nem áll rendelkezésre a szükséges mennyiség ({0} t) a következő termékből: {1}",
                        currentProductQuantity, productexists.First().Recipe.Name);
                        return RedirectToAction("AddIssue", "InventoryOperations");
                    }
                    //HA VAN A TERMÉKBŐL ÉS ELEGENDŐ IS, UPDATE
                    else
                    {
                        var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == currentProductId);
                        var originalproductquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == currentProductId).Quantity;
                        producttoupdate.Quantity = originalproductquantity - currentProductQuantity;
                        producttoupdate.ChangedDate = DateTime.Now;
                        producttoupdate.ChangedBy = Convert.ToInt32(sid);
                        db.Entry(producttoupdate).State = EntityState.Modified;
                    }

                    //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
                    var existingDeliveryNoteId = 0;
                    if (deliveryNoteCreted == false)
                    {
                        var deliveryNote = new DeliveryNote
                        {
                            Customer_Id = inventoryIssue.Customer_Id,
                            Type = "issue",
                            Number = inventoryIssue.DeliveryNote_Number,
                            Remark = inventoryIssue.DeliveryNote_Remark,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedBy = Convert.ToInt32(sid),
                            DeliveryNoteDate = inventoryIssue.DeliveryNote_Date
                        };
                        db.DeliveryNotes.Add(deliveryNote);
                        deliveryNoteCreted = true;
                        existingDeliveryNoteId = deliveryNote.Id;
                        //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
                        var deliveryNoteItem = new DeliveryNoteItem
                        {
                            DeliveryNote = deliveryNote,
                            ProductMovement = productMovement
                        };
                        db.DeliveryNoteItems.Add(deliveryNoteItem);
                    }
                    else
                    {
                        //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
                        var deliveryNoteItem = new DeliveryNoteItem
                        {
                            DeliveryNote_Id = existingDeliveryNoteId,
                            ProductMovement = productMovement
                        };
                        db.DeliveryNoteItems.Add(deliveryNoteItem);
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    TempData["Operation"] = "success";
                }
                else
                {
                    TempData["Operation"] = "danger";
                }
                return RedirectToAction("Index", "CurrentProductStocks");

            }
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View(inventoryIssue);
        }










        //// GET: Issues
        //public ActionResult AddIssue()
        //{
        //    ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
        //    ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
        //    ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
        //    ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
        //    return View();
        //}

        //// POST: Issues
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddIssue([Bind(Include = "Id,Recipe_Id,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryIssue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var identity = (ClaimsIdentity) User.Identity;
        //        var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
        //        var deliveryNoteCreted = false;

        //        for (int b = 0; b < inventoryIssue.Recipe_Id.Length; b++)
        //        {
        //            var actId = inventoryIssue.Recipe_Id[b];
        //            var actQ = inventoryIssue.Quantity[b];
        //            if (db.CurrentProductStocks.Any(e => e.Recipe_Id == actId) &&
        //                actQ <= db.CurrentProductStocks.First(e => e.Recipe_Id == actId).Quantity)
        //            {
        //                //PRODUCTMOVEMENT ELKÉSZÍTÉSE

        //                var productMovement = new ProductMovement
        //                {
        //                    Recipe_Id = actId,
        //                    MovementType_Id = inventoryIssue.MovementType_Id,
        //                    Warehouse_Id = inventoryIssue.Warehouse_Id,
        //                    Quantity = actQ,
        //                    Remark = inventoryIssue.Remark,
        //                    IsActive = inventoryIssue.IsActive,
        //                    CreatedDate = DateTime.Now,
        //                    CreatedBy = Convert.ToInt32(sid)
        //                };
        //                db.ProductMovements.Add(productMovement);

        //                //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
        //                var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == actId);
        //                var originalproductquantity =
        //                    db.CurrentProductStocks.First(i => i.Recipe_Id == actId).Quantity;
        //                producttoupdate.Quantity = originalproductquantity - actQ;
        //                producttoupdate.ChangedDate = DateTime.Now;
        //                producttoupdate.ChangedBy = Convert.ToInt32(sid);
        //                db.Entry(producttoupdate).State = EntityState.Modified;

        //                //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
        //                var existingDeliveryNoteId=0;
        //                if(deliveryNoteCreted==false)
        //                { 
        //                    var deliveryNote = new DeliveryNote
        //                    {
        //                        Customer_Id = inventoryIssue.Customer_Id,
        //                        Type = "issue",
        //                        Number = inventoryIssue.DeliveryNote_Number,
        //                        Remark = inventoryIssue.DeliveryNote_Remark,
        //                        IsActive = true,
        //                        CreatedDate = DateTime.Now,
        //                        CreatedBy = Convert.ToInt32(sid),
        //                    };
        //                    db.DeliveryNotes.Add(deliveryNote);
        //                    deliveryNoteCreted = true;
        //                    existingDeliveryNoteId = deliveryNote.Id;                      
        //                    //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
        //                    var deliveryNoteItem = new DeliveryNoteItem
        //                     {
        //                         DeliveryNote = deliveryNote,
        //                         ProductMovement = productMovement,
        //                     };
        //                     db.DeliveryNoteItems.Add(deliveryNoteItem);
        //                }
        //                else
        //                {
        //                    //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
        //                    var deliveryNoteItem = new DeliveryNoteItem
        //                    {
        //                        DeliveryNote_Id = existingDeliveryNoteId,
        //                        ProductMovement = productMovement,
        //                    };
        //                    db.DeliveryNoteItems.Add(deliveryNoteItem);
        //                }

        //            }
        //            else
        //            {
        //                TempData["Operation"] = "danger";
        //                TempData["OperationMessage"] = string.Format("Nem áll rendelkezésre a szükséges mennyiség ({0} t)",
        //                inventoryIssue.Quantity);
        //                return RedirectToAction("Index", "CurrentProductStocks");
        //            }   
        //        }



        //        if (db.SaveChanges() > 0)
        //        {
        //            TempData["Operation"] = "success";
        //        }
        //        else
        //        {
        //            TempData["Operation"] = "danger";
        //        }
        //        return RedirectToAction("Index", "CurrentProductStocks");
        //    }

        //    ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
        //    ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
        //    ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
        //    ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
        //    return View(inventoryIssue);
        //}

        //GET GetIssueForGoodsReturned
        //public ActionResult GetIssueForGoodsReturned()
        //{
        //    return View();
        //}

        ////POST GetIssueForGoodsReturned
        //[HttpPost]
        //public ActionResult GetIssueForGoodsReturned([Bind(Include="DeliveryNote_Number")]GetIssueForGoodsReturnedViewModel getIssueForGoodsReturned)
        //{
        //    return RedirectToAction("AddGoodsReturned", "InventoryOperations", getIssueForGoodsReturned.DeliveryNote_Number);
        //}


        ////GET
        //public ActionResult AddGoodsReturned(string deliveryNote_Number_to_Find)
        //{
        //    var deliverynote = db.DeliveryNotes.First(x => x.Number == deliveryNote_Number_to_Find&&x.Type=="issue");
        //    var productmovement = db.ProductMovements.Find(deliverynote);

        //    var inventory = new InventoryOperationViewModel();

        //    ViewBag.MovementType_Id = productmovement.MovementType_Id;
        //    ViewBag.Recipe_Id = productmovement.Recipe_Id;
        //    ViewBag.Warehouse_Id = productmovement.Warehouse_Id;
        //    ViewBag.Customer_Id = deliverynote.Customer_Id;
        //    ViewBag.Quantity = productmovement.Quantity;
        //    ViewBag.Remark = productmovement.Remark;
        //    ViewBag.DeliveryNote_Number = deliverynote.Number;
        //    ViewBag.DeliveryNote_Remark = deliverynote.Remark;
        //    return View(inventory);
        //}

        //// POST: Issues
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddGoodsReturned([Bind(Include = "Id,Recipe_Id,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryIssue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var identity = (ClaimsIdentity)User.Identity;
        //        var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
        //        for (int b = 0; b < inventoryIssue.Recipe_Id.Length; b++)
        //        {
        //            //PRODUCTMOVEMENT ELKÉSZÍTÉSE

        //            var productMovement = new ProductMovement
        //            {
        //                Recipe_Id = inventoryIssue.Recipe_Id[b],
        //                MovementType_Id = db.MovementTypes.First(x=>x.MovementKey=="return").Id,
        //                Warehouse_Id = inventoryIssue.Warehouse_Id,
        //                Quantity = -1*inventoryIssue.Quantity[b],
        //                Remark = inventoryIssue.Remark,
        //                IsActive = inventoryIssue.IsActive,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = Convert.ToInt32(sid)
        //            };
        //            db.ProductMovements.Add(productMovement);


        //            //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
        //            var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id[b]);
        //            var originalproductquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id[b]).Quantity;
        //            producttoupdate.Quantity = originalproductquantity + inventoryIssue.Quantity[b];
        //            producttoupdate.ChangedDate = DateTime.Now;
        //            producttoupdate.ChangedBy = Convert.ToInt32(sid);
        //            db.Entry(producttoupdate).State = EntityState.Modified;
        //        }                   
        //        //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
        //            var deliveryNote = new DeliveryNote
        //            {
        //                //ProductMovement_Id = productMovement.Id,
        //                Customer_Id = inventoryIssue.Customer_Id,
        //                Type = "return",
        //                Number = inventoryIssue.DeliveryNote_Number,
        //                Remark = inventoryIssue.DeliveryNote_Remark,
        //                IsActive = true,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = Convert.ToInt32(sid),
        //            };

        //            db.DeliveryNotes.Add(deliveryNote);

        //            if (db.SaveChanges() > 0)
        //            {
        //                TempData["Operation"] = "success";
        //            }
        //            else
        //            {
        //                TempData["Operation"] = "danger";
        //            }
        //            return RedirectToAction("Index", "CurrentProductStocks");
        //    }
        //    ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
        //    ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
        //    ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
        //    ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
        //    return View(inventoryIssue);
        //}
        [HttpGet]
        public ActionResult AddNewIssueRow(int id)
        {

            var existingRecipe = new List<Recipe>();
            foreach (var ai in db.Recipes)
            {
                if (db.CurrentProductStocks.Any(x => x.Recipe_Id == ai.Id))
                {
                    existingRecipe.Add(ai);
                };
            }
            ViewBag.Recipe_Id = new SelectList(existingRecipe.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.rowIndex = id;
            return PartialView("_newRowPartialIssue");
        }
        [HttpGet]
        public ActionResult AddNewReceiptRow(int id)
        {
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.rowIndex = id;
            return PartialView("_newRowPartialReceipt");
        }
    }
}