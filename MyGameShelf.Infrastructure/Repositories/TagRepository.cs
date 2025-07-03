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
public class TagRepository : ITagsRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }
}
