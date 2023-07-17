using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms; //FOR THE MESSAGE BOX AND DIALOG RESUTL
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;
//using System.Windows.Shapes;

namespace LAVtechQuickRecover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    //TODO: filetype selection dropdown (on change) event
    //      in the Copy_click event apply the above logic
    public partial class MainWindow : Window
    {
        private const char DEFAULT_DRIVE_LETTER = 'E';
        private string destinationFolderName = "LAVTechRecovery";
        private string[] fileOptions = { "Pictures", "Videos","Audio", "Documents", "Misc", "All" };
        string[] pictureExtensions = { ".png", ".jpg", ".jpeg", ".gif", "tiff", ".tif", ".bmp" };
        string[] vectorGraphicsExtentions = { ".EPS", ".AI", ".psd",".indd", ".raw", ".svg", ".cdr" };
        string[] audioExtensions = {".AIF", ".IFF", ".M3U", ".M4A", ".MID", ".MP3", ".MPA", ".WAV", ".WMA" };
        string[] videoExtensions = { ".3G2", ".3GP", ".ASF", ".AVI", ".FLV", ".M4V", ".MOV", ".MP4", ".MPG", ".RM", ".SRT", ".SWF", ".VOB", ".WMV" };
        string[] documentExtensions = { ".PDF", ".doc",".xlsx", "docx", ".xls", ".gif",".html",".htm", ".ODT", ".ODS", ".PPT", ".PPTX", ".TXT" };
        string[] programmingExtentions = {".sln",".js", ".html", ".css", ".json", ".c", ".cpp", "cs", ".py", ".pyc", ".pyw", ".pyx", ".class" /*, "", "", "", "", "", "", ""*/ }; 
        /// <summary>
        /// Program to quickly extract selected file types from a drive to another drive. Specifically useful when upgrading your computer and just want to keep photos, documents or videos.
        /// 
        /// </summary>
        /// 

        //TODO: make a git-aware version of software developers. ;) 
        //(add exception for .gitignore files, same as git) )

        string[] miscExtensions = { ".", ".blend"};

        private bool preserveFileStructure = false;
        private bool testing=false;
        private char driveLetter = 'E';
        private string[] driveOptions= Array.Empty<string>();
        List<char> driveOptionsList = new List<char>();
        List<string> extensions = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            //Console.WriteLine("IT WORKS!");
            extensions.Add("txt");//as test

            //DO THE FOLLOWING ASYNC - each with a 'ready' flag. - activate buttons only when ready.
            LoadDrives();
            LoadDriveOptions();
            LoadFileOptions();
            customFolderNameTB.Text = destinationFolderName;
        }

        //INITIALIZATION EVENTS
        private void LoadFileOptions() 
        {
            foreach(string file in fileOptions) { 
                fileTypeCB.Items.Add(file);
            }
            fileTypeCB.SelectedIndex = 0;
        }
        private void LoadDriveOptions()
        {
            int index = 0;

            foreach (char _drive in driveOptionsList)
                {
                    sourceFileCB.Items.Add(_drive + ":\\");
                    destinationFolderCB.Items.Add(_drive + ":\\");
                if (_drive.Equals(DEFAULT_DRIVE_LETTER))  //because they are char could use char1==char2 ;)
                {
                    sourceFileCB.SelectedIndex = index;
                    destinationFolderCB.SelectedIndex = index;
                }
                index++;
                }
            //Set default to first drive should E drive be unavalable.
            if (sourceFileCB.SelectedIndex < 0)
                sourceFileCB.SelectedIndex = 0;

            if (destinationFolderCB.SelectedIndex < 0)
                destinationFolderCB.SelectedIndex = 0;
        }




        // EVENT LISTENERS
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            disableControls();
            string sourcePath = driveLetter + ":\\";
            string destinationPath = driveLetter + $":\\{destinationFolderName}\\";
         
            string[] allExtensions = extensions.ToArray();//ImageExtensions.Concat(DocumentExtensions);

            CopyFilesRecursively(sourcePath, destinationPath, allExtensions);
        }
      

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void FileTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            extensions.Clear();
            //extensions.Add(".txt");
            int choice = fileTypeCB.SelectedIndex;
            switch (choice) { //I could've made an enum .. or finished this section before load shedding, I chose the latter
                case 0: //0 Pictures 
                    extensions.AddRange(pictureExtensions);
                    extensions.AddRange(vectorGraphicsExtentions);
                    break;
                case 1: //1 videos
                    extensions.AddRange(videoExtensions);
                    break;
                case 2: //2 Audio
                    extensions.AddRange(audioExtensions);
                    break;
                case 3: //3 Documents
                    extensions.AddRange(documentExtensions);
                    break;
                case 4: //4 Misc //programming, blend, unreal, git....
                        //throws unimplemented exception ;)
                    extensions.AddRange(programmingExtentions);
                    break;
                case 5:
                default:
                    extensions.AddRange(pictureExtensions);
                    extensions.AddRange(documentExtensions);
                    extensions.AddRange(videoExtensions);
                    extensions.AddRange(programmingExtentions);
                    extensions.AddRange(vectorGraphicsExtentions);
                    extensions.AddRange(audioExtensions);
                    //do everything  ;) that's why the last option is always all.
                    break;
            }


            //if (testing)
            //    Console.WriteLine($"the current slection is: {choice}");


        }
        private void LoadDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady && (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed))
                {
                    string driveName = drive.Name;
                    string driveLabel = string.IsNullOrEmpty(drive.VolumeLabel) ? driveName : drive.VolumeLabel;
                    driveOptionsList.Add(driveName[0]);//takes the first char as char.... ;) because string= char[];
                }
            }
        }
        private void disableControls() //disables ui for the duration of execution (also shows loading log :D)
        { 
            fileTypeCB.IsEnabled = false;
            doneBtn.IsEnabled = false;
            copyBtn.IsEnabled = false;

        } 

        private void CustomFolderNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            destinationFolderName = customFolderNameTB.Text;
        }



        //LOGIC & EXECUTION EVENTS.
        private void CopyFilesRecursively(string sourceDirectory, string destinationDirectory, IEnumerable<string> extensions)
        {
            if (preserveFileStructure)
            {
                Directory.CreateDirectory(destinationDirectory);

                foreach (string file in Directory.EnumerateFiles(sourceDirectory))
                {
                    string extension = Path.GetExtension(file);

                    if (extensions.Contains(extension))
                    {
                        string fileName = Path.GetFileName(file);
                        string destinationFile = Path.Combine(destinationDirectory, fileName);

                        try
                        {
                            File.Copy(file, destinationFile, true);
                        }
                        catch (IOException ex)
                        {
                            // Handle file copying error
                            LogError($"Failed to copy file: {fileName}\n\nError: {ex.Message}");
                        }
                    }
                }

                foreach (string subdirectory in Directory.EnumerateDirectories(sourceDirectory))
                {
                    // Skip the destination directory
                    if (subdirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Prompt the user for permission to access protected directories
                    if (!HasAccessToDirectory(subdirectory))
                    {
                        LogError($"Skipped accessing the protected directory: {subdirectory}");
                        continue;
                    }

                    string directoryName = Path.GetFileName(subdirectory);
                    string newDestinationDirectory = Path.Combine(destinationDirectory, directoryName);

                    //CopyFilesRecursively(subdirectory, newDestinationDirectory, extensions);
                    try
                    {
                        CopyFilesRecursively(subdirectory, newDestinationDirectory, extensions);
                    }
                    catch (UnauthorizedAccessException e) { continue; }
                }
            }
            else
            {
                Directory.CreateDirectory(destinationDirectory);

                foreach (string file in Directory.EnumerateFiles(sourceDirectory))
                {
                    string extension = Path.GetExtension(file);

                    if (extensions.Contains(extension))
                    {
                        string fileName = Path.GetFileName(file);
                        string destinationFile = Path.Combine(destinationDirectory, fileName);

                        try
                        {
                            File.Copy(file, destinationFile, true);
                        }
                        catch (IOException ex)
                        {
                            // Handle file copying error
                            LogError($"Failed to copy file: {file}\n\nError: {ex.Message}");
                        }
                    }
                }

                //if (!preserveFileStructure)
                //    return;

                foreach (string subdirectory in Directory.EnumerateDirectories(sourceDirectory))
                {
                    // Skip the destination directory
                    if (subdirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        CopyFilesRecursively(subdirectory, destinationDirectory, extensions);
                    }
                    catch (UnauthorizedAccessException e) { continue; }

                }


            }
        }

        private bool HasAccessToDirectory(string directory)
        {
            try
            {
                // Attempt to get the directory info to check if it is accessible
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                _ = dirInfo.GetFiles(); // This will throw an exception if the directory is not accessible

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void LogError(string errorMessage)
        {
            // Implement your error logging mechanism here
            // You can log the error to a file, database, or any other desired destination
            // Example: log to a file
            Console.WriteLine($"{errorMessage}\n");


            ///???///?SAVE THIS FOR THE ACTUAL OUTPUT ERROR LOG IN DESTINATION FOLDER
            //string logFilePath = "error.log";
            //File.AppendAllText(logFilePath, $"{errorMessage}\n");
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
############################################### VERSION 2 ################################### RECURSION

using System.IO;
using System.Linq;
using System.Windows;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        // Define your extension list as constant arrays
        private const string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".gif" };
        private const string[] DocumentExtensions = { ".doc", ".docx", ".pdf" };

        private string driveLetter = "C"; // Replace with the desired drive letter

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CopyPicturesButton_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = driveLetter + ":\\";
            string destinationPath = driveLetter + ":\\allpictures\\";

            // Combine all extension lists into one
            var allExtensions = ImageExtensions.Concat(DocumentExtensions);

            // Call recursive method to copy files
            CopyFilesRecursively(sourcePath, destinationPath, allExtensions);

            MessageBox.Show("Pictures copied successfully!");
        }

        private void CopyFilesRecursively(string sourceDirectory, string destinationDirectory, IEnumerable<string> extensions)
        {
            Directory.CreateDirectory(destinationDirectory);

            foreach (string file in Directory.EnumerateFiles(sourceDirectory))
            {
                string extension = Path.GetExtension(file);

                if (extensions.Contains(extension))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(destinationDirectory, fileName);

                    File.Copy(file, destinationFile, true);
                }
            }

            foreach (string subdirectory in Directory.EnumerateDirectories(sourceDirectory))
            {
                // Skip the destination directory
                if (subdirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase))
                    continue;
                string directoryName = Path.GetFileName(subdirectory);
                string newDestinationDirectory = Path.Combine(destinationDirectory, directoryName);

                // Recursive call to handle subdirectories
                CopyFilesRecursively(subdirectory, newDestinationDirectory, extensions);
            }
        }
    }
}


*/