using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BudgetApplication.Models;
using Microsoft.AspNet.Identity;

namespace BudgetApplication.Controllers
{
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Categories
        public IQueryable<Categories> GetCategories()
        {
            return db.Categories;
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Categories))]
        public IHttpActionResult GetCategories(int id)
        {
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategories(int id, Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var category = db.Categories.Where(p => p.Id == categories.Id).FirstOrDefault();
            if(category == null)
            {
                return BadRequest("Category not found");
            }
            if (id != categories.Id)
            {
                return BadRequest();
            }
            var houseHold = category.HouseHold;
            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                category.Name = categories.Name;
                category.HouseHold = categories.HouseHold;
                //db.Entry(categories).State = EntityState.Modified;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Categories
        [ResponseType(typeof(Categories))]
        public IHttpActionResult PostCategories(CreateCategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var creatorId = User.Identity.GetUserId();
            var household = db.HouseHolds.Where(p => p.Id == model.HouseHoldId).FirstOrDefault();
            if (household == null)
            {
                return NotFound();
            }
            if (household.CreatorId == creatorId ||
                household.HouseHoldUser.Any(p => p.Id == creatorId))
            {
                var category = new Categories();
                category.Name = model.Name;
                category.HouseHoldId = model.HouseHoldId;

                db.Categories.Add(category);
                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpGet]
        [ResponseType(typeof(Categories))]
        public IHttpActionResult ViewCategories()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var category = db.Categories.Include(p => p.Name).Include(p => p.HouseHoldId)
                .Select(p => new ViewCategoryViewModel
                {
                    Name = p.Name,
                }).ToList();
            return Ok(category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Categories))]
        public IHttpActionResult DeleteCategories(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var category = db.Categories.FirstOrDefault(p => p.Id == id);
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }
            var houseHold = category.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                db.Categories.Remove(categories);
                db.SaveChanges();
                return Ok(categories);
            }
            else
            {
                return BadRequest("Not Found");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoriesExists(int id)
        {
            return db.Categories.Count(e => e.Id == id) > 0;
        }
    }
}