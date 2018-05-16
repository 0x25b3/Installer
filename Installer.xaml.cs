//TODO: PageTargetDir
//TODO: PageInstall
//TODO: PageCompletion
//TODO: Komponenten-Logik überlegen (z.B. mit Sub-ZIPs)
//TODO: Design für Buttons

//TODO: Unbenutzten Third-Party-Code entfernen
//TODO: Durchkommentieren & -Gruppieren
//TODO: Copyright
//TODO: Name festlegen
//TODO: Name unten links

using ICSharpCode.SharpZipLib.Zip;
using Installer.Base;
using Installer.Pages;
using Installer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Installer
{
    public partial class InstallerWindow : Window
    {
        public new InstallerViewModel DataContext { get { return base.DataContext as InstallerViewModel; } set { base.DataContext = value; } }
        
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

                if (Res != IntPtr.Zero)
                {
                    uint size = Win32.SizeofResource(TargetHandle, Res);
                    IntPtr pt = Win32.LoadResource(TargetHandle, Res);

                    byte[] bPtr = new byte[size];
                    Marshal.Copy(pt, bPtr, 0, (int)size);

                    var TempPath = System.IO.Path.GetTempFileName();

                    File.WriteAllBytes(TempPath, bPtr);

                    Zip = new ZipFile(TempPath);
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
                DataContext.IsEditMode = false;
                Manipulator.Initialize(Location, false);
            }
            #endregion
            #region Compilation-Mode
            else if (File.Exists(args[0]) && args[0].EndsWith(".zip"))
            {
                try
                {
                    string OutputPath = System.IO.Path.ChangeExtension(args[0], "exe");
                    bool Exists = File.Exists(OutputPath);
                    if (!Exists || (Exists && MessageBox.Show($"Ausgabe-Datei \"{OutputPath}\" existiert bereits. Soll die Datei überschrieben werden?", "Überschreiben?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                    {
                        Manipulator.UpdateResource("Content", "Data", File.ReadAllBytes(args[0]));
                        File.Copy(Manipulator.TargetPath, OutputPath);
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
                DataContext.IsEditMode = true;
                
                Manipulator.Initialize(Location);
            }
            #endregion

            DataContext.Pages.Add(new PageIntroViewModel());
            DataContext.Pages.Add(new PageLicenseViewModel());
            DataContext.Pages.Add(new PageTargetDirectoryViewModel());
            DataContext.Pages.Add(new PageInstallViewModel());
            DataContext.Pages.Add(new PageCompletionViewModel());

            DataContext.CurrentPage = DataContext.Pages.FirstOrDefault();
        }

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
                DataContext.BackButtonVisible = !(DataContext.CurrentPage is PageIntroViewModel || DataContext.CurrentPage is PageCompletionViewModel);
                //TODO: Cancel Button?

                if(DataContext.CurrentIndex == DataContext.Pages.Count - 1)
                {
                    Foreward.Content = "Done";
                }
            }
        }
        private void PageBack(object sender, RoutedEventArgs e)
        {
            DataContext.CurrentIndex--;
            DataContext.CurrentPage = DataContext.Pages.ElementAt(DataContext.CurrentIndex);
            DataContext.BackButtonVisible = !(DataContext.CurrentPage is PageIntroViewModel);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            // Question
            Close();
        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save changes, cancel close-event on error
            if (MessageBox.Show("Save changes before exit?", "Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes && Manipulator.Save() == false)
                e.Cancel = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged([CallerMemberName] string PropertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        internal void SetProperty<T>(ref T Variable, T Value, [CallerMemberName] string PropertyName = "")
        {
            Variable = Value;
            NotifyPropertyChanged(PropertyName);
        }
    }
}
