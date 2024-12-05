using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Services.Interfaces;
using System;
using System.IO;
using System.Text;
using static Services.Exceptions.JobExceptions;

namespace Services.Services
{
    public class PdfTextExtractorService : IPdfTextExtractorService
    {
        public string ExtractTextFromPdf(Stream pdfStream)
        {
            pdfStream.Seek(0, SeekOrigin.Begin);
            using (var pdfReader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                StringBuilder bld = new StringBuilder();
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);
                    bld.Append(PdfTextExtractor.GetTextFromPage(page));
                }
                string extractedText = bld.ToString();

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    throw new JobException("Cannot retrieve any text from the PDF, upload another file.");
                }

                return extractedText;
            }
        }
    }
}