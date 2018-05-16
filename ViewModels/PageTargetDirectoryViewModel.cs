using Installer.Base;
using Installer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Installer.ViewModels
{
    public class PageTargetDirectoryViewModel : ViewModel, IStorable
    {
        private ImageSource welcomeImage = new BitmapImage(new Uri("/Installer;component/Resources/Welcome.png", UriKind.Relative));
        private string
            headline = "Target-Directory",
            text = "Please select a target directory.\r\n\r\nThis area can contain additional information about the components being installed.\r\n\r\nIn the future there might be a component-selection embedded here.";
        
        public ImageSource WelcomeImage { get { return welcomeImage; } set { SetProperty(ref welcomeImage, value); } }
        public string Headline { get { return headline; } set { SetProperty(ref headline, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        public PageTargetDirectoryViewModel()
        {
            try
            {
                var Image = Manipulator.GetResourceImage("Image", "WelcomeBanner");
                if (Image != null)
                    welcomeImage = Image;
            
                var HeadlineText = Manipulator.GetResourceString("Text", "Page3_Headline");
                if (HeadlineText != null)
                    headline = HeadlineText;
           
                var Text = Manipulator.GetResourceString("Text", "Page3_Text");
                if (Text != null)
                    text = Text;
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
            Manipulator.UpdateResource("Text", "Page3_Headline", Headline);
            Manipulator.UpdateResource("Text", "Page3_Text", Text);
        }
    }
}
