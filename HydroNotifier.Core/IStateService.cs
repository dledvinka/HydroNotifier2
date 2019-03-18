using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Core
{
    public interface IStateService
    {
        HydroStatus GetStatus();
        void SetStatus(HydroStatus status);
    }
}
