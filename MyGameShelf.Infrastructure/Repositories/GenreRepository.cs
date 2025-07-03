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
public class GenreRepository : IGenreRepository
{
    private readonly ApplicationDbContext _context;
    public GenreRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Genre>> GetAllAsync()
    {
        return await _context.Genres.ToListAsync();
    }

    public async Task<Genre?> GetByNameAsync(string name)
    {
        return await _context.Genres
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }
}
