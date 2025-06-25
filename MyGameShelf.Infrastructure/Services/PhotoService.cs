using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Services;
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> options)
    {
        // Create a new Cloudinary account using the settings provided in the configuration
        var account = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );

        // Set the Cloudinary instance with the account details
        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) 
        { 
            return null;
        }

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Height(500).Width(500).Crop("limit").Width(500).Height(500)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            // Optionally handle or log errors here
            return null;
        }

        return new PhotoUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    public async Task<bool> DeletePhotoAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);

        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        return deletionResult.Result == "ok";
    }
}
