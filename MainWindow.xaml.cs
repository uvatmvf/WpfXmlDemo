using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using WpfXmlDemo.ViewModels;

namespace WpfXmlDemo
{
    /// <summary>
    /// Interaction logic for MainWIndow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Current expander being built
        /// </summary>
        public SimpleDataNodeView buildingStack = new SimpleDataNodeView();

        /// <summary>
        /// Root node of the current building stack
        /// </summary>
        public SimpleDataNodeView rootBuilding = null;

        /// <summary>
        /// All xml files that are in current root folder directories
        /// </summary>
        public ObservableCollection<String> MyFileList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The current xml file opened
        /// </summary>
        public XmlDocument xmlDoc;

        /// <summary>
        /// Name of currently opened template file
        /// </summary>
        public string templateFileName { get; set;} = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            FileListViewModel xmlFiles = new FileListViewModel(@"DataFiles/", "*.xml");
            this.DataContext = xmlFiles; // breaking MVVM here, ViewModel is not used as DataContext
            xmlFiles.Search();
            xmlFiles.results.ForEach(a => MyFileList.Add(a));
            templateFileName = MyFileList[0];
            LoadXml();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            templateFileName = (sender as Button).Content.ToString();
            LoadXml();
            MyTemplateView.Visibility = Visibility.Visible;
        }

        public void LoadXml()
        {
            foreach (string tFileName in MyFileList)
            {
                try
                {                    
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(File.ReadAllText(tFileName));
                    templateFileName = tFileName;
                    rootBuilding = new SimpleDataNodeView()
                    {
                        Name = Path.GetFileName(templateFileName).Replace(".xml", string.Empty),
                        Tag = templateFileName, 
                        BorderThickness = new Thickness(3),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2543FF"))
                    };
                    rootBuilding.expDesc.Header = templateFileName.ToUpper();
                    rootBuilding.expDesc.Background = rootBuilding.BorderBrush;
                    rootBuilding.expDesc.IsExpanded = false;                    
                    stkElements.Children.Add(rootBuilding);
                    buildingStack = rootBuilding;
                    BuildUIOnXml(xmlDoc.FirstChild, rootBuilding);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("Configurator ate error reading xml file {0}\n{1}", tFileName, ex.ToString()));
                }
                finally
                {
                    rootBuilding = null;
                }
            }
        }

        private void BuildUIOnXml(XmlNode doc, SimpleDataNodeView ui)
        {
            if (doc.ChildNodes.Count > 1 || (doc.ChildNodes.Count == 1 && doc.ChildNodes[0].ChildNodes.Count > 0) )
            {
                SimpleDataNodeView uiNode = ui.BuildNode(doc.Name.ToUpper(), templateFileName);
                foreach (XmlNode child in doc.ChildNodes)
                {
                    BuildUIOnXml(child, uiNode); // recurse
                }
            }
            else
            {
                string text = (doc.Attributes.GetNamedItem("description") == null ? doc.Name.ToUpper() : doc.Attributes.GetNamedItem("description").Value.ToUpper());
                ui.BuildLabelText(doc, text, doc.InnerText, doc.Name.ToUpper());
            }
        }        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // move to view model impelmentation.
            foreach (FrameworkElement templateFileElement in stkElements.Children)
            {
                Console.WriteLine(string.Format("I am a fileURI:{0}", templateFileElement.Tag.ToString()));
                if (MyFileList.Contains(templateFileElement.Tag.ToString()))
                {
                    templateFileName = templateFileElement.Tag.ToString();
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(File.ReadAllText(templateFileName));
                    Dictionary<string, object> propertyValues = (templateFileElement as SimpleDataNodeView).TextBoxToDict();
                    XElement templateXml = XElement.Load(templateFileName);
                    foreach (FrameworkElement section in (templateFileElement as SimpleDataNodeView).NodeGrid.Children)
                    {
                        // each loop will grab a different section of xml file
                        if (section is SimpleDataNodeView)
                        {
                            Console.WriteLine(string.Format("I am a section:{0}", section.Name));
                            var query = from el in templateXml.Elements(section.Name)
                                        where el.Name == section.Name
                                        select el;
                                        
                            foreach (XmlNode xmlClass in xmlDoc.ChildNodes)
                            {
                                if (xmlClass.Name.ToUpper() == section.Name)
                                {
                                    foreach (XmlNode xmlObject in xmlClass.ChildNodes)
                                    {
                                        if (xmlObject.ChildNodes.Count == 1)
                                        {
                                            xmlObject.InnerText = (propertyValues[section.Name] as Dictionary<string, object>)[xmlObject.Name.ToUpper()].ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                string fileName = string.Format("{0}{1}", templateFileName, ".xml");
                xmlDoc.Save(fileName);
            }
        }

        private void SaveCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException("Move this command binding to a viewmodel!");
        }
    }
}