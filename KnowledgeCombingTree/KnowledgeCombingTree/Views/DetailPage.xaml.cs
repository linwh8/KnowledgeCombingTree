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

        // ѡ��һ�����ĸ��ڵ�ʱ���ô˺��������ڸ����ӽڵ��б�
        // @param: ���ڵ�
        private void UpdateChildrenNodes(TreeNode root)
        {
            ViewModel.ChildrenItems = DbService.GetItemsByParentId(root.getParentId());
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

        /*-----------------------------------------��ק���------------------------------------------*/
=======
            
        }

>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
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

<<<<<<< HEAD
        Models.TreeNode DelItem;
=======
        Models.FolderItem DelItem;
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8

        // ��ק���ִ�еĺ���(��קɾ������) 
        private void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
<<<<<<< HEAD
            ViewModel.RemoveTreeNode(DelItem);
=======
            ViewModel.RemoveFolderItem(DelItem);
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
        }

        // ��ק������ִ�еĺ���(��קɾ������)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
<<<<<<< HEAD
        }

        // ��ʼ��קItem��׼��ɾ��
        private void FolderList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }

        // ListView�ĵ��ִ�к���
        private void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TreeNode)(e.ClickedItem);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Title.Text = ViewModel.SelectedItem.getName();
            Details.Text = ViewModel.SelectedItem.getDescription();
            Path.Text = ViewModel.SelectedItem.getPath();
            //URL.Text = ViewModel.SelectedItem.URL;
        }

        // update��ť�ĵ��ִ�к���
        private void updateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UploadModification();
            //    ViewModel.UpdateTreeNode(Title.Text, Details.Text, Path.Text, URL.Text);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // cancel��ť�ĵ��ִ�к���
        private void cancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // openURLButton�ĵ��ִ�к���
        private async void openURLButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // �����ļ�
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var local = ApplicationData.Current.LocalFolder;
            var localStorageFolder = await local.CreateFolderAsync("File", CreationCollisionOption.OpenIfExists);
            var file = await localStorageFolder.CreateFileAsync("web.html", CreationCollisionOption.GenerateUniqueName);
            var url = URL.Text;

            // ����ҳת����byte����
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

            // ���ļ��ж�ȡ����ת�����ַ�������ʾ��WebView��
            var webStringTemp = await FileIO.ReadTextAsync(file);
            var webString = webStringTemp.ToString();
            web.NavigateToString(webString);
=======
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
        }

        // ��ʼ��קItem��׼��ɾ��
        private void FolderList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.FolderItem;
        }

        // ListView�ĵ��ִ�к���
        private void FolderList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.FolderItem)(e.ClickedItem);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Title.Text = ViewModel.SelectedItem.Title;
            Details.Text = ViewModel.SelectedItem.Details;
            Path.Text = ViewModel.SelectedItem.FolderPath;
            URL.Text = ViewModel.SelectedItem.URL;
        }

        // update��ť�ĵ��ִ�к���
        private void updateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.UpdateFolderItem(Title.Text, Details.Text, Path.Text, URL.Text);
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // cancel��ť�ĵ��ִ�к���
        private void cancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ItemSetting.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            web.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        // openURLButton�ĵ��ִ�к���
        private async void openURLButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // �����ļ�
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ItemImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var local = ApplicationData.Current.LocalFolder;
            var localStorageFolder = await local.CreateFolderAsync("File", CreationCollisionOption.OpenIfExists);
            var file = await localStorageFolder.CreateFileAsync("web.html", CreationCollisionOption.GenerateUniqueName);
            var url = URL.Text;

            // ����ҳת����byte����
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

            // ���ļ��ж�ȡ����ת�����ַ�������ʾ��WebView��
            var webStringTemp = await FileIO.ReadTextAsync(file);
            var webString = webStringTemp.ToString();
            web.NavigateToString(webString);
        }
    }
}