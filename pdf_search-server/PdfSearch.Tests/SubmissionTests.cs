using pdf_search.Services.Interfaces;
using pdf_search.Services.Implmentations;
using pdf_search.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace pdfSearch.Tests;

public class FileUploadTests {


    private readonly ISubmissionService _submissionService;
    public FileUploadTests(ISubmissionService submissionService) {
        _submissionService = submissionService;
    }

    [Fact]
    public void checkZipExtensionSingleValid() {

        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/pdf_search-server/PdfSearch");
        var baseFile = File.Open("./examples/testDoc.pdf.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "testDoc.pdf.zip")   
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.True(_submissionService.checkZipExtension(testFile));
    }

    [Fact]
    public void checkZipExtensionMultipleValid() {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/pdf_search-server/PdfSearch");
        var baseFile = File.Open("./examples/testMultiple.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "testMultiple.zip")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.True(_submissionService.checkZipExtension(testFile));
    }

    [Fact]
    public void checkZipExtensionInvalid() {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/pdf_search-server/PdfSearch");
        var baseFile = File.Open("./examples/zipInvalidName.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "zipInvalid.zip")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.False(_submissionService.checkZipExtension(testFile));
    }



}
