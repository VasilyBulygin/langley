using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using langley;
using ReactiveUI;

namespace langley.gui.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        public FileViewModel(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    public class MainWindowViewModel : BaseViewModel
    {
        private static readonly List<FileDialogFilter> Filters = new()
        {
            new FileDialogFilter
            {
                Extensions = new List<string> {"cia"},
                Name = "CIA files"
            }
        };

        private FileViewModel? _selectedFile;
        private ReactiveCommand<Unit, Unit>? _addFileCommand;
        private ReactiveCommand<Unit, Unit>? _removeFileCommand;
        private ReactiveCommand<Unit, Unit>? _startLoadCommand;

        public MainWindowViewModel()
        {
            Settings = new ConnectionSettingsViewModel(new ConnectionSettings());
        }

        public ConnectionSettingsViewModel Settings { get; }

        public ObservableCollection<FileViewModel> Files { get; } = new();

        public FileViewModel? SelectedFile
        {
            get => _selectedFile;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
        }

        public Interaction<List<FileDialogFilter>, string[]?> ShowFileOpenDialog { get; } = new();

        #region Commands

        #region AddFileCommand

        public ReactiveCommand<Unit, Unit> AddFileCommand => _addFileCommand ??= ReactiveCommand.CreateFromTask(AddFileAsync);

        private async Task AddFileAsync()
        {
            var files = await ShowFileOpenDialog.Handle(Filters);
            if(files is null)
                return;
            Files.AddRange(files.Select(x=> new FileViewModel(x)));
        }

        #endregion

        #region RemoveFileCommand

        public ReactiveCommand<Unit, Unit> RemoveFileCommand =>
            _removeFileCommand ??= ReactiveCommand.Create(RemoveFile);

        private void RemoveFile()
        {
            if(_selectedFile is null)
                return;
            Files.Remove(_selectedFile);
        }

        #endregion

        #region StartLoadCommand

        public ReactiveCommand<Unit, Unit> StartLoadCommand => _startLoadCommand ??= ReactiveCommand.CreateFromTask(StartLoadAsync);

        private async Task StartLoadAsync()
        {
            using var server = new Server(Settings.Model);
            try
            {
                await server.StartAsync();
            }
            catch (Exception e)
            {
                //show error dialog
            }
            foreach (var fileViewModel in Files)
            { 
                await server.TrySendAsync(fileViewModel.Path, new Progress<int>());
            }
        }

        #endregion

        #endregion
    }
}
