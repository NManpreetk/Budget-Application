using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        [Required]
        public int AccountId { get; set; }
        public virtual Accounts Account { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Categories Category { get; set; }

        public bool IsVoided { get; set; }
        public string EnteredById { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
    }
}