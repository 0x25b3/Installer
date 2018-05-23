#region License
// Copyright (C) 2018 Benjamin Bartels
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ICSharpCode.SharpZipLib.Zip;

namespace Installer
{
    public static class Manipulator
    {
        static bool DeleteTempFile = false;
        public static string TargetPath { get; internal set; }

        public static void Initialize(string SourcePath, bool Copy = true)
        {
            if (Copy)
            {
                TargetPath = Path.GetTempFileName();
                File.Copy(SourcePath, TargetPath, true);
                DeleteTempFile = true;
            }
            else
                TargetPath = SourcePath;
        }

        public static void UpdateResource(string Type, string Name, string Data)
        {
            UpdateResource(Type, Name, Encoding.UTF8.GetBytes(Data));
        }
        public static void UpdateResource(string Type, string Name, Uri SourcePath)
        {
            byte[] Data = File.ReadAllBytes(Uri.UnescapeDataString(SourcePath.AbsolutePath));
            UpdateResource(Type, Name, Data);
        }
        public static void UpdateResource(string Type, string Name, byte[] Data)
        {
            Exception Exception;
            IntPtr TargetHandle = IntPtr.Zero;
            try
            {
                TargetHandle = Win32.BeginUpdateResource(TargetPath, false);
                if (TargetHandle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                
                if (!Win32.UpdateResource(TargetHandle, Type, Name, (short)CultureInfo.CurrentUICulture.LCID, Data, Data.Length))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception ex)
            {
                Exception = ex;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (TargetHandle != IntPtr.Zero)
                    Win32.EndUpdateResource(TargetHandle, false);
            }
        }
        
        public static byte[] GetResource(string Type, string Name)
        {
            Exception Exception;
            IntPtr TargetHandle = IntPtr.Zero;
            try
            {
                TargetHandle = Win32.LoadLibraryEx(TargetPath, IntPtr.Zero, 2);
                if (TargetHandle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var Res = Win32.FindResource(TargetHandle, Name, Type);
                uint size = Win32.SizeofResource(TargetHandle, Res);
                IntPtr pt = Win32.LoadResource(TargetHandle, Res);

                //var E = Marshal.GetLastWin32Error();
                //if (E != 0)
                //    throw new Win32Exception(E);

                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);

                return bPtr;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                if (TargetHandle != IntPtr.Zero)
                    Win32.EndUpdateResource(TargetHandle, false);
            }

            return null;
        }
        public static BitmapImage GetResourceImage(string Type, string Name)
        {
            BitmapImage Result = null;
            try
            {
                byte[] Data = Manipulator.GetResource(Type, Name);
                Result = ByteArrayToImage(Data);
            }
            catch (Exception ex)
            {
            }

            return Result;
        }
        public static string GetResourceString(string Type, string Name)
        {
            string Result = null;
            try
            {
                byte[] Data = Manipulator.GetResource(Type, Name);
                Result = System.Text.Encoding.UTF8.GetString(Data);
            }
            catch(Exception ex)
            {
            }

            return Result;
        }

        public static bool Save()
        {
            bool Result = false;

            try
            {
                var PID = Process.GetCurrentProcess().Id;

                var Batch =
                    $"@ECHO OFF\r\n" +
                    $":LOOP\r\n" +
                    $"TASKLIST /FI \"PID eq {PID}\" | find \":\" > nul\r\n" +
                    $"IF ERRORLEVEL 1 GOTO LOOP\r\n" +
                    $"MOVE /Y \"{TargetPath}\" \"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"\r\n" +
                    $"DEL /Q \"{TargetPath}\"\r\n" +
                    $"(GOTO) 2>NUL & DEL \"%~f0\"";

                var TempFile = Path.GetTempFileName();
                TempFile = Path.ChangeExtension(TempFile, "bat");
                File.WriteAllText(TempFile, Batch);

                ProcessStartInfo StartInfo = new ProcessStartInfo(TempFile);
                StartInfo.UseShellExecute = true;
                StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(StartInfo);

                Result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Result;
        }

        private static BitmapImage ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public static void Close()
        {
            if (DeleteTempFile)
                File.Delete(TargetPath);
        }

        //public static void AddToPrograms(string Path)
        //{
        //    using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
        //    {
        //        if (parent == null)
        //        {
        //            throw new Exception("Uninstall registry key not found.");
        //        }
        //        try
        //        {
        //            RegistryKey key = null;

        //            try
        //            {
        //                string guidText = UninstallGuid.ToString("B");
        //                key = parent.OpenSubKey(guidText, true) ??
        //                      parent.CreateSubKey(guidText);

        //                if (key == null)
        //                {
        //                    throw new Exception(String.Format("Unable to create uninstaller '{0}\\{1}'", UninstallRegKeyPath, guidText));
        //                }

        //                Assembly asm = GetType().Assembly;
        //                Version v = asm.GetName().Version;
        //                string exe = "\"" + asm.CodeBase.Substring(8).Replace("/", "\\\\") + "\"";

        //                key.SetValue("DisplayName", "My Program");
        //                key.SetValue("ApplicationVersion", v.ToString());
        //                key.SetValue("Publisher", "My Company");
        //                key.SetValue("DisplayIcon", exe);
        //                key.SetValue("DisplayVersion", v.ToString(2));
        //                key.SetValue("URLInfoAbout", "http://www.blinemedical.com");
        //                key.SetValue("Contact", "support@mycompany.com");
        //                key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
        //                key.SetValue("UninstallString", exe + " /uninstallprompt");
        //            }
        //            finally
        //            {
        //                if (key != null)
        //                {
        //                    key.Close();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(
        //                "An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.",
        //                ex);
        //        }
        //    }
        //}
    }
}
