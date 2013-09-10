// Type: MouseKeyboardActivityMonitor.BaseHookListener
// Assembly: MouseKeyboardActivityMonitor, Version=3.0.1.9579, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Axio\Documents\Visual Studio 2010\Projects\AxTools\AxLink\bin\Release\MouseKeyboardActivityMonitor.dll

using MouseKeyboardActivityMonitor.WinApi;
using System;

namespace MouseKeyboardActivityMonitor
{
  public abstract class BaseHookListener : IDisposable
  {
    private Hooker m_Hooker;

    protected int HookHandle { get; set; }

    protected HookCallback HookCallbackReferenceKeeper { get; set; }

    internal bool IsGlobal
    {
      get
      {
        return this.m_Hooker.IsGlobal;
      }
    }

    public bool Enabled
    {
      get
      {
        return this.HookHandle != 0;
      }
      set
      {
        if (value)
        {
          if (this.Enabled)
            return;
          this.Start();
        }
        else
        {
          if (!this.Enabled)
            return;
          this.Stop();
        }
      }
    }

    protected BaseHookListener(Hooker hooker)
    {
      if (hooker == null)
        throw new ArgumentNullException("hooker");
      this.m_Hooker = hooker;
    }

    ~BaseHookListener()
    {
      if (this.HookHandle == 0)
        return;
      Hooker.UnhookWindowsHookEx(this.HookHandle);
    }

    protected abstract bool ProcessCallback(int wParam, IntPtr lParam);

    protected int HookCallback(int nCode, int wParam, IntPtr lParam)
    {
      if (nCode == 0 && !this.ProcessCallback(wParam, lParam))
        return -1;
      else
        return this.CallNextHook(nCode, wParam, lParam);
    }

    private int CallNextHook(int nCode, int wParam, IntPtr lParam)
    {
      return Hooker.CallNextHookEx(this.HookHandle, nCode, wParam, lParam);
    }

    public void Start()
    {
      if (this.Enabled)
        throw new InvalidOperationException("Hook listener is already started. Call Stop() method firts or use Enabled property.");
      this.HookCallbackReferenceKeeper = new HookCallback(this.HookCallback);
      try
      {
        this.HookHandle = this.m_Hooker.Subscribe(this.GetHookId(), this.HookCallbackReferenceKeeper);
      }
      catch (Exception ex)
      {
        this.HookCallbackReferenceKeeper = (HookCallback) null;
        this.HookHandle = 0;
        throw;
      }
    }

    public void Stop()
    {
      try
      {
        this.m_Hooker.Unsubscribe(this.HookHandle);
      }
      finally
      {
        this.HookCallbackReferenceKeeper = (HookCallback) null;
        this.HookHandle = 0;
      }
    }

    public void Replace(Hooker hooker)
    {
      bool enabled = this.Enabled;
      this.Enabled = false;
      this.m_Hooker = hooker;
      this.Enabled = enabled;
    }

    protected abstract int GetHookId();

    public virtual void Dispose()
    {
      this.Stop();
    }
  }
}
