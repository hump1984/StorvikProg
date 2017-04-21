using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using StorvikProg.Models;


namespace StorvikProg.Controllers
{
    public class HoursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Hours
        public ActionResult Index()
        {
            var hours = db.Hours.Include(h => h.Employee).Include(h => h.Equipment).Include(h => h.Project);
            return View(hours.ToList());
        }

        // GET: Hours1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hour hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            return View(hour);
        }

        // GET: Hours/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName");
            ViewBag.EquipmentId = new SelectList(db.Equipments, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        // POST: Hours1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeId,RegDate,Hours,ProjectId,EquipmentId,Comment,Controlled,Approved,Billed")] Hour hour)
        {
            if (ModelState.IsValid)
            {
                db.Hours.Add(hour);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", hour.EmployeeId);
            ViewBag.EquipmentId = new SelectList(db.Equipments, "Id", "Name", hour.EquipmentId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", hour.ProjectId);
            return View(hour);
        }

        // GET: Hours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hour hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", hour.EmployeeId);
            ViewBag.EquipmentId = new SelectList(db.Equipments, "Id", "Name", hour.EquipmentId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", hour.ProjectId);
            return View(hour);
        }

        // POST: Hours1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeId,RegDate,Hours,ProjectId,EquipmentId,Comment,Controlled,Approved,Billed")] Hour hour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", hour.EmployeeId);
            ViewBag.EquipmentId = new SelectList(db.Equipments, "Id", "Name", hour.EquipmentId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", hour.ProjectId);
            return View(hour);
        }

        // GET: Hours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hour hour = db.Hours.Find(id);
            if (hour == null)
            {
                return HttpNotFound();
            }
            return View(hour);
        }

        // POST: Hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hour hour = db.Hours.Find(id);
            db.Hours.Remove(hour);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult UploadDocument()
        {
            ViewBag.Message = "Please select file";
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument(HttpPostedFileBase file)
        {
            string path = null;

            if (file != null && file.ContentLength > 0)
                try
                {
                    path = Path.Combine(Server.MapPath("~/UploadFiles"),
                       Path.GetFileName(file.FileName));

                    file.SaveAs(path);
                    
                    ViewBag.Message = "Succesfully uploaded file";
                    
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "Please select file";
            }

            if(path != null)
            ImportHoursFromXlsx(path);

            return View();
        }

        public void ImportHoursFromXlsx(string path)
        {
            
            var dt = DataAccess.DataTable.New.ReadCsv(path);

            var hours = dt.Rows;

            foreach (var row in hours)
            {
                var name = row.Values[1]; //TODO: Check column name
                var split = name.Split(',');
                var lastname = split[0];
                var firstname = split[1];

                if (!CheckEmployeeExist(lastname, firstname))
                {
                    var newEmployee = new Employee
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        BirthDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        StopDate = DateTime.Now

                    };
                    
                    db.Employees.Add(newEmployee);
                    db.SaveChanges();

                }
            }
        }

        public bool CheckEmployeeExist(string lastname, string firstname)
        {
            return Enumerable.Any(db.Employees, employee => employee.FirstName == firstname && employee.LastName == lastname);
        }
    }
}

