using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

[Route("api/v{version:apiVersion}/test")]
[ApiController]
//[ApiVersion("1.0")]
//[ApiVersion("2.0")]
public class TestController : ControllerBase
{
    // 針對版本 1 的邏輯
    [HttpGet]
    //[MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        return Ok(new[] { "測試版本 1" });
    }

    // 針對版本 2 的邏輯
    [HttpGet]
    //[MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        return Ok(new[] { "測試版本 2 - 增加了新功能" });
    }
}
