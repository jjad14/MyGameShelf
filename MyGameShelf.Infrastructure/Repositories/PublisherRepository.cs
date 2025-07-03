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
public class PublisherRepository : IPublisherRepository
{
    private readonly ApplicationDbContext _context;
    public PublisherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Publisher>> GetAllAsync()
    {
        return await _context.Publishers.ToListAsync();
    }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _context.Publishers
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }
}
