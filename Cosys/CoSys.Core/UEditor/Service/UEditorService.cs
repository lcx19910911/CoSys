using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using CoSys.Model;

namespace CoSys.Core.UEditor
{
    public class UEditorService :IUEditorService
    {      /// <summary>
           /// 请求context
           /// </summary>
        public HttpContext ContextCurrent { get; set; }

        public UEditorService()
        {
            this.ContextCurrent = HttpContext.Current;
        }

        public ConfigDTO Get_Config()
        {
            try
            {
                return EditorHelper.GetConfig<ConfigDTO>();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
            }
            return null;
        }

        private UploadResponseDTO Upload_File(FileCode code)
        {
            UploadResponseDTO dto = null;
            if (ContextCurrent.Request.Files.Count == 0)
            {
                dto = new UploadResponseDTO();
                return dto;
            }
            var key = Guid.NewGuid().ToString("N");
            var file = ContextCurrent.Request.Files[0];
            var fileName = file.FileName;
            string imgUrl = UploadHelper.Save(file, code.ToString());
            if (imgUrl == null)
            {
                dto = new UploadResponseDTO();
                return dto;
            }
            dto = new UploadResponseDTO();
            dto.url = imgUrl;
            dto.original = fileName;
            dto.title = fileName;
            dto.state = UploadStateCode.Success;
            return dto;
        }


        public UploadResponseDTO Upload_Image()
        {
            UploadResponseDTO dto = null;
            try
            {
                dto = Upload_File(FileCode.Image);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new UploadResponseDTO();
            }
            return dto;
        }

        public UploadResponseDTO Upload_Scrawl()
        {
            var dto = new UploadResponseDTO();

            try
            {
                var config = EditorHelper.GetConfig<ConfigDTO>();
                var base64 = ContextCurrent.Request[config.scrawlFieldName];

                var key = Guid.NewGuid().ToString("N");
                var suffix = ".png";
                var fileName = key + suffix;
                var code = FileCode.Scrawl;
                string imgUrl = string.Empty;
                using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    imgUrl = UploadHelper.SaveImageStream(stream, suffix);
                    if (imgUrl == null)
                    {
                        return dto;
                    }
                }

                dto.url = imgUrl;
                dto.original = fileName;
                dto.title = fileName;
                dto.state = UploadStateCode.Success;
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new UploadResponseDTO();
            }
            return dto;
        }

        public UploadResponseDTO Upload_Video()
        {
            UploadResponseDTO dto = null;
            try
            {
                dto = Upload_File(FileCode.Video);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new UploadResponseDTO();
            }
            return dto;
        }

        public UploadResponseDTO Upload_File()
        {
            UploadResponseDTO dto = null;
            try
            {
                dto = Upload_File(FileCode.File);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new UploadResponseDTO();
            }
            return dto;
        }

        private ListResponseDTO List_File(FileCode code)
        {
            //ListResponseDTO dto = new ListResponseDTO();
            //var list = OSSHelper.List(code);
            //return new ListResponseDTO
            //{
            //    list = list.Select(x => new ImageUrlDTO
            //    {
            //        url = x
            //    }).ToList(),
            //    start = 0,
            //    total = list.Count,
            //    state = UploadStateCode.Success,
            //};
            return null;
        }

        public ListResponseDTO List_Image()
        {
            ListResponseDTO dto = null;
            try
            {
                dto = List_File(FileCode.Image);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new ListResponseDTO();
            }
            return dto;
        }

        public ListResponseDTO List_File()
        {
            ListResponseDTO dto = null;
            try
            {
                dto = List_File(FileCode.File);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                dto = new ListResponseDTO();
            }
            return dto;
        }

        public CatchImageResponseDTO Catch_Image()
        {
            return new CatchImageResponseDTO();
        }
    }
}
