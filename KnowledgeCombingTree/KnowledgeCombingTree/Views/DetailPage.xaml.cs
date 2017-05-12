using KnowledgeCombingTree.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using KnowledgeCombingTree.Models;
using Windows.UI.Xaml;
using Windows.Storage.Pickers;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Popups;
using KnowledgeCombingTree.Services.DatabaseServices;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Windows.Storage.Search;

namespace KnowledgeCombingTree.Views
{
    public sealed partial class DetailPage : Page
    {
        //private DetailPageViewModel ViewModel;

        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            //            ViewModel.SelectedItem = null;
            ViewModel.RootItems = DbService.GetItemsByParentId("-1");
        }




        /*----------------------------------- api --------------------------------*/
        // ���ļ���Դ��������һ��Ŀ¼
        // @param: Ҫ�򿪵�Ŀ¼��·��
        private async void OpenFolder(string sPath)
        {
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

        // ��folderpickerѡ��һ���ļ���(Ŀ¼)���Զ�ɨ����������Ŀ¼�������ӽڵ�������ݿ�
        // �Զ�����һ����
        private async void CreateTreeAuto()
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

        // ѡ��һ�����ĸ��ڵ�ʱ���ô˺��������ڸ����ӽڵ��б�
        // @param: ���ڵ�
        private void UpdateChildrenNodes(TreeNode root)
        {
            ViewModel.ChildrenItems = DbService.GetItemsByParentId(root.getId());
        }

        // ���һ�����ڵ㣺
        private async void ChooseAndCreateTreeRoot()
        {
            StorageFolder folder = await ChooseFolder();
            string pid = CreateTree(folder.Path, folder.Name);
        }

        // ���һ���ӽڵ㣺
        // 1. ѡ��һ��·��������һ���ڵ㣬��ʱdescriptionΪ"",imageΪ"default.png"
        // 2. ����ViewModel.SelectedItemΪ���½��ڵ㣬���б༭
        // 3. �༭���֮���ύ�޸ģ���ViewModel.SelectedItem��������Ϊnull
        // @param: ���ڵ�
        private async void AddChildNode(TreeNode parent_node)
        {
            TreeNode new_node = await ChooseAndCreateChildNode(parent_node);
            ViewModel.SelectedItem = new_node;
            UploadModification();
        }

        // ���ĳ���ڵ���޸İ�ť�󣬽��ýڵ����˫�����ݰ󶨣����º����������ύ�޸�
        private void UploadModification()
        {
            if (ViewModel.SelectedItem != null)
            {
                UpdateNode(ViewModel.SelectedItem);
                ViewModel.SelectedItem = null;
            }
        }

        // ���û����Դӱ���ѡ���ϴ�ͼƬ,���½ڵ��image�����Լ�����UI�е�ͼƬ
        private async void EditNodeImage(TreeNode node)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                // ɸѡͼƬ��ʽ�������¸�ʽ�޷��ϴ�
                FileTypeFilter = { ".jpg", ".png" }
            };
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // ѡ��ͼƬ�ɹ����ø�ͼƬ�滻��UI�ϵľ�ͼƬ������Source����
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    //bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    // imageΪxaml�ļ��ж����ĳһ���ڵ��һ��Image��ǩ��Name���Ե�ֵ
                    //image.Source = bitmapImage;
                    //image.Stretch = Stretch.Fill;
                }

                string postfix = file.Name.Substring(file.Name.Length - 4);
                string id = Guid.NewGuid().ToString();
                node.setImage("ms-appdata:///Local/" + id + postfix);
                // ���ļ����浽�û���Ŀ¼��
                await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, id + postfix, Windows.Storage.NameCollisionOption.ReplaceExisting);
            }
            else
            {
                var i = new MessageDialog("ȡ���ɹ�").ShowAsync();
                return;
            }
        }

        /*-------------------------------some driver functions-----------------------------------*/

        // ����һ���ӽڵ�
        // @param: ���ڵ�
        // @return: �������ӽڵ�
        private async Task<TreeNode> ChooseAndCreateChildNode(TreeNode parent_node)
        {
            StorageFolder folder = await ChooseFolder();
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), parent_node.getPath(), parent_node.getName());
            return node;
        }

        // ѡ��һ���ļ��в�����
        // @return: folder created just now
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

        // ����һ�����ڵ�
        private string CreateTree(string path, string name)
        {
            TreeNode root = new TreeNode("-1", 0, path, name, "", "");
            ViewModel.AddTreeNode(root);
            DbService.AddItem(root);
            return root.getId();
        }

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

        

        /*-----------------------------------operations on datebase--------------------------------*/
        private void UpdateNode(TreeNode node)
        {
            DbService.UpdateItem(node);
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            //ChooseAndCreateTreeRoot();
            CreateTreeAuto();
        }

        private void RootList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var root = (TreeNode)e.ClickedItem;
            UpdateChildrenNodes(root);
        }

        /*-----------------------------------------��ק���------------------------------------------*/
        // ��ק��ɺ�ִ�еĺ���(��ק��������)
        private async void Border_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                Debug.WriteLine("[Info] DataView Contains StorageItems");
                var items = await e.DataView.GetStorageItemsAsync();
                try
                {
                    StorageFile file = items.OfType<StorageFile>().First();
                    TreeNode new_node = new TreeNode("-1", 0, file.Path, "", "", "");
                    ViewModel.AddTreeNode(new_node);
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
                    TreeNode new_node = new TreeNode("-1", 0, folder.Path, "", "", "");
                    ViewModel.AddTreeNode(new_node);
                }
            }
        }

        // ��קʱִ�еĺ���(��ק��������)
        private void Border_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            Debug.WriteLine("[Info] DragOver");
            // ���ò�������
            e.AcceptedOperation = DataPackageOperation.Copy;
            // ������ʾ����
            e.DragUIOverride.Caption = "Drag here can add the folder";
            // �Ƿ���ʾ�Ϸ�ʱ�����֣�Ĭ��Ϊtrue
            e.DragUIOverride.IsCaptionVisible = true;
            // �Ƿ���ʾ�ļ�ͼ�꣬Ĭ��Ϊtrue
            e.DragUIOverride.IsContentVisible = true;
            // Captionǰ���ͼ���Ƿ���ʾ
            e.DragUIOverride.IsGlyphVisible = true;

        }


        Models.TreeNode DelItem;

        // ��ק���ִ�еĺ���(��קɾ������) 
        private void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            ViewModel.RemoveTreeNode(DelItem);
        }

        // ��ק������ִ�еĺ���(��קɾ������)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
        }

        // ��ʼ��קItem��׼��ɾ��
        private void RootList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }
    }
}

