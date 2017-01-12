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

using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Unicorn
{
    public static class CompositorAnimationExtension
    {
        public static Visual GetVisual(this UIElement targetElement)
        {
            if (targetElement == null)
            {
                return null;
            }

            return ElementCompositionPreview.GetElementVisual(targetElement);
        }

        public static Compositor GetCompositor(this Visual targetVisual)
        {
            return targetVisual?.Compositor;
        }

        /// <summary>
        /// 參考自此範例 https://msdn.microsoft.com/en-us/library/windows.ui.composition.vector3keyframeanimation.aspx
        /// </summary>
        /// <param name="compositor"></param>
        /// <returns></returns>
        public static CompositionEasingFunction GetFadeInEasingFunction(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.5f, 0.0f), new Vector2(1.0f, 1.0f));
        }

        /// <summary>
        /// 參考自此範例 https://msdn.microsoft.com/en-us/library/windows.ui.composition.vector3keyframeanimation.aspx
        /// </summary>
        /// <param name="compositor"></param>
        /// <returns></returns>
        public static CompositionEasingFunction GetFadeOutEasingFunction(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 1.0f));
        }

        public static CompositionPropertySet GetScrollViewerManipulationPropertySet(this ScrollViewer scrollViewer)
        {
            return ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
        }
    }
}
