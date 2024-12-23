using System.Collections.Generic;
using System.Windows.Media;

namespace LightningChartTestsApp
{
    public class ColorTheme
    {
        private List<Color> _colors;
        private Color _starColor;

        private float _firstGenCoefficient;
        private float _secondGenCoefficient;

        public IReadOnlyList<Color> Colors => _colors;
        public Color StarColor => _starColor;
        
        public ColorTheme(IReadOnlyList<Color> baseColors, Color starColor, float firstGenCoefficient = 0.9f, float secondGenCoefficient = 0.5f)
        {
            var colorsCount = baseColors.Count * 2 + baseColors.Count * (baseColors.Count - 1) / 2;
            _firstGenCoefficient = firstGenCoefficient;
            _secondGenCoefficient = secondGenCoefficient;
            _colors = new List<Color>(colorsCount);
            _colors.AddRange(baseColors); //base added
            _starColor = starColor;

            for (int i = 0; i < baseColors.Count; i++)
            {
                //pairs added
                for (int j = i + 1; j < baseColors.Count; j++)
                {
                    _colors.Add(CreateFirstGenColor(baseColors[i], baseColors[j]));
                }
            }

            for (int i = 0; i < baseColors.Count; i++)
            {
                //changed base added
                _colors.Add(CreateSecondGenColor(baseColors[i], starColor));
            }
            _colors.Add(starColor);
        }
        
        private Color CreateFirstGenColor(Color a, Color b) => Color.Multiply(Color.Add(a, b), _firstGenCoefficient);
        private Color CreateSecondGenColor(Color a, Color b) => Color.Multiply(Color.Add(a, b), _secondGenCoefficient);
        
    }
}
