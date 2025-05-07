using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;

namespace ResuMatch.Api.Utils
{
    public static class FileDataExtractor
    {
        public static string ExtractText(string path)
        {
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(path);
            using var page = engine.Process(img);
            return page.GetText();
        }
    }
}