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
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            try
            {
                StringBuilder extractedText = new StringBuilder();

                // Open the PDF file using a using statement for automatic resource disposal
                using (PdfReader pdfReader = new PdfReader(filePath))
                {
                    using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                    {
                        int pageCount = pdfDocument.GetNumberOfPages();
                        Console.WriteLine($"PDF contains {pageCount} pages.");

                        // Loop through pages and extract text
                        for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                        {
                            var page = pdfDocument.GetPage(pageIndex);
                            Console.WriteLine($"--- Page {pageIndex} ---");

                            // Extract text from the page and append it to the StringBuilder
                            extractedText.Append(ExtractTextFromPage(page));
                        }
                        // Return the extracted text
                        return Task.FromResult(extractedText.ToString());
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                // Handle the specific exception for file not found
                Console.WriteLine($"Error: File not found at path: {filePath}.  Error Details: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
            }
            catch (IOException ex)
            {
                // Handle general IO exceptions (e.g., file access issues)
                Console.WriteLine($"Error reading file: {filePath}. Error Details: {ex.Message}");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred while processing the PDF: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        private static string ExtractTextFromPage(iText.Kernel.Pdf.PdfPage page)
        {
            var strategy = new SimpleTextExtractionStrategy();
            string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
            return pageText;
        }
    }
}