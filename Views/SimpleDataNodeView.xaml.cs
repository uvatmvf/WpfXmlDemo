using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfXmlDemo
{
    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class SimpleDataNodeView : UserControl
    {
        public SimpleDataNodeView()
        {
            InitializeComponent();
        }

        public void BuildLabelText(object sender, string text, string value, string key)
        {
            // Create data template controls for the xml elements
            // and add to buildingStack
            Label label = new Label()
            {
                Content = text,
                MinWidth = 100
            };

            TextBox inputControl = new TextBox()
            {
                Text = value,
                Name = key,
                MinWidth = 200,
                Tag = sender
            };
            NodeGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(label, NodeGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(label, 0);
            Grid.SetRow(inputControl, NodeGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(inputControl, 1);
            // Add template to the building stack
            NodeGrid.Children.Add(label);
            NodeGrid.Children.Add(inputControl);
            //NodeBorder.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF383838"));
            //NodeBorder.BorderBrush = NodeBorder.Background;

            //Border  new Color( "#FF383838");
            //NodeGrid.ParentNodeBorder
        }

        public SimpleDataNodeView BuildNode(string name, string tag)
        {
            SimpleDataNodeView request = new SimpleDataNodeView() { Name = name, Tag = tag };
            request.expDesc.Header = name.ToUpper();
            request.expDesc.IsExpanded = false;
            request.expDesc.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2543FF"));
            request.expDesc.Background = new SolidColorBrush(Colors.White);
            this.NodeGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(request, this.NodeGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(request, 2);
            this.NodeGrid.Children.Add(request);
            return request;
        } 

        public Dictionary<string, object> TextBoxToDict() 
        {
            Dictionary<string, object> request = new Dictionary<string, object>();
            foreach(FrameworkElement fe in NodeGrid.Children)
            {
                TextBox input = fe as TextBox;
                if (input != null)
                {
                    if (!request.ContainsKey(input.Name))
                    {
                        request.Add(input.Name, input.Text);
                    }
                }
                SimpleDataNodeView node = fe as SimpleDataNodeView;
                if (node != null)
                {
                    if (!request.ContainsKey(node.Name))
                    {
                        request.Add(node.Name, node.TextBoxToDict());
                    }
                }
            }
            return request;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
