using KnowledgeCombingTree.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
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
using System.Net;
using System.IO;

namespace KnowledgeCombingTree.Views
{
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            
        }

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
                    ViewModel.AddFolderItem(file.Path);
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
                    ViewModel.AddFolderItem(folder.Path);
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

        Models.FolderItem DelItem;

        // 拖拽完成执行的函数(拖拽删除区域) 
        private void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            ViewModel.RemoveFolderItem(DelItem);
        }

        // 拖拽过程中执行的函数(拖拽删除区域)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
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