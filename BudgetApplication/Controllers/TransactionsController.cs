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
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Transactions
        public IQueryable<Transactions> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult GetTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransactions(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var transaction = db.Transactions.Where(p => p.Id == transactions.Id).FirstOrDefault();
            if (id != transactions.Id)
            {
                return BadRequest();
            }
            if (transaction == null)
            {
                return BadRequest("Transaction not found");
            }
            var houseHold = transaction.Account.HouseHold;
            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                transaction.Account.Balance -= transaction.Amount;

                transaction.Description = transactions.Description;
                transaction.Date = transactions.Date;
                transaction.Amount = transactions.Amount;
                transaction.CategoryId = transactions.CategoryId;

                transaction.Account.Balance += transaction.Amount;

                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult updateAccountPerTransaction(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts.Where(p => p.Id == transactions.Id).FirstOrDefault();
            var transaction = db.Transactions.Where(p => p.Id == id).FirstOrDefault();
            var wholeAccount = new Accounts();
            wholeAccount.Balance = account.Balance - transaction.Amount;
            account.Balance = wholeAccount.Balance;
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult VoidTransactions(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts.Where(p => p.Id == transactions.Id).FirstOrDefault();
            var transaction = db.Transactions.Where(p => p.Id == id).FirstOrDefault();
            var wholeAccount = new Accounts();
            wholeAccount.Balance = account.Balance + transaction.Amount;
            account.Balance = wholeAccount.Balance;
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult PostTransactions(Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var creatorId = User.Identity.GetUserId();
            //var transaction = db.Transactions.Where(p => p.Id == transactions.Id).FirstOrDefault();
            var account = db.Accounts.Where(p => p.Id == transactions.AccountId).FirstOrDefault();
            if (account == null)
            {
                return BadRequest("Account not found");
            }
            var houseHold = account.HouseHold;

            if (houseHold.CreatorId == creatorId ||
                houseHold.HouseHoldUser.Any(p => p.Id == creatorId))
            {
                var transaction = new Transactions();
                transaction.AccountId = transactions.AccountId;
                transaction.Description = transactions.Description;
                transaction.Date = transactions.Date;
                transaction.Amount = transactions.Amount;
                transaction.CategoryId = transactions.CategoryId;
                transaction.IsVoided = false;
                transaction.EnteredById = creatorId;
                account.Balance += transaction.Amount;
                db.Transactions.Add(transactions);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult VoidTransaction(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var transaction = db.Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return BadRequest("Transaction doesn't exist");
            }

            var houseHold = transaction.Account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                transaction.Account.Balance -= transaction.Amount;
                transaction.IsVoided = true;

                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }
        

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult DeleteTransactions(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            //Transactions transactions = db.Transactions.Find(id);
            var transactions = db.Transactions.FirstOrDefault(p => p.Id == id);

            if (transactions == null)
            {
                return NotFound();
            }
            var houseHold = transactions.Account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.HouseHoldUser.Any(p => p.Id == userId))
            {
                transactions.Account.Balance -= transactions.Amount;

                db.Transactions.Remove(transactions);

                db.SaveChanges();

                return Ok();
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

        private bool TransactionsExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}