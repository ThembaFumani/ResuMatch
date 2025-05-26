using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IPromptFileService
    {
        Task<PromptFileModel> LoadPromptFileAsync();
    }
}