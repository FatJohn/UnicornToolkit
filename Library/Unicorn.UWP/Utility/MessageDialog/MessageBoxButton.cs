// Copyright (c) 2016 John Shu

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

namespace Unicorn
{
    [Flags]
    public enum MessageBoxButton
    {
        /// <summary>
        /// Displays only the OK button.
        /// </summary>
        OK = 1,
        /// <summary>
        /// Displays only the Cancel button.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// Displays both the OK and Cancel buttons.
        /// </summary>
        OKCancel = OK | Cancel,
        /// <summary>
        /// Displays only the OK button.
        /// </summary>
        Yes = 4,
        /// <summary>
        /// Displays only the Cancel button.
        /// </summary>
        No = 8,
        /// <summary>
        /// Displays both the OK and Cancel buttons.
        /// </summary>
        YesNo = Yes | No,
        CustomOneButton = 16,
        CustomTwoButton = 32,
        CustomThreeButton = 64,
        Close = 128
    }
}
