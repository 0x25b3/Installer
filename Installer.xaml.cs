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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Installer.Base;
using Installer.Pages;
using Installer.ViewModels;
using Path = System.IO.Path;

namespace Installer
{
    public partial class InstallerWindow : Window
    {
        #region Properties
        public static bool IsEditMode { get; private set; } = true;

        public new InstallerViewModel DataContext { get { return base.DataContext as InstallerViewModel; } set { base.DataContext = value; } }
        #endregion

        #region Constructor & OnLoad
        public InstallerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            var Location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            
            ZipFile Zip = null;
            #region Get ZIP if available
            IntPtr TargetHandle = IntPtr.Zero;
            try
            {
                TargetHandle = Win32.LoadLibraryEx(Location, IntPtr.Zero, 2);
                if (TargetHandle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var Res = Win32.FindResource(TargetHandle, "Data", "Content");

                DataContext.HasContent = Res != IntPtr.Zero;

                if (Res != IntPtr.Zero)
                {
                    uint size = Win32.SizeofResource(TargetHandle, Res);
                    IntPtr pt = Win32.LoadResource(TargetHandle, Res);

                    byte[] bPtr = new byte[size];
                    Marshal.Copy(pt, bPtr, 0, (int)size);

                    var zipStream = new MemoryStream(bPtr);

                    var OutputFolder = DataContext.OutputFolder;

                    ZipInputStream zipInputStream = new ZipInputStream(zipStream);
                    ZipEntry zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        String entryFileName = zipEntry.Name;

                        byte[] buffer = new byte[4096];     // 4K is optimum

                        // Manipulate the output filename here as desired.
                        String fullZipToPath = Path.Combine(OutputFolder, entryFileName);
                        string directoryName = Path.GetDirectoryName(fullZipToPath);
                        if (directoryName.Length > 0)
                            Directory.CreateDirectory(directoryName);

                        // Skip directory entry
                        string fileName = Path.GetFileName(fullZipToPath);
                        if (fileName.Length == 0)
                        {
                            zipEntry = zipInputStream.GetNextEntry();
                            continue;
                        }

                        // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                        // of the file, but does not waste memory.
                        // The "using" will close the stream even if an exception occurs.
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                        }
                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Oops! This installer seems to be corrupted! Sorry :(\r\n\r\n" + ex.Message, 
                    "Fatal Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            finally
            {
                if (TargetHandle != IntPtr.Zero)
                    Win32.EndUpdateResource(TargetHandle, false);
            }
            #endregion

            #region Install-Mode
            if (Zip != null)
            {
                IsEditMode = false;
                
            }
            #endregion
            #region Compilation-Mode
            else if (args.Length > 1 && File.Exists(args[1]) && args[1].EndsWith(".zip"))
            {
                try
                {
                    IsEditMode = false;

                    Manipulator.Initialize(Location);

                    string OutputPath = System.IO.Path.ChangeExtension(args[1], "exe");
                    bool Exists = File.Exists(OutputPath);
                    if (!Exists || (Exists && MessageBox.Show($"Ausgabe-Datei \"{OutputPath}\" existiert bereits. Soll die Datei überschrieben werden?", "Überschreiben?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                    {
                        Manipulator.UpdateResource("Content", "Data", File.ReadAllBytes(args[1]));
                        File.Copy(Manipulator.TargetPath, OutputPath, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler bei der Erstellung des Setups:\r\n" + ex.Message, "Fehler!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Close();
            }
            #endregion
            #region Edit-Mode
            else
            {
                IsEditMode = true;
                
                Manipulator.Initialize(Location);
            }
            #endregion

            var desc = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty, typeof(UserControl));
            desc.AddValueChanged(ContentView, OnPageChanged);

            DataContext.Pages.Add(new PageIntroViewModel());
            DataContext.Pages.Add(new PageLicenseViewModel());
            DataContext.Pages.Add(new PageTargetDirectoryViewModel());
            DataContext.Pages.Add(new PageInstallViewModel());
            DataContext.Pages.Add(new PageCompletionViewModel());

            DataContext.CurrentPage = DataContext.Pages.FirstOrDefault();
        }
        #endregion

        #region Pagination
        private void PageContinue(object sender, RoutedEventArgs e)
        {
            if(DataContext.CurrentIndex == DataContext.Pages.Count - 1)
            {
                Close();
            }
            else
            {
                DataContext.CurrentIndex++;
                DataContext.CurrentPage = DataContext.Pages.ElementAt(DataContext.CurrentIndex);
            }
        }
        private void PageBack(object sender, RoutedEventArgs e)
        {
            DataContext.CurrentIndex--;
            DataContext.CurrentPage = DataContext.Pages.ElementAt(DataContext.CurrentIndex);
        }

        private void OnPageChanged(object sender, EventArgs e)
        {
            DataContext.BackButtonVisible = DataContext.CurrentPage is PageIntroViewModel == false && (DataContext.IsEditMode || DataContext.CurrentPage is PageCompletionViewModel);
            DataContext.CancelButtonVisible = DataContext.CurrentPage is PageCompletionViewModel == false;

            Continue.Content = (DataContext.CurrentIndex == DataContext.Pages.Count - 1) ? "Done" : "Continue";
        }
        #endregion

        #region Close
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            // Question
            Close();
        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Ask if we want to save, abort on error
            if (DataContext.IsEditMode && MessageBox.Show("Save changes before exit?", "Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if(Manipulator.Save() == false)
                    e.Cancel = true;
            }

            //// Clean up temp files if we're closing
            //if(e.Cancel == false)
            //    Manipulator.Close();
        }
        #endregion

        /// <summary>
        /// Kewl link to the repo.
        /// </summary>
        private void Website_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/0x25b3/Installer");

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged([CallerMemberName] string PropertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        internal void SetProperty<T>(ref T Variable, T Value, [CallerMemberName] string PropertyName = "")
        {
            Variable = Value;
            NotifyPropertyChanged(PropertyName);
        }
        #endregion
    }
}
