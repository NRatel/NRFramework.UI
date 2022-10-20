// 注意：
// fileName 和 workingDir 最好用 Path.GetFullPath() 处理一下，
// 可以避免由路径错误导致的各种问题。

using UnityEditor;
using System.Diagnostics;
using System.Text;

namespace NRFramework
{
    public class ProcessRunner : Editor
    {
        static public string RunPing(string url)
        {
            return RunFile("ping.exe", new string[] { url });
        }

        static public string RunCmd(string[] cmds, string workingDir = "")
        {
            return Run("cmd.exe", null, cmds, workingDir);
        }

        static public string RunFile(string fileName, string[] arguments, string workingDir = "")
        {
            return Run(fileName, arguments, null, workingDir);
        }

        static private string Run(string fileName, string[] arguments, string[] inputs, string workingDir = "")
        {
            Process p = CreateProcess(fileName, arguments != null ? string.Join(" ", arguments) : "", workingDir);
            
            if (inputs != null)
            {
                p.StandardInput.AutoFlush = true;
                foreach (string input in inputs)
                {
                    p.StandardInput.WriteLine(input);
                }
                p.StandardInput.WriteLine("exit");
            }

            p.WaitForExit();

            string standardOutput = p.StandardOutput.ReadToEnd();
            string standardError = p.StandardError.ReadToEnd();
            string exitCode = p.ExitCode.ToString();

            p.Close();

            return "exitCode: " + exitCode + "\nstandardError: " + standardError + "\nstandardOutput: " + standardOutput;
        }

        static private Process CreateProcess(string fileName, string arguments, string workingDir)
        {
            //System.Console.InputEncoding = Encoding.UTF8; //这句影响首字乱码，但不确定什么情况要加什么情况不要加。。
            Encoding outPutEncoding = Encoding.GetEncoding("GB2312"); //Application.platform == RuntimePlatform.WindowsEditor

            ProcessStartInfo pStartInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = outPutEncoding,
                StandardOutputEncoding = outPutEncoding,
            };
            return Process.Start(pStartInfo);
        }
    }
}

