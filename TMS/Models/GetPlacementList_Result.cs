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
    
    public partial class GetPlacementList_Result
    {
        public Nullable<int> BatchId { get; set; }
        public Nullable<int> PID { get; set; }
        public Nullable<int> PMarketingId { get; set; }
        public string PFullName { get; set; }
        public System.DateTime StartDt { get; set; }
        public string Marketer { get; set; }
        public string Technology { get; set; }
        public int TotalInterviews { get; set; }
        public Nullable<int> TotalCount { get; set; }
    }
}
