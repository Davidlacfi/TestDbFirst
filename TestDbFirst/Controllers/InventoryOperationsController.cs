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
        // GET: Receipts
        public ActionResult AddReceipt()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "receipt"), "Id", "Name");
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients.OrderBy(i=>i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.Where(i => i.IsSupplier == true).OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

        // POST: Receipts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceipt([Bind(Include = "Id,Ingredient_Id,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryReceipt)
        {
            if (ModelState.IsValid)
            {
                //INGREDIENTMOVEMENT ELKÉSZÍTÉSE
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                var ingredientMovement = new IngredientMovement
                {
                    Ingredient_Id = inventoryReceipt.Ingredient_Id,
                    MovementType_Id = inventoryReceipt.MovementType_Id,
                    Warehouse_Id = inventoryReceipt.Warehouse_Id,
                    Quantity = inventoryReceipt.Quantity,
                    Remark = inventoryReceipt.Remark,
                    IsActive = inventoryReceipt.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Convert.ToInt32(sid)
                };
                db.IngredientMovements.Add(ingredientMovement);


                //CURRENTINGREDIENTSTOCK MÓDOSÍTÁSA
                var ingredientexists = db.CurrentIngredientStocks.Where(i => i.Ingredient_Id == inventoryReceipt.Ingredient_Id);
                //HA NINCS AZ ALAPANYAGBÓL, HOZZÁADJUK
                if (!ingredientexists.Any())
                {
                    db.CurrentIngredientStocks.Add(new CurrentIngredientStock
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                        IsActive = true,
                        Warehouse_Id = inventoryReceipt.Warehouse_Id,
                        Ingredient_Id = inventoryReceipt.Ingredient_Id,
                        Quantity = inventoryReceipt.Quantity

                    });
                }
                //HA VAN AZ ALAPANYAGBÓL, UPDATE
                else
                {
                    var ingredienttoupdate = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == inventoryReceipt.Ingredient_Id);
                    var originalingredientquantity = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == inventoryReceipt.Ingredient_Id).Quantity;
                    ingredienttoupdate.Quantity = originalingredientquantity + inventoryReceipt.Quantity;
                    ingredienttoupdate.ChangedDate = DateTime.Now;
                    ingredienttoupdate.ChangedBy = Convert.ToInt32(sid);
                    db.Entry(ingredienttoupdate).State = EntityState.Modified;
                }

                //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
                var deliveryNote = new DeliveryNote
                {
                    //IngredientMovement_Id = ingredientMovement.Id,
                    Customer_Id = inventoryReceipt.Customer_Id,
                    Type = "receipt",
                    Number = inventoryReceipt.DeliveryNote_Number,
                    Remark = inventoryReceipt.DeliveryNote_Remark,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Convert.ToInt32(sid),
                };
                db.DeliveryNotes.Add(deliveryNote);

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
            ViewBag.Customer_Id = new SelectList(db.Customers.Where(i => i.IsSupplier == true), "Id", "Name");
            return View(inventoryReceipt);
        }
        // GET: Issues
        public ActionResult AddIssue()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

        // POST: Issues
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIssue([Bind(Include = "Id,Recipe_Id,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryIssue)
        {
            if (ModelState.IsValid)
            {
                //ELLENŐRIZZÜK, VAN-E ELEGENDŐ KÉSZTERMÉK
                if (db.CurrentProductStocks.Any(e => e.Recipe_Id == inventoryIssue.Recipe_Id) && inventoryIssue.Quantity  <=
                    db.CurrentProductStocks.First(e => e.Recipe_Id == inventoryIssue.Recipe_Id).Quantity)
                    
                {
                    //PRODUCTMOVEMENT ELKÉSZÍTÉSE
                    var identity = (ClaimsIdentity)User.Identity;
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                    var productMovement = new ProductMovement
                    {
                        Recipe_Id = inventoryIssue.Recipe_Id,
                        MovementType_Id = inventoryIssue.MovementType_Id,
                        Warehouse_Id = inventoryIssue.Warehouse_Id,
                        Quantity = inventoryIssue.Quantity,
                        Remark = inventoryIssue.Remark,
                        IsActive = inventoryIssue.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid)
                    };
                    db.ProductMovements.Add(productMovement);


                    //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
                    var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id);
                    var originalproductquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id).Quantity;
                    producttoupdate.Quantity = originalproductquantity - inventoryIssue.Quantity;
                    producttoupdate.ChangedDate = DateTime.Now;
                    producttoupdate.ChangedBy = Convert.ToInt32(sid);
                    db.Entry(producttoupdate).State = EntityState.Modified;


                    //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
                    var deliveryNote = new DeliveryNote
                    {
                        //ProductMovement_Id = productMovement.Id,
                        Customer_Id = inventoryIssue.Customer_Id,
                        Type = "issue",
                        Number = inventoryIssue.DeliveryNote_Number,
                        Remark = inventoryIssue.DeliveryNote_Remark,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                    };
                    db.DeliveryNotes.Add(deliveryNote);

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
                else
                {
                    TempData["Operation"] = "danger";
                    TempData["OperationMessage"] = string.Format("Nem áll rendelkezésre a szükséges mennyiség ({0} t)", inventoryIssue.Quantity);
                    return RedirectToAction("Index", "CurrentProductStocks");
                }
            }
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View(inventoryIssue);
        }

        //GET GetIssueForGoodsReturned
        public ActionResult GetIssueForGoodsReturned()
        {
            return View();
        }

        //POST GetIssueForGoodsReturned
        [HttpPost]
        public ActionResult GetIssueForGoodsReturned([Bind(Include="DeliveryNote_Number")]GetIssueForGoodsReturnedViewModel getIssueForGoodsReturned)
        {
            return RedirectToAction("AddGoodsReturned", "InventoryOperations", getIssueForGoodsReturned.DeliveryNote_Number);
        }


        //GET
        public ActionResult AddGoodsReturned(string deliveryNote_Number_to_Find)
        {
            var deliverynote = db.DeliveryNotes.First(x => x.Number == deliveryNote_Number_to_Find&&x.Type=="issue");
            var productmovement = db.ProductMovements.Find(deliverynote);

            var inventory = new InventoryOperationViewModel();

            ViewBag.MovementType_Id = productmovement.MovementType_Id;
            ViewBag.Recipe_Id = productmovement.Recipe_Id;
            ViewBag.Warehouse_Id = productmovement.Warehouse_Id;
            ViewBag.Customer_Id = deliverynote.Customer_Id;
            ViewBag.Quantity = productmovement.Quantity;
            ViewBag.Remark = productmovement.Remark;
            ViewBag.DeliveryNote_Number = deliverynote.Number;
            ViewBag.DeliveryNote_Remark = deliverynote.Remark;
            return View(inventory);
        }

        // POST: Issues
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGoodsReturned([Bind(Include = "Id,Recipe_Id,Customer_Id,DeliveryNote_Number,DeliveryNote_Remark,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] InventoryOperationViewModel inventoryIssue)
        {
            if (ModelState.IsValid)
            {
                {
                    //PRODUCTMOVEMENT ELKÉSZÍTÉSE
                    var identity = (ClaimsIdentity)User.Identity;
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                    var productMovement = new ProductMovement
                    {
                        Recipe_Id = inventoryIssue.Recipe_Id,
                        MovementType_Id = db.MovementTypes.First(x=>x.MovementKey=="return").Id,
                        Warehouse_Id = inventoryIssue.Warehouse_Id,
                        Quantity = -1*inventoryIssue.Quantity,
                        Remark = inventoryIssue.Remark,
                        IsActive = inventoryIssue.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid)
                    };
                    db.ProductMovements.Add(productMovement);


                    //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
                    var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id);
                    var originalproductquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == inventoryIssue.Recipe_Id).Quantity;
                    producttoupdate.Quantity = originalproductquantity + inventoryIssue.Quantity;
                    producttoupdate.ChangedDate = DateTime.Now;
                    producttoupdate.ChangedBy = Convert.ToInt32(sid);
                    db.Entry(producttoupdate).State = EntityState.Modified;


                    //SZÁLLÍTÓLEVÉL HOZZÁADÁSA
                    var deliveryNote = new DeliveryNote
                    {
                        //ProductMovement_Id = productMovement.Id,
                        Customer_Id = inventoryIssue.Customer_Id,
                        Type = "return",
                        Number = inventoryIssue.DeliveryNote_Number,
                        Remark = inventoryIssue.DeliveryNote_Remark,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                    };

                    db.DeliveryNotes.Add(deliveryNote);

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
            }
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "issue"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.OrderBy(i => i.Name), "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Customer_Id = new SelectList(db.Customers.OrderBy(i => i.Name), "Id", "Name");
            return View(inventoryIssue);
        }
    }
}