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
public class PlatformRepository : IPlatformRepository
{
    private readonly ApplicationDbContext _context;
    public PlatformRepository(ApplicationDbContext context)
    {
       _context = context; 
    }

    public async Task<List<Platform>> GetAllAsync()
    {
        return await _context.Platforms.ToListAsync();
    }

    public async Task<Platform?> GetByNameAsync(string name)
    {
        return await _context.Platforms
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }
}
