using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using langley.gui.ViewModels;
using ReactiveUI;
#pragma warning disable 8602

namespace langley.gui.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(action => action(ViewModel.ShowFileOpenDialog.RegisterHandler(ShowOpenFileDialogAsync)));
        }

        private async Task ShowOpenFileDialogAsync(InteractionContext<List<FileDialogFilter>, string[]?> interaction)
        {
            var dialog = new OpenFileDialog
            {
                Filters = interaction.Input
            };
            var files = await dialog.ShowAsync(this);
            interaction.SetOutput(files);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
