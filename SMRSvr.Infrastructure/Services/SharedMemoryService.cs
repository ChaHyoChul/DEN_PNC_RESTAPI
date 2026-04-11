using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;
using System.Threading;
using System.Text;

namespace SMRSvr.Infrastructure.Services;

public unsafe class SharedMemoryService : IDisposable
{
    private readonly Dictionary<string, MemoryMappedFile> _mappedFiles = new();
    private readonly Dictionary<string, (MemoryMappedViewAccessor accessor, IntPtr pointer, int size)> _views = new();

    /// <summary>
    /// 일반 구조체 공유 메모리 포인터를 획득합니다.
    /// </summary>
    public T* GetPointer<T>(string mapName) where T : unmanaged
    {
        return (T*)InternalGetPointer(mapName, Marshal.SizeOf<T>());
    }

    /// <summary>
    /// 내부적으로 포인터를 관리하고 캐싱합니다.
    /// </summary>
    private byte* InternalGetPointer(string mapName, int size)
    {
        if (_views.TryGetValue(mapName, out var view))
        {
            return (byte*)view.pointer.ToPointer();
        }

        try
        {
            var mmf = MemoryMappedFile.OpenExisting(mapName);
            _mappedFiles[mapName] = mmf;

            var accessor = mmf.CreateViewAccessor(0, size);
            
            byte* ptr = null;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            
            _views[mapName] = (accessor, (IntPtr)ptr, size);
            return ptr;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SharedMemoryService] Error mapping {mapName}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// IPC Queue (CIpcQueue)에 명령을 추가합니다. 
    /// </summary>
    public bool EnqueueCommand(string queueName, SIpcCommCommand command)
    {
        string shmName = "QSHM_" + queueName;
        string mtxName = "QMTX_" + queueName;
        string smpName = "QSMP_" + queueName;

        int itemSize = sizeof(SIpcCommCommand);
        int qSize = 10; 
        int totalSize = 16 + (qSize * itemSize);

        byte* basePtr = InternalGetPointer(shmName, totalSize);
        if (basePtr == null) return false;

        try
        {
            using var mutex = Mutex.OpenExisting(mtxName);
            using var semaphore = Semaphore.OpenExisting(smpName);

            if (mutex.WaitOne(1000)) 
            {
                try
                {
                    int* pDataCount = (int*)basePtr;
                    int* pWritePoint = (int*)(basePtr + 8);
                    SIpcCommCommand* pQueueData = (SIpcCommCommand*)(basePtr + 16);

                    if (*pDataCount >= qSize) return false;

                    pQueueData[*pWritePoint] = command;
                    *pWritePoint = (*pWritePoint + 1) % qSize;
                    (*pDataCount)++;

                    semaphore.Release(1);
                    return true;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SharedMemoryService] EnqueueCommand Error: {ex.Message}");
        }
        return false;
    }

    #region String Helpers

    /// <summary>
    /// Unicode (TCHAR) byte 배열로부터 문자열을 읽어옵니다.
    /// </summary>
    public static string GetUnicodeString(byte* ptr, int maxByteCount)
    {
        if (ptr == null) return string.Empty;
        string full = Marshal.PtrToStringUni((IntPtr)ptr, maxByteCount / 2);
        int nullIdx = full.IndexOf('\0');
        return nullIdx >= 0 ? full.Substring(0, nullIdx) : full;
    }

    /// <summary>
    /// 문자열을 Unicode (TCHAR) byte 배열에 씁니다.
    /// </summary>
    public static void SetUnicodeString(byte* ptr, string source, int maxByteCount)
    {
        if (ptr == null) return;
        int maxChars = (maxByteCount / 2) - 1;
        int length = Math.Min(source.Length, maxChars);
        
        fixed (char* sPtr = source)
        {
            char* dPtr = (char*)ptr;
            for (int i = 0; i < length; i++) dPtr[i] = sPtr[i];
            dPtr[length] = '\0';
        }
    }

    /// <summary>
    /// ANSI (char) byte 배열로부터 문자열을 읽어옵니다.
    /// </summary>
    public static string GetAnsiString(byte* ptr, int maxByteCount)
    {
        if (ptr == null) return string.Empty;
        string full = Marshal.PtrToStringAnsi((IntPtr)ptr, maxByteCount);
        int nullIdx = full.IndexOf('\0');
        return nullIdx >= 0 ? full.Substring(0, nullIdx) : full;
    }

    /// <summary>
    /// 문자열을 ANSI (char) byte 배열에 씁니다.
    /// </summary>
    public static void SetAnsiString(byte* ptr, string source, int maxByteCount)
    {
        if (ptr == null) return;
        byte[] bytes = Encoding.ASCII.GetBytes(source);
        int length = Math.Min(bytes.Length, maxByteCount - 1);
        
        for (int i = 0; i < length; i++) ptr[i] = bytes[i];
        ptr[length] = 0;
    }

    #endregion

    public void Dispose()
    {
        foreach (var view in _views.Values)
        {
            view.accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            view.accessor.Dispose();
        }
        _views.Clear();

        foreach (var mmf in _mappedFiles.Values)
        {
            mmf.Dispose();
        }
        _mappedFiles.Clear();
    }
}
