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
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace Unicorn
{
    public class CompositionAnimationUtility
    {
        /// <summary>
        /// 使用位移絕對數值，跑位移動畫
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="targetZ">targetZ 與 Canvas.ZIndex 不同，應注意</param>
        /// <param name="duration"></param>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        /// <param name="fromZ"></param>
        /// <returns></returns>
        public static Visual Move(UIElement targetElement, float? targetX = null, float? targetY = null, float? targetZ = null, TimeSpan? duration = null, float? fromX = null, float? fromY = null, float? fromZ = null)
        {
            var visual = targetElement.GetVisual();

            if (duration != null)
            {
                var compositor = visual.GetCompositor();
                var easingFunction = compositor.CreateLinearEasingFunction();
                var moveAnimation = compositor.CreateVector3KeyFrameAnimation();
                moveAnimation.Duration = (TimeSpan)duration;

                if (fromX != null || fromY != null || fromZ != null)
                {
                    moveAnimation.InsertKeyFrame(0.0f, new Vector3(fromX ?? visual.Offset.X, fromY ?? visual.Offset.Y, fromZ ?? visual.Offset.Z), easingFunction);
                }

                moveAnimation.InsertKeyFrame(1.0f, new Vector3(targetX ?? visual.Offset.X, targetY ?? visual.Offset.Y, targetZ ?? visual.Offset.Z), easingFunction);
                visual.StartAnimation("Offset", moveAnimation);
            }
            else
            {
                visual.Offset = new Vector3(targetX ?? visual.Offset.X, targetY ?? visual.Offset.Y, targetZ ?? visual.Offset.Z);
            }

            return visual;
        }

        /// <summary>
        /// 使用位移相對數值進行位移
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="deltaZ"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Visual MoveDelta(UIElement targetElement, float? deltaX = null, float? deltaY = null, float? deltaZ = null, TimeSpan? duration = null)
        {
            var visual = targetElement.GetVisual();

            if (duration != null)
            {
                var targetX = visual.Offset.X + deltaX ?? 0;
                var targetY = visual.Offset.Y + deltaY ?? 0;
                var targetZ = visual.Offset.Z + deltaZ ?? 0;

                return Move(targetElement, targetX, targetY, targetZ, duration);
            }

            return visual;
        }

        /// <summary>
        /// 跑 Resize 動畫
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Visual Resize(UIElement targetElement, float? targetWidth = null, float? targetHeight = null, TimeSpan? duration = null)
        {
            var visual = targetElement.GetVisual();

            if (duration != null)
            {
                var compositor = visual.GetCompositor();
                var easingFunction = compositor.CreateLinearEasingFunction();
                var resizeAnimation = compositor.CreateVector2KeyFrameAnimation();
                resizeAnimation.Duration = (TimeSpan)duration;
                resizeAnimation.InsertKeyFrame(1.0f, new Vector2(targetWidth ?? visual.Size.X, targetHeight ?? visual.Size.Y), easingFunction);

                visual.StartAnimation("Size", resizeAnimation);
            }
            else
            {
                visual.Size = new Vector2(targetWidth ?? visual.Size.X, targetHeight ?? visual.Size.Y);
            }

            return visual;
        }

        /// <summary>
        /// 跑淡出淡入動畫
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="toOpacity"></param>
        /// <param name="fromOpacity"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Visual Fade(UIElement targetElement, float toOpacity, float? fromOpacity = null, TimeSpan? duration = null)
        {
            var visual = targetElement.GetVisual();

            if (duration != null)
            {
                var compositor = visual.GetCompositor();
                CompositionEasingFunction easingFunction;
                bool isFadeIn = CheckIsFadeIn(toOpacity, fromOpacity);
                if (isFadeIn)
                {
                    easingFunction = compositor.GetFadeInEasingFunction();
                }
                else
                {
                    easingFunction = compositor.GetFadeOutEasingFunction();
                }

                var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
                opacityAnimation.Duration = (TimeSpan)duration;

                if (fromOpacity != null)
                {
                    opacityAnimation.InsertKeyFrame(0.0f, (float)fromOpacity, easingFunction);
                }

                opacityAnimation.InsertKeyFrame(1.0f, toOpacity, easingFunction);

                visual.StartAnimation("Opacity", opacityAnimation);
            }
            else
            {
                visual.Opacity = toOpacity;
            }

            return visual;
        }

        private static bool CheckIsFadeIn(float toOpacity, float? fromOpacity)
        {
            return fromOpacity == null || toOpacity > fromOpacity;
        }
    }
}
