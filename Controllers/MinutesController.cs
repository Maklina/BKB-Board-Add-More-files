﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBBoard.Models.AppManager;
using BKBBoard.Models;
using System.IO;
using System.Data.Entity.Validation;
using BKBBoard.DataModel;
using System.Configuration;
using System.Data.Entity;
using System.Net;

namespace BKBBoard.Controllers
{
    public class MinutesController : Controller
    {
        BKBBoardEntities db = new BKBBoardEntities();
        string FileFolder = ConfigurationManager.AppSettings["filepath"];
        LookupValue lv = new LookupValue();
        // GET: Minutes
        public ActionResult Index()
        {
            return View();
        }

         public ActionResult UploadMinutes()
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

        public ActionResult UploadMinutes(MeetingViewModel m, string meetingNo)
        {
            //return Json(m, JsonRequestBehavior.AllowGet);
            try
            {
                if (m.Minutesfile != null)
                {
                    m.mMaster.MeetingNo = Convert.ToInt32(meetingNo);
                    var root = db.meetingTypes.Where(o => o.id.ToString().Equals(m.mMaster.MeetingType)).SingleOrDefault();
                    string path = FileFolder + root.type;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }
                    path = path + "/" + meetingNo;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }
                   
                    var minutesExtension = Path.GetExtension(m.Minutesfile.FileName);
                    var minutesFileName = "Minutes-file-" + meetingNo + minutesExtension;
                    var MinutesPath = Path.Combine(Server.MapPath(path), minutesFileName);
                    

                    m.mMaster.MinutesURL = minutesFileName;
                   
                    m.Minutesfile.SaveAs(MinutesPath);


                    var ECminutesExtension = Path.GetExtension(m.ECMinutesfile.FileName);
                    var ECminutesFileName = "ECMinutes-file-" + meetingNo + ECminutesExtension;
                    var ECMinutesPath = Path.Combine(Server.MapPath(path), ECminutesFileName);
                    m.mMaster.ECMinutesURL = ECminutesFileName;
                    m.ECMinutesfile.SaveAs(ECMinutesPath);

                    var AuditminutesExtension = Path.GetExtension(m.AuditMinutesfile.FileName);
                    var AuditminutesFileName = "AuditMinutes-file-" + meetingNo + AuditminutesExtension;
                    var AuditMinutesPath = Path.Combine(Server.MapPath(path), AuditminutesFileName);
                    m.mMaster.AuditMinutesURL = AuditminutesFileName;
                    m.AuditMinutesfile.SaveAs(AuditMinutesPath);
                    if (SaveFile(m.mMaster))
                    {

                        return RedirectToAction("Index", "Meeting", new { meetingType = root.type });
                    }
                    else
                    {
                        TempData["retMsg"] = TempData["AlertMessage"];
                        return Json(TempData, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("UploadMinutes", "Minutes");
                    }
                }
                else
                {
                    TempData["retMsg"] = "File not found!";
                    return Json(TempData, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("UploadMinutes", "Minutes");
                }
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Error:" + ex.Message;
                return Json(TempData, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("UploadMinutes", "Minutes");
            }
        }
        private bool SaveFile(MeetingMasterViewModel m)
        {
            var login = (BKBBoard.Models.LoginModels)Session["LoginCredentials"];
            try
            {
                int meetingType = Convert.ToInt32(m.MeetingType);
                var getMeetingById = db.MeetingMasters.Where(o =>o.MeetingType == meetingType && o.MeetingNo == m.MeetingNo).FirstOrDefault();
                
                getMeetingById.MinutesURL = m.MinutesURL;
                getMeetingById.ECMinutesURL = m.ECMinutesURL;
                db.Entry(getMeetingById).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        TempData["AlertMessage"] = TempData["AlertMessage"] + validationError.PropertyName + "--" + validationError.ErrorMessage;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = ex.Message;
                return false;
            }
        }
        public JsonResult getMeetingNo(string typeId)
        {
            var ddlmtNo = db.MeetingMasters.OrderByDescending(o=>o.MeetingNo).ToList(); 
            List<SelectListItem> liType = new List<SelectListItem>();

            if (ddlmtNo != null)
            {
                foreach (var x in ddlmtNo)
                {
                    if(x.MeetingType.ToString() == typeId && x.MinutesURL == " " )
                    {
                        liType.Add(new SelectListItem { Text = x.MeetingNo.ToString(), Value = x.MeetingNo.ToString() });

                    }
                }
            }
            return Json(liType, JsonRequestBehavior.AllowGet);
        }
        public FileResult Download(String MeetingType, int MeetingNo, String fileName)
        {
            var FileVirtualPath = FileFolder + MeetingType + "/" + MeetingNo + "/" + fileName;

            string path = Server.MapPath(FileVirtualPath);

            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(path);
            if (buffer != null)
            {
                Response.ContentType = "application/pdf";
                // Response.AppendHeader("content-disposition", "inline; filename=filename.pdf");
                // Response.AppendHeader("Content-Disposition", "attachment;inline; filename=FileName.pdf");
                Response.TransmitFile(Server.MapPath(FileVirtualPath));
                Response.AddHeader("content-length", buffer.Length.ToString());
                Response.BinaryWrite(buffer);
                //Response.Flush();
                //Response.Close(); 
                Response.AppendHeader("Content-Disposition", "inline; filename=" + FileVirtualPath);
            }
            return File(buffer, "application/pdf");
        }
    }
}