using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Unicorn
{
    public static class VisualStateManagerHelper
    {
        public static bool GoToState(Control control, string stateName, bool useTransitions)
        {
            try
            {
                return VisualStateManager.GoToState(control, stateName, useTransitions);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
