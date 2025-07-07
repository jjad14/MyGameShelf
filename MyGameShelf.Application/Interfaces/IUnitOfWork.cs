using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IUnitOfWork
{
    IUserGameRepository UserGames { get; }
    IGameRepository Games { get; }
    IReviewRepository Reviews { get; }
    IPlatformRepository Platforms { get; }
    IPublisherRepository Publishers { get; }
    IDeveloperRepository Developers { get; }
    IGenreRepository Genres { get; }
    ITagsRepository Tags { get; }
    IFavoriteGameRepository Favorites { get; }

    Task<bool> SaveChangesAsync();
}
