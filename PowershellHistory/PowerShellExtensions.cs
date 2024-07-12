using System.Diagnostics;

namespace Community.PowerToys.Run.Plugin.PowershellHistory
{
    internal class PowerShellExtensions
    {
        public static string RunPowerShellScript(string script)
        {
            // Create process
            Process process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Run process
            process.Start();

            // Get output
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Wait till it ends
            process.WaitForExit();

            // Check errors
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"PowerShell script error: {error}");
            }

            return output;
        }
    }
}
