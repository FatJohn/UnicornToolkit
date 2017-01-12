using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;

namespace Unicorn
{
    /// <summary>
    /// MessageDialog extension methods
    /// </summary>
    public static class MessageDialogExtensions
    {
        private static TaskCompletionSource<MessageDialog> currentDialogShowRequest;

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// If another dialog is already shown using
        /// ShowAsyncQueue or ShowAsyncIfPossible method - it will wait
        /// for that previous dialog to be dismissed before showing the new one.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">This method can only be invoked from UI thread.</exception>
        public static async Task ShowAsyncQueue(this MessageDialog dialog, Action<IUICommand> callback = null)
        {
            if (!CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
#if DEBUG
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
#else
                return;
#endif
            }

            while (currentDialogShowRequest != null)
            {
                await currentDialogShowRequest.Task;
            }

            var request = currentDialogShowRequest = new TaskCompletionSource<MessageDialog>();
            var uiCommand = await dialog.ShowAsync();
            callback?.Invoke(uiCommand);

            currentDialogShowRequest = null;
            request.SetResult(dialog);
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// If another dialog is already shown using
        /// ShowAsyncQueue or ShowAsyncIfPossible method - it will wait
        /// return immediately and the new dialog won't be displayed.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">This method can only be invoked from UI thread.</exception>
        public static async Task ShowAsyncIfPossible(this MessageDialog dialog)
        {
            if (!CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
#if DEBUG
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
#else
                return;
#endif
            }

            while (currentDialogShowRequest != null)
            {
                return;
            }

            var request = currentDialogShowRequest = new TaskCompletionSource<MessageDialog>();
            await dialog.ShowAsync();
            currentDialogShowRequest = null;
            request.SetResult(dialog);
        }
    }
}
