using Microsoft.AspNetCore.Mvc;
using SMRSvr.Infrastructure.Services;
using SMRSvr.Domain.Entities;

namespace SMRSvr.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineStatusController : ControllerBase
{
    private readonly SharedMemoryService _smService;

    public MachineStatusController(SharedMemoryService smService)
    {
        _smService = smService;
    }

    /// <summary>
    /// 장비의 각 축별 현재 좌표(X, Y, Z, A, B)를 조회합니다.
    /// </summary>
    [HttpGet("position")]
    public unsafe IActionResult GetPosition()
    {
        // 1. PMAC_STATE 공유 메모리 포인터 획득
        var status = _smService.GetPointer<SPAStatus>("PMAC_STATE");

        if (status == null)
        {
            return StatusCode(503, "장비 공유 메모리(PMAC_STATE)에 연결할 수 없습니다.");
        }

        // 2. fixed double 배열에서 값 읽기 (X, Y, Z, A, B 순서)
        double[] currentPositions = new double[5];
        for (int i = 0; i < 5; i++)
        {
            currentPositions[i] = status->fPosition[i];
        }

        // 3. 결과 반환
        return Ok(new
        {
            X = Math.Round(currentPositions[0], 4),
            Y = Math.Round(currentPositions[1], 4),
            Z = Math.Round(currentPositions[2], 4),
            A = Math.Round(currentPositions[3], 4),
            B = Math.Round(currentPositions[4], 4),
            Timestamp = DateTime.Now
        });
    }
}
