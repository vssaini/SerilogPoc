using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Exceptions;

namespace SerilogPoc
{
    public class SerilogConfig
    {
        public static void EnableLogging()
        {
            EnableSerilogSelfLogging();

            const string applicationName = "Serilog POC";

            // Ref - https://ml-software.ch/posts/writing-to-azure-diagnostics-log-stream-using-serilog
            var logPath = $@"D:\home\LogFiles\Application\{applicationName}.txt";

#if DEBUG
            logPath = HostingEnvironment.MapPath("~/Logs/Log-.txt");
#endif

            Log.Logger = new LoggerConfiguration()

                // Ref - https://github.com/serilog/serilog-settings-appsettings
                .ReadFrom.AppSettings()

                // ReSharper disable once AssignNullToNotNullAttribute
                .WriteTo.File(logPath,
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}", rollingInterval: RollingInterval.Day)

                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", applicationName)

                .Enrich.WithMvcControllerName()
                .Enrich.WithMvcActionName()

                // Ref - https://github.com/serilog-web/classic-mvc
                .Enrich.WithMvcRouteData()
                .Enrich.WithUserName()

                // Ref - https://github.com/serilog/serilog-enrichers-environment`
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()

                // Ref - https://github.com/serilog-contrib/serilog-enrichers-clientinfo
                .Enrich.WithClientIp()

                .CreateLogger();

            Log.Information("Application starting up");
        }

        // ReSharper disable once UnusedMember.Local
        private static void EnableSerilogSelfLogging()
        {
            try
            {
                var errorFilePath = $"{Directory.GetCurrentDirectory()}\\Serilog\\SerilogInternalErrors.log";

                // Ref - https://arefinblog.wordpress.com/2011/06/20/thread-safe-streamwriter/
                SelfLog.Enable(TextWriter.Synchronized(File.AppendText(errorFilePath)));
            }
            catch (Exception ex)
            {
                // Error can be seen from Azure Log Stream
                Trace.TraceError(ex.Message);
            }
        }
    }
}