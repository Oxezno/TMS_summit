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
    
    public partial class Schedule
    {
        public int ScheduleID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int WebsiteUserID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> TrainingCourseID { get; set; }
        public string Subject { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public Nullable<int> ScheduleType { get; set; }
    
        public virtual User User { get; set; }
        public virtual TrainingCours TrainingCours { get; set; }
        public virtual WebsiteUser WebsiteUser { get; set; }
    }
}
