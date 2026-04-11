using System;
using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Infrastructure.Services;

public class SharedMemoryTestService
{
    private readonly SharedMemoryService _smService;

    public SharedMemoryTestService(SharedMemoryService smService)
    {
        _smService = smService;
    }

    public unsafe void RunAllTests()
    {
        Console.WriteLine("=== Starting Shared Memory Tests ===");

        TestReadWriteSPAStatus();
        TestReadThreadState();
        TestEnqueueCommand();

        Console.WriteLine("=== Tests Completed ===");
    }

    /// <summary>
    /// 장비 상태(SPAStatus) 읽기 및 쓰기 테스트
    /// </summary>
    private unsafe void TestReadWriteSPAStatus()
    {
        Console.WriteLine("\n[Test] Reading SPAStatus...");
        var status = _smService.GetPointer<SPAStatus>("PMAC_STATE");

        if (status == null)
        {
            Console.WriteLine("FAILED: Could not connect to PMAC_STATE. Is the equipment software running?");
            return;
        }

        // 현재 값 출력
        Console.WriteLine($"Current Line: {status->nLineNumber}");
        Console.WriteLine($"Current Tool: {status->nCurrentToolNo}");
        Console.WriteLine($"X Position: {status->fPosition[0]:F3}");
        Console.WriteLine($"X Position: {status->nAirRecharge}");

        // 값 수정 테스트 (예: 현재 라인 번호를 임시로 변경)
        int originalLine = status->nLineNumber;
        status->nLineNumber = 999;
        Console.WriteLine($"Modified Line to: {status->nLineNumber}");
        // 값 수정 테스트 
        int originalTool = status->nCurrentToolNo;
        status->nCurrentToolNo = 2; 
        Console.WriteLine($"Modified Tool to: {status->nCurrentToolNo}");
        status->nCurrentToolNo = originalTool;

        // 복구 및 확인 (다른 프로그램 다운 방지를 위해 즉시 복구)
        bool success = status->nLineNumber == 999;
        status->nLineNumber = originalLine; 

        if (success)
            Console.WriteLine("SUCCESS: SPAStatus write verified.");
        else
            Console.WriteLine("FAILED: SPAStatus write mismatch.");
    }

    /// <summary>
    /// 스레드 상태(SThreadState) 및 고정 문자열 읽기 테스트
    /// </summary>
    private unsafe void TestReadThreadState()
    {
        Console.WriteLine("\n[Test] Reading SThreadState...");
        var thread = _smService.GetPointer<SThreadState>("PTHREAD_STATE");

        if (thread == null) return;

        Console.WriteLine($"RunMode: {thread->hRunMode}");
        
        // byte* (ANSI)를 sbyte*로 캐스팅하여 C# string으로 변환
        // C++의 char*는 C#의 sbyte*와 호환됩니다.
        string version = new string((sbyte*)thread->szMotionProgVersion);
        Console.WriteLine($"Motion Version: {version.TrimEnd('\0')}");

        Console.WriteLine($"nM28TypeRunning: {thread->nM28TypeRunning_}");
        Console.WriteLine($"nM28TypeRunning: {thread->bIsManualMoving_}");
    }

    /// <summary>
    /// IPC Queue 명령 전송 테스트
    /// </summary>
    private void TestEnqueueCommand()
    {
        Console.WriteLine("\n[Test] Enqueuing IPC Command (RESET)...");
        
        var cmd = new SIpcCommCommand();
        cmd.m_cmd = (byte)EN_IPC_COMMAND.IPC_COMMAND_RESET;

        bool result = _smService.EnqueueCommand("IPC_SERVER", cmd);

        if (result)
            Console.WriteLine("SUCCESS: Reset command enqueued.");
        else
            Console.WriteLine("FAILED: Queue might be full or not found.");
    }
}
