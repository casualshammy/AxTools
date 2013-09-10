// Type: MouseKeyboardActivityMonitor.KeyboardHookListener
// Assembly: MouseKeyboardActivityMonitor, Version=3.0.1.9579, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Axio\Desktop\MouseKeyboardActivityMonitor.dll

using MouseKeyboardActivityMonitor.WinApi;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MouseKeyboardActivityMonitor
{
  public class KeyboardHookListener : BaseHookListener
  {
    private KeyEventHandler KeyDown;
    private KeyPressEventHandler KeyPress;
    private KeyEventHandler KeyUp;

    public event KeyEventHandler KeyDown
    {
      add
      {
        KeyEventHandler keyEventHandler = this.KeyDown;
        KeyEventHandler comparand;
        do
        {
          comparand = keyEventHandler;
          keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyDown, comparand + value, comparand);
        }
        while (keyEventHandler != comparand);
      }
      remove
      {
        KeyEventHandler keyEventHandler = this.KeyDown;
        KeyEventHandler comparand;
        do
        {
          comparand = keyEventHandler;
          keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyDown, comparand - value, comparand);
        }
        while (keyEventHandler != comparand);
      }
    }

    public event KeyPressEventHandler KeyPress
    {
      add
      {
        KeyPressEventHandler pressEventHandler = this.KeyPress;
        KeyPressEventHandler comparand;
        do
        {
          comparand = pressEventHandler;
          pressEventHandler = Interlocked.CompareExchange<KeyPressEventHandler>(ref this.KeyPress, comparand + value, comparand);
        }
        while (pressEventHandler != comparand);
      }
      remove
      {
        KeyPressEventHandler pressEventHandler = this.KeyPress;
        KeyPressEventHandler comparand;
        do
        {
          comparand = pressEventHandler;
          pressEventHandler = Interlocked.CompareExchange<KeyPressEventHandler>(ref this.KeyPress, comparand - value, comparand);
        }
        while (pressEventHandler != comparand);
      }
    }

    public event KeyEventHandler KeyUp
    {
      add
      {
        KeyEventHandler keyEventHandler = this.KeyUp;
        KeyEventHandler comparand;
        do
        {
          comparand = keyEventHandler;
          keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyUp, comparand + value, comparand);
        }
        while (keyEventHandler != comparand);
      }
      remove
      {
        KeyEventHandler keyEventHandler = this.KeyUp;
        KeyEventHandler comparand;
        do
        {
          comparand = keyEventHandler;
          keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyUp, comparand - value, comparand);
        }
        while (keyEventHandler != comparand);
      }
    }

    public KeyboardHookListener(Hooker hooker)
      : base(hooker)
    {
    }

    protected override bool ProcessCallback(int wParam, IntPtr lParam)
    {
      KeyEventArgsExt e = KeyEventArgsExt.FromRawData(wParam, lParam, this.IsGlobal);
      this.InvokeKeyDown(e);
      this.InvokeKeyPress(wParam, lParam);
      this.InvokeKeyUp(e);
      return !e.Handled;
    }

    protected override int GetHookId()
    {
      return !this.IsGlobal ? 2 : 13;
    }

    private void InvokeKeyDown(KeyEventArgsExt e)
    {
      KeyEventHandler keyEventHandler = this.KeyDown;
      if (keyEventHandler == null || e.Handled || !e.IsKeyDown)
        return;
      keyEventHandler((object) this, (KeyEventArgs) e);
    }

    private void InvokeKeyPress(int wParam, IntPtr lParam)
    {
      this.InvokeKeyPress(KeyPressEventArgsExt.FromRawData(wParam, lParam, this.IsGlobal));
    }

    private void InvokeKeyPress(KeyPressEventArgsExt e)
    {
      KeyPressEventHandler pressEventHandler = this.KeyPress;
      if (pressEventHandler == null || e.Handled || e.IsNonChar)
        return;
      pressEventHandler((object) this, (KeyPressEventArgs) e);
    }

    private void InvokeKeyUp(KeyEventArgsExt e)
    {
      KeyEventHandler keyEventHandler = this.KeyUp;
      if (keyEventHandler == null || e.Handled || !e.IsKeyUp)
        return;
      keyEventHandler((object) this, (KeyEventArgs) e);
    }

    public override void Dispose()
    {
      this.KeyPress = (KeyPressEventHandler) null;
      this.KeyDown = (KeyEventHandler) null;
      this.KeyUp = (KeyEventHandler) null;
      base.Dispose();
    }
  }
}
