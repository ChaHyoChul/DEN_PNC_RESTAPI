using Microsoft.AspNetCore.Mvc;
using SMRSvr.Infrastructure.Services;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Controllers;

[ApiController]
[Route("pnc/[controller]")]
public class MachineControlController : ControllerBase
{
    private readonly MachineControlService _controlService;
    private readonly MachineStatusService _statusService;

    public MachineControlController(MachineControlService controlService, MachineStatusService statusService)
    {
        _controlService = controlService;
        _statusService = statusService;
    }

    /// <summary>
    /// 가공을 시작합니다. (인터락: NC파일 오픈, STOP/PAUSE 상태, 원점완료, 서보온, 비상정지 해제)
    /// </summary>
    /// <param name="startLine">가공을 시작할 라인 번호 (기본값: 1)</param>
    [HttpPost("run")]
    public async Task<IActionResult> Run([FromQuery] int startLine = 1)
    {
        // 1. 에러 상태 체크 (가장 먼저 확인하여 리셋 유도)
        var errorInfo = _statusService.GetMachineError();
        if (errorInfo.ErrorCode != 0)
        {
            return BadRequest($"장비에 에러(Code: {errorInfo.ErrorCode})가 발생한 상태입니다. 먼저 리셋이 필요합니다.");
        }

        var status = _statusService.GetMachineStatus();
        
        // 2. NC 파일 오픈 여부 체크
        if (!status.IsNcFileLoaded)
        {
            return BadRequest("가공할 NC 파일이 로드되어 있지 않습니다.");
        }

        // 3. 현재 상태 체크 (STOP 또는 PAUSE에서만 시작 가능)
        if (status.RunMode != EN_RUNMODE.RUNMODE_STOP.ToString() && 
            status.RunMode != EN_RUNMODE.RUNMODE_PAUSE.ToString())
        {
            return BadRequest($"현재 상태({status.RunMode})에서는 가공을 시작할 수 없습니다. STOP 또는 PAUSE 상태여야 합니다.");
        }

        bool result = await Task.Run(() => _controlService.Run(startLine));
        if (result) return Ok(new { Message = $"가공 시작 명령(라인: {startLine})을 전송했습니다." });
        return StatusCode(500, "명령 전송에 실패했습니다.");
    }

    /// <summary>
    /// 가공을 중지합니다. (RUN 또는 PAUSE 상태에서만 가능)
    /// </summary>
    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        var status = _statusService.GetMachineStatus();
        if (status.RunMode != EN_RUNMODE.RUNMODE_RUN.ToString() && 
            status.RunMode != EN_RUNMODE.RUNMODE_PAUSE.ToString())
        {
            return BadRequest($"현재 상태({status.RunMode})에서는 중지 명령을 수행할 수 없습니다.");
        }

        bool result = await Task.Run(() => _controlService.Stop());
        if (result) return Ok(new { Message = "가공 중지 명령을 전송했습니다." });
        return StatusCode(500, "명령 전송에 실패했습니다.");
    }

    /// <summary>
    /// 가공을 일시 정지합니다. (RUN 상태에서만 가능)
    /// </summary>
    [HttpPost("pause")]
    public async Task<IActionResult> Pause()
    {
        var status = _statusService.GetMachineStatus();
        if (status.RunMode != EN_RUNMODE.RUNMODE_RUN.ToString())
        {
            return BadRequest($"현재 상태({status.RunMode})에서는 일시 정지할 수 없습니다. 가공 중(RUN)이어야 합니다.");
        }

        bool result = await Task.Run(() => _controlService.Pause());
        if (result) return Ok(new { Message = "가공 일시 정지 명령을 전송했습니다." });
        return StatusCode(500, "명령 전송에 실패했습니다.");
    }

    /// <summary>
    /// 비상 정지를 실행합니다.
    /// </summary>
    [HttpPost("emergency-stop")]
    public async Task<IActionResult> EmergencyStop()
    {
        bool result = await Task.Run(() => _controlService.EmergencyStop());
        if (result) return Ok(new { Message = "비상 정지 명령을 전송했습니다." });
        return StatusCode(500, "명령 전송에 실패했습니다.");
    }

    /// <summary>
    /// 에러를 리셋하고 초기화합니다. (STOP 또는 ERROR 상태에서만 가능)
    /// </summary>
    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var status = _statusService.GetMachineStatus();
        if (status.RunMode != EN_RUNMODE.RUNMODE_STOP.ToString() && 
            status.RunMode != EN_RUNMODE.RUNMODE_ERROR.ToString())
        {
            return BadRequest($"현재 상태({status.RunMode})에서는 리셋할 수 없습니다. STOP 또는 ERROR 상태여야 합니다.");
        }

        bool result = await Task.Run(() => _controlService.Reset());
        if (result) return Ok(new { Message = "리셋 명령을 전송했습니다." });
        return StatusCode(500, "명령 전송에 실패했습니다.");
    }
}
