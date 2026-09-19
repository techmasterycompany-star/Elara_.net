using Elara.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Interfaces.Service
{
    public interface IStorageService
    {
        Task<CloudinaryUploadResult> UploadAsync(Stream stream, string fileName);

        Task DeleteAsync(string publicId);
    }
}
