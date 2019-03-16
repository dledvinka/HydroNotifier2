using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Core
{
    public interface ILoggingService
    {
        void LogInformation(string message);
    }
}
