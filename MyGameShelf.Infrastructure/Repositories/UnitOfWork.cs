using MyGameShelf.Application.Interfaces;
using MyGameShelf.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        UserGames = new UserGameRepository(_context);
        Games = new GameRepository(_context);
        Reviews = new ReviewRepository(_context);
        Platforms = new PlatformRepository(_context);
        Publishers = new PublisherRepository(_context);
        Developers = new DeveloperRepository(_context);
        Genres = new GenreRepository(_context);
        Tags = new TagRepository(_context);

    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public IUserGameRepository UserGames { get; }
    public IGameRepository Games { get; }
    public IReviewRepository Reviews { get; }
    public IPlatformRepository Platforms { get; }
    public IPublisherRepository Publishers { get; }
    public IDeveloperRepository Developers { get; }
    public IGenreRepository Genres { get; }
    public ITagsRepository Tags { get; }
}
