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

namespace KnowledgeCombingTree.Views
{
    public sealed partial class MainPage : Page
    {
        // ���������еĸ��ڵ�
        // ���ø��ڵ��б�������ݰ�ʵ��UI
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

        // ��folderpickerѡ��һ���ļ���(Ŀ¼)���Զ�ɨ����������Ŀ¼�������ӽڵ�������ݿ�
        // �Զ�����һ����
        private async void test_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await ChooseFolder();
            if (folder != null)
            {
                // ѡ����һ��Ŀ¼������һ�����ڵ�
                string pid = CreateTree(folder.Path, folder.Name);
                // �����������ҳ�������Ŀ¼
                TraverseSubFolders(folder, pid);
            }
            else
            {
                var i = new MessageDialog("ѡ���ļ���ʧ��").ShowAsync();
            }
        }

        // ���ļ���Դ��������һ��Ŀ¼
        private async void test2_Click(object sender, RoutedEventArgs e)
        {
            string sPath = @"E:\ѧϰ����\�ֲ�\week10";  // Ҫ�򿪵�·��

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

        // �Զ��彨��ʱ��ֻ����һ�����ĸ��ڵ�
        private async void ChooseAndCreateTreeRoot()
        {
            StorageFolder folder = await ChooseFolder();
            string pid = CreateTree(folder.Path, folder.Name);
        }

        // ����һ���ӽڵ�
        // @param: ���ڵ�
        // @return: �������ӽڵ�
        private async Task<TreeNode> ChooseAndCreateChildNode(TreeNode parent_node)
        {
            StorageFolder folder = await ChooseFolder();
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), parent_node.getPath(), parent_node.getName());
            return node;
        }


        /*---------------------------------------------------------------------*/

        // ѡ��һ���ļ��в�����
        public async Task<StorageFolder> ChooseFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            // ����ѡ������û�ѡ��
            IAsyncOperation<StorageFolder> folderTask = folderPicker.PickSingleFolderAsync();
            StorageFolder folder = await folderTask;

            return folder;
        }

        // @param: StorageFolder
        // ɨ��������Ŀ¼�����䱣�������ݿ�
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

                // �����ӽڵ�
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


        private void test3_Click(object sender, RoutedEventArgs e)
        {
            string id = "";
            TreeNode t = DbService.GetItem(id);
        }


    }
}
