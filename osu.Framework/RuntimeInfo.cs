// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace osu.Framework
{
    public static class RuntimeInfo
    {
        /// <summary>
        /// The absolute path to the startup directory of this game.
        /// </summary>
        public static string StartupDirectory { get; } = AppContext.BaseDirectory;

        /// <summary>
        /// Returns the absolute path of osu.Framework.dll.
        /// </summary>
        public static string GetFrameworkAssemblyPath()
        {
            var assembly = Assembly.GetAssembly(typeof(RuntimeInfo));
            Debug.Assert(assembly != null);

            return assembly.Location;
        }

        public static Platform OS { get; }
        public static bool IsUnix => OS != Platform.Windows;

        public static bool SupportsJIT => OS != Platform.iOS;
        public static bool IsDesktop => OS == Platform.Linux || OS == Platform.macOS || OS == Platform.Windows;
        public static bool IsMobile => OS == Platform.iOS || OS == Platform.Android;
        public static bool IsApple => OS == Platform.iOS || OS == Platform.macOS;

        static RuntimeInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                OS = Platform.Windows;
            if (detectIOS())
                OS = OS == 0 ? Platform.iOS : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.iOS)}, but is already {Enum.GetName(typeof(Platform), OS)}");
            if (detectAndroid())
                OS = OS == 0 ? Platform.Android : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.Android)}, but is already {Enum.GetName(typeof(Platform), OS)}");
            if (OS != Platform.iOS && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                OS = OS == 0 ? Platform.macOS : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.macOS)}, but is already {Enum.GetName(typeof(Platform), OS)}");
            if (OS != Platform.Android && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                OS = OS == 0 ? Platform.Linux : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.Linux)}, but is already {Enum.GetName(typeof(Platform), OS)}");

            if (OS == 0)
                throw new PlatformNotSupportedException("Operating system could not be detected correctly.");
        }

        private static bool detectAndroid() => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.ToString().Contains("Mono.Android"));
        private static bool detectIOS() => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.ToString().Contains("Xamarin.iOS"));

        public enum Platform
        {
            Windows = 1,
            Linux = 2,
            macOS = 3,
            iOS = 4,
            Android = 5
        }
    }
}
