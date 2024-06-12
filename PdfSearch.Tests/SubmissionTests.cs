using SubmissionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace pdfSearch.Tests;

public class FileUploadTests {
    [Fact]
    public async Task NormalSingleDoc()
    {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/PdfSearch");
        var pdfHandler = new PdfHandler();
         
        using (var testStream = new MemoryStream()) {
            var testFile = File.Open("./examples/testDoc.pdf", FileMode.Open, FileAccess.ReadWrite);
            testFile.CopyTo(testStream);
            await pdfHandler.uploadPdf(testStream.ToArray(), "testFile1.pdf", "testGroup1");
        }
        Assert.True(Directory.Exists("./examples/txt_files/testGroup1"));
        Assert.True(File.Exists("./examples/txt_files/testGroup1/testFile1.txt"));
    }

    [Fact]
    public async Task BlankDoc()
    {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/PdfSearch");
        var pdfHandler = new PdfHandler();
         
        using (var testStream = new MemoryStream()) {
            var testFile = File.Open("./examples/blank.pdf", FileMode.Open, FileAccess.ReadWrite);
            testFile.CopyTo(testStream);
            await pdfHandler.uploadPdf(testStream.ToArray(), "blankDoc.pdf", "testGroup1");
        }
        Assert.True(Directory.Exists("./examples/txt_files/testGroup1"));
        Assert.True(File.Exists("./examples/txt_files/testGroup1/blankDoc.txt"));
    }

    [Fact]
    public void InvalidFileName() {
        Assert.True(PdfHandler.checkNameInvalid("thisfile/new"));
        Assert.False(PdfHandler.checkNameInvalid("validName"));
    }

    [Fact]
    public void checkZipExtensionSingleValid() {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/PdfSearch");
        var baseFile = File.Open("./examples/testDoc.pdf.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "testDoc.pdf.zip")   
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.True(PdfHandler.checkZipExtension(testFile));
    }

    [Fact]
    public void checkZipExtensionMultipleValid() {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/PdfSearch");
        var baseFile = File.Open("./examples/testMultiple.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "testMultiple.zip")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.True(PdfHandler.checkZipExtension(testFile));
    }

    [Fact]
    public void checkZipExtensionInvalid() {
        Directory.SetCurrentDirectory("/");
        Directory.SetCurrentDirectory("/Users/thomas/Documents/SummerProject/PdfSearch");
        var baseFile = File.Open("./examples/zipInvalidName.zip", FileMode.Open, FileAccess.ReadWrite);
        FormFile testFile = new FormFile(baseFile, 0, baseFile.Length, "_charset_", "zipInvalid.zip")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
        Assert.False(PdfHandler.checkZipExtension(testFile));
    }



}

    