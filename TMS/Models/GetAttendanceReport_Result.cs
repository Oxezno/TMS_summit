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
    
    public partial class GetAttendanceReport_Result
    {
        public int PID { get; set; }
        public string pName { get; set; }
        public string Title { get; set; }
        public System.DateTime BatchStartDt { get; set; }
        public Nullable<int> Attended { get; set; }
        public Nullable<int> NotAttended { get; set; }
    }
}
