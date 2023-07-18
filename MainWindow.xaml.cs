using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms; //FOR THE MESSAGE BOX AND DIALOG RESUTL
using System.Windows.Input;
using System.Windows.Interop;
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



    //TODO: DRIVE LETTER SELECTIONS TO UPDATE DRIVES ON CHANGE> EVENTS 
    //TODO: ADD SOFTWARE(DEV) make a git-aware version of software developers. ;) 
    //(add exception for .gitignore files, same as git) )

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

        

        string[] miscExtensions = { ".", ".blend"};
        private Thread processingThread;

        private bool preserveFileStructure = false;
        private bool testing=true;
        private char driveLetter = 'E';
        private char destinationDriveLetter = 'E';
        private bool operationInProgress = false;
        List<char> driveOptionsList = new List<char>();
        List<string> extensions = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            //DO THE FOLLOWING ASYNC - each with a 'ready' flag. - activate buttons only when ready.
            LoadDrives();
            LoadDriveOptions();
            LoadFileOptions();
            //toggleControls();
            //showLoading();
            customFolderNameTB.Text = destinationFolderName;
            //var task = Task.Run(async () => {
                
            //    for (; ; )
            //    {


            //        if (!operationInProgress) {
            //            await Task.Delay(1000);
            //            continue;
            //        }
            //        await Task.Delay(100);
            //        Console.WriteLine("Hello World after .1 seconds");
            //    }
            //});
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
        private async void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (operationInProgress)
               return;

            toggleControls(operationInProgress); //set them not enabled -> Show Loading

                Console.WriteLine("IT's NO RUNNINGWE's JUST AN IDIOTS");
                string sourcePath = driveLetter + ":\\";
                string destinationPath = destinationDriveLetter + $":\\{destinationFolderName}\\";

                string[] allExtensions = extensions.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();//ImageExtensions.Concat(DocumentExtensions);
            try
            {

                await CopyFilesRecursively(sourcePath, destinationPath, allExtensions);// doesnt want to happy async ... yet
                Console.WriteLine("IT's DONE COPYING, WE's JUST AN IDIOTS");
            }
            catch (Exception ex) { LogError($"An error occurred: {ex.Message}"); }
            finally {

                toggleControls(operationInProgress);
            }
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
            operationInProgress = !operationInProgress;

            fileTypeCB.IsEnabled = to;
            doneBtn.IsEnabled = to;
            copyBtn.IsEnabled = to;
            copiedFeedback.Content = "";
            inputFileFeedback.Content = "";
            if (operationInProgress)
            {
                showLoading();
            }
            else
            {
                hideLoading();
            }
        }
        private int showLoading() {
            
            copiedFeedback.Visibility = Visibility.Visible;
            inputFileFeedback.Visibility = Visibility.Visible;
            loadingIndicator.Visibility = Visibility.Visible;
            return 0;
        }
        private int hideLoading()
        {
            copiedFeedback.Visibility = Visibility.Hidden;
            inputFileFeedback.Visibility = Visibility.Hidden;
            loadingIndicator.Visibility = Visibility.Hidden;
            return 0;
        }

        private void CustomFolderNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            destinationFolderName = customFolderNameTB.Text;
        }


        private Color nextColor() {


            return Color.FromRgb(255,254,212);
        } 

        //LOGIC & EXECUTION EVENTS.
        private async Task CopyFilesRecursively(string sourceDirectory, string destinationDirectory, IEnumerable<string> extensions)
        {
            if (preserveFileStructure)
            {
                Directory.CreateDirectory(destinationDirectory);

                await Task.Run(() =>
                {
                    try
                    {
                        foreach (string file in Directory.EnumerateFiles(sourceDirectory))
                        {
                            //SolidColorBrush myBrush = new SolidColorBrush(nextColor());
                            //loadingCircle.Stroke = myBrush;
                            string extension = Path.GetExtension(file);
                            //inputFileFeedback.Content = $"{file}";
                            if (extensions.Contains(extension))
                            {
                                string fileName = Path.GetFileName(file);
                                string destinationFile = Path.Combine(destinationDirectory, fileName);

                                try
                                {
                                    File.Copy(file, destinationFile, true);
                                    //copiedFeedback.Content = $"{fileName}"; //filename (adds directory)
                                }
                                catch (IOException ex)
                                {
                                    // Handle file copying error
                                    LogError($"Failed to copy file: {fileName}\n\nError: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        return;
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
                }).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        // Handle any exceptions occurred in the task
                        foreach (var ex in task.Exception.InnerExceptions)
                        {
                            LogError($"An exception occurred: {ex.Message}");
                        }
                    }
                }, TaskContinuationOptions.OnlyOnFaulted); ;
            }
            else
            {
                Directory.CreateDirectory(destinationDirectory);
                await Task.Run(() =>
                {
                    try
                    {
                        foreach (string file in Directory.EnumerateFiles(sourceDirectory))
                        {
                            string extension = Path.GetExtension(file);
                            //inputFileFeedback.Content = $"{file}";   //UI ITEMS ON MAIN UI THREAD!S
                            if (extensions.Contains(extension))
                            {
                                string fileName = Path.GetFileName(file);
                                string destinationFile = Path.Combine(destinationDirectory, fileName);

                                try
                                {
                                    File.Copy(file, destinationFile, true);
                                    //copiedFeedback.Content = $"{file}";//file doesn't show the folder directory
                                }
                                catch (Exception ex)
                                {
                                    // Handle file copying error
                                    LogError($"Failed to copy file: {file}\n\nError: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        return;
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
                        catch (Exception e) { continue; }

                    }
                }).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        // Handle any exceptions occurred in the task
                        foreach (var ex in task.Exception.InnerExceptions)
                        {
                            LogError($"An exception occurred: {ex.Message}");
                        }
                    }
                }, TaskContinuationOptions.OnlyOnFaulted); ;

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
            string destinationPath = driveLetter+"\\"+destinationFolderName;
                string logFilePath = Path.Combine(destinationPath, "error.log");
                File.AppendAllText(logFilePath, $"{errorMessage}\n");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            preserveFileStructure = (bool)preserveFileStructureCB.IsChecked;
        }


        ///////////////////////ACTIVATING BELOW ALLOWS WINDOW TO BE REPOSITIONED> IT ALSO MAKES THE UI DISABLED
        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(this);
        //    hwndSource.AddHook(WndProcHook);
        //    base.OnSourceInitialized(e);
        //}

        //private static IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handeled)
        //{
        //    if (msg == 0x0084) // WM_NCHITTEST
        //    {
        //        handeled = true;
        //        return (IntPtr)2; // HTCAPTION
        //    }
        //    return IntPtr.Zero;
        //}
    }
}

