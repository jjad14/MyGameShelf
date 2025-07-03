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
    Task AddReview(Review review);
    Task UpdateReview(string userId, int gameId, string content, bool isRecommended);
    Task DeleteReview(Review review);
    Task<bool> SaveChangesAsync();

}
