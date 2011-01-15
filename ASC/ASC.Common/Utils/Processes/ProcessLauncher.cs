using System;
using System.Diagnostics;
using System.IO;
using log4net;

namespace ASC.Common.Utils.Processes
{
    public class ProcessLauncher
    {
        private readonly ILog _log;
        public string PathToExe { get; set; }

        public ProcessLauncher(string pathToExe)
        {
            if (pathToExe == null) throw new ArgumentNullException("pathToExe");
            if (!File.Exists(pathToExe)) throw new FileNotFoundException("Executable not found", pathToExe);

            PathToExe = pathToExe;
            _log = LogHolder.Log("ASC.ProcessLauncher");
        }

        public int Start(CmdLine @params, TimeSpan waitTimeout, bool throwOnBadExitCode)
        {
            var psi = new ProcessStartInfo {ErrorDialog = false, CreateNoWindow = true, Arguments = @params.ToString(),FileName = PathToExe};
            int exitCode;
            Process ps = null;
            try
            {
                _log.DebugFormat("starting process");
                ps = Process.Start(psi);
                if (waitTimeout!=TimeSpan.Zero)
                {
                    ps.WaitForExit((int) waitTimeout.TotalMilliseconds);
                }
                else
                {
                    ps.WaitForExit();//W.o. timeout
                }
                //By that point process should be finished. if not - kill
                if (!ps.HasExited)
                {
                    try
                    {
                        //Kill
                        ps.Kill();
                    }
                    catch (Exception e)
                    {
                        _log.Error("kill failed",e);
                        throw;
                    }
                }
                exitCode = ps.ExitCode;
                if (exitCode!=0 && throwOnBadExitCode)
                {
                    throw new Exception("bad exit code");
                }
            }
            catch (Exception e)
            {
                _log.Error("process failed",e);
                throw;
            }
            finally
            {
                try
                {
                    if (ps!=null)
                    {
                        ps.Close();
                    }

                }
                catch
                {
                    
                }
            }
            return exitCode;
        }
    }
}