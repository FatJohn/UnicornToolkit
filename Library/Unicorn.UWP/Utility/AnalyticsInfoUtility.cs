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

using Windows.System.Profile;

namespace Unicorn
{
    public class DeviceFamilyVersion
    {
        public ulong Major { get; set; }

        public ulong Minor { get; set; }

        public ulong Build { get; set; }

        public ulong Revision { get; set; }
    }

    public class AnalyticsInfoUtility
    {
        public DeviceFamilyVersion GetDeviceFamilyVersion()
        {
            string versionString = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong versionNumber = ulong.Parse(versionString);
            ulong major = (versionNumber & 0xFFFF000000000000L) >> 48;
            ulong minor = (versionNumber & 0x0000FFFF00000000L) >> 32;
            ulong build = (versionNumber & 0x00000000FFFF0000L) >> 16;
            ulong revesion = (versionNumber & 0x000000000000FFFFL);

            return new DeviceFamilyVersion
            {
                Major = major,
                Minor = minor,
                Build = build,
                Revision = revesion,
            };
        }
    }
}
