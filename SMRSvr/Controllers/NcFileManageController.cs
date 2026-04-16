using Microsoft.AspNetCore.Mvc;
using SMRSvr.Infrastructure.Services;
using SMRSvr.Domain.Dtos.NcFile;
using SMRSvr.Domain.Dtos.MachineStatus;

namespace SMRSvr.Controllers;

[ApiController]
[Route("pnc/[controller]")]
public class NcFileManageController : ControllerBase
{
    private readonly NcFileService _ncFileService;

    public NcFileManageController(NcFileService ncFileService)
    {
        _ncFileService = ncFileService;
    }

    /// <summary>
    /// 지정된 경로의 NC 파일을 장비의 가공 리스트(공유 메모리)에 추가합니다.
    /// </summary>
    /// <param name="fullPath">NC 파일의 절대 경로</param>
    [HttpPost("add")]
    public async Task<IActionResult> AddNcFile([FromQuery] string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            return BadRequest("파일 경로가 비어있습니다.");
        }

        bool result = await Task.Run(() => _ncFileService.AddNcFile(fullPath));

        if (result)
        {
            return Ok(new { Message = "NC 파일이 성공적으로 추가되었습니다.", Path = fullPath });
        }
        else
        {
            return StatusCode(500, "NC 파일 추가에 실패했습니다. (리스트 초과 또는 공유 메모리 접근 오류)");
        }
    }

    /// <summary>
    /// 현재 공유 메모리에 등록된 NC 파일 리스트를 조회합니다.
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<NcFileListDto>> GetNcFileList()
    {
        var result = await Task.Run(() => _ncFileService.GetNcFileList());
        return Ok(result);
    }

    /// <summary>
    /// 지정된 이름의 NC 파일을 장비 가공 리스트 및 물리 폴더에서 제거합니다.
    /// </summary>
    /// <param name="fileName">삭제할 NC 파일 이름 (확장자 포함)</param>
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveNcFile([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("파일 이름이 비어있습니다.");
        }

        bool result = await Task.Run(() => _ncFileService.RemoveNcFile(fileName));

        if (result)
        {
            return Ok(new { Message = "NC 파일이 리스트와 폴더에서 삭제되었습니다.", FileName = fileName });
        }
        else
        {
            return NotFound(new { Message = "파일을 찾을 수 없거나 현재 열려있는 파일입니다.", FileName = fileName });
        }
    }

    /// <summary>
    /// 지정된 이름의 NC 파일을 가공을 위해 오픈합니다.
    /// </summary>
    /// <param name="fileName">오픈할 NC 파일 이름 (확장자 포함)</param>
    [HttpPost("open")]
    public async Task<IActionResult> OpenNcFile([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("파일 이름이 비어있습니다.");
        }

        bool result = await Task.Run(() => _ncFileService.OpenNcFile(fileName));

        if (result)
        {
            return Ok(new { Message = "NC 파일 오픈 명령을 전송했습니다.", FileName = fileName });
        }
        else
        {
            return StatusCode(500, "NC 파일 오픈 명령 전송에 실패했습니다.");
        }
    }

    /// <summary>
    /// 현재 오픈된 NC 파일을 닫습니다. (가공 중일 경우 실패)
    /// </summary>
    [HttpPost("close")]
    public async Task<IActionResult> CloseNcFile()
    {
        bool result = await Task.Run(() => _ncFileService.CloseNcFile());

        if (result)
        {
            return Ok(new { Message = "NC 파일 클로즈 명령을 전송했습니다." });
        }
        else
        {
            return BadRequest("현재 가공 중이거나 클로즈 명령을 처리할 수 없는 상태입니다.");
        }
    }
}
