using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Core
{
    public static class Convert
    {
        public static string StatusToText(HydroStatus status)
        {
            string stateName;

            switch (status)
            {
                case HydroStatus.Unknown:
                    stateName = "Neznamy";
                    break;
                case HydroStatus.Low:
                    stateName = "Nizky prutok";
                    break;
                case HydroStatus.Normal:
                    stateName = "Normalni prutok";
                    break;
                case HydroStatus.High:
                    stateName = "Vysoky prutok";
                    break;
                default:
                    stateName = "Neznamy";
                    break;
            }

            return stateName;
        }
    }
}
