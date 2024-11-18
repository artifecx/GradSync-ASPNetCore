using System.IO;

namespace Services.Interfaces
{
    public interface IPdfTextExtractorService
    {
        string ExtractTextFromPdf(Stream pdfStream);
    }
}
