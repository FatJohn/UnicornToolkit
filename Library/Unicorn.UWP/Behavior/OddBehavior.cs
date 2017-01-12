// Copyright (c) 2016 Louis Wu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace Unicorn
{
    /// <summary>
    /// A behavior that performs actions when the bound data meets a specified condition.
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public class OddBehavior : Behavior
    {
        public new DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actionCollection = (ActionCollection)this.GetValue(OddBehavior.ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    this.SetValue(OddBehavior.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            "Actions",
            typeof(ActionCollection),
            typeof(OddBehavior),
            new PropertyMetadata(null));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(OddBehavior), new PropertyMetadata(null, ValuePropertyChanged));

        private static void ValuePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as OddBehavior;
            if (behavior == null || e.NewValue == null)
            {
                return;
            }

            behavior.Apply();
        }

        public bool IsCompatable
        {
            get { return (bool)GetValue(IsCompatableProperty); }
            set { SetValue(IsCompatableProperty, value); }
        }

        public static readonly DependencyProperty IsCompatableProperty =
            DependencyProperty.Register("IsCompatable", typeof(bool), typeof(OddBehavior), new PropertyMetadata(true, IsCompatableChanged));

        private static void IsCompatableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as OddBehavior;
            if (behavior == null || e.NewValue == null)
            {
                return;
            }

            behavior.Apply();
        }

        private void Apply()
        {
            DataBindingHelper.RefreshDataBindingsOnActions(Actions);

            bool isOdd = Value % 2 == 1;
            bool result = (isOdd && IsCompatable) || (isOdd == false && IsCompatable == false);

            if (result)
            {
                Interaction.ExecuteActions(this.AssociatedObject, this.Actions, null);
            }
        }

        public new void Attach(DependencyObject associatedObject)
        {
            var control = associatedObject as Control;
            if (control == null)
            {
                throw new ArgumentException("OddBehavior can be attached only to Control");
            }

            AssociatedObject = associatedObject;
        }

        public new void Detach()
        {
            AssociatedObject = null;
        }
    }
}
