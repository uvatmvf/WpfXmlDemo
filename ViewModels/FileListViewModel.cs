using DemoConfig.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DemoConfig.ViewModels
{
    public class FileListViewModel 
    {
        public FileListViewModel(string rootPath, string extensionFilter)
        {
            ExtensionFilter = extensionFilter;
            contextRootPath = rootPath;
            Search();
        }
        
        public string ExtensionFilter
        {
            get; set;
        }

        public string contextRootPath; // default context root path

        public List<string> results = new List<string>(); // list of files found in root folder recursive directory search 
                                                                 // for files of extension  
        public void Search()
        {
            results = Directory.EnumerateFiles(contextRootPath, ExtensionFilter, SearchOption.AllDirectories).ToList<string>();
        }

        public void Save()
        {
            throw new NotImplementedException("Bind to me from a command!");
        }

        public bool CanSave
        {
            get
            {
                // todo: implement a dirty bit for the xml file contents
                return true;
            }
        }

        RelayCommand _saveCommand; 
        public ICommand SaveCommand 
        { 
            get 
            { 
                if (_saveCommand == null) 
                { 
                    _saveCommand = new RelayCommand(param => this.Save(), param => this.CanSave); 
                } 
                return _saveCommand; 
            } 
        }
    }
}
