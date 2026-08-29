using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class MatchaController : ControllerBase
{
    private readonly IMatchaScraperService _matchaScraperService;

    public MatchaController(IMatchaScraperService service)
    {
        _matchaScraperService = service;
    }



    [HttpGet("testRequest")]
    public async Task<IActionResult> GetProductPageAsync()
    {
        var result = await _matchaScraperService.GetProductPageAsync();

        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}