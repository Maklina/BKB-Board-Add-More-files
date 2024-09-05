using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.Models;
using BKBBoard.DataModel;
using System.Data.Entity.Validation;
using System.IO;
using System.Configuration;
using BKBBoard.Models.AppManager;
using System.Data.Entity;
using PagedList;
using log4net;
using System.Reflection;
using Microsoft.Ajax.Utilities;

namespace BKBBoard.Controllers
{
    public class MeetingController : Controller
    {

        BKBBoardEntities db = new BKBBoardEntities();
        MeetingInfoManager manager = new MeetingInfoManager();
        string FileFolder = ConfigurationManager.AppSettings["filepath"];
        LookupValue lv = new LookupValue();
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Meeting
        public ActionResult Index(string meetingType, string Number, int? page)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                if (String.IsNullOrEmpty(meetingType))
                {
                    meetingType = "BOARD MEETING";
                }
                try
                {
                    if (login.RoleName == "1")
                    {
                        List<MeetingMasterViewModel> meeting = (from q in db.MeetingMasters
                                                                join r in db.meetingTypes on q.MeetingType equals r.id
                                                                where r.type.Equals(meetingType)
                                                                orderby q.Date descending
                                                                select new MeetingMasterViewModel
                                                                {
                                                                    ID = q.ID,
                                                                    MeetingNo = q.MeetingNo,
                                                                    MeetingType = r.type,
                                                                    Title = q.Title,
                                                                    Date = q.Date,
                                                                    AgendaURL = q.AgendaURL,
                                                                    MinutesURL = q.MinutesURL,
                                                                    UserFileName = q.UserFileName,
                                                                    UserMinuteFileName = q.UserMinuteFileName,
                                                                    ECMinutesURL = q.ECMinutesURL,                                                                 
                                                                    UserECMinuteFileName = q.UserECMinutesFileName,
                                                                    AuditMinutesURL = q.AuditMinutesURL,
                                                                    UserAuditMinuteFileName = q.UserAuditMinutesFileName

                                                                }).ToList();

                        if (!String.IsNullOrEmpty(Number))
                        {
                            int meetingNo = Convert.ToInt32(Number);
                            meeting = meeting.Where(o => o.MeetingNo == meetingNo).ToList();
                        }
                        int pageSize = 25;
                        int pageNumber = (page ?? 1);
                        return View(meeting.ToPagedList(pageNumber, pageSize));
                    }
                    else if (login.RoleName == "2")
                    {
                        List<MeetingMasterViewModel> meeting = (from q in db.MeetingMasters
                                                                join r in db.meetingTypes on q.MeetingType equals r.id
                                                                where r.type.Equals(meetingType)
                                                                orderby q.Date descending
                                                                select new MeetingMasterViewModel
                                                                {
                                                                    ID = q.ID,
                                                                    MeetingNo = q.MeetingNo,
                                                                    MeetingType = r.type,
                                                                    Title = q.Title,
                                                                    Date = q.Date,
                                                                    AgendaURL = q.AgendaURL,
                                                                    MinutesURL = q.MinutesURL,
                                                                    UserFileName = q.UserFileName,
                                                                    UserMinuteFileName = q.UserMinuteFileName,
                                                                    ECMinutesURL = q.ECMinutesURL,
                                                                    UserECMinuteFileName = q.UserECMinutesFileName,
                                                                    AuditMinutesURL = q.AuditMinutesURL,
                                                                    UserAuditMinuteFileName = q.UserAuditMinutesFileName

                                                                }).ToList();

                        if (!String.IsNullOrEmpty(Number))
                        {
                            int meetingNo = Convert.ToInt32(Number);
                            meeting = meeting.Where(o => o.MeetingNo == meetingNo).ToList();
                        }
                        int pageSize = 25;
                        int pageNumber = (page ?? 1);
                        return View(meeting.ToPagedList(pageNumber, pageSize));
                    }
                    else/* (login.UserRoleId == 2)*/
                    {
                        var ID = db.MeetingMasters.Max(o => o.ID);
                        //List<MeetingMasterViewModel>
                        var meeting = (from q in db.MeetingMasters
                                       join r in db.meetingTypes on q.MeetingType equals r.id
                                       where q.ID == ID
                                       //orderby q.Date descending
                                       select new MeetingMasterViewModel
                                       {
                                           ID = q.ID,
                                           MeetingNo = q.MeetingNo,
                                           MeetingType = r.type,
                                           Title = q.Title,
                                           Date = q.Date,
                                           AgendaURL = q.AgendaURL,
                                           MinutesURL = q.MinutesURL,
                                           UserFileName = q.UserFileName,
                                           UserMinuteFileName = q.UserMinuteFileName,
                                           ECMinutesURL = q.ECMinutesURL,
                                           UserECMinuteFileName = q.UserECMinutesFileName,
                                           AuditMinutesURL = q.AuditMinutesURL,
                                           UserAuditMinuteFileName = q.UserAuditMinutesFileName

                                       }).ToList();

                        if (!String.IsNullOrEmpty(Number))
                        {
                            int meetingNo = Convert.ToInt32(Number);
                            meeting = meeting.Where(o => o.MeetingNo == meetingNo).ToList();
                        }
                        int pageSize = 25;
                        int pageNumber = (page ?? 1);
                        return View(meeting.ToPagedList(pageNumber, pageSize));
                        //return View(meeting);
                    }
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Error:" + ex.Message;
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }



        }
        [HttpGet]
        public ActionResult CreateMeeting()
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                MeetingViewModel mv = new MeetingViewModel();
                mv.meetingTypeList = lv.getMeetingType();
                return View(mv);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult CreateMeeting(MeetingViewModel m)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {

                try
                {
                    if (m.Agendafile != null)
                    {
                        int ID = Convert.ToInt32(m.mMaster.MeetingType);
                  
                        var root = db.meetingTypes.Where(o => o.id == ID).FirstOrDefault();
                        string path = FileFolder + root.type;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }
                        path = path + "/" + m.mMaster.MeetingNo;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        var agendaExtension = Path.GetExtension(m.Agendafile.FileName);
                        var agendaFileName = "Agenda-file-" + m.mMaster.MeetingNo + agendaExtension;
                        m.mMaster.UserFileName = m.Agendafile.FileName;
                        string MinutesExtension = null, MinutesFileName = null, MinutesPath = null;
                        string ECMinutesExtension = null, ECMinutesFileName = null, ECMinutesPath = null;
                        string AuditMinutesExtension = null, AuditMinutesFileName = null, AuditMinutesPath = null;
                        if (m.Minutesfile != null)
                        {
                            MinutesExtension = Path.GetExtension(m.Minutesfile.FileName);
                            MinutesFileName = "Minutes-file-" + m.mMaster.MeetingNo + MinutesExtension;
                            m.mMaster.UserMinuteFileName = m.Minutesfile.FileName;
                            MinutesPath = Path.Combine(Server.MapPath(path), MinutesFileName);
                        }
                        if (m.ECMinutesfile != null)
                        {
                            ECMinutesExtension = Path.GetExtension(m.ECMinutesfile.FileName);
                            ECMinutesFileName = "ECMinutes-file-" + m.mMaster.MeetingNo + ECMinutesExtension;
                            m.mMaster.UserECMinuteFileName = m.ECMinutesfile.FileName;
                            ECMinutesPath = Path.Combine(Server.MapPath(path), ECMinutesFileName);
                        }
                        if (m.AuditMinutesfile != null)
                        {
                            AuditMinutesExtension = Path.GetExtension(m.AuditMinutesfile.FileName);
                            AuditMinutesFileName = "AuditMinutes-file-" + m.mMaster.MeetingNo + AuditMinutesExtension;
                            m.mMaster.UserAuditMinuteFileName = m.AuditMinutesfile.FileName;
                            AuditMinutesPath = Path.Combine(Server.MapPath(path), AuditMinutesFileName);
                        }
                        string AgendaPath = Path.Combine(Server.MapPath(path), agendaFileName);

                        m.mMaster.AgendaURL = agendaFileName;
                        m.mMaster.MinutesURL = MinutesFileName;
                        m.mMaster.ECMinutesURL = ECMinutesFileName;
                        m.mMaster.AuditMinutesURL = AuditMinutesFileName;
                        m.Agendafile.SaveAs(AgendaPath);
                        if (m.Minutesfile != null)
                        {
                            m.Minutesfile.SaveAs(MinutesPath);
                        }
                        if (m.ECMinutesfile != null)
                        {
                            m.ECMinutesfile.SaveAs(ECMinutesPath);
                        }
                        if (m.AuditMinutesfile != null)
                        {
                            m.AuditMinutesfile.SaveAs(AuditMinutesPath);
                        }
                        string memoExtension, memoFileName, memoPath;
                        List<MeetingDetailViewModel> memoes = new List<MeetingDetailViewModel>();
                        for (int i = 0; i < m.MemoSubject.Count(); i++)
                        {
                            MeetingDetailViewModel memo = new MeetingDetailViewModel();
                            memo.MemoSubject = m.MemoSubject.ElementAt(i);
                            memo.Dept = m.department.ElementAt(i);
                            if (m.Memofile.ElementAt(i) != null)
                            {
                                memo.Memofile = m.Memofile.ElementAt(i);
                                memoExtension = Path.GetExtension(memo.Memofile.FileName);
                                memoFileName = "Memo-file-" + m.mMaster.MeetingNo + "-Serial-" + (i + 1) + memoExtension;
                                memo.UserMemoFileName = memo.Memofile.FileName;
                                memoPath = Path.Combine(Server.MapPath(path), memoFileName);

                                memo.memoURL = memoFileName;
                                memo.Memofile.SaveAs(memoPath);
                            }

                            memoes.Add(memo);
                        }

                        if (SaveFile(m.mMaster, memoes))
                        {
                            TempData["retMsg"] = "Uploaded Successfully !!";

                            return RedirectToAction("Index", "Meeting", new { meetingType = root.type });
                        }
                        else
                        {
                            TempData["errMsg"] = "Couldn't create the meeting!";
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("CreateMeeting", "Meeting");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "File not found!";
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("CreateMeeting", "Meeting");
                    }
                }

                catch (Exception ex)
                {
                    TempData["errMsg"] = "Error:" + ex.Message;
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("CreateMeeting", "Meeting");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //return Json(formData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewDetails(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {

                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                mm = manager.getMeetingMasterData(id);
                //return Json(mm, JsonRequestBehavior.AllowGet);
                MeetingDetailViewModel md = new MeetingDetailViewModel();
                MeetingViewModel mv = new MeetingViewModel();
                mv.mMaster = mm;
                mv.mDetails = manager.getMemoData(id);
                return View(mv);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpGet]
        public ActionResult DeleteMemoFiles(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {   

                try
                {
                    var Remove = db.MeetingDetails.Find(id);

                    db.MeetingDetails.Remove(Remove);
                    db.SaveChanges();

                    TempData["retMsg"] = "Memo Files Removed successfully.";
                    return RedirectToAction("Index", "Meeting");
                }
                catch (Exception e)
                {
                    TempData["errMsg"] = "Error:" + e.Message;
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("ViewDetails", "Meeting");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpGet]
        public ActionResult EditMeeting(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {

                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                mm = manager.getMeetingMasterData(id);
                //return Json(mm, JsonRequestBehavior.AllowGet);
                MeetingDetailViewModel md = new MeetingDetailViewModel();
                MeetingViewModel mv = new MeetingViewModel();
                mv.mMaster = mm;
                mv.mDetails = manager.getMemoData(id);
                var query = from q in db.departments
                            select new
                            {
                                value = q.id,
                                description = q.deptName
                            };
                for (int i = 0; i < mv.mDetails.Count; i++) {
                    mv.mDetails.ElementAt(i).deptList = new SelectList(query.ToList(), "value", "description");
                }
                return View(mv);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult EditMeeting(MeetingViewModel m, MeetingDetailViewModel mdb)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                var memoFiles = m.mDetails;
                string memoExtension, memoFileName, memoPath, agendaExtension, agendaPath, agendaFileName, MinutesExtension, MinutesFileName, MinutesPath, ECMinutesExtension, ECMinutesFileName, ECMinutesPath, AuditMinutesExtension, AuditMinutesFileName, AuditMinutesPath; ;

                string path = FileFolder + m.mMaster.MeetingType + "/" + m.mMaster.MeetingNo;
                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                MeetingMaster mmdb = db.MeetingMasters.Find(m.mMaster.ID);
                var transaction = db.Database.BeginTransaction();
                if (m.agendaChecked == true)
                {
                    // try
                    //{
                    mm.Agendafile = m.Agendafile;
                    agendaExtension = Path.GetExtension(mm.Agendafile.FileName);
                    agendaFileName = "Agenda-file-" + m.mMaster.MeetingNo + agendaExtension;

                    agendaPath = Path.Combine(Server.MapPath(path), agendaFileName);
                    mm.AgendaURL = agendaFileName;
                    if (System.IO.File.Exists(agendaPath))
                    {
                        System.IO.File.Delete(agendaPath);
                    }
                    mm.Agendafile.SaveAs(agendaPath);
                    mmdb.AgendaURL = mm.AgendaURL;
                    mmdb.UserFileName = m.Agendafile.FileName;
                    mmdb.UpdatedBy = login.userName;
                    mmdb.UpdatedOn = DateTime.Now;
                    db.Entry(mmdb).State = EntityState.Modified;
                    db.SaveChanges();
                    /*}
                    catch (Exception e)
                    {
                        TempData["errMsg"] = "Error:" + e.Message;
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("EditMeeting", "Meeting");
                    }*/


                }
                if (m.MinutesChecked == true)
                {
                    //try
                    //{
                    mm.Minutesfile = m.Minutesfile;
                    MinutesExtension = Path.GetExtension(mm.Minutesfile.FileName);
                    MinutesFileName = "Minutes-file-" + m.mMaster.MeetingNo + MinutesExtension;
                    m.mMaster.UserMinuteFileName = m.Minutesfile.FileName;

                    MinutesPath = Path.Combine(Server.MapPath(path), MinutesFileName);
                    mm.MinutesURL = MinutesFileName;
                    if (System.IO.File.Exists(MinutesPath))
                    {
                        System.IO.File.Delete(MinutesPath);
                    }
                    mm.Minutesfile.SaveAs(MinutesPath);
                    mmdb.MinutesURL = mm.MinutesURL;
                    mmdb.UserMinuteFileName = m.Minutesfile.FileName;
                    mmdb.UpdatedBy = login.userName;
                    mmdb.UpdatedOn = DateTime.Now;
                    db.Entry(mmdb).State = EntityState.Modified;
                    db.SaveChanges();
                    /* }
                     catch (Exception e)
                     {
                         TempData["errMsg"] = "Error:" + e.Message;
                         //return Json(TempData, JsonRequestBehavior.AllowGet);
                         return RedirectToAction("EditMeeting", "Meeting");
                     }*/
                }

                if (m.ECMinutesChecked == true)
                {
                    //try
                    //{
                    mm.ECMinutesfile = m.ECMinutesfile;
                    ECMinutesExtension = Path.GetExtension(mm.ECMinutesfile.FileName);
                   ECMinutesFileName = "ECMinutes-file-" + m.mMaster.MeetingNo + ECMinutesExtension;
                    m.mMaster.UserECMinuteFileName = m.ECMinutesfile.FileName;

                    ECMinutesPath = Path.Combine(Server.MapPath(path), ECMinutesFileName);
                    mm.ECMinutesURL = ECMinutesFileName;
                    if (System.IO.File.Exists(ECMinutesPath))
                    {
                        System.IO.File.Delete(ECMinutesPath);
                    }
                    mm.ECMinutesfile.SaveAs(ECMinutesPath);
                    mmdb.ECMinutesURL = mm.ECMinutesURL;
                    mmdb.UserECMinutesFileName = m.ECMinutesfile.FileName;
                    mmdb.UpdatedBy = login.userName;
                    mmdb.UpdatedOn = DateTime.Now;
                    db.Entry(mmdb).State = EntityState.Modified;
                    db.SaveChanges();
                    /* }
                     catch (Exception e)
                     {op8i
                         TempData["errMsg"] = "Error:" + e.Message;
                         //return Json(TempData, JsonRequestBehavior.AllowGet);
                         return RedirectToAction("EditMeeting", "Meeting");
                     }*/
                }
                if (m.AuditMinutesChecked == true)
                {
                    //try
                    //{
                    mm.AuditMinutesfile = m.AuditMinutesfile;
                    AuditMinutesExtension = Path.GetExtension(mm.AuditMinutesfile.FileName);
                    AuditMinutesFileName = "AuditMinutes-file-" + m.mMaster.MeetingNo + AuditMinutesExtension;
                    m.mMaster.UserAuditMinuteFileName = m.AuditMinutesfile.FileName;

                    AuditMinutesPath = Path.Combine(Server.MapPath(path), AuditMinutesFileName);
                    mm.AuditMinutesURL = AuditMinutesFileName;
                    if (System.IO.File.Exists(AuditMinutesPath))
                    {
                        System.IO.File.Delete(AuditMinutesPath);
                    }
                    mm.AuditMinutesfile.SaveAs(AuditMinutesPath);
                    mmdb.AuditMinutesURL = mm.AuditMinutesURL;
                    mmdb.UserAuditMinutesFileName = m.AuditMinutesfile.FileName;
                    mmdb.UpdatedBy = login.userName;
                    mmdb.UpdatedOn = DateTime.Now;
                    db.Entry(mmdb).State = EntityState.Modified;
                    db.SaveChanges();
                    /* }
                     catch (Exception e)
                     {op8i
                         TempData["errMsg"] = "Error:" + e.Message;
                         //return Json(TempData, JsonRequestBehavior.AllowGet);
                         return RedirectToAction("EditMeeting", "Meeting");
                     }*/
                }
                int j = 0;
                for (int i = 0; i < m.mDetails.Count(); i++)
                {
                    if (m.mDetails[i].isChecked == true)
                    {
                        try
                        {
                            MeetingDetailViewModel memo = new MeetingDetailViewModel();
                            MeetingDetail md = new MeetingDetail();
                            int id = m.mDetails[i].ID;
                            md = db.MeetingDetails.Where(o => o.ID.Equals(id)).FirstOrDefault();
                            // md = db.MeetingDetails.Find(m.mDetails[i].ID);

                            //memo.MeetingID = checkedMemo.MeetingID;
                            // memo.MemoNo = checkedMemo.MemoNo;
                            md.MemoSerial = m.mDetails[i].MemoNo;
                            md.Subject = m.mDetails[i].MemoSubject;
                            md.Dept = m.mDetails[i].Dept;

                            memo.Memofile = m.Memofile.ElementAt(j);

                            memoExtension = Path.GetExtension(memo.Memofile.FileName);
                            memoFileName = "Memo-file-" + m.mMaster.MeetingNo + "-Serial-" + (md.MemoSerial) + memoExtension;

                            memoPath = Path.Combine(Server.MapPath(path), memoFileName);

                            memo.memoURL = memoFileName;
                            if (System.IO.File.Exists(memoPath))
                            {
                                System.IO.File.Delete(memoPath);
                            }
                            memo.Memofile.SaveAs(memoPath);
                            md.memoURL = memoFileName;
                            md.UserMemoFileName = memo.Memofile.FileName;
                            md.UpdatedBy = login.userName;
                            md.UpdatedOn = DateTime.Now;
                            try
                            {
                                db.Entry(md).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                TempData["errMsg"] = "Error:" + e.Message;
                                //return Json(TempData, JsonRequestBehavior.AllowGet);
                                return RedirectToAction("EditMeeting", "Meeting");
                            }
                            j++;

                        }

                        catch (Exception e)
                        {
                            TempData["errMsg"] = "Error:" + e.Message;
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("EditMeeting", "Meeting");
                        }

                    }
                }
                transaction.Commit();
                return RedirectToAction("ViewDetails", "Meeting", new { id = m.mMaster.ID });
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpGet]
        public ActionResult AddMoreMemoFiles(int? id)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
              
                MeetingMasterViewModel mm = new MeetingMasterViewModel();
                mm = manager.getMeetingMasterData(id);

                MeetingViewModel mv = new MeetingViewModel();
                mv.mMaster = mm;
                mv.mDetails = mv.mDetails;
                return View(mv);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult AddMoreMemoFiles(MeetingViewModel m)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {


                try
                {

                   // int ID = Convert.ToInt32(m.mMaster.MeetingType);
                    var mt = db.meetingTypes.Where(o => o.type == m.mMaster.MeetingType).FirstOrDefault();

                    var memserial = db.MeetingDetails.Where(o => o.MeetingID == m.mMaster.ID).Max(o => o.MemoSerial);
                    int MID = mt.id;// Convert.ToInt32(mt);
                    var root = db.meetingTypes.Where(o => o.id == MID).FirstOrDefault();

                    //string queryString = "select isnull(max(cast( SUBSTRING(FI_Contract_Code, 9, LEN(FI_Contract_Code)-8) as int)), 0)  from [BKBBoard].[dbo].[MeetingDetails]   where  MeetingID=' + ID +'    ";
                    //string strcon = ConfigurationManager.ConnectionStrings["CIB_Info_EditConnectionString"].ConnectionString;
                    //SqlConnection connection1 = new SqlConnection(strcon);
                    //connection1.Open();
                    //SqlCommand command = new SqlCommand(queryString, connection1);
                    //int result1 = (int)(command.ExecuteScalar());

                    string path = FileFolder + root.type;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }
                    path = path + "/" + m.mMaster.MeetingNo;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }


                    string memoExtension, memoFileName, memoPath;
                    List<MeetingDetailViewModel> memoes = new List<MeetingDetailViewModel>();
                    for (int i = 0; i < m.MemoSubject.Count(); i++)
                    {
                        MeetingDetailViewModel memo = new MeetingDetailViewModel();
                        memo.MemoSubject = m.MemoSubject.ElementAt(i);
                        memo.Dept = m.department.ElementAt(i);
                        if (m.Memofile.ElementAt(i) != null)
                        {
                            memo.Memofile = m.Memofile.ElementAt(i);
                            memoExtension = Path.GetExtension(memo.Memofile.FileName);
                            memoFileName = "Memo-file-" + m.mMaster.MeetingNo + "-Serial-" + (memserial+1+i) + memoExtension;
                            memo.UserMemoFileName = memo.Memofile.FileName;
                            memoPath = Path.Combine(Server.MapPath(path), memoFileName);

                            memo.memoURL = memoFileName;
                            memo.Memofile.SaveAs(memoPath);
                        }

                        memoes.Add(memo);
                    }

                       

                    if (SaveFileMore(m.mMaster, memoes))
                    {
                        //logger.Debug(SaveFileMore(m.mMaster, memoes));
                        TempData["retMsg"] = "Uploaded Successfully !!";

                        return RedirectToAction("Index", "Meeting", new { meetingType = root.type });
                    }
                    else
                    {
                        TempData["errMsg"] = "Couldn't Add More Memo Files of meeting!";
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("AddMoreMemoFiles", "Meeting");
                    }   

                       
                  
                }

                catch (Exception ex)
                {
                    TempData["errMsg"] = "Error:" + ex.Message;
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("AddMoreMemoFiles", "Meeting");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //return Json(formData, JsonRequestBehavior.AllowGet);
        }
        private bool SaveFileMore(MeetingMasterViewModel m, List<MeetingDetailViewModel> mdm)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            var transaction = db.Database.BeginTransaction();
            try
            {
                MeetingMaster mm = new MeetingMaster();
                var mt = db.meetingTypes.Where(o => o.type == m.MeetingType).FirstOrDefault();

                var memserial = db.MeetingDetails.Where(o => o.MeetingID == m.ID).Max(o => o.MemoSerial);
                int MID = mt.id;// Convert.ToInt32(mt);
                var root = db.meetingTypes.Where(o => o.id == MID).FirstOrDefault();

                //int meetingType = Convert.ToInt32(m.MeetingType);
                //var meeting = db.MeetingMasters.Where(o => o.MeetingNo.Equals(m.MeetingNo) && o.MeetingType.Equals(meetingType)).SingleOrDefault();
                int i = memserial+1;
                foreach (var item in mdm)
                {
                    MeetingDetail md = new MeetingDetail();
                    md.MeetingID = m.ID;
                    md.MemoSerial = i;
                    md.Subject = item.MemoSubject;
                    md.Dept = item.Dept;
                    md.memoURL = item.memoURL;
                    md.CreatedBy = login.UserId;
                    md.CreatedOn = DateTime.Now;
                    md.UserMemoFileName = item.UserMemoFileName;
                    db.MeetingDetails.Add(md);
                    i++;
                }
                db.SaveChanges();
                logger.Debug(db);
                transaction.Commit();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error(ex);
                        TempData["AlertMessage"] = validationError.PropertyName + "--" + validationError.ErrorMessage;
                    }
                }
                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                transaction.Rollback();
                TempData["AlertMessage"] = ex.Message;
                return false;
            }
        }

