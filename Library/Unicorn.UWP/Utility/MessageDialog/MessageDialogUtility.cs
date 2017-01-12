// Copyright (c) 2016 John Shu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace Unicorn
{
    public class MessageDialogStrings
    {
        public string DefaultTitle { get; set; }

        public string OK { get; set; }

        public string Cancel { get; set; }

        public string Yes { get; set; }

        public string No { get; set; }
    }

    public static class MessageDialogUtility
    {
        private static IUICommand dialogResult;
        private static MessageDialogStrings dialogStrings;

        public static void InitialStrings(MessageDialogStrings messageDialogStrings)
        {
            dialogStrings = messageDialogStrings;
        }

        public static void Show(string message)
        {
            var commands = CreateCommands(MessageBoxButton.OK);
            var messageDialog = CreateMessageDialog(string.Empty, message, commands);
#pragma warning disable 4014
            messageDialog.ShowAsyncQueue();
#pragma warning restore 4014
        }

        public static Task<MessageBoxResult> ShowAsync(string message)
        {
            string title = dialogStrings == null ? string.Empty : dialogStrings.DefaultTitle;
            return ShowAsync(title, message, MessageBoxButton.OK);
        }

        public static async Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButton button)
        {
            var commands = CreateCommands(button);
            var dialogResult = await ShowAsync(title, message, commands);
            return (MessageBoxResult)dialogResult.Id;
        }

        public static async Task<MessageDialogResult> ShowAsync(string title, string message, object userdata, MessageBoxButton button, UICommandInvokedHandler action = null)
        {
            var commands = CreateCommands(userdata, button, action);
            var dialogResult = await ShowAsync(title, message, commands);
            return dialogResult.Id as MessageDialogResult;
        }

        private static IEnumerable<IUICommand> CreateCommands(MessageBoxButton button)
        {
            var commands = new List<IUICommand>();

            var resourceLoader = ResourceLoader.GetForCurrentView();

            if (button.HasFlag(MessageBoxButton.OK))
            {
                var okString = dialogStrings == null ? resourceLoader.GetString("OK") : dialogStrings.OK;
                commands.Add(new UICommand(okString, null, MessageBoxResult.OK));
            }

            if (button.HasFlag(MessageBoxButton.Yes))
            {
                var yesString = dialogStrings == null ? resourceLoader.GetString("YES") : dialogStrings.Yes;
                commands.Add(new UICommand(yesString, null, MessageBoxResult.Yes));
            }

            if (button.HasFlag(MessageBoxButton.No))
            {
                var noString = dialogStrings == null ? resourceLoader.GetString("NO") : dialogStrings.No;
                commands.Add(new UICommand(noString, null, MessageBoxResult.No));
            }
            if (button.HasFlag(MessageBoxButton.Cancel))
            {
                var cancelString = dialogStrings == null ? resourceLoader.GetString("CANCEL") : dialogStrings.Cancel;
                commands.Add(new UICommand(cancelString, null, MessageBoxResult.Cancel));
            }

            return commands;
        }

        private static IEnumerable<IUICommand> CreateCommands(object userdata, MessageBoxButton button, UICommandInvokedHandler action = null)
        {
            var commands = new List<IUICommand>();

            var resourceLoader = ResourceLoader.GetForCurrentView();

            if (button.HasFlag(MessageBoxButton.OK))
            {
                var okString = dialogStrings == null ? resourceLoader.GetString("OK") : dialogStrings.OK;
                commands.Add(new UICommand(okString, action, new MessageDialogResult(MessageBoxResult.OK, userdata)));
            }

            if (button.HasFlag(MessageBoxButton.Yes))
            {
                var yesString = dialogStrings == null ? resourceLoader.GetString("YES") : dialogStrings.Yes;
                commands.Add(new UICommand(yesString, action, new MessageDialogResult(MessageBoxResult.Yes, userdata)));
            }

            if (button.HasFlag(MessageBoxButton.No))
            {
                var noString = dialogStrings == null ? resourceLoader.GetString("NO") : dialogStrings.No;
                commands.Add(new UICommand(noString, action, new MessageDialogResult(MessageBoxResult.No, userdata)));
            }
            if (button.HasFlag(MessageBoxButton.Cancel))
            {
                var cancelString = dialogStrings == null ? resourceLoader.GetString("CANCEL") : dialogStrings.Cancel;
                commands.Add(new UICommand(cancelString, action, new MessageDialogResult(MessageBoxResult.Cancel, userdata)));
            }

            return commands;
        }

        public static void Show(string title, string message, IEnumerable<IUICommand> commands = null)
        {
            var messageDialog = CreateMessageDialog(title, message, commands);
#pragma warning disable 4014
            messageDialog.ShowAsyncQueue();
#pragma warning restore 4014
        }

        public static async Task<IUICommand> ShowAsync(string title, string message, IEnumerable<IUICommand> commands = null)
        {
            var messageDialog = CreateMessageDialog(title, message, commands);
            await messageDialog.ShowAsyncQueue(DialogCallback);
            return dialogResult;
        }

        private static void DialogCallback(IUICommand dialogResult)
        {
            MessageDialogUtility.dialogResult = dialogResult;
        }

        public static MessageDialog CreateMessageDialog(string title, string message, IEnumerable<IUICommand> commands = null)
        {
            var messageDialog = new MessageDialog(message, title);
            if (commands != null)
            {
                foreach (var uiCommand in commands)
                {
                    messageDialog.Commands.Add(uiCommand);
                }
            }
            return messageDialog;
        }
    }
}
