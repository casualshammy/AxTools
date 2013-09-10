// Type: System.Environment
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace System
{
  /// <summary>
  /// Предоставляет сведения о текущей среде и платформе, а также необходимые для управления ими средства.Данный класс не может наследоваться.
  /// </summary>
  /// <filterpriority>1</filterpriority>
  [ComVisible(true)]
  public static class Environment
  {
    private const int MaxEnvVariableValueLength = 32767;
    private const int MaxSystemEnvVariableLength = 1024;
    private const int MaxUserEnvVariableLength = 255;
    private const int MaxMachineNameLength = 256;
    private static Environment.ResourceHelper m_resHelper;
    private static bool s_IsWindowsVista;
    private static bool s_CheckedOSType;
    private static bool s_IsW2k3;
    private static volatile bool s_CheckedOSW2k3;
    private static object s_InternalSyncObject;
    private static OperatingSystem m_os;
    private static Environment.OSName m_osname;
    private static IntPtr processWinStation;
    private static bool isUserNonInteractive;

    static object InternalSyncObject
    {
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] private get
      {
        if (Environment.s_InternalSyncObject == null)
        {
          object obj = new object();
          Interlocked.CompareExchange<object>(ref Environment.s_InternalSyncObject, obj, (object) null);
        }
        return Environment.s_InternalSyncObject;
      }
    }

    /// <summary>
    /// Возвращает время, истекшее с момента загрузки системы (в миллисекундах).
    /// </summary>
    /// 
    /// <returns>
    /// 32-разрядное целое число со знаком, содержащее время, истекшее с момента с последней загрузки системы (в миллисекундах).
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static int TickCount { [SecuritySafeCritical, MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    /// Возвращает или задает код выхода из процесса.
    /// </summary>
    /// 
    /// <returns>
    /// 32-разрядное целое число со знаком, содержащее код выхода.Значение по умолчанию равно нулю.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static int ExitCode { [SecuritySafeCritical, MethodImpl(MethodImplOptions.InternalCall)] get; [SecuritySafeCritical, MethodImpl(MethodImplOptions.InternalCall)] set; }

    internal static bool IsCLRHosted
    {
      [SecuritySafeCritical] get
      {
        return Environment.GetIsCLRHosted();
      }
    }

    /// <summary>
    /// Возвращает командную строку для данного процесса.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая аргументы командной строки.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="Path"/></PermissionSet>
    public static string CommandLine
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
        string s = (string) null;
        Environment.GetCommandLine(JitHelpers.GetStringHandleOnStack(ref s));
        return s;
      }
    }

    /// <summary>
    /// Возвращает или задает полный путь к текущей рабочей папке.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая путь к каталогу.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">Попытка задать пустую строку ("").</exception><exception cref="T:System.ArgumentNullException">Попытка задать значение null..</exception><exception cref="T:System.IO.IOException">Произошла ошибка ввода-вывода.</exception><exception cref="T:System.IO.DirectoryNotFoundException">Попытка задать локальный путь, который не удается найти.</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет надлежащего разрешения.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    public static string CurrentDirectory
    {
      get
      {
        return Directory.GetCurrentDirectory();
      }
      set
      {
        Directory.SetCurrentDirectory(value);
      }
    }

    /// <summary>
    /// Возвращает полный путь к системному каталогу.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая путь к каталогу.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static string SystemDirectory
    {
      [SecuritySafeCritical] get
      {
        StringBuilder sb = new StringBuilder(260);
        if (Win32Native.GetSystemDirectory(sb, 260) == 0)
          __Error.WinIOError();
        string path = ((object) sb).ToString();
        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
        return path;
      }
    }

    internal static string InternalWindowsDirectory
    {
      [SecurityCritical] get
      {
        StringBuilder sb = new StringBuilder(260);
        if (Win32Native.GetWindowsDirectory(sb, 260) == 0)
          __Error.WinIOError();
        return ((object) sb).ToString();
      }
    }

    /// <summary>
    /// Возвращает имя NetBIOS данного локального компьютера.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая имя данного компьютера.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Не удается получить имя данного компьютера.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="COMPUTERNAME"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    public static string MachineName
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, "COMPUTERNAME").Demand();
        StringBuilder nameBuffer = new StringBuilder(256);
        int bufferSize = 256;
        if (Win32Native.GetComputerName(nameBuffer, ref bufferSize) == 0)
          throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ComputerName"));
        else
          return ((object) nameBuffer).ToString();
      }
    }

    /// <summary>
    /// Возвращает число процессоров на текущем компьютере.
    /// </summary>
    /// 
    /// <returns>
    /// 32-разрядное целое число со знаком, которое задает количество процессоров на текущем компьютере.Значение по умолчанию отсутствует.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="NUMBER_OF_PROCESSORS"/></PermissionSet>
    public static int ProcessorCount
    {
      [SecuritySafeCritical] get
      {
        Win32Native.SYSTEM_INFO lpSystemInfo = new Win32Native.SYSTEM_INFO();
        Win32Native.GetSystemInfo(ref lpSystemInfo);
        return lpSystemInfo.dwNumberOfProcessors;
      }
    }

    /// <summary>
    /// Получает объем памяти для файла подкачки операционной системы.
    /// </summary>
    /// 
    /// <returns>
    /// Размер системного файла подкачки в байтах.
    /// </returns>
    public static int SystemPageSize
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(PermissionState.Unrestricted).Demand();
        Win32Native.SYSTEM_INFO lpSystemInfo = new Win32Native.SYSTEM_INFO();
        Win32Native.GetSystemInfo(ref lpSystemInfo);
        return lpSystemInfo.dwPageSize;
      }
    }

    /// <summary>
    /// Возвращает строку, обозначающую в данной среде начало новой строки.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая "\r\n" для платформ, отличных от Unix, или строка, содержащая "\n" для платформ Unix.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static string NewLine
    {
      get
      {
        return "\r\n";
      }
    }

    /// <summary>
    /// Возвращает объект <see cref="T:System.Version"/>, который описывает основные и второстепенные номера, а также номер построения и редакции среды CLR.
    /// </summary>
    /// 
    /// <returns>
    /// Объект, содержащий версию среды CLR.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Version Version
    {
      get
      {
        return new Version("4.0.30319.269");
      }
    }

    /// <summary>
    /// Возвращает объем физической памяти, сопоставленной контексту процесса.
    /// </summary>
    /// 
    /// <returns>
    /// Целое 64-разрядное число со знаком, содержащее число байтов физической памяти, сопоставленное контексту процесса.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static long WorkingSet
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(PermissionState.Unrestricted).Demand();
        return Environment.GetWorkingSet();
      }
    }

    /// <summary>
    /// Возвращает объект <see cref="T:System.OperatingSystem"/>, который содержит идентификатор текущей платформы и номер версии.
    /// </summary>
    /// 
    /// <returns>
    /// Объект, который содержит идентификатор платформы и номер версии.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Этому свойству не удалось получить версию системы.— или — Полученный идентификатор платформы не является элементом <see cref="T:System.PlatformID."/>.</exception><filterpriority>1</filterpriority>
    public static OperatingSystem OSVersion
    {
      [SecuritySafeCritical] get
      {
        if (Environment.m_os == null)
        {
          Win32Native.OSVERSIONINFO osVer1 = new Win32Native.OSVERSIONINFO();
          if (!Environment.GetVersion(osVer1))
            throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_GetVersion"));
          PlatformID platform;
          bool flag;
          switch (osVer1.PlatformId)
          {
            case 2:
              platform = PlatformID.Win32NT;
              flag = true;
              break;
            case 10:
              platform = PlatformID.Unix;
              flag = false;
              break;
            case 11:
              platform = PlatformID.MacOSX;
              flag = false;
              break;
            default:
              throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InvalidPlatformID"));
          }
          Win32Native.OSVERSIONINFOEX osVer2 = new Win32Native.OSVERSIONINFOEX();
          if (flag && !Environment.GetVersionEx(osVer2))
            throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_GetVersion"));
          Version version = new Version(osVer1.MajorVersion, osVer1.MinorVersion, osVer1.BuildNumber, (int) osVer2.ServicePackMajor << 16 | (int) osVer2.ServicePackMinor);
          Environment.m_os = new OperatingSystem(platform, version, osVer1.CSDVersion);
        }
        return Environment.m_os;
      }
    }

    internal static bool IsWindowsVista
    {
      get
      {
        if (!Environment.s_CheckedOSType)
        {
          OperatingSystem osVersion = Environment.OSVersion;
          Environment.s_IsWindowsVista = osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major >= 6;
          Environment.s_CheckedOSType = true;
        }
        return Environment.s_IsWindowsVista;
      }
    }

    internal static bool IsW2k3
    {
      get
      {
        if (!Environment.s_CheckedOSW2k3)
        {
          OperatingSystem osVersion = Environment.OSVersion;
          Environment.s_IsW2k3 = osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major == 5 && osVersion.Version.Minor == 2;
          Environment.s_CheckedOSW2k3 = true;
        }
        return Environment.s_IsW2k3;
      }
    }

    internal static bool RunningOnWinNT
    {
      get
      {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
      }
    }

    internal static Environment.OSName OSInfo
    {
      [SecuritySafeCritical] get
      {
        if (Environment.m_osname == Environment.OSName.Invalid)
        {
          lock (Environment.InternalSyncObject)
          {
            if (Environment.m_osname == Environment.OSName.Invalid)
            {
              Win32Native.OSVERSIONINFO local_0 = new Win32Native.OSVERSIONINFO();
              if (!Environment.GetVersion(local_0))
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_GetVersion"));
              switch (local_0.PlatformId)
              {
                case 1:
                  Environment.m_osname = Environment.OSName.Unknown;
                  break;
                case 2:
                  switch (local_0.MajorVersion)
                  {
                    case 4:
                      Environment.m_osname = Environment.OSName.Unknown;
                      break;
                    case 5:
                      Environment.m_osname = Environment.OSName.Win2k;
                      break;
                    default:
                      Environment.m_osname = Environment.OSName.WinNT;
                      break;
                  }
                case 11:
                  if (local_0.MajorVersion == 10)
                  {
                    switch (local_0.MinorVersion)
                    {
                      case 4:
                        Environment.m_osname = Environment.OSName.Tiger;
                        break;
                      case 5:
                        Environment.m_osname = Environment.OSName.Leopard;
                        break;
                      default:
                        Environment.m_osname = Environment.OSName.MacOSX;
                        break;
                    }
                  }
                  else
                  {
                    Environment.m_osname = Environment.OSName.MacOSX;
                    break;
                  }
                default:
                  Environment.m_osname = Environment.OSName.Unknown;
                  break;
              }
            }
          }
        }
        return Environment.m_osname;
      }
    }

    /// <summary>
    /// Возвращает текущие сведения о трассировке стека.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, содержащая сведения о трассировке стека.Это значение может быть равно <see cref="F:System.String.Empty."/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Запрошенные сведения о трассировке стека выходят за пределы допустимого диапазона.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
    public static string StackTrace
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(PermissionState.Unrestricted).Demand();
        return Environment.GetStackTrace((Exception) null, true);
      }
    }

    /// <summary>
    /// Определяет, является ли текущий процесс 64-разрядным.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если процесс является 64-разрядным; в противном случае —значение false.
    /// </returns>
    public static bool Is64BitProcess
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Определяет, является ли текущая операционная система 64-разрядной.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если операционная система является 64-разрядной; в противном случае — значение false.
    /// </returns>
    public static bool Is64BitOperatingSystem
    {
      [SecuritySafeCritical] get
      {
        bool isWow64;
        if (Win32Native.DoesWin32MethodExist("kernel32.dll", "IsWow64Process") && Win32Native.IsWow64Process(Win32Native.GetCurrentProcess(), out isWow64))
          return isWow64;
        else
          return false;
      }
    }

    /// <summary>
    /// Получает значение, позволяющее определить, выполняется ли завершение работы среды CLR.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если завершается работа среды CLR; в противном случае — значение false..
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static bool HasShutdownStarted { [SecuritySafeCritical, MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    /// Возвращает имя пользователя, который на данный момент выполнил вход в операционную систему Windows.
    /// </summary>
    /// 
    /// <returns>
    /// Имя пользователя, который на данный момент выполнил вход в операционную систему Windows.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="UserName"/></PermissionSet>
    public static string UserName
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserName").Demand();
        StringBuilder lpBuffer = new StringBuilder(256);
        int capacity = lpBuffer.Capacity;
        Win32Native.GetUserName(lpBuffer, ref capacity);
        return ((object) lpBuffer).ToString();
      }
    }

    /// <summary>
    /// Возвращает значение, позволяющее определить, выполняется ли текущий процесс в режиме взаимодействия с пользователем.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если текущий процесс выполняется в режиме взаимодействия с пользователем; в противном случае — значение false.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static bool UserInteractive
    {
      [SecuritySafeCritical] get
      {
        if ((Environment.OSInfo & Environment.OSName.WinNT) == Environment.OSName.WinNT)
        {
          IntPtr processWindowStation = Win32Native.GetProcessWindowStation();
          if (processWindowStation != IntPtr.Zero && Environment.processWinStation != processWindowStation)
          {
            int lpnLengthNeeded = 0;
            Win32Native.USEROBJECTFLAGS pvBuffer = new Win32Native.USEROBJECTFLAGS();
            if (Win32Native.GetUserObjectInformation(processWindowStation, 1, pvBuffer, Marshal.SizeOf((object) pvBuffer), ref lpnLengthNeeded) && (pvBuffer.dwFlags & 1) == 0)
              Environment.isUserNonInteractive = true;
            Environment.processWinStation = processWindowStation;
          }
        }
        return !Environment.isUserNonInteractive;
      }
    }

    /// <summary>
    /// Возвращает имя сетевого домена, связанное с текущим пользователем.
    /// </summary>
    /// 
    /// <returns>
    /// Имя сетевого домена, связанное с текущим пользователем.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">Операционная система не поддерживает получение имени сетевого домена.</exception><exception cref="T:System.InvalidOperationException">Не удается получить имя сетевого домена.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="UserName;UserDomainName"/></PermissionSet>
    public static string UserDomainName
    {
      [SecuritySafeCritical] get
      {
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserDomain").Demand();
        byte[] sid = new byte[1024];
        int length1 = sid.Length;
        StringBuilder domainName = new StringBuilder(1024);
        int capacity1 = domainName.Capacity;
        if ((int) Win32Native.GetUserNameEx(2, domainName, ref capacity1) == 1)
        {
          string str = ((object) domainName).ToString();
          int length2 = str.IndexOf('\\');
          if (length2 != -1)
            return str.Substring(0, length2);
        }
        int capacity2 = domainName.Capacity;
        int peUse;
        if (!Win32Native.LookupAccountName((string) null, Environment.UserName, sid, ref length1, domainName, ref capacity2, out peUse))
          throw new InvalidOperationException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
        else
          return ((object) domainName).ToString();
      }
    }

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [DllImport("QCall", CharSet = CharSet.Unicode)]
    internal static void _Exit(int exitCode);

    /// <summary>
    /// Завершает этот процесс и возвращает внутренней операционной системе указанный код выхода.
    /// </summary>
    /// <param name="exitCode">Код выхода, возвращаемый операционной системе.</param><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет должного разрешения для выполнения данной функции.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void Exit(int exitCode)
    {
      Environment._Exit(exitCode);
    }

    /// <summary>
    /// Завершает процесс сразу после записи сообщения в журнал событий приложений Windows, после чего включает сообщение в отчет об ошибках, отправляемый в корпорацию Майкрософт.
    /// </summary>
    /// <param name="message">Сообщение, в котором объясняется причина завершения процесса или значение null, если объяснение не предусмотрено.</param>
    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static void FailFast(string message);

    /// <summary>
    /// Завершает процесс сразу после записи сообщения в журнал событий приложений Windows, после чего включает сообщение и сведения об исключении в отчет об ошибках, отправляемый в корпорацию Майкрософт.
    /// </summary>
    /// <param name="message">Сообщение, в котором объясняется причина завершения процесса или значение null, если объяснение не предусмотрено.</param><param name="exception">Исключение, представляющее ошибку, вызвавшую завершение процесса.Обычно это исключение перехватывается в блоке catch.</param>
    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static void FailFast(string message, Exception exception);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    [DllImport("QCall", CharSet = CharSet.Unicode)]
    internal static void TriggerCodeContractFailure(ContractFailureKind failureKind, string message, string condition, string exceptionAsString);

    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [DllImport("QCall", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static bool GetIsCLRHosted();

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [DllImport("QCall", CharSet = CharSet.Unicode)]
    private static void GetCommandLine(StringHandleOnStack retString);

    /// <summary>
    /// Замещает имя каждой переменной среды, внедренной в указанную строку, строчным эквивалентом значения переменной, а затем возвращает результирующую строку.
    /// </summary>
    /// 
    /// <returns>
    /// Строка, в которой каждая переменная среды замещена ее значением.
    /// </returns>
    /// <param name="name">Строка, содержащая либо не содержащая имена переменных среды.Каждая переменная среды с двух сторон окружена знаками процента (%).</param><exception cref="T:System.ArgumentNullException"><paramref name="name"/> имеет значение null;</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static string ExpandEnvironmentVariables(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        return name;
      bool flag1 = CodeAccessSecurityEngine.QuickCheckForAllDemands();
      string[] strArray = name.Split(new char[1]
      {
        '%'
      });
      StringBuilder stringBuilder = flag1 ? (StringBuilder) null : new StringBuilder();
      int num1 = 100;
      StringBuilder lpDst = new StringBuilder(num1);
      bool flag2 = false;
      for (int index = 1; index < strArray.Length - 1; ++index)
      {
        if (strArray[index].Length == 0 || flag2)
        {
          flag2 = false;
        }
        else
        {
          lpDst.Length = 0;
          string lpSrc = "%" + strArray[index] + "%";
          int num2 = Win32Native.ExpandEnvironmentStrings(lpSrc, lpDst, num1);
          if (num2 == 0)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
          while (num2 > num1)
          {
            num1 = num2;
            lpDst.Capacity = num1;
            lpDst.Length = 0;
            num2 = Win32Native.ExpandEnvironmentStrings(lpSrc, lpDst, num1);
            if (num2 == 0)
              Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
          }
          if (!flag1)
          {
            flag2 = ((object) lpDst).ToString() != lpSrc;
            if (flag2)
            {
              stringBuilder.Append(strArray[index]);
              stringBuilder.Append(';');
            }
          }
        }
      }
      if (!flag1)
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, ((object) stringBuilder).ToString()).Demand();
      lpDst.Length = 0;
      int num3 = Win32Native.ExpandEnvironmentStrings(name, lpDst, num1);
      if (num3 == 0)
        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
      while (num3 > num1)
      {
        num1 = num3;
        lpDst.Capacity = num1;
        lpDst.Length = 0;
        num3 = Win32Native.ExpandEnvironmentStrings(name, lpDst, num1);
        if (num3 == 0)
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
      }
      return ((object) lpDst).ToString();
    }

    /// <summary>
    /// Возвращает строковый массив, содержащий аргументы командной строки для текущего процесса.
    /// </summary>
    /// 
    /// <returns>
    /// Массив строк, каждый элемент которого содержит аргумент командной строки.Первым элементом является имя исполняемого файла. Последующие элементы, если они существуют, содержат аргументы командной строки.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">Система не поддерживает аргументы командной строки.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="Path"/></PermissionSet>
    [SecuritySafeCritical]
    public static string[] GetCommandLineArgs()
    {
      new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
      return Environment.GetCommandLineArgsNative();
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static string[] GetCommandLineArgsNative();

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static string nativeGetEnvironmentVariable(string variable);

    /// <summary>
    /// Возвращает из текущего процесса значение переменной среды.
    /// </summary>
    /// 
    /// <returns>
    /// Значение переменной среды, заданное параметром <paramref name="variable"/> или значение null, если переменная среды не найдена.
    /// </returns>
    /// <param name="variable">Имя переменной среды.</param><exception cref="T:System.ArgumentNullException"><paramref name="variable"/> имеет значение null;</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static string GetEnvironmentVariable(string variable)
    {
      if (variable == null)
        throw new ArgumentNullException("variable");
      new EnvironmentPermission(EnvironmentPermissionAccess.Read, variable).Demand();
      StringBuilder lpValue = new StringBuilder(128);
      int environmentVariable = Win32Native.GetEnvironmentVariable(variable, lpValue, lpValue.Capacity);
      if (environmentVariable == 0 && Marshal.GetLastWin32Error() == 203)
        return (string) null;
      for (; environmentVariable > lpValue.Capacity; environmentVariable = Win32Native.GetEnvironmentVariable(variable, lpValue, lpValue.Capacity))
      {
        lpValue.Capacity = environmentVariable;
        lpValue.Length = 0;
      }
      return ((object) lpValue).ToString();
    }

    /// <summary>
    /// Возвращает из текущего процесса или раздела реестра операционной системы Windows значение переменной среды для текущего пользователя или локального компьютера.
    /// </summary>
    /// 
    /// <returns>
    /// Значение переменной среды, заданное параметрами <paramref name="variable"/> и <paramref name="target"/> или значение null, если переменная среды не найдена.
    /// </returns>
    /// <param name="variable">Имя переменной среды.</param><param name="target">Одно из значений <see cref="T:System.EnvironmentVariableTarget"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="variable"/> имеет значение null;</exception><exception cref="T:System.NotSupportedException">Значение параметра <paramref name="target"/> равно <see cref="F:System.EnvironmentVariableTarget.User"/> или <see cref="F:System.EnvironmentVariableTarget.Machine"/>, а текущей операционной системой является Windows 95, Windows 98 или Windows Me.</exception><exception cref="T:System.ArgumentException">Параметр <paramref name="target"/> не является допустимым значением <see cref="T:System.EnvironmentVariableTarget"/>.</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    [SecuritySafeCritical]
    public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
    {
      if (variable == null)
        throw new ArgumentNullException("variable");
      if (target == EnvironmentVariableTarget.Process)
        return Environment.GetEnvironmentVariable(variable);
      new EnvironmentPermission(PermissionState.Unrestricted).Demand();
      if (target == EnvironmentVariableTarget.Machine)
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", false))
        {
          if (registryKey == null)
            return (string) null;
          else
            return registryKey.GetValue(variable) as string;
        }
      }
      else if (target == EnvironmentVariableTarget.User)
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", false))
        {
          if (registryKey == null)
            return (string) null;
          else
            return registryKey.GetValue(variable) as string;
        }
      }
      else
        throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[1]
        {
          (object) target
        }));
    }

    [SecurityCritical]
    private static unsafe char[] GetEnvironmentCharArray()
    {
      char[] chArray = (char[]) null;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        char* chPtr1 = (char*) null;
        try
        {
          chPtr1 = Win32Native.GetEnvironmentStrings();
          if ((IntPtr) chPtr1 == IntPtr.Zero)
            throw new OutOfMemoryException();
          char* chPtr2 = chPtr1;
          while ((int) *chPtr2 != 0 || (int) chPtr2[1] != 0)
            ++chPtr2;
          int len = (int) (chPtr2 - chPtr1 + 1L);
          chArray = new char[len];
          fixed (char* pDest = chArray)
            Buffer.memcpy(chPtr1, 0, pDest, 0, len);
        }
        finally
        {
          if ((IntPtr) chPtr1 != IntPtr.Zero)
            Win32Native.FreeEnvironmentStrings(chPtr1);
        }
      }
      return chArray;
    }

    /// <summary>
    /// Возвращает из текущего процесса имена всех переменных среды и их значения.
    /// </summary>
    /// 
    /// <returns>
    /// Словарь, в котором содержатся имена всех переменных среды и их значения; в противном случае, если переменные среды не найдены, — пустой словарь.
    /// </returns>
    /// <exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции.</exception><exception cref="T:System.OutOfMemoryException">Не хватает памяти для буфера.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static IDictionary GetEnvironmentVariables()
    {
      bool flag1 = CodeAccessSecurityEngine.QuickCheckForAllDemands();
      char[] environmentCharArray = Environment.GetEnvironmentCharArray();
      Hashtable hashtable = new Hashtable(20);
      StringBuilder stringBuilder = flag1 ? (StringBuilder) null : new StringBuilder();
      bool flag2 = true;
      for (int index = 0; index < environmentCharArray.Length; ++index)
      {
        int startIndex1 = index;
        while ((int) environmentCharArray[index] != 61 && (int) environmentCharArray[index] != 0)
          ++index;
        if ((int) environmentCharArray[index] != 0)
        {
          if (index - startIndex1 == 0)
          {
            while ((int) environmentCharArray[index] != 0)
              ++index;
          }
          else
          {
            string str1 = new string(environmentCharArray, startIndex1, index - startIndex1);
            ++index;
            int startIndex2 = index;
            while ((int) environmentCharArray[index] != 0)
              ++index;
            string str2 = new string(environmentCharArray, startIndex2, index - startIndex2);
            hashtable[(object) str1] = (object) str2;
            if (!flag1)
            {
              if (flag2)
                flag2 = false;
              else
                stringBuilder.Append(';');
              stringBuilder.Append(str1);
            }
          }
        }
      }
      if (!flag1)
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, ((object) stringBuilder).ToString()).Demand();
      return (IDictionary) hashtable;
    }

    internal static IDictionary GetRegistryKeyNameValuePairs(RegistryKey registryKey)
    {
      Hashtable hashtable = new Hashtable(20);
      if (registryKey != null)
      {
        foreach (string name in registryKey.GetValueNames())
        {
          string str = registryKey.GetValue(name, (object) "").ToString();
          hashtable.Add((object) name, (object) str);
        }
      }
      return (IDictionary) hashtable;
    }

    /// <summary>
    /// Возвращает из текущего процесса или раздела реестра операционной системы Windows имена и значения всех переменных среды для текущего пользователя или локального компьютера.
    /// </summary>
    /// 
    /// <returns>
    /// Словарь, в котором содержатся имена всех переменных среды и их значения, извлеченные из источника, заданного параметром <paramref name="target"/>; в противном случае, если переменные среды не найдены, — пустой словарь.
    /// </returns>
    /// <param name="target">Одно из значений <see cref="T:System.EnvironmentVariableTarget"/>.</param><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции для указанного значения параметра <paramref name="target"/>.</exception><exception cref="T:System.NotSupportedException">Этот метод нельзя использовать на платформах Windows 95 и Windows 98.</exception><exception cref="T:System.ArgumentException">Параметр <paramref name="target"/> содержит недопустимое значение.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    [SecuritySafeCritical]
    public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
    {
      if (target == EnvironmentVariableTarget.Process)
        return Environment.GetEnvironmentVariables();
      new EnvironmentPermission(PermissionState.Unrestricted).Demand();
      if (target == EnvironmentVariableTarget.Machine)
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", false))
          return Environment.GetRegistryKeyNameValuePairs(registryKey);
      }
      else if (target == EnvironmentVariableTarget.User)
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", false))
          return Environment.GetRegistryKeyNameValuePairs(registryKey);
      }
      else
        throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[1]
        {
          (object) target
        }));
    }

    /// <summary>
    /// Создает, изменяет или удаляет переменную среды, хранящуюся в текущем процессе.
    /// </summary>
    /// <param name="variable">Имя переменной среды.</param><param name="value">Значение, которое необходимо присвоить параметру <paramref name="variable"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="variable"/> имеет значение null;</exception><exception cref="T:System.ArgumentException">В параметре <paramref name="variable"/> содержится строка нулевой длины, начальный шестнадцатеричный символ нуля (0x00) или знак равенства (=). — или —Длина параметра <paramref name="variable"/> или параметра <paramref name="value"/> больше или равна 32 767 символам.— или —При выполнении этой операции произошла ошибка.</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static void SetEnvironmentVariable(string variable, string value)
    {
      Environment.CheckEnvironmentVariableName(variable);
      new EnvironmentPermission(PermissionState.Unrestricted).Demand();
      if (string.IsNullOrEmpty(value) || (int) value[0] == 0)
        value = (string) null;
      else if (value.Length >= (int) short.MaxValue)
        throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
      if (Win32Native.SetEnvironmentVariable(variable, value))
        return;
      int lastWin32Error = Marshal.GetLastWin32Error();
      switch (lastWin32Error)
      {
        case 203:
          break;
        case 206:
          throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
        default:
          throw new ArgumentException(Win32Native.GetMessage(lastWin32Error));
      }
    }

    private static void CheckEnvironmentVariableName(string variable)
    {
      if (variable == null)
        throw new ArgumentNullException("variable");
      if (variable.Length == 0)
        throw new ArgumentException(Environment.GetResourceString("Argument_StringZeroLength"), "variable");
      if ((int) variable[0] == 0)
        throw new ArgumentException(Environment.GetResourceString("Argument_StringFirstCharIsZero"), "variable");
      if (variable.Length >= (int) short.MaxValue)
        throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
      if (variable.IndexOf('=') != -1)
        throw new ArgumentException(Environment.GetResourceString("Argument_IllegalEnvVarName"));
    }

    /// <summary>
    /// Создает, изменяет или удаляет переменную среды, хранящуюся в текущем процессе или разделе реестра операционной системы Windows, зарезервированном для текущего пользователя или локального компьютера.
    /// </summary>
    /// <param name="variable">Имя переменной среды.</param><param name="value">Значение, которое необходимо присвоить параметру <paramref name="variable"/>.</param><param name="target">Одно из значений <see cref="T:System.EnvironmentVariableTarget"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="variable"/> имеет значение null;</exception><exception cref="T:System.ArgumentException">В параметре <paramref name="variable"/> содержится строка нулевой длины, начальный шестнадцатеричный символ нуля (0x00) или знак равенства (=). — или —Длина параметра <paramref name="variable"/> больше или равна 32 767 символам.— или —Параметр <paramref name="target"/> не является элементом перечисления <see cref="T:System.EnvironmentVariableTarget"/>. — или —Значение параметра <paramref name="target"/> равно <see cref="F:System.EnvironmentVariableTarget.Machine"/> или <see cref="F:System.EnvironmentVariableTarget.User"/>, а длина параметра <paramref name="variable"/> больше или равна 255.— или —Значение параметра <paramref name="target"/> равно <see cref="F:System.EnvironmentVariableTarget.Process"/>, а длина параметра <paramref name="value"/> больше или равна 32 767 символам. — или —При выполнении этой операции произошла ошибка.</exception><exception cref="T:System.NotSupportedException">Значение параметра <paramref name="target"/> равно <see cref="F:System.EnvironmentVariableTarget.User"/> или <see cref="F:System.EnvironmentVariableTarget.Machine"/>, а текущей операционной системой является Windows 95, Windows 98 или Windows Me.</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений на выполнение этой операции.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    [SecuritySafeCritical]
    public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
    {
      if (target == EnvironmentVariableTarget.Process)
      {
        Environment.SetEnvironmentVariable(variable, value);
      }
      else
      {
        Environment.CheckEnvironmentVariableName(variable);
        if (variable.Length >= 1024)
          throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarName"));
        new EnvironmentPermission(PermissionState.Unrestricted).Demand();
        if (string.IsNullOrEmpty(value) || (int) value[0] == 0)
          value = (string) null;
        if (target == EnvironmentVariableTarget.Machine)
        {
          using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
          {
            if (registryKey != null)
            {
              if (value == null)
                registryKey.DeleteValue(variable, false);
              else
                registryKey.SetValue(variable, (object) value);
            }
          }
        }
        else if (target == EnvironmentVariableTarget.User)
        {
          if (variable.Length >= (int) byte.MaxValue)
            throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
          using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", true))
          {
            if (registryKey != null)
            {
              if (value == null)
                registryKey.DeleteValue(variable, false);
              else
                registryKey.SetValue(variable, (object) value);
            }
          }
        }
        else
          throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[1]
          {
            (object) target
          }));
        int num = Win32Native.SendMessageTimeout(new IntPtr((int) ushort.MaxValue), 26, IntPtr.Zero, "Environment", 0U, 1000U, IntPtr.Zero) == IntPtr.Zero ? 1 : 0;
      }
    }

    /// <summary>
    /// Возвращает массив строк, содержащий имена логических дисков текущего компьютера.
    /// </summary>
    /// 
    /// <returns>
    /// Массив строк, в каждом элементе которого содержится имя логического диска.Например, если первым логическим диском является жесткий диск компьютера, первым возвращаемым элементом будет "C:\".
    /// </returns>
    /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода.</exception><exception cref="T:System.Security.SecurityException">У вызывающего оператора нет необходимых разрешений.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static string[] GetLogicalDrives()
    {
      new EnvironmentPermission(PermissionState.Unrestricted).Demand();
      int logicalDrives = Win32Native.GetLogicalDrives();
      if (logicalDrives == 0)
        __Error.WinIOError();
      uint num1 = (uint) logicalDrives;
      int length = 0;
      while ((int) num1 != 0)
      {
        if (((int) num1 & 1) != 0)
          ++length;
        num1 >>= 1;
      }
      string[] strArray = new string[length];
      char[] chArray = new char[3]
      {
        'A',
        ':',
        '\\'
      };
      uint num2 = (uint) logicalDrives;
      int num3 = 0;
      while ((int) num2 != 0)
      {
        if (((int) num2 & 1) != 0)
          strArray[num3++] = new string(chArray);
        num2 >>= 1;
        ++chArray[0];
      }
      return strArray;
    }

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [DllImport("QCall", CharSet = CharSet.Unicode)]
    private static long GetWorkingSet();

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static bool GetVersion(Win32Native.OSVERSIONINFO osVer);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static bool GetVersionEx(Win32Native.OSVERSIONINFOEX osVer);

    internal static string GetStackTrace(Exception e, bool needFileInfo)
    {
      return (e != null ? new StackTrace(e, needFileInfo) : new StackTrace(needFileInfo)).ToString(StackTrace.TraceFormat.Normal);
    }

    [SecuritySafeCritical]
    private static void InitResourceHelper()
    {
      bool lockTaken = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        Monitor.Enter(Environment.InternalSyncObject, ref lockTaken);
        if (Environment.m_resHelper != null)
          return;
        Environment.ResourceHelper resourceHelper = new Environment.ResourceHelper("mscorlib");
        Thread.MemoryBarrier();
        Environment.m_resHelper = resourceHelper;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(Environment.InternalSyncObject);
      }
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static string GetResourceFromDefault(string key);

    internal static string GetResourceStringLocal(string key)
    {
      if (Environment.m_resHelper == null)
        Environment.InitResourceHelper();
      return Environment.m_resHelper.GetResourceString(key);
    }

    [SecuritySafeCritical]
    internal static string GetResourceString(string key)
    {
      return Environment.GetResourceFromDefault(key);
    }

    [SecuritySafeCritical]
    internal static string GetResourceString(string key, params object[] values)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Environment.GetResourceFromDefault(key), values);
    }

    [SecuritySafeCritical]
    internal static string GetRuntimeResourceString(string key)
    {
      return Environment.GetResourceFromDefault(key);
    }

    [SecuritySafeCritical]
    internal static string GetRuntimeResourceString(string key, params object[] values)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Environment.GetResourceFromDefault(key), values);
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static bool GetCompatibilityFlag(CompatibilityFlag flag);

    /// <summary>
    /// Получает путь к особой системной папке, указанной в заданном перечислении.
    /// </summary>
    /// 
    /// <returns>
    /// Путь к указанной особой системной папке, если эта папка физически существует на компьютере; в противном случае — пустая строка ("").Папка физически не существует, если она не была создана операционной системой, была удалена или является виртуальным каталогом, таким как "Мой компьютер", которому не сопоставлен физический путь.
    /// </returns>
    /// <param name="folder">Перечислимая константа, позволяющая определить особую системную папку.</param><exception cref="T:System.ArgumentException">Объект <paramref name="folder"/> не является членом <see cref="T:System.Environment.SpecialFolder"/>.</exception><exception cref="T:System.PlatformNotSupportedException">Текущая платформа не поддерживается.</exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    [SecuritySafeCritical]
    public static string GetFolderPath(Environment.SpecialFolder folder)
    {
      return Environment.GetFolderPath(folder, Environment.SpecialFolderOption.None);
    }

    /// <summary>
    /// Получает путь к особой системной папке, указанной в заданном перечислении, и использует заданный параметр для доступа к особым папкам.
    /// </summary>
    /// 
    /// <returns>
    /// Путь к указанной особой системной папке, если эта папка физически существует на компьютере; в противном случае — пустая строка ("").Папка физически не существует, если она не была создана операционной системой, была удалена или является виртуальным каталогом, таким как "Мой компьютер", которому не сопоставлен физический путь.
    /// </returns>
    /// <param name="folder">Перечислимая константа, позволяющая определить особую системную папку.</param><param name="option">Задает параметры, используемые для доступа к особой папке.</param><exception cref="T:System.ArgumentException">Объект <paramref name="folder"/> не является элементом <see cref="T:System.Environment.SpecialFolder."/>.</exception><exception cref="T:System.PlatformNotSupportedException"><see cref="T:System.PlatformNotSupportedException"/></exception>
    [SecuritySafeCritical]
    public static string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
    {
      if (!Enum.IsDefined(typeof (Environment.SpecialFolder), (object) folder))
        throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[1]
        {
          (object) folder
        }));
      else if (!Enum.IsDefined(typeof (Environment.SpecialFolderOption), (object) option))
      {
        throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[1]
        {
          (object) option
        }));
      }
      else
      {
        if (option == Environment.SpecialFolderOption.Create)
          new FileIOPermission(PermissionState.None)
          {
            AllFiles = FileIOPermissionAccess.Write
          }.Demand();
        StringBuilder lpszPath = new StringBuilder(260);
        int folderPath = Win32Native.SHGetFolderPath(IntPtr.Zero, (int) (folder | (Environment.SpecialFolder) option), IntPtr.Zero, 0, lpszPath);
        if (folderPath < 0 && folderPath == -2146233031)
          throw new PlatformNotSupportedException();
        string path = ((object) lpszPath).ToString();
        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
        return path;
      }
    }

    internal sealed class ResourceHelper
    {
      private string m_name;
      private ResourceManager SystemResMgr;
      private Stack currentlyLoading;
      internal bool resourceManagerInited;

      internal ResourceHelper(string name)
      {
        this.m_name = name;
      }

      [SecuritySafeCritical]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      internal string GetResourceString(string key)
      {
        if (key == null || key.Length == 0)
          return "[Resource lookup failed - null or empty resource name]";
        else
          return this.GetResourceString(key, (CultureInfo) null);
      }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [SecuritySafeCritical]
      internal string GetResourceString(string key, CultureInfo culture)
      {
        if (key == null || key.Length == 0)
          return "[Resource lookup failed - null or empty resource name]";
        Environment.ResourceHelper.GetResourceStringUserData resourceStringUserData = new Environment.ResourceHelper.GetResourceStringUserData(this, key, culture);
        RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(new RuntimeHelpers.TryCode(this.GetResourceStringCode), new RuntimeHelpers.CleanupCode(this.GetResourceStringBackoutCode), (object) resourceStringUserData);
        return resourceStringUserData.m_retVal;
      }

      [SecuritySafeCritical]
      private void GetResourceStringCode(object userDataIn)
      {
        Environment.ResourceHelper.GetResourceStringUserData resourceStringUserData = (Environment.ResourceHelper.GetResourceStringUserData) userDataIn;
        Environment.ResourceHelper resourceHelper = resourceStringUserData.m_resourceHelper;
        string name = resourceStringUserData.m_key;
        CultureInfo cultureInfo = resourceStringUserData.m_culture;
        Monitor.Enter((object) resourceHelper, ref resourceStringUserData.m_lockWasTaken);
        if (resourceHelper.currentlyLoading != null && resourceHelper.currentlyLoading.Count > 0)
        {
          if (resourceHelper.currentlyLoading.Contains((object) name))
          {
            try
            {
              new StackTrace(true).ToString(StackTrace.TraceFormat.NoResourceLookup);
            }
            catch (StackOverflowException ex)
            {
            }
            catch (NullReferenceException ex)
            {
            }
            catch (OutOfMemoryException ex)
            {
            }
            resourceStringUserData.m_retVal = "[Resource lookup failed - infinite recursion or critical failure detected.]";
            return;
          }
        }
        if (resourceHelper.currentlyLoading == null)
          resourceHelper.currentlyLoading = new Stack(4);
        if (!resourceHelper.resourceManagerInited)
        {
          RuntimeHelpers.PrepareConstrainedRegions();
          try
          {
          }
          finally
          {
            RuntimeHelpers.RunClassConstructor(typeof (ResourceManager).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof (ResourceReader).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof (RuntimeResourceSet).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof (BinaryReader).TypeHandle);
            resourceHelper.resourceManagerInited = true;
          }
        }
        resourceHelper.currentlyLoading.Push((object) name);
        if (resourceHelper.SystemResMgr == null)
          resourceHelper.SystemResMgr = new ResourceManager(this.m_name, typeof (object).Assembly);
        string @string = resourceHelper.SystemResMgr.GetString(name, (CultureInfo) null);
        resourceHelper.currentlyLoading.Pop();
        resourceStringUserData.m_retVal = @string;
      }

      [PrePrepareMethod]
      private void GetResourceStringBackoutCode(object userDataIn, bool exceptionThrown)
      {
        Environment.ResourceHelper.GetResourceStringUserData resourceStringUserData = (Environment.ResourceHelper.GetResourceStringUserData) userDataIn;
        Environment.ResourceHelper resourceHelper = resourceStringUserData.m_resourceHelper;
        if (exceptionThrown && resourceStringUserData.m_lockWasTaken)
        {
          resourceHelper.SystemResMgr = (ResourceManager) null;
          resourceHelper.currentlyLoading = (Stack) null;
        }
        if (!resourceStringUserData.m_lockWasTaken)
          return;
        Monitor.Exit((object) resourceHelper);
      }

      internal class GetResourceStringUserData
      {
        public Environment.ResourceHelper m_resourceHelper;
        public string m_key;
        public CultureInfo m_culture;
        public string m_retVal;
        public bool m_lockWasTaken;

        public GetResourceStringUserData(Environment.ResourceHelper resourceHelper, string key, CultureInfo culture)
        {
          this.m_resourceHelper = resourceHelper;
          this.m_key = key;
          this.m_culture = culture;
        }
      }
    }

    [Serializable]
    internal enum OSName
    {
      Invalid = 0,
      Unknown = 1,
      WinNT = 128,
      Nt4 = 129,
      Win2k = 130,
      MacOSX = 256,
      Tiger = 257,
      Leopard = 258,
    }

    /// <summary>
    /// Задает параметры, используемые для получения пути к особой папке.
    /// </summary>
    public enum SpecialFolderOption
    {
      None = 0,
      DoNotVerify = 16384,
      Create = 32768,
    }

    /// <summary>
    /// Указывает перечислимые константы, используемые для получения путей к системным папкам.
    /// </summary>
    [ComVisible(true)]
    public enum SpecialFolder
    {
      Desktop = 0,
      Programs = 2,
      MyDocuments = 5,
      Personal = 5,
      Favorites = 6,
      Startup = 7,
      Recent = 8,
      SendTo = 9,
      StartMenu = 11,
      MyMusic = 13,
      MyVideos = 14,
      DesktopDirectory = 16,
      MyComputer = 17,
      NetworkShortcuts = 19,
      Fonts = 20,
      Templates = 21,
      CommonStartMenu = 22,
      CommonPrograms = 23,
      CommonStartup = 24,
      CommonDesktopDirectory = 25,
      ApplicationData = 26,
      PrinterShortcuts = 27,
      LocalApplicationData = 28,
      InternetCache = 32,
      Cookies = 33,
      History = 34,
      CommonApplicationData = 35,
      Windows = 36,
      System = 37,
      ProgramFiles = 38,
      MyPictures = 39,
      UserProfile = 40,
      SystemX86 = 41,
      ProgramFilesX86 = 42,
      CommonProgramFiles = 43,
      CommonProgramFilesX86 = 44,
      CommonTemplates = 45,
      CommonDocuments = 46,
      CommonAdminTools = 47,
      AdminTools = 48,
      CommonMusic = 53,
      CommonPictures = 54,
      CommonVideos = 55,
      Resources = 56,
      LocalizedResources = 57,
      CommonOemLinks = 58,
      CDBurning = 59,
    }
  }
}
