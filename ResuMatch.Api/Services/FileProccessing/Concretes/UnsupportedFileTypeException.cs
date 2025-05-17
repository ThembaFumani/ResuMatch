using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.FileProccessing.Concretes
{
    public class UnsupportedFileTypeException : Exception
    {
        public UnsupportedFileTypeException(string message) : base(message)
        {
        }
    }
}