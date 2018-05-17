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
        private ImageSource completionImage = new BitmapImage(new Uri("/Installer;component/Resources/Welcome.png", UriKind.Relative));
        private string
            headline = "Thank you!",
            text = "We're grateful, that you've installed our software.\r\n\r\nHave fun!";
        
        public ImageSource CompletionImage { get { return completionImage; } set { SetProperty(ref completionImage, value); } }
        public string Headline { get { return headline; } set { SetProperty(ref headline, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        public PageCompletionViewModel()
        {
            try
            {
                var Image = Manipulator.GetResourceImage("Image", "CompletionBanner");
                if (Image != null)
                    completionImage = Image;
            
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
