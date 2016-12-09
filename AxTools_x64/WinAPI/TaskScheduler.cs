using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AxTools.WinAPI
{
    internal static class TaskScheduler
    {

        internal static ScheduledTaskInfo GetTaskInfo(string taskName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/query /tn \"{0}\" /xml", taskName))
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    try
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        if (output.Contains("<?xml"))
                        {
                            ScheduledTaskInfo ret = new ScheduledTaskInfo {TaskName = taskName};
                            Match enabled = Regex.Match(output, "<Enabled>(.+)</Enabled>");
                            if (enabled.Success)
                            {
                                ret.Enabled = bool.Parse(enabled.Groups[1].Value);
                            }
                            Match taskToRun = Regex.Match(output, "<Command>(.*)</Command>");
                            if (taskToRun.Success)
                            {
                                ret.TaskToRun = taskToRun.Groups[1].Value;
                            }
                            return ret;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// XML file must be in Unicode (UTF-16 LE)
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="xmlPath"></param>
        internal static void CreateTask(string taskName, string xmlPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/create /f /tn \"{0}\" /xml \"{1}\"", taskName, xmlPath))
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(psi);
            if (process != null) process.WaitForExit();
        }

        internal static void DeleteTask(string taskName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/delete /f /tn \"{0}\"", taskName))
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(psi);
            if (process != null) process.WaitForExit();
        }
        
    }

    internal class ScheduledTaskInfo
    {
        internal string TaskName;
        internal bool Enabled;
        internal string TaskToRun;
    }
}
