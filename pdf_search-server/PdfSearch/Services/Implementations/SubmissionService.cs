using System.Text.Json;
using System.IO.Compression;
using NLog;
using pdf_search.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using pdf_search.Services.Interfaces;
namespace pdf_search.Services.Implmentations;

public class SubmissionService : ISubmissionService {
    // URL for ocr api
    static string apiURL = "https://api.ocr.space";
    //key for ocr api 
    static string apikey = "K84689311588957";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ApplicationDbContext _dbContext;
    public SubmissionService(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task uploadPdf(byte[] fileBytes, string pdfName, string group) {

        string file = await parsePdf(fileBytes, pdfName);


/*         // checks if the directory for the given group is null and initalizes one if it is not 
        initGroupDirectory(group); */

        Submissions submission = new Submissions() {
            batch = group,
            name = pdfName,
        };
        int previous = 0;
        for (int i = 1; i <= 10; i++) {
            List<int> indices = parseQuestion(file, i, previous);
            if (indices.Count == 2) {
                submission.setQuestion(i, file.Substring(indices[0], indices[1] - indices[0]));
            } else if (indices.Count == 1) {
                submission.setQuestion(i, file.Substring(indices[0]));
                break;
            } else {
                break;
            }
        }
        submission.wholeDoc = file;
        submission.date = DateOnly.FromDateTime(DateTime.Now);
        await AddOrUpdate(submission);



    }


    async Task AddOrUpdate(Submissions submission) {
        var existing = await _dbContext.Submissions.FindAsync([submission.batch, submission.name]);
        if (existing  != null) {
            _dbContext.Entry(existing).CurrentValues.SetValues(submission);
        } else {
            _dbContext.Add(submission);
        }

        await _dbContext.SaveChangesAsync();
    }
    //calls the API to parse the pdf and returns it as a string 
    static async Task<string> parsePdf(byte[] fileBytes, string pdfName) {
        //converts pdf to base64
        var content = Convert.ToBase64String(fileBytes);
        //initializes httpClient
        HttpClient httpClient = new HttpClient();
        httpClient.Timeout = new TimeSpan(1, 1, 1);

        //initializes form for API with pdf data
        MultipartFormDataContent form = new MultipartFormDataContent();
        form.Add(new StringContent(apikey), "apikey");
        form.Add(new StringContent("2"), "ocrengine"); 
        form.Add(new StringContent("true"), "scale");
        form.Add(new StringContent("false"), "istable");
        form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "PDF", pdfName);

        //posts to ocr api and parses response
        HttpResponseMessage response = await httpClient.PostAsync(apiURL + "/Parse/Image", form);
        string strResponse = await response.Content.ReadAsStringAsync() ?? throw new FileNotFoundException();
        
        Rootobject ocrResult = JsonSerializer.Deserialize<Rootobject>(strResponse) ?? throw new FileNotFoundException();

        //check if the body of the pdf is null
        if (ocrResult.ParsedResults is null) {
            throw new FileNotFoundException();
        }

        string docString = "";
        
        foreach (Parsedresult parsedResult in ocrResult.ParsedResults) {
            if (parsedResult.ParsedText != null) {
                docString += parsedResult.ParsedText;
            }
        }

        return docString;

    }

          



    // checks if zip file only cotntaines pdfs
    public bool checkZipExtension(IFormFile file) {
        if (file.ContentType != "application/zip") {
            throw new ArgumentException("file must be a zip");
        }

        using (var zip = new ZipArchive(file.OpenReadStream())) {
            foreach (ZipArchiveEntry entry in zip.Entries) {
                if (!entry.FullName.EndsWith("pdf")) {
                    return false;
                }
            }
        }
        return true;            
    }

    public async Task<List<string>> getGroups() {
        return await _dbContext.Submissions
            .Select(s => s.batch)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<string>> getNames(string group) {
    return await _dbContext.Submissions
        .Where(s => s.batch == group)
        .Select(s => s.name)
        .Distinct()
        .ToListAsync();
    }

    public async Task<string> getSubmission(string group, string name) {
        var submission = await _dbContext.Submissions
            .Where(s => (s.batch == group && s.name == name))
            .Select(s => s.wholeDoc)
            .FirstOrDefaultAsync();
        submission = submission ?? "";
        return submission;
    }

    public async Task<DateOnly?> getDate(string group, string name) {
        var submission = await _dbContext.Submissions
            .Where(s => (s.batch == group && s.name == name))
            .Select(s => s.date)
            .FirstOrDefaultAsync();
        submission = submission ?? throw new ArgumentException("invalid date selected");
        return submission;
    }

    public async Task<string> getQuestion(string group, string name, int number) {
        var submission = "";
        switch(number)
        {
            case 1:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q1)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 2:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q2)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 3:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q3)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;
            
