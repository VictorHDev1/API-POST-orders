using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Logging
{
    public static class SerilogConfiguration
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/orders-.log",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();
        }
    }
}
