using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    public class ValidateModels
    {
        # region Variable Declaration
        private static byte[] KEY_64 = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV_64 = { 55, 103, 246, 79, 36, 99, 167, 3 };
        # endregion

        #region Private Methods
        public string Encrypt(string value)
        {
            //try
            //{
            System.Security.Cryptography.DESCryptoServiceProvider cryptoProvider = new System.Security.Cryptography.DESCryptoServiceProvider();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), System.Security.Cryptography.CryptoStreamMode.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(cs);
            sw.Write(value);
            sw.Flush();
            cs.FlushFinalBlock();
            ms.Flush();
            //'convert back to a string
            return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt16(ms.Length));
            //}
            //catch
            //{
            //    return null;
            //}
        }
        #endregion
    }

    public class GrossMarginRpt
    {
        [Required(ErrorMessage = "Provide Start Date.")]
        public DateTime StartDt { get; set; }
        [Required(ErrorMessage = "Provide End Date.")]
        public DateTime EndDt { get; set; }
        public int? Flag { get; set; }
        public List<getGrossMarginRpt_Result> RptList { get; set; }
    }

    public class AttendanceRpt
    {
        [Required(ErrorMessage = "Provide Start Date.")]
        public DateTime StartDt { get; set; }
        [Required(ErrorMessage = "Provide End Date.")]
        public DateTime EndDt { get; set; }
        public int? Position { get; set; }
        public List<GetAttendanceReport_Result> RptList { get; set; }
    }

    public class BatRpt
    {
        public List<AtteRptbyBatch_Result> RptList { get; set; }
    }

    public class PayingTrainees
    {
        public List<GetPayingTrainees_Result> RptList { get; set; }
    }

    public class DashBoaradInfo
    {
        public int InEval { get; set; }
        public int SelTrain { get; set; }
        public int ClientPlaced { get; set; }
        public int InMarket { get; set; }

        public List<getDashBoardMarket_Result> Market { get; set; }

        public List<getDashBoardTraining_Result> Training { get; set; }
    }

    public class UserLogin
    {
        [Required(ErrorMessage = "Provide Password.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Provide Username.")]
        public string username { get; set; }

        public bool Remember { get; set; }
    }


    public class Users
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Provide Full Name.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Provide Password.")]
        public string password { get; set; }
        [Required(ErrorMessage = "Provide Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Provide Username.")]
        public string username { get; set; }
        [Required(ErrorMessage = "Select Role.")]
        public int Roles_id { get; set; }
        public bool ActiveInactive { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }

        public string RoleName { get; set;  }

        public bool Dashboard { get; set; }
        public bool BatchList { get; set; }
        public bool Placements { get; set; }
        public bool TraineePortal { get; set; }
        public bool AdminPortal { get; set; }
        public bool Settings { get; set; }
        public bool WebsiteDashboard { get; set; }
        public bool AdminWebsite { get; set; }
        public bool Marketing { get; set; }
    }

    public class Batch
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Select Position.")]
        public int PositionId { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Provide Batch Start Date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //public Nullable<System.DateTime> BatchStartDt { get; set; }
        public System.DateTime BatchStartDt { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Provide Evaluation Date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //public Nullable<System.DateTime> BatchStartDt { get; set; }
        public System.DateTime EvaluationDt { get; set; }

        public bool Active { get; set; }
    }

    public class viewProfile
    {
        public ProfileUser pu { get; set; }

        public List<Profileinteraction> pi { get; set; }

        public List<ProfileAdditionalCalls> pac { get; set; }

        public GetProfileEvaluation_Result EvaLoc { get; set; }

        public List<PMarketing> pMar { get; set; }

        public List<PCInterview> pci { get; set; }
    }

    public class ProfileUser
    {
        public int PID { get; set; }
        [Required(ErrorMessage = "Provide First Name.")]
        public string Fname { get; set; }
        [Required(ErrorMessage = "Provide Last Name.")]
        public string LName { get; set; }
        [Required(ErrorMessage = "Provide Contact #.")]
        public string Contact { get; set; }
        [Required(ErrorMessage = "Provide Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Provide Address.")]
        public string Address { get; set; }
        public string Education { get; set; }

        public string Referred_by { get; set; }

        public string Applied_Via { get; set; }
        public string Resume { get; set; }
        public string Notes { get; set; }
        public int BatchId { get; set; }

        public DateTime? BatchStartDt { get; set; }
        public string Position { get; set; }
        public string OfferLetterSent { get; set; }
        public string OfferAccepted { get; set; }
        public int created_by { get; set; }
        public System.DateTime created_On { get; set; }
        public bool offer { get; set; }

        public string Pwd { get; set; }

        public string CPwd { get; set; }

        public string SalStartDt { get; set; }

        public decimal Salary { get; set; }

        public string Filter { get; set; }

        public bool PayingTrainee { get; set; }

        public string SkypeID { get; set; }
        public string Birthday { get; set; }
        public string BirthdayYear { get; set; }
        public string BirthdayMonth { get; set; }
        public string BirthdayDay { get; set; }
        public bool VisaHelp { get; set; }
        public Nullable<int> SexTypeID { get; set; }
        public Nullable<int> RecruiterID { get; set; }
        public bool BackgroundCheck { get; set; }
        public Nullable<bool> WillingToRelocate { get; set; }
        public bool AccommodationHelp { get; set; }
        public string ExperienceYears { get; set; }
        public string NationalityStatus { get; set; }

        public string BackgroundCheckOpt { get; set; }
        public string AccommodationHelpOpt { get; set; }
        public string VisaHelpOpt { get; set; }

        public int UserCodingExperienceID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> CodingExperienceBefore { get; set; }
        public int FormID { get; set; }
        public string Gender { get; set; }
        public string Recuiter { get; set; }
        public int Age { get; set; }
        public Nullable<int> EducationID { get; set; }
        public Nullable<int> VisaStatusID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Visa { get; set; }
        public string Country { get; set; }
        public string TrainingMode { get; set; }
        public Nullable<bool> CriminalRecords { get; set; }
        public string Records { get; set; }
        public string CriminalRecordsOpt { get; set; }
        public string WillingToRelocateOpt { get; set; }

        public Nullable<bool> TaughtMyselfCoding { get; set; }
        public Nullable<bool> AttendedComputerBootCamp { get; set; }
        public Nullable<bool> StudiedCodingCollege { get; set; }
        public Nullable<bool> LiberalArtsBackground { get; set; }
        public Nullable<bool> ScienceBackground { get; set; }
        public Nullable<bool> StudiedOnlineCodeCamp { get; set; }
        public Nullable<bool> StudiedW3Schools { get; set; }
        public Nullable<bool> StudiedYoutubeVideos { get; set; }
        public Nullable<bool> ResearchStackOverFlow { get; set; }
        public Nullable<bool> StudiedOnlineUdemy { get; set; }
        public Nullable<bool> PracticedAlgorithms { get; set; }
        public Nullable<bool> GoogleAnswers { get; set; }
        public Nullable<bool> NoExperience { get; set; }
    }

    public class ProfilePwd
    {
        public int PID { get; set; }
        public string Fname { get; set; }
        public string LName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Provide Password.")]
        public string Pwd { get; set; }
        [Required(ErrorMessage = "Provide Password.")]
        [Compare("Pwd", ErrorMessage = "The confirm password and password do not match!")]
        public string CPwd { get; set; }
    }

    public class ChangePwd
    {
        public int Flag { get; set; }

        [Required(ErrorMessage = "Provide Current Password.")]
        public string OPwd { get; set; }

        [Required(ErrorMessage = "Provide New Password.")]
        public string Pwd { get; set; }
        [Required(ErrorMessage = "Provide Password.")]
        [Compare("Pwd", ErrorMessage = "The confirm password and password do not match!")]
        public string CPwd { get; set; }
    }

    public class Profileinteraction
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Provide Recruitment Interview by.")]
        public int RecruitmentInterview_by { get; set; }
        [Required(ErrorMessage = "Provide Recruitment Interview On.")]
        public System.DateTime RecruitmentInterview_On { get; set; }
        [Required(ErrorMessage = "Provide Results.")]
        public string Results { get; set; }
        [Required(ErrorMessage = "Provide Results.")]
        public string Notes { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Profile_users_PID { get; set; }

        public string ProfileName { get; set; }

        public string RecruitmentInterviewbyName { get; set; }
        public string CreatedByName { get; set; }
    }

    public class ProfileAdditionalCalls
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Provide Contacted On.")]
        public System.DateTime ContactedOn { get; set; }
        [Required(ErrorMessage = "Provide Contacted By.")]
        public int ContactedBy { get; set; }
        [Required(ErrorMessage = "Provide Notes.")]
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int Profile_users_PID { get; set; }
        public string ProfileName { get; set; }

        public string CreatedByName { get; set; }
        public string ContactedByName { get; set; }
    }

    public class EvalutionLoc
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Provide Evaluation Dt.")]
        public System.DateTime EvalDate { get; set; }
        public bool AppearedInEval { get; set; }
        
        [Foolproof.RequiredIf("AppearedInEval", true, ErrorMessage = "Provide Result")]
        public string Result { get; set; }
        public string Comment { get; set; }
        public bool Selected { get; set; }
        public int? reasonForNotSelectingId { get; set; }
        public int Profile_users_PID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public string ReasonName { get; set; }
    }


    public class PMarketing
    {
        public int MId { get; set; }
        [Required(ErrorMessage = "Provide Marketing Start Date.")]
        public System.DateTime StartDt { get; set; }
        [Required(ErrorMessage = "Select Sales Team.")]
        public int SalesTeamId { get; set; }
        [Required(ErrorMessage = "Select Final Resume.")]
        public string FinalResume { get; set; }
        public bool Placed { get; set; }
        //[Foolproof.RequiredIf("Placed", true, ErrorMessage = "Provide Client Name")]
        public string ClientName { get; set; }
        //[Foolproof.RequiredIf("Placed", true, ErrorMessage = "Provide Client Location")]
        public string ClientLocation { get; set; }
        //[Foolproof.RequiredIf("Placed", true, ErrorMessage = "Provide Project Start Date")]
        public Nullable<System.DateTime> ProjectStartDt { get; set; }
        //[Foolproof.RequiredIf("Placed", true, ErrorMessage = "Provide Hourly Rate")]
        public Nullable<decimal> HourlyRate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public string SalesTeamName { get; set; }
        public string CreatedByName { get; set; }

        public int Profile_users_PID { get; set; }
        public string ProfileName { get; set; }
    }

    public class Reason
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provide Reason.")]
        public string Reasons { get; set; }
        public bool Active { get; set; }
    }

    public class Positions
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provide Title.")]
        public string Title { get; set; }
        public bool Active { get; set; }
    }

    public class Trainee
    {
        public ProfileUser pu { get; set; }

        public List<PCInterview> pci { get; set; }

        public int taId { get; set; }
        public System.DateTime taDt { get; set; }
        public bool taAbsent { get; set; }
        [Foolproof.RequiredIf("taAbsent", true, ErrorMessage = "Select Reason")]
        public int? taReasonId { get; set; }
        public int taProfileId { get; set; }
    }

    public partial class PCInterview
    {
        public int pcId { get; set; }
        public int ProfileUserId { get; set; }
        [Required(ErrorMessage = "Provide Interview Date.")]
        public System.DateTime InverviewDt { get; set; }
        [Required(ErrorMessage = "Provide Client Name.")]
        public string ClientName { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ProfileName { get; set; }
    }
}