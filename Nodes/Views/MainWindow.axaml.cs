using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Nodes.ViewModels;
using System;
using System.Threading.Tasks;
using System.Reactive;

namespace Nodes.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(d =>
            {
                d(ViewModel!.ShowFileOpenDialog.RegisterHandler(RunfFileOpenDialog));
                d(ViewModel!.ShowFileSaveDialog.RegisterHandler(RunfFileSaveDialog));
            });
        }

        private async Task RunfFileSaveDialog(InteractionContext<Unit, string?> interaction)
        {
            interaction.SetOutput(await new SaveFileDialog().ShowAsync(this));
        }

        private async Task RunfFileOpenDialog(InteractionContext<Unit, string?> interaction)
        {
            var files = await new OpenFileDialog().ShowAsync(this);
            string? fileName = null;
            if (files !=null && files.Length != 0)
            {
                fileName = files[0];
            }
            interaction.SetOutput(fileName);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
