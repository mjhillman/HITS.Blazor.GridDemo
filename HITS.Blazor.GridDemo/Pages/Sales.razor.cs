using HITS.Blazor.Grid;
using HITS.LIB.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace HITS.Blazor.GridDemo.Pages
{
    /// <summary>
    /// This class is the home page.
    /// </summary>
    public partial class Sales : ComponentBase, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// The view model for the product grid component
        /// </summary>
        private SalesGridViewModel _gridViewModel { get; set; }

        /// <summary>
        /// The model for the dialog component
        /// </summary>
        private DialogViewModel _dialogViewModel { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                //creating the view model will get the product data and grid parameters
                _gridViewModel = new SalesGridViewModel();

                _dialogViewModel = new DialogViewModel() { ConfirmButtonName = "Ok", ShowCancelButton = false };
            }
            catch (System.Exception ex)
            {
                _dialogViewModel.Show("Error", ex.Message, ex);
            }
            base.OnInitialized();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _gridViewModel?.Dispose();
                    _dialogViewModel?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Index()
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
