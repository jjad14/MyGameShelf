using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Configurations;
public class GoogleAuthSettings
{
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}