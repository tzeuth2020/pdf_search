using System.Text.Json;
using System.IO.Compression;
namespace SubmissionServices;

public class PdfHandler() {
    // URL for ocr api
    static string apiURL = "https://api.ocr.space";
    //key for ocr api 
    static string apikey = "K84689311588957";

    public async Task uploadPdf(byte[] fileBytes, string pdfName, string group) {

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
        Console.Write(strResponse);
        
        Rootobject ocrResult = JsonSerializer.Deserialize<Rootobject>(strResponse) ?? throw new FileNotFoundException();

        //check if the body of the pdf is null
        if (ocrResult.ParsedResults is null) {
            throw new FileNotFoundException();
        }

        // checks if the directory for the given group is null and initalizes one if it is not 
        initGroupDirectory(group);

        //makes the path to the output text files 
        string outputName = Path.ChangeExtension(pdfName, ".txt");

        // writes the result of the api call to the text file
        using (StreamWriter outputFile = new StreamWriter("examples/txt_files/" + group + "/" + outputName)) {
            foreach (Parsedresult result in ocrResult.ParsedResults) {
                
                await outputFile.WriteAsync(result.ParsedText + "\n");
            } 
        }
    }


          
    // initalizes the dtirecory for a given group
    private static void initGroupDirectory(string group) {
        
        Console.Write('\n' + "Went in initGroupDirectory group: " + Directory.GetCurrentDirectory() + '\n');
        if (!Directory.Exists(Path.Combine("./examples/txt_files", group))) {
            Console.Write("Went in if statement");
            Directory.CreateDirectory(Path.Combine("./examples/txt_files", group));
        }      
    }

    // checks if a filename contains any illegal characters 
    public static bool checkNameInvalid(string name) {
        char[] invalidNameChars = Path.GetInvalidFileNameChars();
        return name.IndexOfAny(invalidNameChars) >= 0;
    }

    // checks if zip file only cotntaines pdfs
    public static bool checkZipExtension(IFormFile file) {
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