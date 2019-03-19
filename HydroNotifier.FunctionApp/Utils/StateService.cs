using System;
using System.IO;
using HydroNotifier.FunctionApp.Core;

namespace HydroNotifier.FunctionApp.Utils
{
    public class StateService : IStateService
    {
        private string _path;

        public StateService(string path)
        {
            _path = path;
        }

        public HydroStatus GetStatus()
        {
            var fullPath = Path.Combine(_path, "HydroNotifierStatus.dat");

            if (File.Exists(fullPath))
            {
                string content = File.ReadAllText(fullPath);
                if (Enum.TryParse<HydroStatus>(content, out HydroStatus status))
                {
                    return status;
                }
            }

            return HydroStatus.Unknown;
        }

        public void SetStatus(HydroStatus status)
        {
            var fullPath = Path.Combine(_path, "HydroNotifierStatus.dat");

            File.WriteAllText(fullPath, status.ToString());
        }
    }
}
