using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;

namespace Prime.Ui.Wpf
{
    public class PrimeDateAxis : DateAxis
    {
        public override AxisCore AsCoreElement(ChartCore chart, AxisOrientation source)
        {
            if (this.Model == null)
                this.Model = (AxisCore)new PrimeDateAxisCore((IWindowAxisView)this);

            this.Model.ShowLabels = this.ShowLabels;
            this.Model.Chart = chart;
            this.Model.IsMerged = this.IsMerged;
            this.Model.Labels = this.Labels;
            this.Model.LabelFormatter = this.LabelFormatter;
            this.Model.MaxValue = this.MaxValue;
            this.Model.MinValue = this.MinValue;
            this.Model.Title = this.Title;
            this.Model.Position = this.Position;
            this.Model.Separator = this.Separator.AsCoreElement(this.Model, source);
            this.Model.DisableAnimations = this.DisableAnimations;
            this.Model.Sections = this.Sections.Select<AxisSection, AxisSectionCore>((Func<AxisSection, AxisSectionCore>)(x => x.AsCoreElement(this.Model, source))).ToList<AxisSectionCore>();

            this.Windows.Clear();
            this.Windows.AddRange(DateAxisWindows.GetDateAxisWindows());

            ((WindowAxisCore)this.Model).Windows = this.Windows.ToList<AxisWindow>();
            ((WindowAxisCore)this.Model).Windows.ForEach((Action<AxisWindow>)(w => ((DateAxisWindow)w).DateAxisCore = (PrimeDateAxisCore)this.Model));
            return this.Model;
        }
    }
}