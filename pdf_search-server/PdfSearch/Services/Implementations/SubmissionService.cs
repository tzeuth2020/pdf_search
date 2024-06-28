using System.Text.Json;
using System.IO.Compression;
using SearchServices;
using NLog;
using pdf_search.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
            List<int> indices = SearchDoc.getQuestion(file, i, previous);
            /// bug here with inserting and finding questions question 1 should give 2 indices but it only returns one
            Console.Write(i + " " + indices[0] + " " + indices[1] + '\n');
            if (indices.Count == 2) {
                submission.setQuestion(i, file.Substring(indices[0], indices[1] - indices[0]));
            } else if (indices.Count == 1) {
                submission.setQuestion(i, file.Substring(indices[0]));
                break;
            } else {
                break;
            }
        }
        Console.Write(submission.batch);
        await _dbContext.Set<Submissions>().AddAsync(submission);
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

          
/*     // initalizes the dtirecory for a given group
    private static void initGroupDirectory(string group) {
        
        Console.Write('\n' + "Went in initGroupDirectory group: " + Directory.GetCurrentDirectory() + '\n');
        if (!Directory.Exists(Path.Combine("./examples/txt_files", group))) {
            Console.Write("Went in if statement");
            Directory.CreateDirectory(Path.Combine("./examples/txt_files", group));
        }      
    } */

    // checks if a filename contains any illegal characters 
/*     public static bool checkNameInvalid(string name) {
        char[] invalidNameChars = Path.GetInvalidFileNameChars();
        return name.IndexOfAny(invalidNameChars) >= 0;
    }
 */
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