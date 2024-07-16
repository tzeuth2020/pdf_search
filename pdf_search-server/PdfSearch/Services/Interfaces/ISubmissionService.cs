
namespace pdf_search.Services.Interfaces;

public interface ISubmissionService {
    public Task uploadPdf(byte[] fileBytes, string pdfName, string group);
    public bool checkZipExtension(IFormFile file);

    public Task<List<string>> getGroups();

    public Task<List<string>> getNames(string group);

    public Task<string> getSubmission(string group, string name);

    public Task<string> getQuestion(string group, string name, int number);
    
    public Task removeSubmission(string group, string name);

    public Task removeGroup(string group);

    public List<int> findMatches(string pattern, string text, int limit, int start = 0, int end = -1);

    public List<int> parseQuestion(string text, int number, int start = 0);

    public Task<List<int>> queryMatches(string group, string name, string pattern, int limit, int question = 0);

    public Task<List<int>> getFilledQuestions(string group, string name);

}