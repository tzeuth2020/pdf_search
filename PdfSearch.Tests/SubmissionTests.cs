using SubmissionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace pdf_search.Tests;

public class FileUploadTests {
    [Fact]
    public async Task NormalSingleDoc()
    {
        Directory.SetCurrentDirectory("../../../../PdfSearch");
        Console.WriteLine("testing");
        var pdfHandler = new PdfHandler();
         
        using (var testStream = new MemoryStream()) {
            var testFile = File.Open("./examples/testDoc.pdf", FileMode.Open, FileAccess.ReadWrite);
            testFile.CopyTo(testStream);
            await pdfHandler.uploadPdf(testStream.ToArray(), "testFile1.pdf", "testGroup1");
        }
        Assert.True(Directory.Exists("./examples/txt_files/testGroup1"));
        Assert.True(File.Exists("./examples/txt_files/testGroup1/testFile1.txt"));
    }
}

    