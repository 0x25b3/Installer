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
using Microsoft.Win32;
using Installer.ViewModels;

namespace Installer.Pages
{
    public partial class PageIntro : UserControl
    {
        public new PageIntroViewModel DataContext { get { return base.DataContext as PageIntroViewModel; } set { base.DataContext = value; } }

        public PageIntro()
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
                    Banner.Source = DataContext.WelcomeImage = new BitmapImage(new Uri(OPF.FileName));
                    Manipulator.UpdateResource("Image", "WelcomeBanner", new Uri(OPF.FileName));
                }
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
