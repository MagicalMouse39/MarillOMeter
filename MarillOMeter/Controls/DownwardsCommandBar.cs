using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MarillOMeter.Controls
{
    public class DownwardsCommandBarVisualStateManager : VisualStateManager
    {
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            //replace OpenUp state change with OpenDown one and continue as normal
            if (!string.IsNullOrWhiteSpace(stateName) && stateName.EndsWith("OpenUp"))
                stateName = stateName.Substring(0, stateName.Length - 6) + "OpenDown";
            return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
        }
    }

    internal class DownwardsCommandBar : CommandBar
    {
        public DownwardsCommandBar()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, new DownwardsCommandBarVisualStateManager());
        }
    }
}
