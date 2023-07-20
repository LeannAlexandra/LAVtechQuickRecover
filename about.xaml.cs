using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAVtechQuickRecover
{
    /// <summary>
    /// Interaction logic for about.xaml
    /// </summary>
    public partial class about : Window
    {
        //About strings
        private int firstEditionYear = 2023;
        private string message= "COPYRIGHT © 2023";
        private string author = "LeAnn Alexandra";
        private string authorContact = "LeAnnAlexandraViolet@gmail.com";
        private string authorURL = "github.com/LeannAlexandra";
        private string logoAuthor = "WilliamsCreativity0";
        private string logoURL = "https://pixabay.com/vectors/folder-icon-document-file-hosting-5502835/";
        private string logoArtistURL = "https://pixabay.com/users/williamscreativity-17210051/ ";
        public about()
        {
            InitializeComponent();
            
            UpdateAppInformation();
        }
        private void UpdateAppInformation() {
            string version
                    = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DateTime now = DateTime.Now;
            int year =now.Year;

            message = "Author:\n";
            message += author;
            message += "\n";
            message += authorContact;
            message += "\n";
            message += authorURL;
            string copyright = year > firstEditionYear ? $"{firstEditionYear} - {year}": $"{firstEditionYear}";
            message += "\nCOPYRIGHT © " + copyright;
            AboutTextBlock.Text = message;


            message = "";
            message += "Logo author: \n";
            message += logoAuthor;
            message += "\n";
            message += logoURL;
            message += "\n"; 
            message += logoArtistURL;

            LogoAuthorTextBlock.Text = message;


            //Label authorLabel = new Label();
            //authorLabel.Content = author;
            //SP.Children.Add(authorLabel);
        }
    }
}
