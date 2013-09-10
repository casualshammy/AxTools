// Type: System.IDisposable
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Runtime.InteropServices;

namespace System
{
  /// <summary>
  /// Определяет методы высвобождения распределенных ресурсов.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  [ComVisible(true)]
  public interface IDisposable
  {
    /// <summary>
    /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    void Dispose();
  }
}
