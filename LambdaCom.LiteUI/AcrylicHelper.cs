/*
 * MIT License
 * 
 * Copyright (c) 2016 minami_SC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
 * Based on code from https://github.com/sourcechord/FluentWPF/blob/master/FluentWPF/Utility/AcrylicHelper.cs
 */

using System;
using System.Runtime.InteropServices;

namespace LambdaCom.LiteUI
{
    internal enum AccentFlagsType
    {
        Window = 0,
        Popup,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    internal static class AcrylicHelper
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal static void EnableBlur(IntPtr hwnd, AccentFlagsType style = AccentFlagsType.Window)
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);

            accent.AccentState = SystemInfo.Version.Value switch
            {
                OSVersion.W10_1903 => AccentState.ACCENT_ENABLE_BLURBEHIND,
                OSVersion.W10_1809 => AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                OSVersion.W10 => AccentState.ACCENT_ENABLE_BLURBEHIND,
                OSVersion.Old => AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT,
                _ => throw new ArgumentOutOfRangeException(nameof(SystemInfo.Version.Value), "Unknown OS version")
            };

            accent.AccentFlags = style == AccentFlagsType.Window
                ? 2
                : 0x20 | 0x40 | 0x80 | 0x100;

            accent.GradientColor = 0x00FFFFFF;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }

    internal enum OSVersion { Old, W10, W10_1809, W10_1903 };

    internal class SystemInfo
    {
        public static Lazy<OSVersion> Version { get; } = new Lazy<OSVersion>(() => GetVersionInfo());

        internal static OSVersion GetVersionInfo()
        {
            var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", false);

            if (regkey == null) return default;

            var majorValue = regkey.GetValue("CurrentMajorVersionNumber");
            var minorValue = regkey.GetValue("CurrentMinorVersionNumber");
            var buildValue = (string)regkey.GetValue("CurrentBuild", 7600);
            var canReadBuild = int.TryParse(buildValue, out var build);

            var version = majorValue is int major && minorValue is int minor && canReadBuild
                ? new Version(major, minor, build) 
                : Environment.OSVersion.Version;

            return version switch
            {
                _ when version >= new Version(10, 0, 18362) => OSVersion.W10_1903,
                _ when version >= new Version(10, 0, 17763) => OSVersion.W10_1809,
                _ when version >= new Version(10, 0, 10240) => OSVersion.W10,
                _ => OSVersion.Old,
            };
        }
    }
}
