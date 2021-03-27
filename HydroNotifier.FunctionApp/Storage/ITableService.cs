using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;

namespace HydroNotifier.FunctionApp.Storage
{
    public interface ITableService
    {
        void AddEntity<T>(T tableEntity) where T : class, ITableEntity, new();
    }
}
