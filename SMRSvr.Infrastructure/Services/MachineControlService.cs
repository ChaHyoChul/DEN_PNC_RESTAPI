using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Infrastructure.Services;

/// <summary>
/// 장비의 동작 제어(Run, Stop, Pause, EMG 등)를 담당하는 서비스
/// </summary>
public class MachineControlService
{
    private readonly SharedMemoryService _smService;
    private const string QueueName = "IPC_SERVER";

    public MachineControlService(SharedMemoryService smService)
    {
        _smService = smService;
    }

    /// <summary>
    /// IPC 명령을 큐에 추가합니다.
    /// </summary>
    private bool SendCommand(EN_IPC_COMMAND mainCmd, byte subCmd = 0, string param = "")
    {
        var command = new SIpcCommCommand
        {
            m_cmd = (byte)mainCmd
        };

        // TODO: subCmd 및 param 처리 로직 추가 (SIpcCommCommand 구조체 레이아웃에 맞게)
        // 현재는 메인 명령 위주로 구현
        
        return _smService.EnqueueCommand(QueueName, command);
    }

    public bool Run() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RUN);
    public bool Stop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_STOP);
    public bool Pause() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_PAUSE);
    public bool EmergencyStop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_EMG);
    public bool Reset() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RESET);
}
