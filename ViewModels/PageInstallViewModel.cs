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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Installer.Base;
using Installer.Interfaces;

namespace Installer.ViewModels
{
    public class PageInstallViewModel : ViewModel, IStorable
    {
        private ImageSource installImage = new BitmapImage(new Uri("/Installer;component/Resources/Welcome.png", UriKind.Relative));
        private string
            headline = "Installing..",
            text = "Please wait a moment, while the software is being installed.",
            text2 = "Progress:";
        
        public ImageSource InstallImage { get { return installImage; } set { SetProperty(ref installImage, value); } }
        public string Headline { get { return headline; } set { SetProperty(ref headline, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }
        public string Text2 { get { return text2; } set { SetProperty(ref text2, value); } }

        public PageInstallViewModel()
        {
            try
            {
                var Image = Manipulator.GetResourceImage("Image", "InstallBanner");
                if (Image != null)
                    installImage = Image;
            
                var HeadlineText = Manipulator.GetResourceString("Text", "Page4_Headline");
                if (HeadlineText != null)
                    headline = HeadlineText;
           
                var Text = Manipulator.GetResourceString("Text", "Page4_Text");
                if (Text != null)
                    text = Text;

                var Text2 = Manipulator.GetResourceString("Text2", "Page4_Text2");
                if (Text2 != null)
                    text2 = Text2;
            }
            catch { }

            this.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Store();
        }

        public void Store()
        {
            Manipulator.UpdateResource("Text", "Page4_Headline", Headline);
            Manipulator.UpdateResource("Text", "Page4_Text", Text);
        }
    }
}
