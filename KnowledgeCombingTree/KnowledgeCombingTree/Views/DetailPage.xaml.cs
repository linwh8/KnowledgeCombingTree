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
        // 用文件资源管理器打开一个目录
        // @param: 要打开的目录的路径
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

        // 用folderpicker选择一个文件夹(目录)，自动扫描其所有子目录并生成子节点存入数据库
        // 自动生成一棵树
        private async void CreateTreeAuto()
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

        // 选中一棵树的根节点时调用此函数，用于更新子节点列表
        // @param: 根节点
        private void UpdateChildrenNodes(TreeNode root)
        {
            ViewModel.ChildrenItems.Clear();
            foreach (var node in DbService.GetItemsByParentId(root.getId()))
            {
                ViewModel.ChildrenItems.Add(node);
            }
        }

        // 添加一个根节点：
        private async void ChooseAndCreateTreeRoot()
        {
            StorageFolder folder = await ChooseFolder();
            string pid = CreateTree(folder.Path, folder.Name);
        }

        // 添加一个子节点：
        // 1. 选择一个路径，生成一个节点，此时description为"",image为"default.png"
        // 2. 设置ViewModel.SelectedItem为该新建节点，进行编辑
        // 3. 编辑完成之后提交修改，将ViewModel.SelectedItem重新设置为null
        // @param: 父节点
        private async void AddChildNode(TreeNode parent_node)
        {
            TreeNode new_node = await ChooseAndCreateChildNode(parent_node);
            ViewModel.SelectedItem = new_node;
            ViewModel.ChildrenItems.Add(new_node);
            UploadModification();
        }

        // 点击某个节点的修改按钮后，将该节点进行双向数据绑定，以下函数则用于提交修改
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

        // 让用户可以从本地选择并上传图片,更新节点的image属性以及更新UI中的图片
        private async void EditNodeImage(TreeNode node)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                // 筛选图片格式，非以下格式无法上传
                FileTypeFilter = { ".jpg", ".png" }
            };
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // 选择图片成功后用该图片替换掉UI上的旧图片：设置Source属性
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    //bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    // image为xaml文件中定义的某一个节点的一个Image标签的Name属性的值
                    //image.Source = bitmapImage;
                    //image.Stretch = Stretch.Fill;
                }

                string postfix = file.Name.Substring(file.Name.Length - 4);
                string id = Guid.NewGuid().ToString();
                node.setImage("ms-appdata:///Local/" + id + postfix);
                // 将文件储存到用户的目录下
                await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, id + postfix, Windows.Storage.NameCollisionOption.ReplaceExisting);
            }
            else
            {
                var i = new MessageDialog("取消成功").ShowAsync();
                return;
            }
        }

        /*-------------------------------some driver functions-----------------------------------*/

        // 创建一个子节点
        // @param: 父节点
        // @return: 创建的子节点
        private async Task<TreeNode> ChooseAndCreateChildNode(TreeNode parent_node)
        {
            StorageFolder folder = await ChooseFolder();
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), folder.Path, folder.Name);
            return node;
        }

        // 选择一个文件夹并返回
        // @return: folder created just now
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

        // 创建一个根节点
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

                // 创建子节点
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

        /*-----------------------------------------拖拽相关------------------------------------------*/
        // 拖拽完成后执行的函数(拖拽接受区域)
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
            }
        }

        // 拖拽时执行的函数(拖拽接受区域)
        private void Border_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            Debug.WriteLine("[Info] DragOver");
            // 设置操作类型
            e.AcceptedOperation = DataPackageOperation.Copy;
            // 设置提示文字
            e.DragUIOverride.Caption = "Drag here can add the folder";
            // 是否显示拖放时的文字，默认为true
            e.DragUIOverride.IsCaptionVisible = true;
            // 是否显示文件图标，默认为true
            e.DragUIOverride.IsContentVisible = true;
            // Caption前面的图标是否显示
            e.DragUIOverride.IsGlyphVisible = true;

        }


        Models.TreeNode DelItem;

        // 拖拽完成执行的函数(拖拽删除区域) 
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

        // 拖拽过程中执行的函数(拖拽删除区域)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
        }

        // 子列表拖拽实现
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
                        var i = new MessageDialog("选择文件失败").ShowAsync();
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
                        var i = new MessageDialog("选择文件夹失败").ShowAsync();
                    }
                }
            }
        }

        // 开始拖拽Item以准备删除
        private void RootList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }

        /*-----------------------------------------拖拽相关------------------------------------------*/
        private void RootList_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode clickedItem = (TreeNode)e.OriginalSource;
            ViewModel.SelectedItem = clickedItem;
            //Create
        }

        // 增加项目到根目录
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            //ChooseAndCreateTreeRoot();
            CreateTreeAuto();
        }

        // RootList的ItemClick处理函数
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

        // ChildList的ItemClick处理函数
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

        // RootList 的Item 鼠标右击处理函数
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

