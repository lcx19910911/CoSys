using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core.UEditor
{
    public interface IUEditorService
    {
        ConfigDTO Get_Config();

        UploadResponseDTO Upload_Image();
        
        UploadResponseDTO Upload_Scrawl();

        UploadResponseDTO Upload_Video();
        
        UploadResponseDTO Upload_File();
        
        ListResponseDTO List_Image();

        ListResponseDTO List_File();

        CatchImageResponseDTO Catch_Image();
        
    }
}
