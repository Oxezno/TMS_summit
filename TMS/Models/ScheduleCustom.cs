using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    //[MetadataType(typeof(ScheduleCustom))]
    //public partial class Schedule
    //{
        
    //}
    public class ScheduleCustom
    {
        public int ScheduleID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int WebsiteUserID { get; set; }
        
        public int CreatedBy { get; set; }
        public string scheduledBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> TrainingCourseID { get; set; }
        public string Subject { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public Nullable<int> ScheduleType { get; set; }
        public virtual User User { get; set; }
        public virtual TrainingCours TrainingCours { get; set; }
        public virtual WebsiteUser WebsiteUser { get; set; }
        public string Type { get; set; }
        public string WebsiteUserName { get; set; }
        public string TrainingCourseName { get; set; }
    }
}