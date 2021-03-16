using System;

namespace HITS.Blazor.GridDemo
{
    public class DialogViewModel : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// bind value for pop-up visibility
        /// </summary>
        public bool PopupVisible { get; set; } = false;

        /// <summary>
        /// bind value for dialog title
        /// </summary>
        public string DialogTitle { get; set; }

        /// <summary>
        /// bind value for dialog message
        /// </summary>
        public string DialogMessage { get; set; }

        public string BackgroundColor { get; set; }

        /// <summary>
        /// name of the button used to close dialog
        /// </summary>
        public string ButtonName { get; set; }

        public string ConfirmButtonName { get; set; }

        public string CancelButtonName { get; set; }

        public bool ShowCancelButton { get; set; }

        public DialogViewModel()
        {

        }

        public DialogViewModel(bool popupVisible, string dialogTitle, string dialogMessage)
        {
            PopupVisible = popupVisible;
            DialogTitle = dialogTitle;
            DialogMessage = dialogMessage;
        }

        public void Show(string dialogTitle, string dialogMessage, Exception ex = null)
        {
            PopupVisible = true;
            DialogTitle = dialogTitle;
            DialogMessage = dialogMessage;
            ButtonName = string.Empty;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
