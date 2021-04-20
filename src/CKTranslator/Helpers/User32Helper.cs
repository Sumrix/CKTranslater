using System;
using System.Runtime.InteropServices;

using Microsoft.UI.Xaml;

using Windows.Storage.Pickers;

using WinRT;

namespace CKTranslator.Helpers
{
	public static class User32Helper
    {
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        public static void SetWindowSize(this Window window, int width, int height)
        {
            IntPtr hwnd = window.As<IWindowNative>().WindowHandle;

            var dpi = PInvoke.User32.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        public static void Initialize(this FolderPicker folderPicker, Window window)
        {
            IntPtr hwnd = window.As<IWindowNative>().WindowHandle;

            var initializeWithWindow = folderPicker.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);
            folderPicker.FileTypeFilter.Add("*");
        }
    }
}