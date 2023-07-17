﻿using System;
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
        private bool testing=true;
        private char driveLetter = 'E';
        List<char> driveOptionsList = new List<char>();
        List<string> extensions = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
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

            //set default. 
            fileTypeCB.SelectedIndex = 0;
            extensions.AddRange(pictureExtensions);
            extensions.AddRange(vectorGraphicsExtentions);

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
            toggleControls(false); //set them not enabled
            string sourcePath = driveLetter + ":\\";
            string destinationPath = driveLetter + $":\\{destinationFolderName}\\";
         
            string[] allExtensions = extensions.ToArray();//ImageExtensions.Concat(DocumentExtensions);

            CopyFilesRecursively(sourcePath, destinationPath, allExtensions);
            toggleControls(true);
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
            if (testing)
            {
                extensions.Clear();
                extensions.Add(".txt");

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
        private void toggleControls(bool to = false) //disables ui for the duration of execution (also shows loading log :D)
        {
            fileTypeCB.IsEnabled = to;
            doneBtn.IsEnabled = to;
            copyBtn.IsEnabled = to;
            copiedFeedback.Content = "";
            inputFileFeedback.Content = "";

            if (to) {
                copiedFeedback.Visibility = Visibility.Hidden;
                inputFileFeedback.Visibility = Visibility.Hidden;
                loadingIndicator.Visibility = Visibility.Hidden;
            }
            else
            {
                copiedFeedback.Visibility = Visibility.Visible;
                inputFileFeedback.Visibility = Visibility.Visible;
                loadingIndicator.Visibility = Visibility.Visible;
            }
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
                    SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);
                    loadingCircle.Stroke = myBrush;
                    string extension = Path.GetExtension(file);
                    inputFileFeedback.Content = $"{file}";
                    if (extensions.Contains(extension))
                    {
                        string fileName = Path.GetFileName(file);
                        string destinationFile = Path.Combine(destinationDirectory, fileName);

                        try
                        {
                            File.Copy(file, destinationFile, true);
                            copiedFeedback.Content = $"{fileName}"; //filename (adds directory)
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
                    inputFileFeedback.Content = $"{file}";
                    if (extensions.Contains(extension))
                    {
                        string fileName = Path.GetFileName(file);
                        string destinationFile = Path.Combine(destinationDirectory, fileName);

                        try
                        {
                            File.Copy(file, destinationFile, true);
                            copiedFeedback.Content = $"{file}";//file doesn't show the folder directory
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            preserveFileStructure = (bool)preserveFileStructureCB.IsChecked;
        }
    }
}

