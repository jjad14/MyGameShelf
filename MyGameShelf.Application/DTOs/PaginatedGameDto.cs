using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class PaginatedGameDto
{
    public IEnumerable<GameDto> Games { get; set; }
    public int TotalCount { get; set; }
}
