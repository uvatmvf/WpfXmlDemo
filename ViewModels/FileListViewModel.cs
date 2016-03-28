using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfXmlDemo.ViewModels
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
    }
}
