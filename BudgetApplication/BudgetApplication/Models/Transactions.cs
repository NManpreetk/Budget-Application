using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public virtual Accounts Account { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public virtual Categories category { get; set; }
        public bool IsVoided { get; set; }
        public string EnteredById { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
    }
}