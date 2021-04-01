using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.FunctionApp.Storage
{
    public interface ITableService
    {
        void AddEntity<T>(T tableEntity);
    }
}
