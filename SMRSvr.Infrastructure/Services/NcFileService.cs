using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;
using SMRSvr.Domain.Dtos.NcFile;
using System.IO;

namespace SMRSvr.Infrastructure.Services;

/// <summary>
/// NC 파일 리스트 조회, 업로드, 삭제 및 Open/Close 명령 제어를 담당하는 서비스
/// </summary>
public class NcFileService
{
    private readonly SharedMemoryService _smService;
    private readonly MachineControlService _controlService;
    private const string FileMgrMapName = "NCFILE_MGR";
    private const string TargetNcPath = @"c:\Pnc\Data\NCFILES";

    public NcFileService(SharedMemoryService smService, MachineControlService controlService)
    {
        _smService = smService;
        _controlService = controlService;
        
        if (!Directory.Exists(TargetNcPath))
        {
            Directory.CreateDirectory(TargetNcPath);
        }
    }

    /// <summary>
    /// 지정된 경로의 NC 파일을 장비의 NCFILES 폴더로 복사하고 공유 메모리 리스트에 추가합니다.
    /// </summary>
    public unsafe bool AddNcFile(string fullPath)
    {
        if (!File.Exists(fullPath)) return false;

        string fileName = Path.GetFileName(fullPath);
        string destPath = Path.Combine(TargetNcPath, fileName);

        try
        {
            if (!fullPath.Equals(destPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(fullPath, destPath, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NcFileService] File Copy Error: {ex.Message}");
            return false;
        }

        var fileMgr = _smService.GetPointer<SNCFileMgr>(FileMgrMapName);
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        
        if (fileMgr == null || threadState == null) return false;

        SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;

        for (int i = 0; i < fileMgr->nNumNCFile; i++)
        {
            string registeredName = SharedMemoryService.GetUnicodeString(pFileInfoBase[i].file_name, 257 * 2);
            if (registeredName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                pFileInfoBase[i].finish = 0;
                pFileInfoBase[i].state = (ushort)EN_NC_FILESTATE.NCFILE_STATE_BEFORE;
                
                try {
                    pFileInfoBase[i].file_size = (uint)new FileInfo(destPath).Length;
                } catch { }

                threadState->bUpdateNcFileList_ = 1;
                return true;
            }
        }

        if (fileMgr->nNumNCFile >= 100) return false;

        int newIndex = fileMgr->nNumNCFile;
        SNCFileInfo newInfo = new SNCFileInfo();
        
        string id = DateTime.Now.ToString("yyMMddHHmmss");
        SharedMemoryService.SetUnicodeString(newInfo.id, id, 26 * 2);
        SharedMemoryService.SetUnicodeString(newInfo.file_name, fileName, 257 * 2);
        
        try {
            var fi = new FileInfo(destPath);
            newInfo.file_size = (uint)fi.Length;
        } catch {
            newInfo.file_size = 0;
        }

        newInfo.state = (ushort)EN_NC_FILESTATE.NCFILE_STATE_BEFORE;
        newInfo.finish = 0;
        newInfo.is_select = 1;

        pFileInfoBase[newIndex] = newInfo;
        fileMgr->nNumNCFile++;

        threadState->bUpdateNcFileList_ = 1;

        return true;
    }

    /// <summary>
    /// 공유 메모리 리스트에서 NC 파일을 제거하고 물리 파일도 삭제합니다.
    /// </summary>
    public unsafe bool RemoveNcFile(string fileName)
    {
        var fileMgr = _smService.GetPointer<SNCFileMgr>(FileMgrMapName);
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        
        if (fileMgr == null || threadState == null) return false;

        SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;
        int targetIndex = -1;

        for (int i = 0; i < fileMgr->nNumNCFile; i++)
        {
            string registeredName = SharedMemoryService.GetUnicodeString(pFileInfoBase[i].file_name, 257 * 2);
            if (registeredName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return false;

        if (targetIndex == fileMgr->nCurNCFileIndex) return false;

        for (int i = targetIndex; i < fileMgr->nNumNCFile - 1; i++)
        {
            pFileInfoBase[i] = pFileInfoBase[i + 1];
        }

        fileMgr->nNumNCFile--;

        try
        {
            string fullPath = Path.Combine(TargetNcPath, fileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        catch { }

        threadState->bUpdateNcFileList_ = 1;

        return true;
    }

    /// <summary>
    /// 특정 NC 파일을 가공 준비 상태로 오픈합니다.
    /// </summary>
    public unsafe bool OpenNcFile(string fileName)
    {
        var fileMgr = _smService.GetPointer<SNCFileMgr>(FileMgrMapName);
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        
        if (fileMgr == null || threadState == null) return false;

        SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;
        int targetIndex = -1;

        for (int i = 0; i < fileMgr->nNumNCFile; i++)
        {
            string registeredName = SharedMemoryService.GetUnicodeString(pFileInfoBase[i].file_name, 257 * 2);
            if (registeredName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return false;

        //// 선제적 데이터 업데이트 (PNC UI 갱신용)
        //for (int i = 0; i < fileMgr->nNumNCFile; i++)
        //{
        //    pFileInfoBase[i].is_select = 0;
        //}
        //pFileInfoBase[targetIndex].state = (ushort)EN_NC_FILESTATE.NCFILE_STATE_OPEN;
        //pFileInfoBase[targetIndex].is_select = 1;
        //fileMgr->nCurNCFileIndex = targetIndex;

        bool result = _controlService.SendCommand(EN_IPC_COMMAND.IPC_COMMAND_OPEN, (byte)targetIndex);
        if (result)
        {
            threadState->bUpdateNcFileList_ = 1;
        }
        return result;
    }

    /// <summary>
    /// 현재 열려있는 NC 파일을 닫습니다. (가공 중일 경우 실패)
    /// </summary>
    public unsafe bool CloseNcFile()
    {
        //var fileMgr = _smService.GetPointer<SNCFileMgr>(FileMgrMapName);
        var threadState = _smService.GetPointer<SThreadState>("PTHREAD_STATE");
        
        if (threadState == null) return false;

        // 가공 동작 중(RUNMODE_RUN)인 경우 Close 불가
        if (threadState->hRunMode == EN_RUNMODE.RUNMODE_RUN)
        {
            return false;
        }

        //// 선제적 데이터 업데이트 (PNC UI 갱신용)
        //if (fileMgr->nCurNCFileIndex >= 0 && fileMgr->nCurNCFileIndex < fileMgr->nNumNCFile)
        //{
        //    SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;
        //    pFileInfoBase[fileMgr->nCurNCFileIndex].state = (ushort)EN_NC_FILESTATE.NCFILE_STATE_BEFORE;
        //}
        
        //for (int i = 0; i < fileMgr->nNumNCFile; i++)
        //{
        //    SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;
        //    pFileInfoBase[i].is_select = 0;
        //}
        //fileMgr->nCurNCFileIndex = -1;

        bool result = _controlService.SendCommand(EN_IPC_COMMAND.IPC_COMMAND_CLOSE);
        if (result)
        {
            threadState->bUpdateNcFileList_ = 1;
        }
        return result;
    }

    public unsafe NcFileListDto GetNcFileList()
    {
        var result = new NcFileListDto();
        var fileMgr = _smService.GetPointer<SNCFileMgr>(FileMgrMapName);
        
        if (fileMgr != null)
        {
            result.TotalCount = fileMgr->nNumNCFile;
            SNCFileInfo* pFileInfoBase = (SNCFileInfo*)fileMgr->hNCFileInfo;

            for (int i = 0; i < fileMgr->nNumNCFile; i++)
            {
                var state = (EN_NC_FILESTATE)pFileInfoBase[i].state;
                string stateString = state.ToString().Replace("NCFILE_STATE_", "");

                result.Files.Add(new NcFileDto
                {
                    ID = SharedMemoryService.GetUnicodeString(pFileInfoBase[i].id, 26 * 2),
                    FileName = SharedMemoryService.GetUnicodeString(pFileInfoBase[i].file_name, 257 * 2),
                    FileSize = pFileInfoBase[i].file_size,
                    State = stateString,
                    IsFinished = pFileInfoBase[i].finish != 0,
                    IsSelected = pFileInfoBase[i].is_select != 0
                });
            }
        }
        
        return result;
    }
}
