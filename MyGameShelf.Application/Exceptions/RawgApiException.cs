using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Exceptions;
public class RawgApiException : Exception
{
    public RawgApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
