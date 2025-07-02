using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class UserGameWithReviewDto
{
    public UserGame UserGame { get; set; }
    public Review? Review { get; set; }
}

