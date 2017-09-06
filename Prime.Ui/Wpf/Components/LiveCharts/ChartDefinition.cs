using System.Collections.Generic;

namespace Prime.Ui.Wpf
{
    public class ChartDefinition
    {
        public string Name { get; set; }

        public Dictionary<string, SeriesDefinition> Series = new Dictionary<string, SeriesDefinition>();
    }
}