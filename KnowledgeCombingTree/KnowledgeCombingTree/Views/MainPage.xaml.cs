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
using Windows.UI.Xaml.Navigation;
using Windows.Storage.AccessCache;

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

            //InitialHistoryAccessList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitialHistoryAccessList();
        }

        private async void InitialHistoryAccessList()
        {
            ViewModel.HistoryItems.Clear();
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (Windows.Storage.AccessCache.AccessListEntry entry in mru.Entries)
            {
                string mruToken = entry.Token;
                string mruMetadata = entry.Metadata;
                Windows.Storage.IStorageItem item = await mru.GetItemAsync(mruToken);
                // The type of item will tell you whether it's a file or a folder.
                foreach (TreeNode node in DbService.GetItemsByPath(item.Path))
                {
                    ViewModel.HistoryItems.Insert(0, node);
                }
            }
            foreach (Windows.Storage.AccessCache.AccessListEntry entry in mru.Entries)
            {
                string mruToken = entry.Token;
                string mruMetadata = entry.Metadata;
                Windows.Storage.IStorageItem item = await mru.GetItemAsync(mruToken);
            }
        }

        /*-------------------------------- UI��غ��� ------------------------------------*/
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            var text = SearchTextBox.Text;
            if (text == "") return;
            ViewModel.SearchedItems.Clear();
            foreach (var item in DbService.SearchText(text))
            {
                ViewModel.SearchedItems.Add(item);
            }
            if (ViewModel.SearchedItems.Count == 0)
            {
                var i = new MessageDialog("������������").ShowAsync();
            }
        }

        private void SecondSreachButton_Click(object sender, RoutedEventArgs e)
        {
            var text = SearchTextBox.Text;
            if (text == "") return;
            List<TreeNode> delNodes = new List<TreeNode>();
            foreach (var item in ViewModel.SearchedItems)
            {
                if (!item.getDescription().Contains(text) && !item.getName().Contains(text) && !item.getPath().Contains(text))
                {
                    delNodes.Add(item);
                }
            }
            if (delNodes.Count == ViewModel.SearchedItems.Count)
            {
                var i = new MessageDialog("ɸѡ��������").ShowAsync();
            }
            foreach (var delItem in delNodes)
            {
                ViewModel.SearchedItems.Remove(delItem);
            }
        }

        // ���ĳһ��֮��ִ�еĺ���
        private void ResultBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var root = (TreeNode)e.ClickedItem;
            ViewModel.SelectedItem = root;
            path.Text = root.getPath();
            name.Text = root.getName();
            description.Text = root.getDescription();
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void HistoryAccessBox_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode item = (TreeNode)HistoryAccessBox.SelectedItems[0];
            OpenFolder(item.getFeature_id());
        }

        private async void ResultBox_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode item = (TreeNode)ResultBox.SelectedItems[0];
            OpenFolder(item.getFeature_id());
            var target = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(item.getFeature_id());
            StorageApplicationPermissions.MostRecentlyUsedList.Add(target);
            foreach (TreeNode node in DbService.GetItemsByPath(target.Path))
            {
                ViewModel.HistoryItems.Insert(0, node);
            }
        }

        /*----------------------------------- api --------------------------------------*/
        // ���ļ���Դ��������һ��Ŀ¼
        // @param: Ҫ�򿪵�Ŀ¼��·��
        private async void OpenFolder(string feature_id)
        {
            try
            {
                //StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(sPath);
                //var tmp = AddInFeatureAccessList(folder);
                var target = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(feature_id);
                await Launcher.LaunchFolderAsync(target);

            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
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

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UploadModification();
            InfoGrid.Visibility = Visibility.Collapsed;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            InfoGrid.Visibility = Visibility.Collapsed;
        }

        // ���ĳ���ڵ���޸İ�ť�󣬽��ýڵ����˫�����ݰ󶨣����º����������ύ�޸�
        private void UploadModification()
        {
            if (ViewModel.SelectedItem != null)
            {
                //ViewModel.SelectedItem.name = name.Text;
                //ViewModel.SelectedItem.description = description.Text;
                UpdateNode(ViewModel.SelectedItem);
                ViewModel.SelectedItem = null;
            }
        }

        /*-----------------------------------operations on datebase--------------------------------*/
        private void UpdateNode(TreeNode node)
        {
            DbService.UpdateItem(node);
        }

        private void toggleSwitch1_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch1.IsOn)
            {
                name.IsReadOnly = false;
                description.IsReadOnly = false;
                Update.IsEnabled = true;
            }
            else
            {
                name.IsReadOnly = true;
                description.IsReadOnly = true;
                Update.IsEnabled = false;
            }
        }

    }
}
