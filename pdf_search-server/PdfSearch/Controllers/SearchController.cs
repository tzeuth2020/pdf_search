using Microsoft.AspNetCore.Mvc;
using pdf_search.Services.Implmentations;
using NLog;
using pdf_search.Services.Interfaces;
namespace Controllers;


[Route("[controller]/[action]")]
public class SearchController : Controller {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ISubmissionService _submissionService;

    public SearchController(ISubmissionService submissionService) {
        _submissionService = submissionService;
    }

    /** gets text from group and name, finds all indices in the text which begin a section which have an edit distance atmost limit away from the pattern, 
    optionally can specify start and end indices to search only part of the text  **/
    [HttpGet]
    public async Task<IActionResult> FindMatches
        (string group, string name, string pattern, int limit, int question = 0) {
        Logger.Info($"/Search/FindMatches - Group({group}), Name({name}), Pattern({pattern}), Limit({limit})");
        try {
           var indices = await _submissionService.queryMatches(group, name, pattern, limit, question);
           return Ok(indices);
        } catch (ArgumentException e) {
            Logger.Error(e.Message);
            return BadRequest(e.Message);
        }
    }
    

} 