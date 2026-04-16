using SMRSvr.Domain.Dtos;
using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Infrastructure.Services;

public class MachineStatusService
{
    private readonly SharedMemoryService _smService;

    public MachineStatusService(SharedMemoryService smService)
    {
        _smService = smService;
    }

    /// <summary>
    /// 장비의 현재 전반적인 상태를 조회합니다.
    /// </summary>
    public unsafe MachineStatusDto GetMachineStatus()
    {
        var dto = new MachineStatusDto();

        // 1. PMAC_STATE (SPAStatus) 읽기
        var status = _smService.GetPointer<SPAStatus>("PMAC_STATE");
        if (status != null)
        {
            for (int i = 0; i < 5; i++)
            {
                dto.Position[i] = Math.Round(status->fPosition[i], 4);
            }

            dto.CurrentToolNo = status->nCurrentToolNo;

            for (int i = 0; i < 40; i++)
            {
                dto.Inputs[i] = status->bInput[i] != 0;
                dto.Outputs[i] = status->bOutput[i] != 0;
            }
        }

        // 2. PTHREAD_STATE (SThreadState) 읽기
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        if (threadState != null)
        {
            dto.RunMode = threadState->hRunMode.ToString();
            dto.ErrorCode = threadState->nErrorCode;
            dto.IsNcFileLoaded = threadState->bIsOpenNCFile != 0;
            
            if (dto.IsNcFileLoaded)
            {
                dto.NcFileName = SharedMemoryService.GetUnicodeString(threadState->hNCFileInfo.file_name, 257 * 2);
                dto.TotalLines = threadState->hNCFileInfo.total_lines;
                dto.CurrentLine = threadState->hNCFileInfo.machining_lines;
            }
        }

        return dto;
    }

    /// <summary>
    /// 지정된 좌표계(53~59)에 따른 각 축의 위치를 계산하여 반환합니다.
    /// </summary>
    public unsafe PositionDto GetPosition(int coordNo)
    {
        var status = _smService.GetPointer<SPAStatus>("PMAC_STATE");
        var config = _smService.GetPointer<SConfigData>("CONFIG_DATA");

        var dto = new PositionDto { CoordinateNo = coordNo };

        if (status == null) return dto;

        int coordIndex = coordNo - 53;
        if (coordIndex < 0 || coordIndex > 6) coordIndex = 0;

        for (int i = 0; i < 5; i++)
        {
            double machinePos = status->fPosition[i];
            double offset = 0;

            if (config != null && coordIndex > 0)
            {
                offset = config->fCoordOffset[coordIndex * 5 + i];
            }

            double val = machinePos - offset;
            switch (i)
            {
                case 0: dto.X = Math.Round(val, 4); break;
                case 1: dto.Y = Math.Round(val, 4); break;
                case 2: dto.Z = Math.Round(val, 4); break;
                case 3: dto.A = Math.Round(val, 4); break;
                case 4: dto.B = Math.Round(val, 4); break;
            }
        }

        return dto;
    }

    /// <summary>
    /// 장비의 각 툴 상태 정보를 조회합니다.
    /// </summary>
    public unsafe ToolListDto GetToolStatus()
    {
        var toolData = _smService.GetPointer<SToolData>("TOOL_MGR");
        var result = new ToolListDto();

        if (toolData == null) return result;

        int num_tools = 16; // 실제 툴 개수는 16개로 고정

        result.TotalToolCount = num_tools;
        STool* pTools = (STool*)toolData->hTool;

        for (int i = 1; i <= num_tools; i++)
        {
            STool tool = pTools[i];

            int mappedErrCode = 0;
            string errMsg = "None";

            if (tool.fUsingRate > 99.0)
            {
                mappedErrCode = 6;
                errMsg = "Timeout";
            }
            else
            {
                switch (tool.dwErrCode)
                {
                    case 0:  mappedErrCode = 0; errMsg = "None"; break;
                    case 10: mappedErrCode = 1; errMsg = "Empty"; break;
                    case 14: mappedErrCode = 2; errMsg = "Broken"; break;
                    case 18: mappedErrCode = 3; errMsg = "Long"; break;
                    case 19: mappedErrCode = 4; errMsg = "Short"; break;
                    default: mappedErrCode = 5; errMsg = "Unknown"; break;
                }
            }

            result.Tools.Add(new ToolStatusDto
            {
                ToolNo = i,
                UsingRate = Math.Round(tool.fUsingRate, 2),
                ErrorCode = mappedErrCode,
                ErrorMessage = errMsg,
                UsingTime = tool.dwUsingTime,
                MaximumTime = tool.dwMaximumTime
            });
        }

        return result;
    }

    /// <summary>
    /// 장비의 입출력(I/O) 상태 정보를 조회합니다.
    /// </summary>
    public unsafe IODtos GetIOStatus()
    {
        var status = _smService.GetPointer<SPAStatus>("PMAC_STATE");
        var dto = new IODtos();

        if (status != null)
        {
            for (int i = 0; i < 40; i++)
            {
                dto.Inputs[i] = status->bInput[i] != 0;
                dto.Outputs[i] = status->bOutput[i] != 0;
            }
        }

        return dto;
    }

    /// <summary>
    /// 장비의 현재 에러 정보를 조회합니다.
    /// </summary>
    public unsafe MachineErrorDto GetMachineError()
    {
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        var dto = new MachineErrorDto();

        if (threadState != null)
        {
            dto.ErrorType = threadState->nErrorType;
            dto.ErrorCode = threadState->nErrorCode;
            dto.Message = $"ErrorType: {dto.ErrorType}, ErrorCode: {dto.ErrorCode}";
        }

        return dto;
    }
}
