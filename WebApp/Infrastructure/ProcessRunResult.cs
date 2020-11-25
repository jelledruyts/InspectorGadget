using System;

namespace InspectorGadget.WebApp.Infrastructure
{
    /// <summary>
    /// Represents the result of a process that has run.
    /// </summary>
    public class ProcessRunResult
    {
        #region Properties

        /// <summary>
        /// Gets the original filename of the process that was started.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the original command-line arguments that were used when starting the process.
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets the original timeout to wait for the process to exit.
        /// </summary>
        public TimeSpan? Timeout { get; private set; }

        /// <summary>
        /// Gets a value that determines if the process had exited before the timeout was reached.
        /// </summary>
        public bool Exited { get; private set; }

        /// <summary>
        /// Gets the exit code of the process (if it exited, otherwise <see langword="null"/>).
        /// </summary>
        public int? ExitCode { get; private set; }

        /// <summary>
        /// Gets the standard output of the process, or <see langword="null"/> if no standard output was requested to be captured.
        /// </summary>
        public string StandardOutput { get; private set; }

        /// <summary>
        /// Gets the standard error output of the process, or <see langword="null"/> if no error output was requested to be captured.
        /// </summary>
        public string StandardError { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ProcessRunResult"/> instance.
        /// </summary>
        /// <param name="fileName">The original filename of the process that was started.</param>
        /// <param name="arguments">The original command-line arguments that were used when starting the process.</param>
        /// <param name="exitCode">The exit code of the process.</param>
        /// <param name="standardOutput">The standard output of the process.</param>
        /// <param name="standardError">The standard error output of the process.</param>
        public ProcessRunResult(string fileName, string arguments, int exitCode, string standardOutput, string standardError)
            : this(fileName, arguments, null, true, exitCode, standardOutput, standardError)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ProcessRunResult"/> instance.
        /// </summary>
        /// <param name="fileName">The original filename of the process that was started.</param>
        /// <param name="arguments">The original command-line arguments that were used when starting the process.</param>
        /// <param name="timeout">The original timeout to wait for the process to exit.</param>
        /// <param name="exited">A value that determines if the process had exited before the timeout was reached.</param>
        /// <param name="exitCode">The exit code of the process (if it exited, otherwise <see langword="null"/>).</param>
        /// <param name="standardOutput">The standard output of the process.</param>
        /// <param name="standardError">The standard error output of the process.</param>
        public ProcessRunResult(string fileName, string arguments, TimeSpan? timeout, bool exited, int? exitCode, string standardOutput, string standardError)
        {
            this.FileName = fileName;
            this.Arguments = arguments;
            this.Timeout = timeout;
            this.Exited = exited;
            this.ExitCode = exitCode;
            this.StandardOutput = standardOutput;
            this.StandardError = standardError;
        }

        #endregion
    }
}