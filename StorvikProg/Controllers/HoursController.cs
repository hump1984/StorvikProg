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
            StreamReader reader = new StreamReader(path,System.Text.Encoding.Default);
            
            var dt = DataAccess.DataTable.New.Read(reader,',');

            var hours = dt.Rows;

            var newhour = new Hour();

            foreach (var row in hours)
            {
                if (row.ColumnNames.ElementAt(1) == "Ansatt")
                {
                    var name = row.Values[1];
                    newhour.Employee = CheckEmployee(name); //TODO: ADD Employee Number
                }

                if (row.ColumnNames.ElementAt(2) == "Dato")
                {
                    newhour.RegDate = DateTime.Parse(row.Values[2]);
                }

                if (row.ColumnNames.ElementAt(3) == "Antall")
                {
                    newhour.Hours = Convert.ToDouble(row.Values[3]);
                }

                if (row.ColumnNames.ElementAt(6) == "Prosjekt id")
                {
                    newhour.Project = CheckProject(Convert.ToInt32(row.Values[6]), row.Values[7]);
                }

            }
        }

        public Project CheckProject(int number, string projectname)
        {
            foreach (var project in db.Projects)
            {
                if (project.Number == number)
                    return project;
            }

            if (projectname.StartsWith(number.ToString()))
            {
                projectname = projectname.Replace(number + " ", "");
            }

            var newProject = new Project
            {
                Number = number,
                Name = projectname
            };

            db.Projects.Add(newProject);
            db.SaveChanges();

            return newProject;

        }  //Checks if project exists, if not create project. Also returns project

        public Employee CheckEmployee(string name)
        {
            var split = name.Split(',');
            var lastname = split[0];
            var firstname = split[1];

            foreach (var employee in db.Employees) //Check for existing employee by name
            {
                if (employee.LastName == lastname && employee.FirstName == firstname) 
                    return employee;
            }
            
            var newEmployee = new Employee //Creates new employee record if it doesnt exist in db.
            {
                FirstName = firstname,
                LastName = lastname,
                BirthDate = DateTime.Now, //Temporary, datetime2 and datetime troubles as usual.
                StartDate = DateTime.Now,
                StopDate = DateTime.Now
            };

            db.Employees.Add(newEmployee);
            db.SaveChanges();

            return newEmployee;

        }  //Checks if employee exist, if not create employee. Also returns Employee
    }
}

