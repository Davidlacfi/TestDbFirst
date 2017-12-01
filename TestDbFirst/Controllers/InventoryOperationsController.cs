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

        [HttpGet]
        public ActionResult AddReceipt()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "receipt"), "Id", "Name");
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients.OrderBy(i=>i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.Where(i => i.IsSupplier == true).OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

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

        [HttpGet]
        public ActionResult AddIssue()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View();
        }


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

        [HttpGet]
        public ActionResult ListReceipts()
        {
            using (db = new MecsekTransitEntities())
            {
                var ret = new List<DTOReceipt>();

                foreach (var delNote in db.DeliveryNotes.Include(c=>c.DeliveryNoteItems).Where(c => c.Type == "receipt" && c.IsActive).OrderByDescending(c => c.CreatedDate).ToList())
                {
                    var dn = new DTODeliveryNote()
                    {
                        Id = delNote.Id,
                        DeliveryNoteDate = delNote.DeliveryNoteDate,
                        Customer_Id = delNote.Id,
                        CustomerName = db.Customers?.First(c=>c.Id == delNote.Customer_Id).Name,
                        Type = delNote.Type,
                        Number = delNote.Number,
                        Remark = delNote.Remark,
                        CreatedBy = delNote.CreatedBy,
                        CreatedDate = delNote.CreatedDate,
                        CreatedByName = db.SystemUsers?.Find(delNote.CreatedBy)?.Username,
                        ChangedBy = delNote.ChangedBy,
                        ChangedDate = delNote.ChangedDate,
                        ChangedByName = db.SystemUsers?.Find(delNote.ChangedBy)?.Username,
                    };

                    foreach (var item in delNote.DeliveryNoteItems)
                    {
                        var im = db.IngredientMovements.First(c => c.Id == item.IngredientMovement_Id);
                        
                        var dni = new DTODeliveryNoteItem()
                        {
                            Id = item.Id,
                            DeliveryNote_Id = item.DeliveryNote_Id,
                            IngredientMovement_Id = item.IngredientMovement_Id,
                            IngredientMovementRemark = im.Remark,
                            IngredientName = db.Ingredients.Find(im.Ingredient_Id).Name,
                            Quantity = im.Quantity,
                            Warehouse_Id = im.Warehouse_Id, 
                            WarehouseName = im.Warehouse.Name,
                            
                        };
                        dn.DeliveryNoteItems.Add(dni);
                    }

                    var r = new DTOReceipt()
                    {
                        DeliveryNote =dn,
                        MovementType_Id = "receipt",
                        MovementTypeName = "Bevételezés", 
                        
                    };
                    ret.Add(r);
                }
                return View(ret);
            }
        }

        [HttpGet]
        public ActionResult ListIssues()
        {
            using (db = new MecsekTransitEntities())
            {
                var ret = new List<DTOIssue>();

                foreach (var delNote in db.DeliveryNotes.Include(c => c.DeliveryNoteItems).Where(c => c.Type == "issue").OrderByDescending(c=>c.CreatedDate).ToList())
                {
                    var dn = new DTODeliveryNote()
                    {
                        Id = delNote.Id,
                        DeliveryNoteDate = delNote.DeliveryNoteDate,
                        Customer_Id = delNote.Id,
                        CustomerName = db.Customers?.First(c => c.Id == delNote.Customer_Id).Name,
                        Type = delNote.Type,
                        Number = delNote.Number,
                        Remark = delNote.Remark,
                        CreatedBy = delNote.CreatedBy,
                        CreatedDate = delNote.CreatedDate,
                        CreatedByName = db.SystemUsers?.Find(delNote.CreatedBy)?.Username,
                        ChangedBy = delNote.ChangedBy,
                        ChangedDate = delNote.ChangedDate,
                        ChangedByName = db.SystemUsers?.Find(delNote.ChangedBy)?.Username,
                    };

                    foreach (var item in delNote.DeliveryNoteItems)
                    {
                        var pm = db.ProductMovements.First(c => c.Id == item.ProductMovement_Id);

                        var dni = new DTODeliveryNoteItem()
                        {
                            Id = item.Id,
                            DeliveryNote_Id = item.DeliveryNote_Id,
                            ProductMovement_Id = item.ProductMovement_Id,
                            ProductMovementRemark = pm.Remark,
                            ProductName = db.Recipes.Find(pm.Recipe_Id).Name,
                            Quantity = pm.Quantity,
                            Warehouse_Id = pm.Warehouse_Id,
                            WarehouseName = pm.Warehouse.Name,

                        };
                        dn.DeliveryNoteItems.Add(dni);
                    }

                    var r = new DTOIssue()
                    {
                        DeliveryNote = dn,
                        MovementType_Id = "issue",
                        MovementTypeName = "Kiadás",

                    };
                    ret.Add(r);
                }
                return View(ret);
            }
        }

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

        [HttpGet]
        public ActionResult AddNewEditReceiptRow(int id)
        {
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.rowIndex = id;
            return PartialView("_newRowPartialEditReceipt");
        }

        [HttpGet]
        public ActionResult EditReceipt(int id)
        {
            
            var existingDnote = db.DeliveryNotes.Find(id);

            var ret = new DTOEditReceipt()
            {
                Id = existingDnote.Id,
                MovementType_Id = "receipt",
                MovementTypeName = "Bevételezés", 
                RowId = -1
            };
            foreach (var mt in db.MovementTypes)
            {
                ret.MovementType.Add(mt);
            }

            var dNote = new DTODeliveryNote()
            {
                Id = existingDnote.Id,
                DeliveryNoteDate = existingDnote.DeliveryNoteDate,
                Customer_Id = existingDnote.Customer_Id,
                CustomerName = db.Customers?.First(c => c.Id == existingDnote.Customer_Id).Name,
                Type = existingDnote.Type,
                Number = existingDnote.Number,
                Remark = existingDnote.Remark,
                CreatedBy = existingDnote.CreatedBy,
                CreatedDate = existingDnote.CreatedDate,
                CreatedByName = db.SystemUsers?.Find(existingDnote.CreatedBy)?.Username,
                ChangedBy = existingDnote.ChangedBy,
                ChangedDate = existingDnote.ChangedDate,
                ChangedByName = db.SystemUsers?.Find(existingDnote.ChangedBy)?.Username,
            };
            foreach (var cu in db.Customers)
            {
                dNote.Customer.Add(cu);
            }

            foreach (var item in existingDnote.DeliveryNoteItems.Where(c=>c.IngredientMovement.IsActive))
            {
                var im = db.IngredientMovements.First(c => c.Id == item.IngredientMovement_Id);

                var dni = new DTODeliveryNoteItem()
                {
                    Id = item.Id,
                    DeliveryNote_Id = item.DeliveryNote_Id,
                    IngredientMovement_Id = item.IngredientMovement_Id,
                    IngredientMovementRemark = im.Remark,
                    Ingredient_Id = im.Ingredient_Id,
                    IngredientName = db.Ingredients.Find(im.Ingredient_Id).Name,
                    Quantity = im.Quantity,
                    Warehouse_Id = im.Warehouse_Id,
                    WarehouseName = im.Warehouse.Name,
                    IngredientMovementIsActive = im.IsActive
                };
                ret.RowId++;
                foreach (var wh in db.Warehouses)
                {
                    dni.Warehouse.Add(wh);
                }
                foreach (var ing in db.Ingredients)
                {
                    dni.Ingredient.Add(ing);
                }
                
                dNote.DeliveryNoteItems.Add(dni);
            }
            ret.DeliveryNote = dNote;
            return View(ret);
        }

        [HttpPost]
        public ActionResult EditReceipt(DTOEditReceipt r)
        {
            if (ModelState.IsValid)
            {

                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                //var deliveryNoteCreted = false;

                var existingDeliveryNote = db.DeliveryNotes.Find(r.Id);

                //DELIVERYNOTE UPDATE
                existingDeliveryNote.DeliveryNoteDate = (DateTime) r.DeliveryNote.DeliveryNoteDate;
                existingDeliveryNote.Customer_Id = r.DeliveryNote.Customer_Id;
                existingDeliveryNote.Number = r.DeliveryNote.Number;
                existingDeliveryNote.Remark = r.DeliveryNote.Remark;
                existingDeliveryNote.IsActive = r.DeliveryNote.IsActive;
                existingDeliveryNote.ChangedBy = Convert.ToInt32(sid);
                existingDeliveryNote.ChangedDate = DateTime.Now;

                db.Entry(existingDeliveryNote).State=EntityState.Modified;


                for (int x = 0; x < r.DeliveryNote.DeliveryNoteItems.Count; x++)
                {
                    //INGREDIENTMOVEMENT UPDATE
                    var currentIndex = x;
                    var currentIngredientId = r.DeliveryNote.DeliveryNoteItems[currentIndex].Ingredient_Id;
                    var currentWarehouseId = r.DeliveryNote.DeliveryNoteItems[currentIndex].Warehouse_Id;
                    var currentIngredientQuantity = r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity;

                    if (r.DeliveryNote.DeliveryNoteItems[currentIndex].IngredientMovementIsActive || r.DeliveryNote.DeliveryNoteItems[currentIndex].Id == 0)
                    {
                        //HA VAN MÁR INGREDIENT MOVEMENT -- UPDATE
                        if (r.DeliveryNote.DeliveryNoteItems[currentIndex].IngredientMovement_Id != null)
                        {

                            var existingIngredientMovement =
                                db.IngredientMovements.Find(r.DeliveryNote.DeliveryNoteItems[currentIndex].IngredientMovement_Id);
                            var originalQuantity = existingIngredientMovement.Quantity;
                            //HA NEM EGYEZNEK A MENNYISÉGEK VAGY A RAKTÁR, UPDATE
                            if (
                                existingIngredientMovement.Quantity != r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity
                                || existingIngredientMovement.Warehouse_Id != r.DeliveryNote.DeliveryNoteItems[currentIndex].Warehouse_Id
                                )
                            {
                                existingIngredientMovement.Quantity =
                                    (decimal)r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity;
                                existingIngredientMovement.Warehouse_Id = (int)r.DeliveryNote.DeliveryNoteItems[currentIndex].Warehouse_Id;
                                existingIngredientMovement.IsActive = r.DeliveryNote.DeliveryNoteItems[currentIndex]
                                    .IngredientMovementIsActive;
                                existingIngredientMovement.ChangedBy = Convert.ToInt32(sid);
                                existingIngredientMovement.ChangedDate = DateTime.Now;
                                db.Entry(existingIngredientMovement).State = EntityState.Modified;

                                var quantityDifference = originalQuantity -
                                    (decimal)r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity;

                                //HA VAN MENNYISÉG VÁLTOZÁS, INGREDIENTSTOCK MÓDOSÍTÁSA
                                if (quantityDifference != 0)
                                {
                                    var currentIngredientStock = db.CurrentIngredientStocks.SingleOrDefault(c => c.Ingredient_Id == currentIngredientId && c.Warehouse_Id == currentWarehouseId);
                                    currentIngredientStock.Quantity = currentIngredientStock.Quantity - quantityDifference;
                                    currentIngredientStock.ChangedBy = Convert.ToInt32(sid);
                                    currentIngredientStock.ChangedDate = DateTime.Now;
                                    db.Entry(currentIngredientStock).State = EntityState.Modified;
                                }
                            }
                        }
                        //HA MÉG NINCS INGREDIENT MOVEMENT -- CREATE
                        else
                        {
                            var ingredientMovement = new IngredientMovement
                            {
                                Ingredient_Id = (int)r.DeliveryNote.DeliveryNoteItems[currentIndex].Ingredient_Id,
                                MovementType_Id = db.MovementTypes.SingleOrDefault(c => c.MovementKey == "receipt").Id,
                                Warehouse_Id = (int)r.DeliveryNote.DeliveryNoteItems[currentIndex].Warehouse_Id,
                                Quantity = (decimal)r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity,
                                Remark = r.DeliveryNote.Remark,
                                IsActive = true,
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
                                    Ingredient_Id = (int)r.DeliveryNote.DeliveryNoteItems[currentIndex].Ingredient_Id,
                                    Warehouse_Id = (int)r.DeliveryNote.DeliveryNoteItems[currentIndex].Warehouse_Id,
                                    Quantity = (decimal)r.DeliveryNote.DeliveryNoteItems[currentIndex].Quantity,
                                });
                            }
                            //HA VAN AZ ALAPANYAGBÓL, UPDATE
                            else
                            {
                                var ingredienttoupdate = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == currentIngredientId);
                                var originalingredientquantity = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == currentIngredientId).Quantity;
                                ingredienttoupdate.Quantity = (decimal)(originalingredientquantity + currentIngredientQuantity);
                                ingredienttoupdate.ChangedDate = DateTime.Now;
                                ingredienttoupdate.ChangedBy = Convert.ToInt32(sid);
                                db.Entry(ingredienttoupdate).State = EntityState.Modified;
                            }

                            //SZÁLLÍTÓLEVÉL TÉTELEK HOZZÁADÁSA
                            var existingDeliveryNoteId = r.DeliveryNote.Id;
                            var deliveryNoteItem = new DeliveryNoteItem
                            {
                                DeliveryNote_Id = existingDeliveryNoteId,
                                IngredientMovement = ingredientMovement
                            };
                            db.DeliveryNoteItems.Add(deliveryNoteItem);
                        } 
                    }
                    else 
                    {
                        //HA TÖRÖLTEK SORT A SZÁLLÍTÓLEVÉLRŐL

                        //DELIVERYNOTEITEM TÖRLÉSE
                        var deliveryNoteItemToDelete = db.DeliveryNoteItems.Find(r.DeliveryNote.DeliveryNoteItems[currentIndex].Id);
                        var affectedIngredientMovementId = deliveryNoteItemToDelete.IngredientMovement_Id;

                        db.Entry(deliveryNoteItemToDelete).State = EntityState.Deleted;

                        //INGREDIENTMOVEMENT ISACTIVE=0
                        var ingredientMovementToDelete = db.IngredientMovements.Find(affectedIngredientMovementId);
                        var affectedIngredientId = ingredientMovementToDelete.Ingredient_Id;
                        ingredientMovementToDelete.IsActive = false;
                        ingredientMovementToDelete.ChangedBy = Convert.ToInt32(sid);
                        ingredientMovementToDelete.ChangedDate = DateTime.Now;

                        db.Entry(ingredientMovementToDelete).State = EntityState.Modified;

                        //CURRENTINGREDIENTSTOCK MÓDOSÍTÁSA
                        var affectedCurrentIngredientStock = db.CurrentIngredientStocks.First(c=>c.Ingredient_Id == affectedIngredientId);
                        affectedCurrentIngredientStock.Quantity =
                            (decimal)(affectedCurrentIngredientStock.Quantity - r.DeliveryNote.DeliveryNoteItems
                                            .First(c => c.Ingredient_Id == affectedIngredientId).Quantity);
                        affectedCurrentIngredientStock.ChangedBy = Convert.ToInt32(sid);
                        affectedCurrentIngredientStock.ChangedDate = DateTime.Now;

                        db.Entry(affectedCurrentIngredientStock).State = EntityState.Modified;

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
            return View(r);
        }
    }
}