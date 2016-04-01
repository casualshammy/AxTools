using System;

namespace Components.TaskbarProgressbar
{
    public class TBProgressBar
    {
        private static readonly object LockObject = new object();
        
        private static ITaskbarList3 _taskbarList;

        private static ITaskbarList3 TaskbarList
        {
            get
            {
                if (_taskbarList == null)
                {
                    lock (LockObject)
                    {
                        if (_taskbarList == null)
                        {
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            _taskbarList = (ITaskbarList3) new CTaskbarList();
                            _taskbarList.HrInit();
                        }
                    }
                }
                return _taskbarList;
            }
        }

        public static void SetProgressState(IntPtr hwnd, ThumbnailProgressState state)
        {
            TaskbarList.SetProgressState(hwnd, state);
        }

        public static void SetProgressValue(IntPtr hwnd, ulong current, ulong maximum)
        {
            TaskbarList.SetProgressValue(hwnd, current, maximum);
        }

        public static void SetProgressValue(IntPtr hwnd, int current, int maximum)
        {
            TaskbarList.SetProgressValue(hwnd, (ulong) current, (ulong) maximum);
        }
    }
}
