
namespace pdf_search.Services.Interfaces;

public interface ISubmissionService {
    public Task uploadPdf(byte[] fileBytes, string pdfName, string group);
    public bool checkZipExtension(IFormFile file);
}