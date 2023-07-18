using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;
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
        //CURRENTLY HARDCODED TO ONLY E DRIVE, EVEN THOUGH DRIVE SELECTORS ARE PRESENT AND WORK> 
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
        string[] documentExtensions = { ".PDF", ".doc",".xlsx", "docx", ".xls",".html",".htm", ".ODT", ".ODS", ".PPT", ".PPTX", ".TXT" };
        string[] programmingExtentions = {".sln",".js", ".html", ".css", ".json", ".c", ".cpp", "cs", ".py", ".pyc", ".pyw", ".pyx", ".class" /*, "", "", "", "", "", "", ""*/ }; 
        /// <summary>
        /// Program to quickly extract selected file types from a drive to another drive. Specifically useful when upgrading your computer and just want to keep photos, documents or videos.
        /// 
        /// </summary>
        /// 

        

        //string[] miscExtensions = { ".", ".blend"};
        //private Thread processingThread;

        private bool preserveFileStructure = false;
        private bool testing=false;
        private char driveLetter = 'E';
        private char destinationDriveLetter = 'E';
        private bool operationInProgress = false;
        List<char> driveOptionsList = new List<char>();
        List<string> extensions = new List<string>();

        //Window Dragging Variables
        private const int DragStartDelayMilliseconds = 500; // Adjust the duration as needed
        private bool isDragging = false;
        private Point dragStartPosition;
        private DispatcherTimer dragTimer;
        private IntPtr hookHandle;
        private LowLevelMouseProc hookProc;

 //////////////////////////////////////////////////////////////////////////////////////////////////////// BIG CREDIT TO THE ARTIST OF THE LOGO /////////////////////////////////////////////
        private string aboutInformationIcon = "WilliamsCreativity https://pixabay.com/users/williamscreativity-17210051/ : Light blue circle with folder containing file - by Williams Creativity Available at https://pixabay.com/vectors/folder-icon-document-file-hosting-5502835/";
        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
            LoadDriveOptions();
            LoadFileOptions();
            customFolderNameTB.Text = destinationFolderName;

            //Add the window dragging functionality
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
            MouseLeftButtonUp += Window_MouseLeftButtonUp;
            MouseMove += Window_MouseMove;

            dragTimer = new DispatcherTimer();
            dragTimer.Interval = TimeSpan.FromMilliseconds(DragStartDelayMilliseconds);
            dragTimer.Tick += DragTimer_Tick;

            Loaded += Window_Loaded;
            Closing += Window_Closing;
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

        //LOGIC & EXECUTION EVENTS.
        private async Task CopyFilesRecursively(string sourceDirectory, string destinationDirectory, IEnumerable<string> extensions)
        {
            if (preserveFileStructure)
            {
                Directory.CreateDirectory(destinationDirectory);

                await Task.Run(async () =>
                {
                    try
                    {
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
                                    LogError($"Failed to copy file: {fileName}\n\nError: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        LogError($"ERROR WITH MESSAGE {ex}");
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
                            await Task.Run(() => { Task recursionTask = CopyFilesRecursively(subdirectory, newDestinationDirectory, extensions); });
                        }
                        catch (UnauthorizedAccessException e) {
                            LogError($"ERROR WITH MESSAGE {e}");
                            continue; }
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
                await Task.Run(async () =>
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
                        LogError($"ERROR WITH MESSAGE {ex}");
                        return;
                    }
            
                    foreach (string subdirectory in Directory.EnumerateDirectories(sourceDirectory))
                    {
                        // Skip the destination directory
                        if (subdirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase))
                            continue;
                        try
                        {
                            await Task.Run(() => { Task recursionTask = CopyFilesRecursively(subdirectory, destinationDirectory, extensions); });
                            
                        }
                        catch (Exception e) {
                            LogError($"An exception occurred: {e.Message}");
                            continue; }

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
            //string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //string logFilePath = Path.Combine(executableDirectory, "error.log");
            //File.AppendAllText(logFilePath, $"{errorMessage}\n");
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            preserveFileStructure = (bool)preserveFileStructureCB.IsChecked; //ignore warning (bool)null = false   ;)
           
        }

        private void SourceFileCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            driveLetter = driveOptionsList[sourceFileCB.SelectedIndex]; //the first character // because we're working with drives.
        }

        private void DestinationFolderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            destinationDriveLetter= driveOptionsList[destinationFolderCB.SelectedIndex];    //first character because ... drive letter.
        }

        ///???????????????/ WINDOW DRAGGING LOGIC
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            hookProc = MouseHookCallback;
            hookHandle = SetMouseHook(hookProc, source.Handle);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnhookMouseHook();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPosition = e.GetPosition(this);
            CaptureMouse();
            dragTimer.Start();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragTimer.Stop();
            isDragging = false;
            ReleaseMouseCapture();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                Vector dragDelta = currentPosition - dragStartPosition;
                Left += dragDelta.X;
                Top += dragDelta.Y;
            }
        }

        private void Window_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                dragTimer.Stop();
                isDragging = false;
            }
        }

        private void DragTimer_Tick(object sender, EventArgs e)
        {
            dragTimer.Stop();
            isDragging = true;
        }

        private IntPtr SetMouseHook(LowLevelMouseProc proc, IntPtr handle)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private void UnhookMouseHook()
        {
            if (hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookHandle);
                hookHandle = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONUP)
            {
                if (isDragging)
                {
                    isDragging = false;
                    ReleaseMouseCapture();
                }
            }

            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONUP = 0x0202;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);



    }
}

