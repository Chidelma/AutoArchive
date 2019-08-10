using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUTO_ARCHIVE.Services
{
    public interface IBlobManager
    {
        Task UploadFile(IFormFile filePath, string bucketName);
    }
}
