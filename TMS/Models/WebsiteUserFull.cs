using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    public class WebsiteUserFull
    {
        public int WebsiteUserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SkypeID { get; set; }
        public string AboutUs { get; set; }
        public string WhyInterested { get; set; }
        public bool AcceptPP { get; set; }
        public Nullable<bool> ReceiveUpdates { get; set; }
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

        public string HighestEducation { get; set; }
        public string CompletionYear { get; set; }
        public string WorkExperienceIT { get; set; }
        public string WorkExperienceNonIT { get; set; }
        public Nullable<int> CurrentlyEmployed { get; set; }
        public Nullable<int> AuthorizedWorkUSA { get; set; }
        public string VisaStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Nullable<int> StateID { get; set; }
        public string ZIPCode { get; set; }
        public Nullable<int> OpenToRelocate { get; set; }
        public Nullable<int> TrainingCourseID { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<int> Discount { get; set; }

        public virtual WebsiteUser WebsiteUser { get; set; }
        public virtual CountryState CountryState { get; set; }
    }
}