        private bool SaveFile(MeetingMasterViewModel m, List<MeetingDetailViewModel> mdm)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            var transaction = db.Database.BeginTransaction();
            try
            {
                MeetingMaster mm = new MeetingMaster();
                mm.MeetingNo = m.MeetingNo;
                mm.MeetingType = Convert.ToInt32(m.MeetingType);
                mm.Title = m.Title;
                mm.Date = m.Date;
                mm.AgendaURL = m.AgendaURL;
                mm.MinutesURL = m.MinutesURL == null ? " " : m.MinutesURL;
                mm.ECMinutesURL = m.ECMinutesURL;
                mm.AuditMinutesURL = m.AuditMinutesURL;
                mm.CreatedBy = login.userName;
                mm.CreatedOn = DateTime.Now;
                mm.UserFileName = m.UserFileName;
                mm.UserMinuteFileName = m.UserMinuteFileName;
                mm.UserECMinutesFileName = m.UserECMinuteFileName;
                mm.UserAuditMinutesFileName = m.UserAuditMinuteFileName;
                db.MeetingMasters.Add(mm);
                db.SaveChanges();

                int meetingType = Convert.ToInt32(m.MeetingType);
                var meeting = db.MeetingMasters.Where(o => o.MeetingNo.Equals(m.MeetingNo) && o.MeetingType.Equals(meetingType)).SingleOrDefault();
                int i = 1;
                foreach (var item in mdm)
                {
                    MeetingDetail md = new MeetingDetail();
                    md.MeetingID = meeting.ID;
                    md.MemoSerial = i;
                    md.Subject = item.MemoSubject;
                    md.Dept = item.Dept;
                    md.memoURL = item.memoURL;
                    md.CreatedBy = login.UserId;
                    md.CreatedOn = DateTime.Now;
                    md.UserMemoFileName = item.UserMemoFileName;
                    db.MeetingDetails.Add(md);
                    i++;
                }
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        TempData["AlertMessage"] = validationError.PropertyName + "--" + validationError.ErrorMessage;
                    }
                }
                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["AlertMessage"] = ex.Message;
                return false;
            }
        }
        //[Route("Meeting/Download/{fileName?}")]

     public ActionResult Download(String MeetingType, int MeetingNo, String fileName)
    {
            var FileVirtualPath = FileFolder + MeetingType + "/" + MeetingNo + "/" + fileName;

            string path = Server.MapPath(FileVirtualPath);
            string remoteFileUrl = path;
      
            var destinationDir = "D:\\" + "DownloadedFiles\\"; 
            //stores images in IIS Express directory
            //var destinationDir = Environment.CurrentDirectory + "\\NIDImages\\";
            var destinationPath = destinationDir + fileName;
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
          
                if (!Directory.Exists(destinationPath))
                {
                    var myWebClient = new WebClient();
                    //Download file from remote url and save it in destination dir
                    myWebClient.DownloadFile(remoteFileUrl, destinationPath);

                    Byte[] buffer = myWebClient.DownloadData(destinationPath);

                    return File(buffer, "application/pdf");
                }
                else
                {
                    TempData["errMsg"] = "Couldn't Download/Open Files!";
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "Meeting");
                }
          
            return File(destinationPath, "application/pdf");//To open file

        }
        //public ActionResult Download(String MeetingType, int MeetingNo, String fileName)
        //    {
        //        var FileVirtualPath = FileFolder + MeetingType + "/" + MeetingNo + "/" + fileName;

        //        string path = Server.MapPath(FileVirtualPath);

        //        WebClient client = new WebClient();
        //        Byte[] buffer = client.DownloadData(path);
        //        if (buffer != null)
        //        {
        //            Response.ContentType = "application/pdf";
        //            // Response.AppendHeader("content-disposition", "inline; filename=filename.pdf");
        //            // Response.AppendHeader("Content-Disposition", "attachment;inline; filename=FileName.pdf");
        //            Response.TransmitFile(Server.MapPath(FileVirtualPath));
        //            Response.AddHeader("content-length", buffer.Length.ToString());
        //            Response.BinaryWrite(buffer);
        //            //Response.Flush();
        //            //Response.Close(); 
        //            Response.AppendHeader("Content-Disposition", "inline; filename=" + FileVirtualPath);
        //        }
        //        return File(buffer, "application/pdf");
        //    }
        //public void Download(String MeetingType, int MeetingNo, String fileName)
        //    {
        //        var FileVirtualPath = FileFolder + MeetingType + "/" + MeetingNo + "/" + fileName;
        //        //return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));

        //        string path = Server.MapPath(FileVirtualPath);

        //        WebClient client = new WebClient();
        //        Byte[] buffer = client.DownloadData(path);
        //        if (buffer != null)
        //        {
        //            // Response.ContentType = "application/pdf";
        //            //// Response.AppendHeader("content-disposition", "inline; filename=filename.pdf");
        //            //// Response.AppendHeader("Content-Disposition", "attachment;inline; filename=FileName.pdf");
        //            // Response.TransmitFile(Server.MapPath(FileVirtualPath));
        //            // Response.AddHeader("content-length", buffer.Length.ToString());
        //            // Response.BinaryWrite(buffer);
        //            // ////Response.Flush();
        //            // //Response.Close();

        //            this.Response.Clear();
        //            this.Response.Buffer = true;
        //            this.Response.ContentType = "application/pdf";
        //            Response.AppendHeader("content-disposition", "inline;");
        //            this.Response.AddHeader("content-length", buffer.Length.ToString());
        //            this.Response.BinaryWrite(buffer);

        //            this.Response.End();
        //        }

        //    }
        public JsonResult getType()
        {
            var ddlType = db.meetingTypes.ToList();
            List<SelectListItem> liType = new List<SelectListItem>();

            if (ddlType != null)
            {
                foreach (var x in ddlType)
                {
                    liType.Add(new SelectListItem { Text = x.type, Value = x.id.ToString() });
                }
            }
            return Json(liType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDepartment()
        {
            var ddlDept = db.departments.ToList();
            List<SelectListItem> deptList = new List<SelectListItem>();

            if (ddlDept != null)
            {
                foreach (var x in ddlDept)
                {
                    deptList.Add(new SelectListItem { Text = x.deptName, Value = x.id.ToString() });
                }
            }
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getNoAcdType(String type)
        {
            var query = db.meetingTypes.Where(o=>o.type==type).FirstOrDefault();
            var t = query.id; 
            var ddlNo = db.MeetingMasters.Where(o=>o.MeetingType==t).ToList();
            List<SelectListItem> numList = new List<SelectListItem>();

            if (ddlNo != null)
            {
                foreach (var x in ddlNo)
                {
                    numList.Add(new SelectListItem { Text = x.MeetingNo.ToString(), Value = x.MeetingNo.ToString() });
                }
            }
            return Json(numList, JsonRequestBehavior.AllowGet);
        }
       

    }
}