using KnowledgeCombingTree.ViewModels;
using KnowledgeCombingTree.Models;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Search;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Popups;
using KnowledgeCombingTree.Services.DatabaseServices;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;

namespace KnowledgeCombingTree.Views
{
    public sealed partial class MainPage : Page
    {
        // 储存了所有的根节点
        // 将该根节点列表进行数据绑定实现UI
        //private ObservableCollection<TreeNode> roots { set; get; }

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            //roots = DbService.GetRootItems("-1");
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            var text = SearchTextBox.Text;
            ViewModel.SearchedItems.Clear();
            foreach (var item in DbService.SearchText(text))
            {
                ViewModel.SearchedItems.Add(item);
            }
        }

        private void SecondSraechButton_Click(object sender, RoutedEventArgs e)
        {
            var text = SecondSearchTextBox.Text;
            List<TreeNode> delNodes = new List<TreeNode>();
            foreach (var item in ViewModel.SearchedItems)
            {
                if (!item.getDescription().Contains(text) && !item.getName().Contains(text) && !item.getPath().Contains(text))
                {
                    delNodes.Add(item);
                }
            }
            foreach (var delItem in delNodes)
            {
                ViewModel.SearchedItems.Remove(delItem);
            }
        }

        // 用folderpicker选择一个文件夹(目录)，自动扫描其所有子目录并生成子节点存入数据库
        // 自动生成一棵树
        private async void test_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await ChooseFolder();
            if (folder != null)
            {
                // 选择了一个目录，创建一个根节点
                string pid = CreateTree(folder.Path, folder.Name);
                // 接下来遍历找出所有子目录
                TraverseSubFolders(folder, pid);
            }
            else
            {
                var i = new MessageDialog("选择文件夹失败").ShowAsync();
            }
        }

        // 用文件资源管理器打开一个目录
        private async void test2_Click(object sender, RoutedEventArgs e)
        {
            string sPath = @"E:\学习资料\现操\week10";  // 要打开的路径

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(sPath);
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        // 自定义建树时，只创建一棵树的根节点
        private async void ChooseAndCreateTreeRoot()
        {
            StorageFolder folder = await ChooseFolder();
            string pid = CreateTree(folder.Path, folder.Name);
        }

        // 创建一个子节点
        // @param: 父节点
        // @return: 创建的子节点
        private async Task<TreeNode> ChooseAndCreateChildNode(TreeNode parent_node)
        {
            StorageFolder folder = await ChooseFolder();
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), parent_node.getPath(), parent_node.getName());
            return node;
        }


        /*---------------------------------------------------------------------*/

        // 选择一个文件夹并返回
        public async Task<StorageFolder> ChooseFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            // 弹出选择框让用户选择
            IAsyncOperation<StorageFolder> folderTask = folderPicker.PickSingleFolderAsync();
            StorageFolder folder = await folderTask;

            return folder;
        }

        // @param: StorageFolder
        // 扫描所有子目录并将其保存如数据库
        public async void TraverseSubFolders(StorageFolder folder, string pid)
        {
            // Get the files in the subfolders of the user's Pictures folder
            // details: https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder
            IReadOnlyList<StorageFolder> groupedItems = await folder.GetFoldersAsync(CommonFolderQuery.DefaultQuery);

            // Iterate over the results and print the list of folders
            // and files to the Visual Studio Output window.
            foreach (StorageFolder subfolder in groupedItems)
            {
                Debug.WriteLine(subfolder.Name);

                // 创建子节点
                CreateChildNode(pid, folder.Path, subfolder.Name);

                // To iterate over the files in each folder, uncomment the following lines. 
                // foreach(StorageFile file in await folder.GetFilesAsync())
                //    Debug.WriteLine(" " + file.Name);
            }
        }

        private string CreateTree(string path, string name)
        {
            TreeNode root = new TreeNode("-1", 0, path, name, "", "");
            DbService.AddItem(root);
            return root.getId();
        }

        private void CreateChildNode(string pid, string path, string name)
        {
            TreeNode node = new TreeNode(pid, 1, path + "\\" + name, name, "", "");
            DbService.AddItem(node);
        }

        private TreeNode CreateAndGetChildNode(string pid, string path, string name)
        {
            TreeNode node = new TreeNode(pid, 1, path + "\\" + name, name, "", "");
            DbService.AddItem(node);
            return node;
        }

        
    }
}
