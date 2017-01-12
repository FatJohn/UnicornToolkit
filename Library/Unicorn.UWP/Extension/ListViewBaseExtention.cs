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

using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Unicorn
{
    public static class ListViewBaseExtention
    {
        public static int IndexFromControl(this ListViewBase listViewBase, Control targetControl)
        {
            if (listViewBase == null || targetControl == null)
            {
                return -1;
            }

            var containers = targetControl.Ancestors<SelectorItem>();
            if (containers.Count() <= 0)
            {
                return -1;
            }

            int index = -1;
            foreach (var container in containers)
            {
                index = listViewBase.IndexFromContainer(container);
                if (index >= 0)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
