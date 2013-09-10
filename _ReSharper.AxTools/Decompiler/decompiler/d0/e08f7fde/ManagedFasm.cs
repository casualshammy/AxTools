// Type: Fasm.ManagedFasm
// Assembly: fasmdll_managed, Version=1.0.4696.21858, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Axio\Desktop\DirectX\CoolFish\ProgramCode\FASM\bin\Release\fasmdll_managed.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Fasm
{
  public class ManagedFasm : IDisposable
  {
    private StringBuilder m_AssemblyString;
    private List<IntPtr> m_ThreadHandles;
    private IntPtr m_hProcess;
    private int m_MemorySize;
    private int m_PassLimit;

    public ManagedFasm(IntPtr hProcess)
    {
      this.m_hProcess = hProcess;
      this.m_AssemblyString = new StringBuilder("use32\n");
      this.m_ThreadHandles = new List<IntPtr>();
      this.m_MemorySize = 4096;
      this.m_PassLimit = 100;
    }

    public ManagedFasm()
    {
      this.m_AssemblyString = new StringBuilder("use32\n");
      this.m_ThreadHandles = new List<IntPtr>();
      this.m_MemorySize = 4096;
      this.m_PassLimit = 100;
    }

    private unsafe void \u007EManagedFasm()
    {
      int index = 0;
      if (0 < this.m_ThreadHandles.Count)
      {
        do
        {
          \u003CModule\u003E.CloseHandle((void*) this.m_ThreadHandles[index].ToInt32());
          ++index;
        }
        while (index < this.m_ThreadHandles.Count);
      }
      this.m_ThreadHandles.Clear();
    }

    public void AddLine(string szFormatString, params object[] args)
    {
      this.m_AssemblyString.AppendFormat(szFormatString + "\n", args);
    }

    public void AddLine(string szLine)
    {
      this.m_AssemblyString.Append(szLine + "\n");
    }

    public void Add(string szFormatString, params object[] args)
    {
      this.m_AssemblyString.AppendFormat(szFormatString, args);
    }

    public void Add(string szLine)
    {
      this.m_AssemblyString.Append(szLine);
    }

    public void InsertLine(string szLine, int nIndex)
    {
      this.m_AssemblyString.Insert(nIndex, szLine + "\n");
    }

    public void Insert(string szLine, int nIndex)
    {
      this.m_AssemblyString.Insert(nIndex, szLine);
    }

    public void Clear()
    {
      this.m_AssemblyString = new StringBuilder("use32\n");
    }

    public static unsafe byte[] Assemble(string szSource, int nMemorySize, int nPassLimit)
    {
      IntPtr num1 = new IntPtr();
      IntPtr hglobal = Marshal.StringToHGlobalAnsi(szSource);
      int num2 = (int) \u003CModule\u003E._c_FasmAssemble((sbyte*) hglobal.ToPointer(), (uint) nMemorySize, (uint) nPassLimit);
      _c_FasmState* cFasmStatePtr = (_c_FasmState*) \u003CModule\u003E._c_fasm_memorybuf;
      Marshal.FreeHGlobal(hglobal);
      if (*(int*) cFasmStatePtr != 0)
        throw new Exception(string.Format("Assembly failed!  Error code: {0};  Error Line: {1}", (object) *(int*) ((IntPtr) cFasmStatePtr + 4), (object) (uint) *(int*) (*(int*) ((IntPtr) cFasmStatePtr + 8) + 4)));
      byte[] destination = new byte[*(int*) ((IntPtr) cFasmStatePtr + 4)];
      Marshal.Copy((IntPtr) ((void*) *(int*) ((IntPtr) cFasmStatePtr + 8)), destination, 0, *(int*) ((IntPtr) cFasmStatePtr + 4));
      return destination;
    }

    public static byte[] Assemble(string szSource, int nMemorySize)
    {
      return ManagedFasm.Assemble(szSource, nMemorySize, 100);
    }

    public static byte[] Assemble(string szSource)
    {
      return ManagedFasm.Assemble(szSource, 4096, 100);
    }

    public byte[] Assemble()
    {
      return ManagedFasm.Assemble(this.m_AssemblyString.ToString(), this.m_MemorySize, this.m_PassLimit);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Inject(uint dwAddress)
    {
      ManagedFasm managedFasm = this;
      IntPtr hProcess = managedFasm.m_hProcess;
      int num = (int) dwAddress;
      return managedFasm.Inject(hProcess, (uint) num);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public unsafe bool Inject(IntPtr hProcess, uint dwAddress)
    {
      if (hProcess == IntPtr.Zero)
        return false;
      if (this.m_AssemblyString.ToString().Contains("use64") || this.m_AssemblyString.ToString().Contains("use16"))
        this.m_AssemblyString.Replace("use32\n", "");
      if (!this.m_AssemblyString.ToString().Contains("org "))
        this.m_AssemblyString.Insert(0, string.Format("org 0x{0:X08}\n", (object) dwAddress));
      IntPtr hglobal = IntPtr.Zero;
      try
      {
        bool flag;
        try
        {
          hglobal = Marshal.StringToHGlobalAnsi(this.m_AssemblyString.ToString());
          int num = (int) \u003CModule\u003E._c_FasmAssemble((sbyte*) hglobal.ToPointer(), (uint) this.m_MemorySize, (uint) this.m_PassLimit);
          goto label_13;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          flag = false;
        }
        return flag;
      }
      finally
      {
        if (hglobal != IntPtr.Zero)
          Marshal.FreeHGlobal(hglobal);
      }
label_13:
      _c_FasmState* cFasmStatePtr = (_c_FasmState*) \u003CModule\u003E._c_fasm_memorybuf;
      if (*(int*) cFasmStatePtr != 0)
        throw new Exception(string.Format("Assembly failed!  Error code: {0};  Error Line: {1}", (object) *(int*) ((IntPtr) cFasmStatePtr + 4), (object) (uint) *(int*) (*(int*) ((IntPtr) cFasmStatePtr + 8) + 4)));
      else
        return \u003CModule\u003E.WriteProcessMemory((void*) hProcess, (void*) dwAddress, (void*) *(int*) ((IntPtr) cFasmStatePtr + 8), (uint) *(int*) ((IntPtr) cFasmStatePtr + 4), (uint*) 0) != 0;
    }

    public uint InjectAndExecute(uint dwAddress)
    {
      ManagedFasm managedFasm = this;
      IntPtr hProcess = managedFasm.m_hProcess;
      int num1 = (int) dwAddress;
      int num2 = 0;
      return managedFasm.InjectAndExecute(hProcess, (uint) num1, (uint) num2);
    }

    public uint InjectAndExecute(IntPtr hProcess, uint dwAddress)
    {
      return this.InjectAndExecute(hProcess, dwAddress, 0U);
    }

    public unsafe uint InjectAndExecute(IntPtr hProcess, uint dwAddress, uint dwParameter)
    {
      if (hProcess == IntPtr.Zero)
        throw new ArgumentNullException("hProcess");
      if ((int) dwAddress == 0)
        throw new ArgumentNullException("dwAddress");
      uint num = 0U;
      if (!this.Inject(hProcess, dwAddress))
        throw new Exception("Injection failed for some reason.");
      // ISSUE: cast to a function pointer type
      void* remoteThread = \u003CModule\u003E.CreateRemoteThread((void*) hProcess.ToInt32(), (_SECURITY_ATTRIBUTES*) 0, 0U, (__FnPtr<uint (void*)>) (int) dwAddress, (void*) dwParameter, 0U, (uint*) 0);
      if ((IntPtr) remoteThread == IntPtr.Zero)
        throw new Exception("Remote thread failed.");
      try
      {
        if ((int) \u003CModule\u003E.WaitForSingleObject(remoteThread, 10000U) == 0)
        {
          if (\u003CModule\u003E.GetExitCodeThread(remoteThread, &num) == 0)
            throw new Exception("Could not get thread exit code.");
        }
      }
      finally
      {
        \u003CModule\u003E.CloseHandle(remoteThread);
      }
      return num;
    }

    public IntPtr InjectAndExecuteEx(uint dwAddress)
    {
      ManagedFasm managedFasm = this;
      IntPtr hProcess = managedFasm.m_hProcess;
      int num1 = (int) dwAddress;
      int num2 = 0;
      return managedFasm.InjectAndExecuteEx(hProcess, (uint) num1, (uint) num2);
    }

    public IntPtr InjectAndExecuteEx(IntPtr hProcess, uint dwAddress)
    {
      return this.InjectAndExecuteEx(hProcess, dwAddress, 0U);
    }

    public unsafe IntPtr InjectAndExecuteEx(IntPtr hProcess, uint dwAddress, uint dwParameter)
    {
      this.Inject(hProcess, dwAddress);
      // ISSUE: cast to a function pointer type
      void* remoteThread = \u003CModule\u003E.CreateRemoteThread((void*) hProcess.ToInt32(), (_SECURITY_ATTRIBUTES*) 0, 0U, (__FnPtr<uint (void*)>) (int) dwAddress, (void*) dwParameter, 0U, (uint*) 0);
      this.m_ThreadHandles.Add((IntPtr) remoteThread);
      return (IntPtr) remoteThread;
    }

    public IntPtr GetProcessHandle()
    {
      return this.m_hProcess;
    }

    public void SetProcessHandle(IntPtr Value)
    {
      this.m_hProcess = Value;
    }

    public int GetMemorySize()
    {
      return this.m_MemorySize;
    }

    public void SetMemorySize(int Value)
    {
      this.m_MemorySize = Value;
    }

    public int GetPassLimit()
    {
      return this.m_PassLimit;
    }

    public void SetPassLimit(int Value)
    {
      this.m_PassLimit = Value;
    }

    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool _param1)
    {
      if (param0)
      {
        this.\u007EManagedFasm();
      }
      else
      {
        // ISSUE: explicit finalizer call
        this.Finalize();
      }
    }

    public virtual void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
