using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nodes.Models;
using Nodes.Services;
using Nodes.Utils;
using ReactiveUI;

namespace Nodes.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private Graph _graph;
        public readonly Interaction<Unit, string?> ShowFileOpenDialog;
        public readonly Interaction<Unit, string?> ShowFileSaveDialog;
        private readonly IMessageBoxService _msgbox;

        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }

        public MainWindowViewModel(IMessageBoxService msgbox)
        {
            _msgbox = msgbox;
            _graph = new Graph();
            ShowFileOpenDialog = new Interaction<Unit, string?>();
            ShowFileSaveDialog = new Interaction<Unit, string?>();
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileName = await ShowFileOpenDialog.Handle(Unit.Default);
                if (fileName != null)
                {
                    var g = await LoadGraphFromJsonFile(fileName);
                    if (g != null)
                    {
                        Graph = g;
                    }
                }
            });
            SaveFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileName = await ShowFileSaveDialog.Handle(Unit.Default);
                if (fileName != null)
                {
                    await SaveGraphToJsonFile(fileName);
                }
            });
        }

        public Graph Graph 
        { 
            get => _graph;
            set
            {
                this.RaiseAndSetIfChanged(ref _graph, value);
            }
        }

        private async Task<Graph?> LoadGraphFromJsonFile(string fileName)
        {
            try
            {
                using var file = File.OpenRead(fileName);
                using var stream = new StreamReader(file);
                using var jsonReader = new JsonTextReader(stream);
                var serializer = new JsonSerializer { ContractResolver = new GraphContractResolver() };
                var json = await JObject.LoadAsync(jsonReader);
                return json.ToObject<Graph?>(serializer);
            }
            catch(Exception e)
            {
                await _msgbox.ShowMessageAsync($"Error loading graph from {fileName}", e.Message);
            }
            return null;
        }

        private async Task SaveGraphToJsonFile(string fileName)
        {
            try
            {
                using var file = File.OpenWrite(fileName);
                using var stream = new StreamWriter(file);
                using var jsonWriter = new JsonTextWriter(stream);
                var serializer = new JsonSerializer { ContractResolver = new GraphContractResolver() };
                var json = JObject.FromObject(Graph, serializer);
                await json.WriteToAsync(jsonWriter);
            }
            catch (Exception e)
            {
                await _msgbox.ShowMessageAsync($"Error saving graph to {fileName}", e.Message);
            }
        }

    }
}
