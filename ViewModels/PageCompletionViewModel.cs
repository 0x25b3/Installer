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
    public class PageCompletionViewModel : ViewModel, IStorable
    {
        private ImageSource welcomeImage = new BitmapImage(new Uri("/Installer;component/Resources/Welcome.png", UriKind.Relative));
        private string
            headline = "Vielen Dank!",
            text = "Wir danken Ihnen, dass Sie unsere Software installiert haben.\r\n\r\nViel Freude!";
        
        public ImageSource WelcomeImage { get { return welcomeImage; } set { SetProperty(ref welcomeImage, value); } }
        public string Headline { get { return headline; } set { SetProperty(ref headline, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        public PageCompletionViewModel()
        {
            try
            {
                var Image = Manipulator.GetResourceImage("Image", "WelcomeBanner");
                if (Image != null)
                    welcomeImage = Image;
            
                var HeadlineText = Manipulator.GetResourceString("Text", "Page5_Headline");
                if (HeadlineText != null)
                    headline = HeadlineText;
           
                var Text = Manipulator.GetResourceString("Text", "Page5_Text");
                if (Text != null)
                    text = Text;
            }
            catch { }

            this.PropertyChanged += Page5ViewModel_PropertyChanged;
        }

        private void Page5ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Store();
        }

        public void Store()
        {
            Manipulator.UpdateResource("Text", "Page5_Headline", Headline);
            Manipulator.UpdateResource("Text", "Page5_Text", Text);
        }
    }
}
