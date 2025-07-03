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

    public async Task<Review?> GetReviewAsync(string userId, int gameId)
    {
        var result = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

        return result;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public Task UpdateReview(string userId, int gameId, string content, bool isRecommended)
    {
        throw new NotImplementedException();
    }
}
