using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class StockMonitorController : ControllerBase
{
    private readonly IStockMonitorService _stockMonitorService;

    public StockMonitorController(IStockMonitorService service)
    {
        _stockMonitorService = service;
    }



    [HttpGet]
    public async Task<IActionResult> GetStockMonitor()
    {

        try
        {
            var result = await _stockMonitorService.CheckStockAsync();

            if (!result)
            {
                return NotFound();
            }

            return Ok(new
            {
                Message = "抓取抹茶監控資料成功",
                Data = result
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                Message = "抓取抹茶監控資料失敗"
            });
        }





    }

}