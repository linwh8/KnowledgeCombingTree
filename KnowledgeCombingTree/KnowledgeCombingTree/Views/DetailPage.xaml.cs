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
using Windows.UI;
using Windows.Storage.AccessCache;

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

            this.DataContextChanged += (s, e) => Bindings.Update();
        }




        /*----------------------------------- api --------------------------------*/
        // ���ļ���Դ��������һ��Ŀ¼
        // @param: Ҫ�򿪵�Ŀ¼��·��
        private async void OpenFolder(string sPath)
        {
            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(sPath);
                var tmp = StorageApplicationPermissions.FutureAccessList.Add(folder);
                var target = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(tmp);
                await Launcher.LaunchFolderAsync(target);
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
            ViewModel.ChildrenItems.Clear();
            foreach (var node in DbService.GetItemsByParentId(root.getId()))
            {
                ViewModel.ChildrenItems.Add(node);
            }
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
            ViewModel.ChildrenItems.Add(new_node);
            UploadModification();
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
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), folder.Path, folder.Name);
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
            TreeNode node = new TreeNode(pid, 1, path, name, "", "");
            DbService.AddItem(node);
            return node;
        }

        

        /*-----------------------------------operations on datebase--------------------------------*/
        private void UpdateNode(TreeNode node)
        {
            DbService.UpdateItem(node);
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
                    //TreeNode new_node = new TreeNode("-1", 0, file.Path, "", "", "");
                    //ViewModel.AddTreeNode(new_node);
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
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
            /*
                         ViewModel.ChildrenItems.Clear();
            foreach (var node in DbService.GetItemsByParentId(root.getId()))
            {
                ViewModel.ChildrenItems.Add(node);
            }
             */
            if (DelItem.getLevel() == 0) {
                foreach (var node in ViewModel.ChildrenItems) {
                    DbService.DeleteItem(node.getId());
                }
                ViewModel.ChildrenItems.Clear();
            }
            DbService.DeleteItem(DelItem.getId());
            ViewModel.RemoveTreeNode(DelItem);
        }

        // ��ק������ִ�еĺ���(��קɾ������)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
        }

        // ���б���קʵ��
        private async void ChildList_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                Debug.WriteLine("[Info] DataView Contains StorageItems");
                var items = await e.DataView.GetStorageItemsAsync();
                try
                {
                    StorageFile file = items.OfType<StorageFile>().First();
                    //             TreeNode node = new TreeNode(pid, 1, path + "\\" + name, name, "", "");
                    if (file != null)
                    {
                        TreeNode new_node = new TreeNode(ViewModel.SelectedItem.getId(), 1, file.Path + "\\" + file.Name, file.Name, "", "");
                        ViewModel.AddTreeNode(new_node);
                        DbService.AddItem(new_node);
                    }
                    else
                    {
                        var i = new MessageDialog("ѡ���ļ�ʧ��").ShowAsync();
                    }
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
                    if (folder != null)
                    {
                        TreeNode new_node = new TreeNode(ViewModel.SelectedItem.getId(), 1, folder.Path + "\\" + folder.Name, folder.Name, "", "");
                        ViewModel.AddTreeNode(new_node);
                        DbService.AddItem(new_node);
                    }
                    else
                    {
                        var i = new MessageDialog("ѡ���ļ���ʧ��").ShowAsync();
                    }
                }
            }
        }

        // ��ʼ��קItem��׼��ɾ��
        private void RootList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }

        /*-----------------------------------------��ק���------------------------------------------*/
        private void RootList_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode clickedItem = (TreeNode)e.OriginalSource;
            ViewModel.SelectedItem = clickedItem;
            //Create
        }

        // ������Ŀ����Ŀ¼
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            //ChooseAndCreateTreeRoot();
            CreateTreeAuto();
        }

        // RootList��ItemClick������
        private void RootList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var root = (TreeNode)e.ClickedItem;
            ViewModel.SelectedItem = root;
            UpdateChildrenNodes(root);
            path.Text = root.getPath();
            name.Text = root.getName();
            description.Text = root.getDescription();
            InfoGrid.Visibility = Visibility.Visible;
        }

        // ChildList��ItemClick������
        private void ChildList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var child = (TreeNode)e.ClickedItem;
            ViewModel.SelectedItem = (TreeNode)e.ClickedItem;
            path.Text = child.getPath();
            name.Text = child.getName();
            description.Text = child.getDescription();
            InfoGrid.Visibility = Visibility.Visible;
            OpenFolder(child.path);
        }

        // RootList ��Item ����һ�������
        private void RootList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedItem == null) return;
            MenuFlyout rootFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "AddChild" };
            MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "DeleteAllChildren"};
            firstItem.Click += AddChild;
            //secondItem.Click +=;
            rootFlyout.Items.Add(firstItem);
            rootFlyout.Items.Add(secondItem);
            rootFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
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

        private void AddChild(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null) {
                AddChildNode((TreeNode)ViewModel.SelectedItem);
            }
        }

        private void toggleSwitch1_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch1.IsOn)
            {
                name.IsReadOnly = false;
                description.IsReadOnly = false;
                Update.IsEnabled = true;
            }
            else {
                name.IsReadOnly = true;
                description.IsReadOnly = true;
                Update.IsEnabled = false;
            }
        }
    }
}

