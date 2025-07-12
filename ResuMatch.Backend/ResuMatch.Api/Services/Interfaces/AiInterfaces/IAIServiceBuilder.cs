using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.Interfaces.AiInterfaces
{
    public interface IAIServiceBuilder
    {
        IAIService Build();
    }
}