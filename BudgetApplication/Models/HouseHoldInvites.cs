using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class HouseHoldInvites
    {
        public int Id { get; set; }

        public int HouseHoldId { get; set; }
        public virtual HouseHolds HouseHold { get; set; }

        public string InvitedUserId { get; set; }
        public virtual ApplicationUser InvitedUser { get; set; }

    }
}