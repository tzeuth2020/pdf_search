/* using Microsoft.AspNetCore.Mvc;
using SearchServices;
using NLog;
namespace Controllers;


[Route("[controller]/[action]")]
public class SearchController : Controller {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /** gets text from group and name, finds all indices in the text which begin a section which have an edit distance atmost limit away from the pattern, 
    optionally can specify start and end indices to search only part of the text  **
    [HttpGet]
    public async Task<IActionResult> FindMatches
        (string group, string name, string pattern, int limit, int start = -1, int end = -1) {
        Logger.Info($"/Search/FindMatches - Group({group}), Name({name}), Pattern({pattern}), Limit({limit})");

        string path = Path.Join(group, name);
        path = Path.Join("examples/txt_files", path);
        FileStream searchFile;
        try {
            searchFile = System.IO.File.Open(path, FileMode.Open);
        } catch (FileNotFoundException) {
            return BadRequest($"No file {group}/{name}");
        }

        try {
            var indices = await Task.Run(() => SearchDoc.findMatches(pattern, searchFile, limit, start, end));
            searchFile.Close();
            return Ok(indices);
        } catch (ArgumentException e) {
            searchFile.Close();
            return BadRequest(e.Message);
        }
    }
    
    // gets file from group and name, find the start and end of a given question number in the file 
    [HttpGet]
    public async Task<IActionResult> FindQuestion(string group, string name, int question) {
        Logger.Info($"/Search/FindQuestion - Group({group}), Name({name}), Question({question})");

        string path = Path.Join(group, name);
        path = Path.Join("examples/txt_files", path);
        FileStream searchFile;
        try {
            searchFile = System.IO.File.Open(path, FileMode.Open);
        } catch (FileNotFoundException) {
            return BadRequest($"No file {group}/{name}");
        }

        try {
            var indices = await Task.Run(() => SearchDoc.getQuestion(searchFile, question));
            searchFile.Close();
            return Ok(indices);
        } catch (ArgumentException e) {
            searchFile.Close();
            return BadRequest(e.Message);
        }
    }
} */