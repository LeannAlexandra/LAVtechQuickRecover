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
using System.Windows.Shapes;

namespace LAVtechQuickRecover
{
    /// <summary>
    /// Interaction logic for ChooseFolderWindow.xaml
    /// </summary>
    public partial class ChooseFolderWindow : Window
    {
        public string SelectedFolder { get; private set; }
        public ChooseFolderWindow(string winTitle="choose folder")
        {

            InitializeComponent();
            this.Title = winTitle;

            //holdoff on this point for now.
            PopulateInputTreeViewAsync();
        }

        private async Task PopulateInputTreeViewAsync()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = drive.Name,
                    Tag = drive.RootDirectory.FullName
                };
                item.Expanded += FolderTreeViewItem_Expanded; // Hook the Expanded event
                SelectFolderTreeView.Items.Add(item);

                await PopulateSubdirectoriesAsync(item, 1);
            }
        }
        private async Task PopulateSubdirectoriesAsync(TreeViewItem parentItem, int levelsToIndex)
        {
            try
            {
                DirectoryInfo parentDirInfo = new DirectoryInfo(parentItem.Tag.ToString());
                DirectoryInfo[] subdirectories = parentDirInfo.GetDirectories();

                foreach (DirectoryInfo subdirectory in subdirectories)
                {
                    // Exclude system and hidden folders
                    if ((subdirectory.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                    {
                        TreeViewItem subItem = new TreeViewItem
                        {
                            Header = subdirectory.Name,
                            Tag = subdirectory.FullName
                        };
                        subItem.Expanded += FolderTreeViewItem_Expanded; // Hook the Expanded event for subitems
                        parentItem.Items.Add(subItem);

                        if (levelsToIndex > 0)
                        {
                            // Index one additional level of subdirectories for the current node
                            await PopulateSubdirectoriesAsync(subItem, levelsToIndex - 1);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access if needed
            }
        }
        private async Task PopulateSubdirectories(TreeViewItem parentItem, int levelsToIndex)
        {
            try
            {
                DirectoryInfo parentDirInfo = new DirectoryInfo(parentItem.Tag.ToString());
                DirectoryInfo[] subdirectories = parentDirInfo.GetDirectories();

                foreach (DirectoryInfo subdirectory in subdirectories)
                {
                    // Exclude system and hidden folders
                    if ((subdirectory.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                    {
                        TreeViewItem subItem = new TreeViewItem
                        {
                            Header = subdirectory.Name,
                            Tag = subdirectory.FullName
                        };
                        subItem.Expanded += FolderTreeViewItem_Expanded; // Hook the Expanded event for subitems
                        parentItem.Items.Add(subItem);

                        if (levelsToIndex > 0)
                        {
                            // Index one additional level of subdirectories for the current node
                            await PopulateSubdirectories(subItem, levelsToIndex - 1);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access if needed
            }
        }
        private async void ChooseFolderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await PopulateInputTreeViewAsync();
        }

        private void FolderTreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem expandedItem && expandedItem.Tag is string directory)
            {
                if (expandedItem.Items.Count == 1 && expandedItem.Items[0] is string dummyItem)
                {
                    expandedItem.Items.Clear(); // Remove the dummy item if present

                    try
                    {
                        DirectoryInfo parentDirInfo = new DirectoryInfo(directory);
                        DirectoryInfo[] subdirectories = parentDirInfo.GetDirectories();

                        foreach (DirectoryInfo subdirectory in subdirectories)
                        {
                            // Exclude system and hidden folders
                            if ((subdirectory.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                            {
                                TreeViewItem subItem = new TreeViewItem
                                {
                                    Header = subdirectory.Name,
                                    Tag = subdirectory.FullName
                                };
                                subItem.Items.Add("Loading..."); // Add a dummy item to indicate that it's loading

                                subItem.Expanded += SubFolderTreeViewItem_Expanded;
                                expandedItem.Items.Add(subItem);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Handle unauthorized access if needed
                    }
                }
            }
        }
        private void SubFolderTreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem expandedItem && expandedItem.Tag is string directory)
            {
                if (expandedItem.Items.Count == 1 && expandedItem.Items[0] is string dummyItem)
                {
                    expandedItem.Items.Clear(); // Remove the dummy item if present
                    expandedItem.Items.Add("Loading...");

                    LoadSubdirectoriesAsync(expandedItem);
                }
            }
        }

        private async void LoadSubdirectoriesAsync(TreeViewItem parentItem)
        {
            try
            {
                if (parentItem.Tag is string directory)
                {
                    DirectoryInfo parentDirInfo = new DirectoryInfo(directory);
                    DirectoryInfo[] subdirectories = parentDirInfo.GetDirectories();

                    int currentLevel = (int)parentItem.GetValue(LevelProperty);
                    currentLevel--; // Decrement the current level

                    foreach (DirectoryInfo subdirectory in subdirectories)
                    {
                        // Exclude system and hidden folders
                        if ((subdirectory.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                TreeViewItem subItem = new TreeViewItem
                                {
                                    Header = subdirectory.Name,
                                    Tag = subdirectory.FullName
                                };

                                if (currentLevel > 0)
                                {
                                    subItem.SetValue(LevelProperty, currentLevel); // Set the current level for the subitem
                                    subItem.Items.Add("Loading..."); // Add a dummy item to indicate that it's loading
                                    subItem.Expanded += SubFolderTreeViewItem_Expanded;
                                }

                                parentItem.Items.Add(subItem);
                            });
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access if needed
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectFolderTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag is string selectedFolder)
            {
                SelectedFolder = selectedFolder;
                Console.WriteLine("selected: " + SelectedFolder);
                DialogResult = true;
                
            }
        }
    }
}