using CloudinaryDotNet;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Elara.Application.DTOs.Product;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Service;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Services
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
    }
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IOptions<CloudinarySettings> settings)
        {
            var account = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }
        public async Task<CloudinaryUploadResult> UploadAsync(Stream file, string fileName)
        {

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, file),

                Folder = "elara/products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new BadRequestException(
                    $"Image upload failed: {result.Error.Message}");
            }

            return new CloudinaryUploadResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException($"Failed to delete image from Cloudinary.");
            }
        }
    }
    
}
