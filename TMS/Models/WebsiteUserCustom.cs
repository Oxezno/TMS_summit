using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    [MetadataType(typeof(WebsiteUserCustom))]
    public partial class WebsiteUser
    {

    }

    public class WebsiteUserCustom
    {
        public int WebsiteUserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SkypeID { get; set; }
        public string AboutUs { get; set; }
        public string WhyInterested { get; set; }
        public bool AcceptPP { get; set; }
        public Nullable<bool> ReceiveUpdates { get; set; }
        [Required]
        public string Password { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string ProfilePic { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> TakeEvaluation { get; set; }
        public Nullable<bool> EvaluationStatus { get; set; }

        public string Token { get; set; }
        public string RecoveryCode { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Ready4Resume { get; set; }
        public string RecruiterName { get; set; }
        public Nullable<bool> SignedContract { get; set; }

    }
}