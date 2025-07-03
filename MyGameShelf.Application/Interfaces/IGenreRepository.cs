using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IGenreRepository
{
    Task<Genre?> GetByNameAsync(string name);
    Task<List<Genre>> GetAllAsync();
}
