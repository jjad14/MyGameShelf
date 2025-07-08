using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Repositories;
public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckUserReviewExists(string userId, int gameId)
    {
        return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.GameId == gameId);
    }

    public async Task<int> CountUserReviewsAsync(string userId)
    {
        return await _context.Reviews
            .Where(r => r.UserId == userId)
            .CountAsync();
    }

    public async Task<Review?> GetReviewAsync(string userId, int gameId)
    {
        var result = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

        return result;
    }

    public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, string? sort, int page = 1, int pageSize = 10)
    {
        IQueryable<Review> query = _context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Game);

        query = sort switch
        {
            "name" => query.OrderBy(r => r.Game.Name),
            "rating" => query.OrderByDescending(r => r.IsRecommended),
            _ => query.OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task AddReview(Review review)
    {
        _context.Reviews.Add(review);

        return Task.CompletedTask;
    }

    public Task DeleteReview(Review review)
    {
        _context.Reviews.Remove(review);

        return Task.CompletedTask;
    }

    public async Task DeleteReviewAsync(string userId, int reviewId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
        if (review == null) return; // or throw if you prefer

        _context.Reviews.Remove(review);
    }

    public Task UpdateReview(string userId, int gameId, string content, bool isRecommended)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }


}
