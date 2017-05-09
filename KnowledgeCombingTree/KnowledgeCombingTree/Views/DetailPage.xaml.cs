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
                    ViewModel.AddFolderItem(file.Path);
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
                    ViewModel.AddFolderItem(folder.Path);
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

        Models.FolderItem DelItem;

        // ��ק���ִ�еĺ���(��קɾ������) 
        private void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            ViewModel.RemoveFolderItem(DelItem);
        }

        // ��ק������ִ�еĺ���(��קɾ������)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
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