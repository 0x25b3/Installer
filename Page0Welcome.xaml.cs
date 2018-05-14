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

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für _0_Welcome.xaml
    /// </summary>
    public partial class Page0Welcome : Page
    {
        public Page0Welcome()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debugger.Break();

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                var OPF = new OpenFileDialog();
                if(OPF.ShowDialog() == true)
                {
                    Manipulator.UpdateResource("Image", "WelcomeBanner", OPF.FileName);
                }
            }
        }
        
        private void Install_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }
    }
}
