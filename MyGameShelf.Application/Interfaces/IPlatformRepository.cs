using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IPlatformRepository
{
    Task<Platform?> GetByNameAsync(string name);
    Task<List<Platform>> GetAllAsync();
}
