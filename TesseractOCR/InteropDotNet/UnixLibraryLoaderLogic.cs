//
// UnixLibraryLoaderLogic.cs
//
// Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//
// Copyright (c) 2014 Andrey Akinshin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using TesseractOCR.Helpers;

namespace TesseractOCR.InteropDotNet
{
    internal class UnixLibraryLoaderLogic : ILibraryLoaderLogic
    {
        #region Fields
        private static readonly string FileExtension = SystemManager.GetOperatingSystem() == OperatingSystem.MacOSX ? ".dylib" : ".so";
        #endregion

        #region LoadLibrary
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                Logger.LogInformation($"Trying to load native library '{fileName}'");

                NativeLibrary.TryLoad(fileName, out libraryHandle);
                
                if (libraryHandle != IntPtr.Zero)
                    Logger.LogInformation($"Successfully loaded native library '{fileName}' with handle '{libraryHandle}'");
                else
                    Logger.LogError($"Failed to load native library '{fileName}', set logging to debug level and check logging");
            }
            catch (Exception exception)
            {
                Logger.LogError($"Failed to load native library '{fileName}', inner Exception: {exception}");
            }

            return libraryHandle;
        }
        #endregion

        #region FreeLibrary
        public bool FreeLibrary(IntPtr libraryHandle)
        {
            NativeLibrary.Free(libraryHandle);
            return true;
        }
        #endregion

        #region GetProcAddress
        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            try
            {
                Logger.LogDebug($"Trying to load native function '{functionName}' from the library with handle '{libraryHandle}'");
            
                var functionHandle = NativeLibrary.GetExport(libraryHandle, functionName);
            
                if (functionHandle != IntPtr.Zero)
                    Logger.LogDebug($"Successfully loaded native function '{functionName}' with handle '{functionHandle}'");
                else
                    Logger.LogError($"Failed to load native function '{functionName}', with handle '{functionHandle}'");
            
                return functionHandle;

            }
            catch (Exception exception)
            {
                Logger.LogError($"Failed to load native function '{functionName}', exception: {exception}");
                return IntPtr.Zero;
            }
        }
        #endregion

        #region FixUpLibraryName
        public string FixUpLibraryName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            if (!fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
                fileName += FileExtension;

            if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                fileName = "lib" + fileName;

            return fileName;
        }
        #endregion
    }
}
#endif