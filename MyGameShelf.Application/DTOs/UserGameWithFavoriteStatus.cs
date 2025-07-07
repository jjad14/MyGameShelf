using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class UserGameWithFavoriteStatus
{
    public UserGame UserGame { get; set; }
    public bool IsFavorited { get; set; }
}

