/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VoiceToKeys.Converters {
    [ValueConversion(typeof(string), typeof(ImageSource))]
    class StringToImageConverter : IValueConverter {
        static IntPtr processHandle;
        static StringToImageConverter() {
            var process = Process.GetCurrentProcess();

            processHandle = process.Handle;
        }

        [DllImport("shell32.dll")]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll")]
        static extern void DestroyIcon(IntPtr iconHandle);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var url = new Uri(parameter.ToString());
            var urlScheme = url.Scheme;

            if (targetType != typeof(ImageSource))
                return Binding.DoNothing;

            switch (urlScheme.ToLower()) {
                case "dll-icon":
                    return DLLIcon(url);
                default:
                    return Binding.DoNothing;
            }
        }

        private object DLLIcon(Uri uri) {
            var dllIconIndex = 0;
            var dllPath = uri.LocalPath;

            if (uri.Host != string.Empty) {
                throw new ArgumentException("Host must be empty in uri");
            }

            dllPath = Regex.Replace(dllPath, @"/\$(\w+)", (me) => {
                var folderName = me.Groups[1].ToString();
                var folder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), folderName, true);

                return Environment.GetFolderPath(folder);
            });

            dllPath = Regex.Replace(dllPath, @".dll/(\d+)", (me) => {
                dllIconIndex = int.Parse(me.Groups[1].ToString());
                return ".dll";
            });

            var iconHandle = ExtractIcon(processHandle, dllPath, dllIconIndex);
            var icon = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            DestroyIcon(iconHandle);

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
