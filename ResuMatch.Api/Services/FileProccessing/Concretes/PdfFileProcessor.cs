using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using ResuMatch.Api.Services.FileProccessing.Interfaces;

namespace ResuMatch.Api.Services.FileProccessing.Concretes
{

    public class PdfFileProcessor : IFileProcessor
    {
        private readonly ILogger<PdfFileProcessor>? _logger;
        public PdfFileProcessor(ILogger<PdfFileProcessor>? logger)
        {
            _logger = logger;
        }

        public Task<string> ExtractTextAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger?.LogError("File path cannot be null or empty. Parameter: {FilePathParam}", nameof(filePath));
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            try
            {
                StringBuilder extractedText = new StringBuilder();

                using PdfReader pdfReader = new PdfReader(filePath);
                using PdfDocument pdfDocument = new PdfDocument(pdfReader);
                int pageCount = pdfDocument.GetNumberOfPages();
                Console.WriteLine($"PDF contains {pageCount} pages.");

                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    var page = pdfDocument.GetPage(pageIndex);
                    Console.WriteLine($"--- Page {pageIndex} ---");

                    extractedText.Append(ExtractTextFromPage(page));
                }
                
                return Task.FromResult(extractedText.ToString());
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found at path: {filePath}.  Error Details: {ex.Message}");
                throw; 
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {filePath}. Error Details: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while processing the PDF: {ex.Message}");
                throw;
            }
        }

        private static string ExtractTextFromPage(PdfPage page)
        {
            var strategy = new SimpleTextExtractionStrategy();
            string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
            return pageText;
        }
    }
}