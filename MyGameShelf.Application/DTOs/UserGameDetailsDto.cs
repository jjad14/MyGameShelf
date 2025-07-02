using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class UserGameDetailsDto
{
    public GameStatus Status { get; set; }
    public double? Rating { get; set; }
    public double? Difficulty { get; set; }
    public string? ReviewContent { get; set; }
    public bool? IsRecommended { get; set; }
    public DateTime? ReviewUpdatedAt { get; set; }
}