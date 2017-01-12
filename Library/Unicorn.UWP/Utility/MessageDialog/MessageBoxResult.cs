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

namespace Unicorn
{
    /// <summary>
    /// Represents a user's response to a message box.
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// This value is not currently used.
        /// </summary>
        None = 0,
        /// <summary>
        ///  The user clicked the OK button.
        /// </summary>
        OK = 1,
        /// <summary>
        /// The user clicked the Cancel button or pressed ESC.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// This value is not currently used.
        /// </summary>
        Yes = 6,
        /// <summary>
        /// This value is not currently used.
        /// </summary>
        No = 7,
        CustomFirst = 8,
        CustomSecond = 9,
        CustomThird = 10,
        /// <summary>
        /// 由於彈出了另一個視窗，導致此視窗被關閉
        /// </summary>
        CloseForce = 11,
        Close = 12
    }
}
