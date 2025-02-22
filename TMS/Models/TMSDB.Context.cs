﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TMSEntities : DbContext
    {
        public TMSEntities()
            : base("name=TMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<TrainingResource> TrainingResources { get; set; }
        public virtual DbSet<TrainingSession> TrainingSessions { get; set; }
        public virtual DbSet<WebsiteusersAssignment> WebsiteusersAssignments { get; set; }
        public virtual DbSet<TrainingTrainer> TrainingTrainers { get; set; }
        public virtual DbSet<TrainingType> TrainingTypes { get; set; }
        public virtual DbSet<WebsiteUserDetail> WebsiteUserDetails { get; set; }
        public virtual DbSet<WebsiteUser> WebsiteUsers { get; set; }
        public virtual DbSet<WebsiteUserAttendance> WebsiteUserAttendances { get; set; }
        public virtual DbSet<Recruiter> Recruiters { get; set; }
        public virtual DbSet<MockInterview> MockInterviews { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TrainingCours> TrainingCourses { get; set; }
        public virtual DbSet<WebsiteUserTraining> WebsiteUserTrainings { get; set; }
        public virtual DbSet<WebsiteInvoice> WebsiteInvoices { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<CountryState> CountryStates { get; set; }
        public virtual DbSet<ResumeSendApproval> ResumeSendApprovals { get; set; }
        public virtual DbSet<ContractTemplate> ContractTemplates { get; set; }
        public virtual DbSet<WebsiteUserResume> WebsiteUserResumes { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<InterviewQuestion> InterviewQuestions { get; set; }
    
        public virtual ObjectResult<GetProfileEvaluation_Result> GetProfileEvaluation(Nullable<int> pId)
        {
            var pIdParameter = pId.HasValue ?
                new ObjectParameter("PId", pId) :
                new ObjectParameter("PId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetProfileEvaluation_Result>("GetProfileEvaluation", pIdParameter);
        }
    
        public virtual ObjectResult<GetUsersList_Result> GetUsersList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUsersList_Result>("GetUsersList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter);
        }
    
        public virtual ObjectResult<GetPlacementList_Result> GetPlacementList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPlacementList_Result>("GetPlacementList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter);
        }
    
        public virtual ObjectResult<GetPositionList_Result> GetPositionList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPositionList_Result>("GetPositionList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter);
        }
    
        public virtual ObjectResult<GetReasonList_Result> GetReasonList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetReasonList_Result>("GetReasonList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter);
        }
    
        public virtual ObjectResult<getDashBoard_Result> getDashBoard()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getDashBoard_Result>("getDashBoard");
        }
    
        public virtual ObjectResult<getDashBoardTraining_Result> getDashBoardTraining()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getDashBoardTraining_Result>("getDashBoardTraining");
        }
    
        public virtual ObjectResult<getDashBoardMarket_Result> getDashBoardMarket()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getDashBoardMarket_Result>("getDashBoardMarket");
        }
    
        public virtual ObjectResult<GetAttendanceReport_Result> GetAttendanceReport(Nullable<System.DateTime> startDt, Nullable<System.DateTime> endDt, Nullable<int> tech)
        {
            var startDtParameter = startDt.HasValue ?
                new ObjectParameter("StartDt", startDt) :
                new ObjectParameter("StartDt", typeof(System.DateTime));
    
            var endDtParameter = endDt.HasValue ?
                new ObjectParameter("EndDt", endDt) :
                new ObjectParameter("EndDt", typeof(System.DateTime));
    
            var techParameter = tech.HasValue ?
                new ObjectParameter("Tech", tech) :
                new ObjectParameter("Tech", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAttendanceReport_Result>("GetAttendanceReport", startDtParameter, endDtParameter, techParameter);
        }
    
        public virtual ObjectResult<getGrossMarginRpt_Result> getGrossMarginRpt(Nullable<System.DateTime> startDt, Nullable<System.DateTime> endDt, Nullable<int> flag)
        {
            var startDtParameter = startDt.HasValue ?
                new ObjectParameter("StartDt", startDt) :
                new ObjectParameter("StartDt", typeof(System.DateTime));
    
            var endDtParameter = endDt.HasValue ?
                new ObjectParameter("EndDt", endDt) :
                new ObjectParameter("EndDt", typeof(System.DateTime));
    
            var flagParameter = flag.HasValue ?
                new ObjectParameter("Flag", flag) :
                new ObjectParameter("Flag", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getGrossMarginRpt_Result>("getGrossMarginRpt", startDtParameter, endDtParameter, flagParameter);
        }
    
        public virtual ObjectResult<GetBatchesList_Result> GetBatchesList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetBatchesList_Result>("GetBatchesList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter);
        }
    
        public virtual ObjectResult<getProfileList_Result> getProfileList(Nullable<int> pageNum, Nullable<int> pageSize, string sortExp, string searchText, Nullable<int> batchId, Nullable<int> filter)
        {
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("pageNum", pageNum) :
                new ObjectParameter("pageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpParameter = sortExp != null ?
                new ObjectParameter("SortExp", sortExp) :
                new ObjectParameter("SortExp", typeof(string));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            var batchIdParameter = batchId.HasValue ?
                new ObjectParameter("BatchId", batchId) :
                new ObjectParameter("BatchId", typeof(int));
    
            var filterParameter = filter.HasValue ?
                new ObjectParameter("Filter", filter) :
                new ObjectParameter("Filter", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getProfileList_Result>("getProfileList", pageNumParameter, pageSizeParameter, sortExpParameter, searchTextParameter, batchIdParameter, filterParameter);
        }
    
        public virtual ObjectResult<AtteRptbyBatch_Result> AtteRptbyBatch(Nullable<int> batchId)
        {
            var batchIdParameter = batchId.HasValue ?
                new ObjectParameter("BatchId", batchId) :
                new ObjectParameter("BatchId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AtteRptbyBatch_Result>("AtteRptbyBatch", batchIdParameter);
        }
    
        public virtual ObjectResult<GetPayingTrainees_Result> GetPayingTrainees()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPayingTrainees_Result>("GetPayingTrainees");
        }
    
        public virtual ObjectResult<GetForms_Result> GetForms()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetForms_Result>("GetForms");
        }
    
        public virtual ObjectResult<GetRecruiters_Result> GetRecruiters(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("companyID", companyID) :
                new ObjectParameter("companyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetRecruiters_Result>("GetRecruiters", companyIDParameter);
        }
    
        public virtual ObjectResult<GetBatches_Result> GetBatches()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetBatches_Result>("GetBatches");
        }
    
        public virtual ObjectResult<GetCountries_Result> GetCountries()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCountries_Result>("GetCountries");
        }
    
        public virtual ObjectResult<GetVisaStatus_Result> GetVisaStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVisaStatus_Result>("GetVisaStatus");
        }
    
        public virtual ObjectResult<GetBatchCandidates_Result> GetBatchCandidates(Nullable<int> batchID)
        {
            var batchIDParameter = batchID.HasValue ?
                new ObjectParameter("batchID", batchID) :
                new ObjectParameter("batchID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetBatchCandidates_Result>("GetBatchCandidates", batchIDParameter);
        }
    
        public virtual ObjectResult<GetWebsiteUsers_Result> GetWebsiteUsers()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetWebsiteUsers_Result>("GetWebsiteUsers");
        }
    
        public virtual ObjectResult<GetTrainingCoursesByName_Result> GetTrainingCoursesByName(string name)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTrainingCoursesByName_Result>("GetTrainingCoursesByName", nameParameter);
        }
    
        public virtual ObjectResult<GetTrainingCourses_Result> GetTrainingCourses()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTrainingCourses_Result>("GetTrainingCourses");
        }
    }
}
