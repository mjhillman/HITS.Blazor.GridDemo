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
    public partial class Index : ComponentBase, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// The view model for the product grid component
        /// </summary>
        private ProductGridViewModel _gridViewModel { get; set; }

        /// <summary>
        /// The view model for the product editor
        /// </summary>
        private ProductEditorViewModel _editorViewModel { get; set; }

        /// <summary>
        /// The model for the dialog component
        /// </summary>
        private DialogViewModel _dialogViewModel { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                //creating the view model will get the product data and grid parameters
                _gridViewModel = new ProductGridViewModel();

                _dialogViewModel = new DialogViewModel() { ConfirmButtonName = "Ok", ShowCancelButton = false };

                _editorViewModel = new ProductEditorViewModel();
            }
            catch (System.Exception ex)
            {
                _dialogViewModel.Show("Error", ex.Message, ex);
            }
            base.OnInitialized();
        }

        private void OnAddNewHandler(string message)
        {
            _dialogViewModel.Show("ADD NEW", "You clicked the add new button");
        }

        private void OnEditHandler(string productId)
        {
            //get a copy of the object to be edited
            _editorViewModel.ModelClone = _gridViewModel.ProductList.First(x => x.ProductId.AsString() == productId);

            //display the editor
            _editorViewModel.PopupVisible = true;
        }

        private void OnSaveHandler()
        {
            //get the grid model for the edited object
            ProductModel model = _gridViewModel.ProductList.First(x => x.ProductId == _editorViewModel.ModelClone.ProductId);
            
            //update the list with the edited object
            _editorViewModel.ModelClone.CopyTo(model);
            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _gridViewModel?.Dispose();
                    _editorViewModel?.Dispose();
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
