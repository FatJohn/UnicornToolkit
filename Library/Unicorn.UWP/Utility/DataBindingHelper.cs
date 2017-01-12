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
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;

namespace Unicorn
{
    public static class DataBindingHelper
    {
        private static readonly Dictionary<Type, List<DependencyProperty>> DependenciesPropertyCache = new Dictionary<Type, List<DependencyProperty>>();

        /// <summary>
        /// Ensures that all binding expression on actions are up to date.
        /// </summary>
        /// <remarks>
        /// DataTriggerBehavior fires during data binding phase. Since the ActionCollection is a child of the behavior,
        /// bindings on the action  may not be up-to-date. This routine is called before the action
        /// is executed in order to guarantee that all bindings are refreshed with the most current data.
        /// </remarks>
        public static void RefreshDataBindingsOnActions(ActionCollection actions)
        {
            foreach (DependencyObject action in actions)
            {
                if (action == null)
                {
                    continue;
                }

                foreach (DependencyProperty property in GetDependencyProperties(action.GetType()))
                {
                    RefreshBinding(action, property);
                }
            }
        }

        private static IEnumerable<DependencyProperty> GetDependencyProperties(Type type)
        {
            List<DependencyProperty> propertyList = null;

            if (!DependenciesPropertyCache.TryGetValue(type, out propertyList))
            {
                propertyList = new List<DependencyProperty>();

                while (type != null && type != typeof(DependencyObject))
                {
                    foreach (FieldInfo fieldInfo in type.GetRuntimeFields())
                    {
                        if (fieldInfo.IsPublic && fieldInfo.FieldType == typeof(DependencyProperty))
                        {
                            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;
                            if (property != null)
                            {
                                propertyList.Add(property);
                            }
                        }
                    }

                    type = type.GetTypeInfo().BaseType;
                }

                DependenciesPropertyCache[type] = propertyList;
            }

            return propertyList;
        }

        private static void RefreshBinding(DependencyObject target, DependencyProperty property)
        {
            if (target == null)
            {
                return;
            }

            BindingExpression binding = target.ReadLocalValue(property) as BindingExpression;
            if (binding != null && binding.ParentBinding != null)
            {
                BindingOperations.SetBinding(target, property, binding.ParentBinding);
            }
        }
    }
}
