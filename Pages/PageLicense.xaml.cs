using Installer.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Installer.Pages
{
    public partial class PageLicense : UserControl
    {
        public new PageLicenseViewModel DataContext { get { return base.DataContext as PageLicenseViewModel; } set { base.DataContext = value; } }

        public PageLicense()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                var OPF = new OpenFileDialog();
                if (OPF.ShowDialog() == true)
                {
                    WelcomeBanner.Source = new BitmapImage(new Uri(OPF.FileName));
                    Manipulator.UpdateResource("Image", "WelcomeBanner", new Uri(OPF.FileName));
                }
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
