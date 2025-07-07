using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IReviewRepository
{
    Task<bool> CheckUserReviewExists(string userId, int gameId);
    Task<Review?> GetReviewAsync(string userId, int gameId);
    Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, string? status, string? sort, int page = 1, int pageSize = 10);
    Task AddReview(Review review);
    Task DeleteReview(Review review);
    Task UpdateReview(string userId, int gameId, string content, bool isRecommended);
    Task<bool> SaveChangesAsync();

}
