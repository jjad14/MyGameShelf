using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using MyGameShelf.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Services;
public class PhotoServiceTests
{
    private readonly Mock<Cloudinary> _mockCloudinary;
    private readonly PhotoService _photoService;

    public PhotoServiceTests()
    {
        _mockCloudinary = new Mock<Cloudinary>(new Account("test", "test", "test"));
        _photoService = new PhotoService(_mockCloudinary.Object);
    }

    [Fact]
    public async Task AddPhotoAsync_ReturnsNull_WhenFileIsNull()
    {
        // Act
        var result = await _photoService.AddPhotoAsync(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddPhotoAsync_ReturnsNull_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        // Act
        var result = await _photoService.AddPhotoAsync(fileMock.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddPhotoAsync_ReturnsPhotoUploadResult_WhenSuccessful()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var fileName = "test.jpg";
        var content = "fake image data";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var uploadResult = new ImageUploadResult
        {
            SecureUrl = new Uri("https://res.cloudinary.com/demo/image/upload/sample.jpg"),
            PublicId = "sample"
        };

        _mockCloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), default))
            .ReturnsAsync(uploadResult);

        // Act
        var result = await _photoService.AddPhotoAsync(fileMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://res.cloudinary.com/demo/image/upload/sample.jpg", result.Url);
        Assert.Equal("sample", result.PublicId);
    }

    [Fact]
    public async Task DeletePhotoAsync_ReturnsTrue_WhenDeletionIsOk()
    {
        // Arrange
        var deletionResult = new DeletionResult { Result = "ok" };

        _mockCloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(deletionResult);

        // Act
        var result = await _photoService.DeletePhotoAsync("sample");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeletePhotoAsync_ReturnsFalse_WhenDeletionFails()
    {
        // Arrange
        var deletionResult = new DeletionResult { Result = "not found" };

        _mockCloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(deletionResult);

        // Act
        var result = await _photoService.DeletePhotoAsync("invalid");

        // Assert
        Assert.False(result);
    }
}