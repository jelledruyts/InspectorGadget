using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace InspectorGadget.WebApp.Infrastructure
{
    /// <summary>
    /// Provides methods to run processes and capture output in a safe way.
    /// </summary>
    public static class ProcessRunner
    {
        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">The filename of the process that was started.</param>
        /// <param name="arguments">The command-line arguments that were used when starting the process.</param>
        /// <param name="timeout">The timeout to wait for the process to exit; if a timeout is specified, output may be truncated even if the process exited.</param>
        /// <returns>The result of the process that has run.</returns>
        public static ProcessRunResult RunProcess(string fileName, string arguments, TimeSpan? timeout, bool captureStandardOutput, bool captureErrorOutput)
        {
            var standardOutputBuilder = new StringBuilder();
            var standardErrorBuilder = new StringBuilder();
            var outputDataReceivedHandler = (DataReceivedEventHandler)((sender, e) =>
            {
                if (standardOutputBuilder.Length > 0) { standardOutputBuilder.AppendLine(); }
                standardOutputBuilder.Append(e.Data);
            });
            var errorDataReceivedHandler = (DataReceivedEventHandler)((sender, e) =>
            {
                if (standardErrorBuilder.Length > 0) { standardErrorBuilder.AppendLine(); }
                standardErrorBuilder.Append(e.Data);
            });

            // Set up the process.
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false, // Do no use a graphical shell; this also allows the output and error streams to be captured.
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = captureStandardOutput,
                RedirectStandardError = captureErrorOutput
            };
            if (captureStandardOutput)
            {
                process.OutputDataReceived += outputDataReceivedHandler;
            }
            if (captureErrorOutput)
            {
                process.ErrorDataReceived += errorDataReceivedHandler;
            }

            // Start the process and start listening to the redirected streams asynchronously.
            // See https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput.
            process.Start();
            if (captureStandardOutput)
            {
                process.BeginOutputReadLine();
            }
            if (captureErrorOutput)
            {
                process.BeginErrorReadLine();
            }

            // Wait for the process to exit.
            var exitCode = default(int?);
            var exited = false;
            if (timeout.HasValue)
            {
                var timeoutMilliseconds = timeout.Value.TotalMilliseconds > int.MaxValue ? int.MaxValue : (int)timeout.Value.TotalMilliseconds;
                exited = process.WaitForExit(timeoutMilliseconds);
            }
            else
            {
                process.WaitForExit();
                exited = true;
            }
            if (exited)
            {
                exitCode = process.ExitCode;
            }

            // Remove event handlers to avoid that data is being written asynchronously
            // to the StringBuilders while they are being read from this thread.
            if (captureStandardOutput)
            {
                process.OutputDataReceived -= outputDataReceivedHandler;
            }
            if (captureErrorOutput)
            {
                process.ErrorDataReceived -= errorDataReceivedHandler;
            }

            // Aggregate all information.
            var standardOutput = captureStandardOutput ? standardOutputBuilder.ToString() : null;
            var errorOutput = captureErrorOutput ? standardErrorBuilder.ToString() : null;
            return new ProcessRunResult(fileName, arguments, timeout, exited, exitCode, standardOutput, errorOutput);
        }
    }
}