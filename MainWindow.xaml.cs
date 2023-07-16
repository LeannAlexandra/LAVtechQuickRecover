using System;
using System.Collections.Generic;
using System.IO;
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
//using System.Windows.Shapes;

namespace LAVtechQuickRecover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char DEFAULT_DRIVE_LETTER = 'E';
        private string[] fileOptions = { "Pictures", "Videos","Documents", "Misc", "All" };
        private char driveLetter = 'E';
        private string[] driveOptions= Array.Empty<string>();
        
        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
            LoadDriveOptions();
            LoadFileOptions();
            //Find Available Drives ()
            //Load Defaults 
            
        }
        private void LoadFileOptions() 
        {
            fileTypeCB.Items.Add(fileOptions[0]);
            fileTypeCB.SelectedIndex = 0;
        }
        private void LoadDriveOptions()
        {
            if (driveOptions.Length > 0)
            {
                foreach (string drive in driveOptions)
                {
                    sourceFileCB.Items.Add(DEFAULT_DRIVE_LETTER + ":\\");
                    destinationFolderCB.Items.Add(DEFAULT_DRIVE_LETTER + ":\\");
                }

            }
            else
            {
                sourceFileCB.Items.Add(DEFAULT_DRIVE_LETTER + ":\\");
                destinationFolderCB.Items.Add(DEFAULT_DRIVE_LETTER + ":\\");
            }

            ///TODO: IMPLEMENT WIN CODE TO FIND ALL AVAILABLE DRIVES. -> Use Drive it is running from as destination. use default as input, if it is available, otherwise use another drive

            sourceFileCB.SelectedIndex = 0;
            destinationFolderCB.SelectedIndex = 0;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = driveLetter + ":\\";
            string destinationPath = driveLetter + ":\\allpictures\\";

            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif" };

            foreach (string extension in extensions)
            {
                foreach (string file in Directory.EnumerateFiles(sourcePath, "*" + extension))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(destinationPath, fileName);

                    File.Copy(file, destinationFile, true);
                }
            }

            MessageBox.Show("Pictures copied successfully!");
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void fileTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadDrives()
        {
            Console.WriteLine("ATTEMPTING TO FIND DRIVES NOW");
            DriveInfo[] drives = DriveInfo.GetDrives();
            //Console.WriteLine("ATTEMPTING TO FIND DRIVES NOW");
            //int i = 0;
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady && (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed))
                {
                    string driveName = drive.Name;
                    string driveLabel = string.IsNullOrEmpty(drive.VolumeLabel) ? driveName : drive.VolumeLabel;
                    driveOptions.Append(driveLabel);
                    Console.WriteLine(driveName);
                    // Add driveName and driveLabel to your UI elements (e.g., ComboBox, ListBox, etc.)
                    
                }
            }
        }
    }
}



/* CHATGPT CODE TO SNIP FROM

using System.IO;
using System.Windows;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private string driveLetter = "C"; // Replace with the desired drive letter

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CopyPicturesButton_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = driveLetter + ":\\";
            string destinationPath = driveLetter + ":\\allpictures\\";

            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif" };

            foreach (string extension in extensions)
            {
                foreach (string file in Directory.EnumerateFiles(sourcePath, "*" + extension))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(destinationPath, fileName);

                    File.Copy(file, destinationFile, true);
                }
            }

            MessageBox.Show("Pictures copied successfully!");
        }
    }
}
*/