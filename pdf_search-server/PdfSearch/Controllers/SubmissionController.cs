using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using NLog;
using pdf_search.Data;
using pdf_search.Services.Interfaces;
namespace Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class SubmissionController : Controller {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ISubmissionService _submissionService;

    public SubmissionController(ISubmissionService submissionService) {
        _submissionService  = submissionService;
    }

    //Route for taking pdf path and group number, running through ocr and uploading text file
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile uploadFile, string group) {
        Logger.Info($"/Submission/Uplaod - UploadFile({uploadFile.FileName}), Group({group})");

        try {
            if (uploadFile.ContentType == "application/pdf") {
                string fileName = uploadFile.FileName;
                using (var ms = new MemoryStream()) {
                    uploadFile.CopyTo(ms);
                    
                    await _submissionService.uploadPdf(ms.ToArray(), fileName, group);
                }
                
            } else if (uploadFile.ContentType == "application/zip" &&  _submissionService.checkZipExtension(uploadFile)) {
                using (var zip = new ZipArchive(uploadFile.OpenReadStream())) {
                    foreach (ZipArchiveEntry entry in zip.Entries) {
                        if (!entry.FullName.StartsWith("__MACOSX/")) {
                            using (var ms = new MemoryStream()) {
                                entry.Open().CopyTo(ms);
                                await  _submissionService.uploadPdf(ms.ToArray(), entry.FullName, group);
                            } 
                        }
                    }
                } 
            } else {
                throw new ArgumentException("Invalid file type uploaded");
            }
            return Ok("File uploaded and parsed succesfully");

        } catch(Exception e) {
            Console.Write(e);
            return BadRequest(e.Message);
        } 
    }


    //lists the name of all pdfs successfully uploaded in a given group, and removes the path and file extension
    [HttpGet]
    public async Task<IActionResult> SubmissionNames(string group) {
        Logger.Info($"/Submission/TxtNames - Group({group})");
        List<string> names = await _submissionService.getNames(group);
        return Ok(names);
    } 

    //list the name of all current groups 
    [HttpGet]
    public async Task<IActionResult> GroupNames() {
        Logger.Info($"/Submission/GroupNames");
        List<string> names = await _submissionService.getGroups();
        return Ok(names);       
    } 

    //deletes all files with a group of a given name
    [HttpDelete]
    public async Task<IActionResult> RemoveGroup(string group) {
        Logger.Info($"/Submission/RemoveGroup - Group({group})");
        await _submissionService.removeGroup(group);
        return Ok(group + " deleted.");
    }

    //deletes all files with a group of a given name
    [HttpDelete]
    public async Task<IActionResult> RemoveSubmission(string group, string name) {
        Logger.Info($"/Submission/RemoveSubmission - Group({group}, Name({name}))");
        await _submissionService.removeSubmission(group, name);
        return Ok(name  + " in " + group + " deleted.");
    }

    // returns the whole submission from a given name and group
    [HttpGet]
    public async Task<IActionResult> GetSubmission(string group, string name) {
        Logger.Info($"/Submission/GetSubmission - Group({group}, Name({name}))");

        var submission = await _submissionService.getSubmission(group, name);

        return Ok(submission);

    }

    // returns as question from a text file from a given name and group
    [HttpGet]
    public async Task<IActionResult> GetQuestion(string group, string name, int question) {
        Logger.Info($"/Submission/GetQuestion - Group({group}), Name({name}), Question({question}))");

        var submission = await _submissionService.getQuestion(group, name, question);

        return Ok(submission);

    }

    
    [HttpGet]
    public async Task<IActionResult> GetFilledQuestions(string group, string name) {
        Logger.Info($"/Submission/GetFilledQuestion - Group({group}), Name({name})");

        var numbers = await _submissionService.getFilledQuestions(group, name);

        return Ok(numbers);

    }
    
}    
