using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace WpfXmlDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class XmlTemplateView : UserControl
    {
       
        public XmlTemplateView()
        {
            InitializeComponent();
           
            DataContext = this;
        }

        public XmlDocument doc;
        public string templateFile {
            get; set;
        }
         
        public SimpleDataNodeView expanderBuilding; // current expander being built

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            LoadXml();
        }

        public void RecurseLoadXml(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {


                if (node.ChildNodes.Count > 1)
                {
                    expanderBuilding = new SimpleDataNodeView();
                    // Create header/expander container        
                    expanderBuilding.expDesc.Header = node.Name.ToUpper();
                    expanderBuilding.Name = node.Name.ToUpper();
                    // only add on create expander
                    stkItems.Children.Add(expanderBuilding);
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        RecurseLoadXml(child);
                    }

                }
                else
                {

                    expanderBuilding.NodeGrid.RowDefinitions.Add(new RowDefinition());
                    Label label = new Label()
                    {
                        Content = node.Name.ToUpper(),
                        MinWidth = 200
                    };

                    TextBox inputControl = new TextBox()
                    {
                        Text = node.InnerText,
                        Name = string.Format("{0}_{1}", expanderBuilding.Name, node.Name.ToUpper())
                    };
                    Grid.SetRow(label, expanderBuilding.NodeGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(label, 0);
                    Grid.SetRow(inputControl, expanderBuilding.NodeGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(inputControl, 1);
                    expanderBuilding.NodeGrid.Children.Add(label);
                    expanderBuilding.NodeGrid.Children.Add(inputControl);
                }

            }
        }

        public void LoadXml()
        { 
            doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(templateFile));
            
            XmlNode root = doc.SelectSingleNode(string.Format(@"/{0}", System.IO.Path.GetFileName(templateFile)));
            RecurseLoadXml(root);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(templateFile)); // open template
           
            XmlNode root = doc.SelectSingleNode(string.Format(@"/{0}", System.IO.Path.GetFileName(templateFile)));
            //doc.ApplyFuncToAllNodes(
            //delegate (XmlNode node)
            //{
            //    if (node.NodeType == XmlNodeType.Element)
            //    {
            //        if (node.ChildNodes.Count > 1)
            //        {

            //            foreach (FrameworkElement element in stkItems.Children)
            //            {
            //                if (string.Equals(element.Name, node.Name, StringComparison.InvariantCultureIgnoreCase))
            //                {
            //                    expanderBuilding = element as WpfXmlDemo.SimpleDataNodeView;
            //                    break;
            //                }
            //            }
            //        }
            //        else
            //        {

            //            foreach (FrameworkElement fe in expanderBuilding.NodeGrid.Children)
            //            {
            //                if (fe is TextBox && fe.Name.Contains(node.Name.ToUpper()))
            //                {
            //                    node.InnerText = (fe as TextBox).Text;
            //                }
            //            }

            //        }
            //    }               
            //});
            string fileName = string.Format("{0}{1}", templateFile, ".xml");
            doc.Save(fileName);
            //ShareDialog sd = new ShareDialog();
            //sd.FeedbackTextBox.Text = System.IO.Path.GetFullPath(fileName);
            //Window popup = new Window()
            //{
            //    Title = "Saved",
            //    Content = sd,
            //    SizeToContent = SizeToContent.WidthAndHeight,
            //    ResizeMode = ResizeMode.NoResize,
            //    WindowState = WindowState.Maximized,
               
            //};
            //popup.ShowDialog();
            
        }




    }
}
