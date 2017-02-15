using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoSys.Core;
using CoSys.Core.UEditor;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class UEditorController : BaseController
    {
        private IUEditorService _UEditorService = new UEditorService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Config()
        {
            var r = _UEditorService.Get_Config();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadImage()
        {
            var r = _UEditorService.Upload_Image();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.original,
                r.title,
                r.url
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传涂鸦
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadScrawl()
        {
            var r = _UEditorService.Upload_Scrawl();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.original,
                r.title,
                r.url
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传视频
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadVideo()
        {
            var r = _UEditorService.Upload_Video();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.original,
                r.title,
                r.url
            }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            var r = _UEditorService.Upload_File();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.original,
                r.title,
                r.url
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListImage()
        {
            var r = _UEditorService.List_Image();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.list,
                r.start,
                r.total
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListFile()
        {
            var r = _UEditorService.List_File();
            return Json(new
            {
                state = r.state.GetDescription(),
                r.list,
                r.start,
                r.total
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <returns></returns>
        public ActionResult CatchImage()
        {
            return Json(new
            {
                state = "SUCCESS",
                url = "upload/demo.jpg",
                title = "demo.jpg",
                original = "demo.jpg"
            });

        }
    }
}