using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace LightningChartTestsApp
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        private ColorTheme _colorTheme;
        private IReadOnlyList<string> _titles;
        private List<UserControl1> _legendBoxes;

        public UserControl2()
        {
            InitializeComponent();//?
        }

        public UserControl2(int numberOfCharts, ColorTheme theme, IEnumerable<string> titles, bool isPhaseChart = false)
        {
            if (theme.Colors.Count >= numberOfCharts)
                _colorTheme = theme;
            else
                _colorTheme = CreateDefaultTheme();

            if (titles.Count() >= numberOfCharts)
                _titles = titles.ToList();
            else
                _titles = CreateDefaultTitles(numberOfCharts, isPhaseChart);

            _legendBoxes = new List<UserControl1>(numberOfCharts);
            
            
            InitializeComponent();
        }

        private ColorTheme CreateDefaultTheme() => 
            new ColorTheme(
                baseColors: new[]
                {
                    Color.FromRgb(255, 50, 50),
                    Color.FromRgb(50, 255, 50),
                    Color.FromRgb(0, 100, 255)
                },
                starColor: Color.FromRgb(0, 0, 0));

        private List<string> CreateDefaultTitles(int numberOfCharts, bool isPhaseChart)
        {
            var output = new List<string>(numberOfCharts);
            if (isPhaseChart)
            {
            }
            else
                for (int i = 0; i < numberOfCharts; i++)
                    output.Add($"{i + 1}");
            return output;
        }
    }
}
