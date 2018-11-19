using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class HouseHolds
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public HouseHolds()
        {
            HouseHoldInvites = new HashSet<HouseHoldInvites>();
            ApplicationUserHouseHolds = new HashSet<ApplicationUserHouseHolds>();
            Accounts = new HashSet<Accounts>();
            Categories = new HashSet<Categories>();
            HouseHoldUser = new HashSet<ApplicationUser>();
        }

        public virtual ICollection<HouseHoldInvites> HouseHoldInvites { get; set; }
        public virtual ICollection<ApplicationUserHouseHolds> ApplicationUserHouseHolds { get; set; }
        public virtual ICollection<Categories> Categories { get; set; }
        public virtual ICollection<Accounts> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> HouseHoldUser { get; set; }

    }
}