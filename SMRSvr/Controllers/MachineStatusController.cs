using Microsoft.AspNetCore.Mvc;
using SMRSvr.Infrastructure.Services;
using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Dtos.MachineStatus;

namespace SMRSvr.Controllers;

[ApiController]
[Route("pnc/[controller]")]
public class MachineStatusController : ControllerBase
{
    private readonly SharedMemoryService _smService;
    private readonly MachineStatusService _machineStatusService;

    public MachineStatusController(SharedMemoryService smService, MachineStatusService machineStatusService)
    {
        _smService = smService;
        _machineStatusService = machineStatusService;
    }

    /// <summary>
    /// 장비의 현재 전반적인 상태를 조회합니다.
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await Task.Run(() => _machineStatusService.GetMachineStatus());
        return Ok(status);
    }

    /// <summary>
    /// 장비의 각 축별 현재 좌표(X, Y, Z, A, B)를 조회합니다.
    /// </summary>
    /// <param name="coordNo">좌표계 번호 (53~59)</param>
    [HttpGet("position")]
    public async Task<IActionResult> GetPosition([FromQuery] int coordNo = 53)
    {
        if (coordNo < 53 || coordNo > 59)
        {
            return BadRequest("Invalid coordinate number. Valid range is 53~59.");
        }

        var result = await Task.Run(() => _machineStatusService.GetPosition(coordNo));
        return Ok(result);
    }

    /// <summary>
    /// 장비의 전체 툴 상태 정보를 조회합니다.
    /// </summary>
    [HttpGet("tool-status")]
    public async Task<IActionResult> GetToolStatus()
    {
        var result = await Task.Run(() => _machineStatusService.GetToolStatus());
        return Ok(result);
    }

    /// <summary>
    /// 장비의 입출력(I/O) 상태 정보를 조회합니다.
    /// </summary>
    [HttpGet("io")]
    public async Task<IActionResult> GetIOStatus()
    {
        var result = await Task.Run(() => _machineStatusService.GetIOStatus());
        return Ok(result);
    }

    /// <summary>
    /// 장비의 현재 에러 정보를 조회합니다.
    /// </summary>
    [HttpGet("error")]
    public async Task<IActionResult> GetError()
    {
        var result = await Task.Run(() => _machineStatusService.GetMachineError());
        return Ok(result);
    }
}
