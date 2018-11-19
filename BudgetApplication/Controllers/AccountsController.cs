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
    public class AccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Accounts
        public IQueryable<Accounts> GetAccounts()
        {
            return db.Accounts;
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult GetAccounts(int id)
        {
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return NotFound();
            }

            return Ok(accounts);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAccounts(int id, Accounts accounts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var account = db.Accounts.Where(p => p.Id == accounts.Id).FirstOrDefault();
            if (id != accounts.Id)
            {
                return BadRequest();
            }
            if(account == null)
            {
                return BadRequest("account doesn't exist");
            }
            account.Name = accounts.Name;
            account.Balance = accounts.Balance;
            account.HouseHoldId = accounts.HouseHoldId;

            //db.Entry(accounts).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult UpdateAccount(Accounts accounts, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts.Where(p => p.Id == accounts.Id).FirstOrDefault();
            var transaction = db.Transactions.Where(p => p.Id == id).FirstOrDefault();
            var wholeAccount = new Accounts();
            wholeAccount.Balance = account.Balance - transaction.Amount;
            account.Balance = wholeAccount.Balance;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult PostAccounts(Accounts accounts)
            {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var creatorId = User.Identity.GetUserId();
            var houseHold = db.HouseHolds.Where(p => p.Id == accounts.HouseHoldId);
            if(houseHold == null){
                return BadRequest("No household found");
            };
            var account = new Accounts();
            account.Name = accounts.Name;
            account.HouseHoldId = accounts.HouseHoldId;
            account.Balance = 0;
            db.Accounts.Add(accounts);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = accounts.Id }, accounts);
        }

        [HttpGet]
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult ViewAccounts(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts
                 .Include(p => p.HouseHold)
                 .FirstOrDefault(p => p.Id == id);

            var household = db.HouseHolds.Where(p => p.Id == account.HouseHoldId).FirstOrDefault();
           
            if (account == null)
            {
                return NotFound();
            }
            var userId = User.Identity.GetUserId();
            var accountViewModel = new AccountViewModel();
            accountViewModel.Id = account.Id;
            accountViewModel.Name = account.Name;
            accountViewModel.HouseHoldName = household.Name;
            return Ok(accountViewModel);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult DeleteAccounts(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var account = db.Accounts.Where(p => p.Id == id).FirstOrDefault();
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return NotFound();
            }
            var houseHold = account.HouseHold;
            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                db.Accounts.Remove(accounts);
                db.SaveChanges();

                return Ok(accounts);
            }
            else
            {
                return BadRequest("Unauthorized");
            }
        }

        [HttpPost]
        [Route("recalculatebalance/{id}")]
        public IHttpActionResult RecalculateBalance(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var bankAccount = db.Accounts
               .FirstOrDefault(p => p.Id == id);
            if (bankAccount == null)
            {
                return BadRequest("Bank account doesn't exist");
            }
            var houseHold = bankAccount.HouseHold;
            if (houseHold.CreatorId == userId ||
               houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                var balance = bankAccount.Transactions
                    .Where(p => !p.IsVoided)
                    .Sum(p => p.Amount);
                bankAccount.Balance = balance;
                db.SaveChanges();
                return Ok("The new balance is: " + balance);
            }
            else
            {
                return BadRequest("Not authorized");
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

        private bool AccountsExists(int id)
        {
            return db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}