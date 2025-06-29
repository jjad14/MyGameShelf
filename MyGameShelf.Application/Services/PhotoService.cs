using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Services;
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(Cloudinary cloudinary) // 👈 change this
    {
        _cloudinary = cloudinary;
    }

    public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file)
    {
        // Check if file exist
        if (file == null || file.Length == 0) 
        {
            // Return null to indicate no photo was uploaded
            return null;
        }

        // Open a stream to read the file contents
        using var stream = file.OpenReadStream();

        // Prepare parameters for the Cloudinary image upload
        // Apply image transformation:
        // Limit the image size to max 500x500 pixels without cropping
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

        // Return the result containing the secure URL and public ID of the uploaded image
        return new PhotoUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    public async Task<bool> DeletePhotoAsync(string publicId)
    {
        // Delete Cloudinary Photo using public ID - Stored in Users table
        var deletionParams = new DeletionParams(publicId);

        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        return deletionResult.Result == "ok";
    }
}
