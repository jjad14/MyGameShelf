using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Exceptions;

public class PhotoServiceException : Exception
{
    public PhotoServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}