using Microsoft.AspNetCore.Http;
using MyGameShelf.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IPhotoService
{
    Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
    Task<bool> DeletePhotoAsync(string publicId);
}
