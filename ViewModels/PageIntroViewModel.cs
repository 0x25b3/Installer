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
    public class PageIntroViewModel : ViewModel, IStorable
    {
        private ImageSource welcomeImage = new BitmapImage(new Uri("/Installer;component/Resources/Welcome.png", UriKind.Relative));
        private string
            headline = "Welcome!",
            subHeadline = "This is a Sub-Headline. Write something here.",
            text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque non nibh congue, fermentum metus nec, dapibus risus. Integer id tellus quis sapien interdum varius. Donec fermentum velit libero, in bibendum lacus euismod et. Nunc tristique, risus ac condimentum mattis, purus nisl rhoncus justo, at faucibus odio purus a ante. Aenean urna lorem, sodales a rutrum eget, blandit quis justo. In nec viverra nulla. Mauris eu neque ut sapien malesuada bibendum sed quis elit. Nullam facilisis ipsum nunc, ut convallis sem lobortis at. Duis quis lorem nunc.\r\n\r\nNunc a sapien non neque posuere accumsan.Nunc maximus faucibus pulvinar.Nulla nec ultrices lorem, ut sagittis mauris.Etiam quis tristique tellus. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Curabitur ac purus a ligula porttitor luctus.Donec egestas, dui vel molestie pretium, turpis turpis.";
        
        public ImageSource WelcomeImage { get { return welcomeImage; } set { SetProperty(ref welcomeImage, value); } }
        public string Headline { get { return headline; } set { SetProperty(ref headline, value); } }
        public string SubHeadline { get { return subHeadline; } set { SetProperty(ref subHeadline, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        public PageIntroViewModel()
        {
            try
            {
                var Image = Manipulator.GetResourceImage("Image", "WelcomeBanner");
                if (Image != null)
                    welcomeImage = Image;
            
                var HeadlineText = Manipulator.GetResourceString("Text", "Page0_Headline");
                if (HeadlineText != null)
                    headline = HeadlineText;
            
                var SubHeadlineText = Manipulator.GetResourceString("Text", "Page0_SubHeadline");
                if (SubHeadlineText != null)
                    subHeadline = SubHeadlineText;
           
                var Text = Manipulator.GetResourceString("Text", "Page0_Text");
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
            Manipulator.UpdateResource("Text", "Page0_Headline", Headline);
            Manipulator.UpdateResource("Text", "Page0_SubHeadline", SubHeadline);
            Manipulator.UpdateResource("Text", "Page0_Text", Text);
        }
    }
}
