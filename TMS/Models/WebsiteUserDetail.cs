//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WebsiteUserDetail
    {
        public int WebsiteUserDetailsID { get; set; }
        public Nullable<int> WebsiteUserID { get; set; }
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
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<int> TrainingCourseID { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<int> Discount { get; set; }
    
        public virtual WebsiteUser WebsiteUser { get; set; }
        public virtual CountryState CountryState { get; set; }
    }
}
