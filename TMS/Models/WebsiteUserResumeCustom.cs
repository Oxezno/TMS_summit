using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    [MetadataType(typeof(WebsiteUserResumeCustom))]
    public partial class WebsiteUserResume
    {

    }

    public class WebsiteUserResumeCustom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WebsiteUserResumeCustom()
        {
            this.ResumeSendApprovals = new HashSet<ResumeSendApproval>();
        }

        public int ResumeID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> WebsiteUserID { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<int> UserCreate { get; set; }
        public List<int> SendToID { get; set; }
        public List<bool> Send { get; set; }
        public string Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResumeSendApproval> ResumeSendApprovals { get; set; }
        public virtual User User { get; set; }
        public virtual WebsiteUser WebsiteUser { get; set; }
    }
}