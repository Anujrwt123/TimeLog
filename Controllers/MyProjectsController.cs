using demoPro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace demoPro.Models
{
    public class MyProjectsController : Controller
    {
        private NewDbContext db = new NewDbContext();
        private static string activedrop;
        
        // GET: MyProjects
        public ActionResult Index()
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            if(cookie ==null || String.IsNullOrEmpty(cookie.Value))
            {
               return RedirectToAction("Login", "Account");
            }

            //getting user
            int uId; int.TryParse(cookie.Value, out uId);
            var usr = db.Users.Find(uId);

            //getting aaj k projects
            var Active = db.Projects.Where(x => x.Type == "Order" && x.Status != "Closed").OrderByDescending(x=>x.Lead_ID).ToList();
            if (usr != null)
            {
                ViewBag.ActiveProjects = Active.Where(x => x.Application == usr.Department);
                ViewBag.UserName = usr.Developer;
            }
            else
            {
                ViewBag.ActiveProjects = Active;
            }
            ViewBag.ALLProjects = db.Projects.OrderByDescending(x=>x.Lead_ID).ToList();
            return View();
        }

        // GET: MyProjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyProject myProject = db.MyProjects.Find(id);
            if (myProject == null)
            {
                return HttpNotFound();
            }
            return View(myProject);
        }

        // GET: MyProjects/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Create(String Name , String Desc)
        {
            //end all projects first
            EndProject(activedrop, "");

            MyProject myProject = new MyProject();
            myProject.Date = System.DateTime.Now.Date;
            myProject.StartTime = new TimeSpan();
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            myProject.StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            //myProject.UserEmail = cookie.Value;
            myProject.ProjectId =Convert.ToInt32(activedrop);
            myProject.Active = "Active";
            myProject.Name = Name;
            myProject.Description = Desc;
            db.MyProjects.Add(myProject);
            db.SaveChanges();
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        // GET: MyProjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyProject myProject = db.MyProjects.Find(id);
            if (myProject == null)
            {
                return HttpNotFound();
            }
            return View(myProject);
        }

        // POST: MyProjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,UserId,Name,Description,Quantity,TimeTaken,Date,StartTime,EndTime")] MyProject myProject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(myProject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(myProject);
        }

        // GET: MyProjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyProject myProject = db.MyProjects.Find(id);
            if (myProject == null)
            {
                return HttpNotFound();
            }
            return View(myProject);
        }

        // POST: MyProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MyProject myProject = db.MyProjects.Find(id);
            db.MyProjects.Remove(myProject);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeDropdown()
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            int uId;int.TryParse(cookie.Value, out uId);
            
            //var ssp = db.Database.SqlQuery<MyProject>("exec Sp_GetUniqueProjects").ToList();
            var data =  db.MyProjects.Where(x => x.Dev_Id == uId && x.Active == "Active").ToList();
            ViewBag.MyProjects = data.OrderBy(x=> x.Id);
            return View();
        }

        public ActionResult StartEndProject(string value, string Comment="")
        {
            int pId;
            int.TryParse(value, out pId);
            EndProject(value, Comment);

            //creating a new record now
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            int uid; int.TryParse(cookie.Value, out uid);
            CreateProject(pId, uid);

            int prjId;
            int.TryParse(activedrop, out prjId);
            var data = db.MyProjects.Where(item => item.Dev_Id == uid).OrderBy(x=> x.StartTime).OrderByDescending(x=>x.Id).ToList();
            string name = "";
            if (data != null){
                var dtd = data.FirstOrDefault(x => x.Active == "Active");
                name = dtd != null ? dtd.Name : "";
            }
            ViewBag.Name = name;
            return View("GetMyProjetcs", data);
        }

        public void CreateProject(int pId, int uId)
        {
            var myProject2 = db.Projects.Find(pId);
            var myProject = new MyProject();

            myProject.Name = myProject2.Project_Desc;
            myProject.Description = myProject2.Name;
            myProject.Date = System.DateTime.Now.Date;
            myProject.StartTime = new TimeSpan();
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            myProject.StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            int uid; int.TryParse(cookie.Value, out uid);
            myProject.Dev_Id = uid;
            myProject.ProjectId = Convert.ToInt32(activedrop);
            myProject.Active = "Active";
            db.MyProjects.Add(myProject);
            db.SaveChanges();
        }

        public void EndProject(string value, string Comment = "")
        {
            int pId;
            int.TryParse(value, out pId);
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");

            int uId;int.TryParse(cookie.Value, out uId);

            //lets end the active projects first
            var end = db.MyProjects.Where(x => x.Dev_Id==uId && x.Active == "Active").ToList();
            if (end != null)
            {
                foreach (var itm in end)
                {
                    itm.Active = "Closed";
                    itm.EndTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    if (Comment != "")
                    {
                        itm.Comment = Comment;
                    }
                    db.Entry(itm).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        public ActionResult GetMyProjetcs(string value)
        {
            int pId = 0;
            int.TryParse(value, out pId);
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            int uId;int.TryParse(cookie.Value, out uId);
            var data = db.MyProjects.Where(item => item.Dev_Id == uId).OrderByDescending(x => x.Id).ToList();

            string name = "";
            if (data != null)
            {
                var dtd= data.FirstOrDefault(x => x.Active == "Active");
                name = dtd != null  ? dtd.Name : "";
            }
            ViewBag.Name = name;
            return View(data); 
        }

        public ActionResult EndDay()
           {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("UserId");
            int uId; int.TryParse(cookie.Value, out uId);
            var data = db.MyProjects.Where(item => item.Dev_Id == uId).OrderByDescending(x => x.Id).ToList();

            var dd = data.Select(x => new EndDayProjectViewModel()
            {
                ID = x.Id,
                Name = x.Name,
                TimeTaken = x.EndTime - x.StartTime,
                AdjustTime = x.AdjustTime,
                ProjectId = x.ProjectId,
                Description = x.Description,
                Quantity = x.Quantity,

            }).OrderBy(x=> x.ID).ToList();

            string name = "";
            if (data != null)
            {
                var dtd = data.FirstOrDefault(x => x.Active == "Active");
                name = dtd != null ? dtd.Name : "";
            }

            ViewBag.Projectlist = data;
            ViewBag.Name = name;
       
            return PartialView("_EndDay", dd);
        }

        public ActionResult SaveEndDayprojlist(int Id, string AdjustTime)
        {

            var dd = db.MyProjects.Where(x => x.Id == Id).FirstOrDefault();
            dd.AdjustTime = AdjustTime;
            db.SaveChanges();
            return Json("ok", JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
