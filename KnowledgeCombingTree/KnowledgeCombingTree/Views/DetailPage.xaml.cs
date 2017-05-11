using KnowledgeCombingTree.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
<<<<<<< HEAD
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
=======
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer.DragDrop;
using System;
using Windows.UI.Popups;
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
using System.Net;
using System.IO;

namespace KnowledgeCombingTree.Views
{
    public sealed partial class DetailPage : Page
    {
        //private DetailPageViewModel ViewModel;

        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
<<<<<<< HEAD

//            ViewModel.SelectedItem = null;
        }




        /*----------------------------------- api --------------------------------*/
        // 用文件资源管理器打开一个目录
        // @param: 要打开的目录的路径
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

        // 选中一棵树的根节点时调用此函数，用于更新子节点列表
        // @param: 根节点
        private void UpdateChildrenNodes(TreeNode root)
        {
            ViewModel.ChildrenItems = DbService.GetItemsByParentId(root.getParentId());
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
            UploadModification();
        }

        // 点击某个节点的修改按钮后，将该节点进行双向数据绑定，以下函数则用于提交修改
        private void UploadModification()
        {
            if (ViewModel.SelectedItem != null)
            {
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
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), parent_node.getPath(), parent_node.getName());
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
            DbService.AddItem(root);
            return root.getId();
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

        /*-----------------------------------------拖拽相关------------------------------------------*/
=======
            
        }

>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
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
<<<<<<< HEAD
                    TreeNode new_node = new TreeNode("-1", 0, file.Path, "", "", "");
                    ViewModel.AddTreeNode(new_node);
=======
                    ViewModel.AddFolderItem(file.Path);
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
<<<<<<< HEAD
                    TreeNode new_node = new TreeNode("-1", 0, folder.Path, "", "", "");
                    ViewModel.AddTreeNode(new_node);
=======
                    ViewModel.AddFolderItem(folder.Path);
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
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

<<<<<<< HEAD
        Models.TreeNode DelItem;
=======
        Models.FolderItem DelItem;
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8

        // 拖拽完成执行的函数(拖拽删除区域) 
        private void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
<<<<<<< HEAD
            ViewModel.RemoveTreeNode(DelItem);
=======
            ViewModel.RemoveFolderItem(DelItem);
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
        }

        // 拖拽过程中执行的函数(拖拽删除区域)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
<<<<<<< HEAD
        }

        // 开始拖拽Item以准备删除
        private void FolderList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }

        // ListView的点击执行函数
        private void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TreeNode)(e.ClickedItem);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Title.Text = ViewModel.SelectedItem.getName();
            Details.Text = ViewModel.SelectedItem.getDescription();
            Path.Text = ViewModel.SelectedItem.getPath();
            //URL.Text = ViewModel.SelectedItem.URL;
        }

        // update按钮的点击执行函数
        private void updateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UploadModification();
            //    ViewModel.UpdateTreeNode(Title.Text, Details.Text, Path.Text, URL.Text);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // cancel按钮的点击执行函数
        private void cancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // openURLButton的点击执行函数
        private async void openURLButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // 建立文件
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var local = ApplicationData.Current.LocalFolder;
            var localStorageFolder = await local.CreateFolderAsync("File", CreationCollisionOption.OpenIfExists);
            var file = await localStorageFolder.CreateFileAsync("web.html", CreationCollisionOption.GenerateUniqueName);
            var url = URL.Text;

            // 将网页转化成byte数组
            List<Byte> allbytes = new List<byte>();
            using (var response = await WebRequest.Create(url).GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[4000];
                    int bytesRead = 0;
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, 4000)) > 0)
                    {
                        allbytes.AddRange(buffer.Take(bytesRead));
                    }
                }
            }
            await FileIO.WriteBytesAsync(file, allbytes.ToArray());

            // 从文件中读取内容转化成字符串，显示在WebView上
            var webStringTemp = await FileIO.ReadTextAsync(file);
            var webString = webStringTemp.ToString();
            web.NavigateToString(webString);
=======
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
        }

        // 开始拖拽Item以准备删除
        private void FolderList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.FolderItem;
        }

        // ListView的点击执行函数
        private void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.FolderItem)(e.ClickedItem);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Title.Text = ViewModel.SelectedItem.Title;
            Details.Text = ViewModel.SelectedItem.Details;
            Path.Text = ViewModel.SelectedItem.FolderPath;
            URL.Text = ViewModel.SelectedItem.URL;
        }

        // update按钮的点击执行函数
        private void updateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.UpdateFolderItem(Title.Text, Details.Text, Path.Text, URL.Text);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // cancel按钮的点击执行函数
        private void cancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // openURLButton的点击执行函数
        private async void openURLButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // 建立文件
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var local = ApplicationData.Current.LocalFolder;
            var localStorageFolder = await local.CreateFolderAsync("File", CreationCollisionOption.OpenIfExists);
            var file = await localStorageFolder.CreateFileAsync("web.html", CreationCollisionOption.GenerateUniqueName);
            var url = URL.Text;

            // 将网页转化成byte数组
            List<Byte> allbytes = new List<byte>();
            using (var response = await WebRequest.Create(url).GetResponseAsync()) {
                using (Stream responseStream = response.GetResponseStream()) {
                    byte[] buffer = new byte[4000];
                    int bytesRead = 0;
                    while ((bytesRead = await responseStream.ReadAsync(buffer,0,4000)) > 0) {
                        allbytes.AddRange(buffer.Take(bytesRead));
                    }
                }
            }
            await FileIO.WriteBytesAsync(file, allbytes.ToArray());

            // 从文件中读取内容转化成字符串，显示在WebView上
            var webStringTemp = await FileIO.ReadTextAsync(file);
            var webString = webStringTemp.ToString();
            web.NavigateToString(webString);
        }
    }
}