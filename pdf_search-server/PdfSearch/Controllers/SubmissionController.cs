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

        } catch(ArgumentException e) {
            return BadRequest(e.Message);
        } catch (FileNotFoundException) {
            return BadRequest("Cannot find file");
        } catch(Exception e) {
            Console.Write(e);
            return BadRequest();
        } 
    }


    //lists the name of all pdfs successfully uploaded in a given group, and removes the path and file extension
    [HttpGet]
    public IActionResult TxtNames(string group) {
        Logger.Info($"/Submission/TxtNames - Group({group})");
        string path = Path.Join("examples/txt_files", group);
        if (!Directory.Exists(path)) {
            return BadRequest("No group with name " + group);
        }
        List<string> fileNames = Directory.EnumerateFiles(path).ToList(); 
        return Ok(fileNames.Select(s => Path.GetFileNameWithoutExtension(s)));
    } 

    //list the name of all current groups 
    [HttpGet]
    public IActionResult GroupNames() {
        Logger.Info($"/Submission/GroupNames");
        List<string> groupNames = Directory.EnumerateDirectories("examples/txt_files").ToList();
        return Ok(groupNames.Select(s => Path.GetFileName(s)));         
    } 

    //deletes all files with a group of a given name
    [HttpDelete]
    public async Task<IActionResult> RemoveGroup(string group) {
        Logger.Info($"/Submission/RemoveGroup - Group({group})");
        string path = Path.Join("examples/txt_files", group);
        if (!Directory.Exists(path)) {
            return BadRequest("No group with name " + group);
        }
        var  deleteDirectoryTask = Task.Run(() => Directory.Delete(path, true));
        await deleteDirectoryTask;

        return Ok(group + " deleted.");
    }

    //deletes all files with a group of a given name
    [HttpDelete]
    public async Task<IActionResult> RemoveTxt(string group, string name) {
        Logger.Info($"/Submission/RemoveTxt - Group({group}, Name({name}))");
        string path = Path.Join("examples/txt_files", group);
        if (!Directory.Exists(path)) {
            return BadRequest("No group with name " + group);
        }
        path = Path.Join(path, name);
        if (!System.IO.File.Exists(path)) {
            return BadRequest("No File with name " + name);
        }
        var  deleteDirectoryTask = Task.Run(() => System.IO.File.Delete(path));
        await deleteDirectoryTask;
        return Ok(name + " deleted.");
    }

    // returns the text file from a given name and group
    [HttpGet]
    public IActionResult GetTxt(string group, string name) {
        Logger.Info($"/Submission/RetTxt - Group({group}, Name({name}))");

        string path = Path.Join(group, name);
        path = Path.Join("examples/txt_files", path);
        FileStream searchFile;
        try {
            searchFile = System.IO.File.Open(path, FileMode.Open);
        } catch (FileNotFoundException) {
            return BadRequest($"No file {group}/{name}");
        }

        return Ok(searchFile);

    }

    

    
}    