            case 4:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q4)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 5:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q5)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 6:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q6)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 7:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q7)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

            case 8:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q8)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;
            
            case 9:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q9)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;

                        
            case 10:
                submission = await _dbContext.Submissions
                    .Where(s => (s.batch == group && s.name == name))
                    .Select(s => s.q10)
                    .FirstOrDefaultAsync();
                submission = submission ?? "";
                break;
            
            default:
                break;
        }

        return submission;
    }

    public async Task removeSubmission(string group, string name) {
        var existing = await _dbContext.Submissions.FindAsync([group, name]);
        if (existing != null) {
            _dbContext.Remove(existing);
        }
        await _dbContext.SaveChangesAsync();
    }
    public async Task removeGroup(string group) {
        var existing = _dbContext.Submissions.Where(s => s.batch == group);
        if (existing != null) {
            _dbContext.RemoveRange(existing);
        }

        await _dbContext.SaveChangesAsync();
    }

    /** takes in a text and a pattern and returns a list of integers that are the indices in the text where the edit distance
    of the string of the same length as the pattern starting at that distane is at most the limit, if start and end are specified
    only the part of the text within the indices start and end are checked **/
    public List<int> findMatches(string pattern, string text, int limit, int start = 0, int end = -1) {

        List<int> indices = new List<int>();

        if (pattern.Length == 0 || text.Length == 0) {
            return indices;
        }

        if (pattern.Length > 32) {
            throw new ArgumentException("Pattern is too long");
        }

        if (limit >= pattern.Length || limit < 0) {
            throw new ArgumentException("Limit is out of range");
        }
        //initalizes charmap that has a bitmask for each character in the pattern
        Dictionary<int, int> charMap = new Dictionary<int, int>();

        char[] p = pattern.ToCharArray();
        char[] t = text.ToCharArray();


        //makes bitmask all 1s if char is not there an a 0 at the position the char appears
        foreach (char c in p) {
            if (!charMap.ContainsKey(c)) {
                charMap.Add(c, ~0);
            }
        }
        for (int i = 0; i < p.Length; i++) {
            charMap[p[i]] &= ~(1 << i);
        }

        if (start > text.Length || end > text.Length || start < -1 || end < -1) {
            throw new ArgumentException("Bounds outside of file Length");
        }

        long stop = end != -1 ? end : text.Length;

        
        int[] R = new int[limit + 1];
        for (int i = 0; i < R.Length; i ++) {
            R[i] = ~1;
        }
        // iterates through the text keeping limit + 1 bit maps for each edit distance away if the last one has a 0 then there is a fuzzy match 
        for (int i = start; i < stop; i++) {
            int old_Rd1 = R[0];

            int textByte = t[i];
            if (!charMap.ContainsKey(textByte)) {
                charMap.Add(textByte, ~0);
            }

            R[0] |= charMap[textByte];
            R[0] <<= 1;

            for (int d = 1; d <= limit; d++) {
                int tmp = R[d];

                R[d] = (old_Rd1 & (R[d] | charMap[textByte])) << 1;
                old_Rd1 = tmp;
            }

            if (0 == (R[limit] & (1 << pattern.Length))) {
                indices.Add(i - p.Length + 1);
            }
        }

        return indices;
    }

    // same as function above but only looks for the first exact match of the patern in the text
    private static int firstMatch(string pattern, string text, int start = 0) {

        List<int> indices = new List<int>();

        Dictionary<int, int> charMap = new Dictionary<int, int>();

        char[] p = pattern.ToCharArray();
        char[] txt = text.ToCharArray();
        long t = text.Length;

        foreach (char c in p) {
            if (!charMap.ContainsKey(c)) {
                charMap.Add(c, ~0);
            }
        }
        for (int i = 0; i < p.Length; i++) {
            charMap[p[i]] &= ~(1 << i);
        }

        int R = ~1;

        for (int i = start; i < t; i++) {
            int old_Rd1 = R;

            int textByte = text[i];
            if (!charMap.ContainsKey(textByte)) {
                charMap.Add(textByte, ~0);
            }

            R |= charMap[textByte];
            R <<= 1;


            if (0 == (R & (1 << pattern.Length))) {
                return i - p.Length + 1 + start;
            }
        }

        return -1;
    }

    
    // find the start and end of a given question number in the file 
    public List<int> parseQuestion(string text, int number, int start = 0) {
        List<int> result = new List<int>();
        if (number < 0) {
            return result;
        }

        string firstQuestion = "Q" + number.ToString() + ".";
        string nextQuestion = "Q" + (number + 1).ToString() + ".";

        int firstIndex = firstMatch(firstQuestion, text); 

        if (firstIndex == -1) {
            return result;
        }

        result.Add(firstIndex);

        int secondIndex = firstMatch(nextQuestion, text);

        if (secondIndex == -1) {
            return result;
        }

        result.Add(secondIndex);
        return result;
    }

    public async Task<List<int>> queryMatches(string group, string name, string pattern, int limit, int question = 0) {
        string text;
        if (question == 0) {
            text = await getSubmission(group, name);
        } else {
            text = await getQuestion(group, name, question);
        }

        if (text == null || text.Length == 0) {
            throw new ArgumentException("Question " + question + " in group " + group + " and submission " + 
            name + " does not exist.");
        }
        var indices = await  Task.Run(() => findMatches(pattern, text, limit));
        return indices;
    }

    public async Task<List<int>> getFilledQuestions(string group, string name) {
        List<int> questions = new List<int>();
        var submission = await _dbContext.Submissions
            .Where(s => (s.batch == group && s.name == name))
            .FirstOrDefaultAsync();
        if (submission == null) {
            return questions;
        }
        for (int i = 1; i < 11; ++i) {
            string propertyName = "q" + i;
            PropertyInfo? propertyInfo = typeof(Submissions).GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.GetValue(submission) != null) {
                questions.Add(i);
            }
        }
        return questions;
    }
}


//classes for ocr result
class Rootobject {
    public Parsedresult[]? ParsedResults { get; set; }
    public int? OCRExitCode { get; set; }
    public bool? IsErroredOnProcessing { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }
}

class Parsedresult {
    public object? FileParseExitCode { get; set; }
    public string? ParsedText { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }
}