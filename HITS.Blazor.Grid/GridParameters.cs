using System;
using System.Collections.Generic;

namespace HITS.Blazor.Grid
{
    public class GridParameters<T> : IDisposable
    {
        private bool disposedValue;

        public enum SummaryTypeEnum { Count, Sum, Average };

        public string GridTitle { get; set; } = "List";

        public List<T> DataList { get; set; }

        public bool ShowEditColumn { get; set; } = false;

        public bool ShowFilterRow { get; set; } = false;

        public bool ShowPager { get; set; } = false;

        public bool ShowAddNew { get; set; } = false;

        public int PageSize { get; set; } = 5;

        public string PrimaryKeyName { get; set; }

        public string EditButtonText { get; set; } = "Edit";

        public string BootstrapContainerClass { get; set; } = "container-fluid";

        public string BootstrapColumnClass { get; set; } = "col-md";

        public string SummaryColumnName { get; set; }

        public string SummaryTitle { get; set; }
        
        public SummaryTypeEnum SummaryType { get; set; }
        
        public string SummaryFormat { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    DataList?.Clear();
                    DataList = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GridParameters()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
