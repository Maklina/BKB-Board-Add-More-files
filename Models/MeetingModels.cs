using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BKBBoard.Models
{
    public class MeetingViewModel
    {
        public MeetingMasterViewModel mMaster{get; set;}
        public List<MeetingDetailViewModel> mDetails { get; set; }
        public IEnumerable<SelectListItem> meetingTypeList { get; set; }
        
        [Display(Name = "Agenda")]
        public string AgendaURL { get; set; }
        [Display(Name = "Agenda")]

        public HttpPostedFileBase Agendafile { get; set; }

        [Display(Name = "Minutes")]
        public string MinutesURL { get; set; }
        [Display(Name = "Board Minutes")]

        public HttpPostedFileBase Minutesfile { get; set; }
        [Display(Name = "EC Minutes")]

        public HttpPostedFileBase ECMinutesfile { get; set; }
        [Display(Name = "Audit Minutes")]

        public HttpPostedFileBase AuditMinutesfile { get; set; }
        public IEnumerable<HttpPostedFileBase> Memofile { get; set; }

        [Display(Name = "Meeting Type")]
        public IEnumerable<string> MemoSubject { get; set; }
        [Display(Name = "Associating Dept")]
        public IEnumerable<int> department { get; set; }
        public IEnumerable<string> UserMemoFileName { get; set; }
        public bool agendaChecked { get; set; }
        public bool MinutesChecked { get; set;  }
        public bool ECMinutesChecked { get; set; }
        public bool AuditMinutesChecked { get; set; }
        public string UserFileName { get; set; }
        public string UserMinuteFileName { get; set; }
        public string UserECMinuteFileName { get; set; }
        public string UserAuditMinuteFileName { get; set; }
    }

    public class MeetingMasterViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Meeting No")]

        public int MeetingNo { get; set; }
        [Display(Name = "Meeting Name")]
        public string Title { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Date { get; set; }
        
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
        [Display(Name = "Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name = "Agenda")]
        public string AgendaURL { get; set; }
        [Display(Name = "Agenda")]

        public HttpPostedFileBase Agendafile { get; set; }

        [Display(Name = "Minutes")]
        public string MinutesURL { get; set; }
        [Display(Name = "Minutes")]

        public HttpPostedFileBase Minutesfile { get; set; }

        [Display(Name = "ECMinutes")]
        public string ECMinutesURL { get; set; }
        [Display(Name = "ECMinutes")]

        public HttpPostedFileBase ECMinutesfile { get; set; }

        [Display(Name = "AuditMinutes")]
        public string AuditMinutesURL { get; set; }
        [Display(Name = "AuditMinutes")]

        public HttpPostedFileBase AuditMinutesfile { get; set; }
        [Display(Name = "Meeting Type")]

        public string MeetingType { get; set; }

        public string UserFileName { get; set; }
        public string UserMinuteFileName { get; set;  }
        public string UserECMinuteFileName { get; set; }
        public string UserAuditMinuteFileName { get; set; }
    }
    
    public class MeetingDetailViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Meeting No.")]
        public int MeetingID { get; set; }
        [Display(Name = "Memo Serial")]
        public int MemoNo { get; set; }
        [Display(Name = "Memo Sub Serial")]
        public string MemoSubSerial { get; set; }
        [Display(Name = "Memo Subject")]
        public string MemoSubject { get; set; }
        [Display(Name = "Associating Dept")]
        public int Dept { get; set; }
        public string deptName { get; set; }
        public IEnumerable<SelectListItem> deptList { get; set; }

        [Display(Name = "Memo")]
        public string memoURL { get; set; }
        public HttpPostedFileBase Memofile { get; set; }
      
        public bool isChecked { get; set;  }
        public string UserMemoFileName { get; set; }
       

    }
}