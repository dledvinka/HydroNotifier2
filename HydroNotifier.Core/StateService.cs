using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HydroNotifier.Core
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
