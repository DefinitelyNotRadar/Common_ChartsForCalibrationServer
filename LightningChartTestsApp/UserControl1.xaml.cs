using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LightningChartTestsApp
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private int _size = 60;
        private Color _boxBoxColor;
        private string _title;
        private string _checkBoxName;

        public event EventHandler<bool> immaclickclack;

        public UserControl1() : this(Colors.Orange)
        {}

        public UserControl1(Color boxColor, int size = 60, string title = "4 - 5", string checkBoxName = "Points")
        {
            _size = size;
            _boxBoxColor = boxColor;
            _checkBoxName = checkBoxName;
            _title = title;
            this.MinHeight = _size;
            this.MaxHeight = _size;
            this.MinWidth = _size;
            this.MaxWidth = _size;
            this.Width = _size;
            this.Height = _size;

            InitializeComponent();
            Grid.Children.Add(
                new Label()
                {
                    FontSize = 8,
                    Height = 10,
                    Padding = new Thickness(0, 0, 0, 0),
                    Content = _title,
                    Margin = new Thickness(0, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            Grid.Children.Add(
                new Rectangle()
                {
                    Width = _size * 0.583f,
                    Height = _size * 0.583f,
                    Fill = new SolidColorBrush(_boxBoxColor),
                    Margin = new Thickness(_size * 0.2f, _size * 0.166f, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                });
            Grid.Children.Add(
                new CheckBox()
                {
                    IsChecked = true,
                    Height = 15,
                    Padding = new Thickness(0, 0, 0, 0),
                    Content = _checkBoxName,
                    Margin = new Thickness(0, _size * 0.75f, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            ((CheckBox)Grid.Children[2]).Checked += (sender, args) => immaclickclack?.Invoke(sender, true);
            ((CheckBox)Grid.Children[2]).Unchecked += (sender, args) => immaclickclack?.Invoke(sender, false); //not sure that this is right tho : /
        }
    }
}
