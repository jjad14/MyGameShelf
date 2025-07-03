
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
public class DeveloperRepository : IDeveloperRepository
{
    private readonly ApplicationDbContext _context;
    public DeveloperRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Developer>> GetAllAsync()
    {
        return await _context.Developers.ToListAsync();
    }

    public async Task<Developer?> GetByNameAsync(string name)
    {
        return await _context.Developers
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }
}
