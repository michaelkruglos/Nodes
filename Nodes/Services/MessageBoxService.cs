using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Nodes.Views;
using ReactiveUI;

namespace Nodes.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private readonly Window _parentWindow;

        public MessageBoxService(Window parentWindow)
        {
            _parentWindow = parentWindow;
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            var window = new MessageBox();
            window.Title = title;
            ICommand close = ReactiveCommand.Create(() => window.Close());
            window.DataContext = new { Message = message, Close = close };
            await window.ShowDialog(_parentWindow);
        }
    }
}
