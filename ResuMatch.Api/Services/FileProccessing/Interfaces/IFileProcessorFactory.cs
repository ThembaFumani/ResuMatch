using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.FileProccessing.Interfaces
{
    public interface IFileProcessorFactory
    {
        IFileProcessor CreateProcessor(string filePath);
    }
}