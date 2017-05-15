using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KnowledgeCombingTree.ViewModels;
using KnowledgeCombingTree.Services.DatabaseServices;
using Windows.UI.Popups;
using KnowledgeCombingTree.Models;
using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace KnowledgeCombingTree.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InternetPage : Page
    {
        InternetPageViewModel ViewModel = new InternetPageViewModel();
        private bool CreatingNewNode = false;

        public InternetPage()
        {
            this.InitializeComponent();

            ViewModel.RootItems = DbService.GetRootUrlItems();
        }

        /*------------------------------------- api ------------------------------------*/
        private void CreateRootNode()
        {
            CreatingNewNode = true;
            // 所有url的节点创建的时候level都赋值为-1
            ViewModel.SelectedItem = new Models.TreeNode("-1", -1, "", "", "", "");
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void CreateChildNode(string parent_id)
        {
            CreatingNewNode = true;
            ViewModel.SelectedItem = new Models.TreeNode(parent_id, -1, "", "", "", "");
            path.Text = ViewModel.SelectedItem.getPath();
            name.Text = ViewModel.SelectedItem.getName();
            description.Text = ViewModel.SelectedItem.getDescription();
            toggleSwitch1.IsOn = true;
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void EditNode(TreeNode node)
        {
            CreatingNewNode = false;
            ViewModel.SelectedItem = node;
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void CreateNode()
        {
            if (ViewModel.SelectedItem == null)
            {
                var i = new MessageDialog("没有正在编辑的节点").ShowAsync();
                return;
            }
            DbService.AddItem(ViewModel.SelectedItem);
            if (ViewModel.SelectedItem.getParentId() == "-1")
                ViewModel.RootItems.Add(ViewModel.SelectedItem);
            else
                ViewModel.ChildrenItems.Add(ViewModel.SelectedItem);
            ViewModel.SelectedItem = null;
            CreatingNewNode = false;
            InfoGrid.Visibility = Visibility.Collapsed;
        }

        // 点击某个节点的修改按钮后，将该节点进行双向数据绑定，以下函数则用于提交修改
        private void UploadModification()
        {
            if (ViewModel.SelectedItem != null)
            {
                if (ViewModel.SelectedItem.getParentId() == "-1")
                {
                    ViewModel.RootItems.Remove(ViewModel.RootItems.Where(x => x.getId() == ViewModel.SelectedItem.getId()).FirstOrDefault());
                    ViewModel.RootItems.Add(ViewModel.SelectedItem);
                }
                else
                {
                    ViewModel.ChildrenItems.Remove(ViewModel.ChildrenItems.Where(x => x.getId() == ViewModel.SelectedItem.getId()).FirstOrDefault());
                    ViewModel.ChildrenItems.Add(ViewModel.SelectedItem);
                }
                UpdateNode(ViewModel.SelectedItem);
                ViewModel.SelectedItem = null;
                ViewModel.SelectedItem = null;
                InfoGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateNode()
        {
            if (ViewModel.SelectedItem == null)
            {
                var i = new MessageDialog("没有正在编辑的节点").ShowAsync();
                return;
            }
            DbService.UpdateItem(ViewModel.SelectedItem);
            ViewModel.SelectedItem = null;
            InfoGrid.Visibility = Visibility.Collapsed;
        }

        private void CancelOperationsOnNode()
        {
            ViewModel.SelectedItem = null;
            CreatingNewNode = false;
        }

        private async void OpenUrl(string url)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        

        private void UpdateNode(TreeNode node)
        {
            DbService.UpdateItem(node);
        }

        /***************************************** UI相关函数 *******************************************/
        /*-----------------------------------------拖拽相关------------------------------------------*/
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
        private async void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            var msgDialog = DelItem.getParentId() == "-1" ?
                            new Windows.UI.Popups.MessageDialog("确定删除该节点(包括其子节点)？") { Title = "删除提示" } :
                            new Windows.UI.Popups.MessageDialog("确定删除该节点？") { Title = "删除提示" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { this.DeleteItem(); }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand => { this.CancelDelete(); }));
            await msgDialog.ShowAsync();
        }

        private void DeleteItem()
        {
            if (DelItem.getId() == ViewModel.SelectedItem.getId())
            {
                // 如果删除的节点是SelectedItem，则删除之后应该把编辑栏隐藏
                InfoGrid.Visibility = Visibility.Collapsed;
            }

            if (DelItem.getParentId() == "-1")
            {
                if (ViewModel.ChildrenItems.Count > 0 && ViewModel.ChildrenItems.ElementAt(0).getParentId() == DelItem.getId())
                {
                    ViewModel.ChildrenItems.Clear();
                }
                DbService.DeleteChildrenItemsByParent_id(DelItem.getId());
            }
            DbService.DeleteItem(DelItem.getId());
            ViewModel.RemoveTreeNode(DelItem);
            var i = new MessageDialog("删除成功").ShowAsync();
        }

        private void CancelDelete()
        {
            var i = new MessageDialog("取消成功").ShowAsync();
        }

        // 拖拽过程中执行的函数(拖拽删除区域)
        private void DelBoder_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
        }

        // 开始拖拽Item以准备删除
        private void RootList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DelItem = e.Items.FirstOrDefault() as Models.TreeNode;
        }

        // 增加项目到根目录
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            CreateRootNode();
        }

        // RootList的ItemClick处理函数
        private void RootList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var root = (TreeNode)e.ClickedItem;
            ViewModel.SelectedItem = root;
            path.IsEnabled = false;
            path.Text = root.getPath();
            name.Text = root.getName();
            description.Text = root.getDescription();
            InfoGrid.Visibility = Visibility.Visible;
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Directory_new.jpg"));
            InfoImage.Source = bitmapImage;
            ViewModel.ChildrenItems.Clear();
            foreach (var item in DbService.GetItemsByParentId(root.getId()))
            {
                ViewModel.ChildrenItems.Add(item);
            }
        }

        // ChildList的ItemClick处理函数
        private void ChildList_ItemClick(object sender, ItemClickEventArgs e)
        {
            path.IsEnabled = true;
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Internet.jpg"));
            InfoImage.Source = bitmapImage;
            var child = (TreeNode)e.ClickedItem;
            ViewModel.SelectedItem = (TreeNode)e.ClickedItem;
            path.Text = child.getPath();
            name.Text = child.getName();
            description.Text = child.getDescription();
            InfoGrid.Visibility = Visibility.Visible;
        }

        // ChildList的Item双击处理函数
        private void ChildList_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode item = (TreeNode)ChildList.SelectedItems[0];
            OpenUrl(item.getPath());
        }

        // RootList 的Item 鼠标右击处理函数
        private void RootList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedItem == null) return;
            MenuFlyout rootFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "AddChild" };
            MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "DeleteAllChildren" };
            firstItem.Click += AddChild;
            secondItem.Click += DeleteChildrenItems;
            rootFlyout.Items.Add(firstItem);
            rootFlyout.Items.Add(secondItem);
            rootFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private async void DeleteChildrenItems(object sender, RoutedEventArgs e)
        {
            var msgDialog = new MessageDialog("确定删除该节点的所有子节点？") { Title = "删除提示" };
            msgDialog.Commands.Add(new UICommand("确定", uiCommand => { this.DeleteChildrenItems(); }));
            msgDialog.Commands.Add(new UICommand("取消", uiCommand => { this.CancelDelete(); }));
            await msgDialog.ShowAsync();
        }

        private void DeleteChildrenItems()
        {
            // 由于只有点击根节点之后才能够按右键以选择删除所有子节点，所以此时ViewModel.ChildrenItems一定是该节点的子节点列表
            ViewModel.ChildrenItems.Clear();
            // 根据parent id从数据库中删除子节点
            DbService.DeleteChildrenItemsByParent_id(ViewModel.SelectedItem.getId());
            var i = new MessageDialog("删除成功").ShowAsync();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (CreatingNewNode)
            {
                CreateNode();
            }
            else
            {
                UploadModification();
            }
            InfoGrid.Visibility = Visibility.Collapsed;
            toggleSwitch1.IsOn = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            InfoGrid.Visibility = Visibility.Collapsed;
            toggleSwitch1.IsOn = false;
        }

        private void AddChild(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                CreateChildNode(ViewModel.SelectedItem.getId());
            }
        }

        private void toggleSwitch1_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch1.IsOn)
            {
                path.IsReadOnly = false;
                name.IsReadOnly = false;
                description.IsReadOnly = false;
                Update.IsEnabled = true;
            }
            else
            {
                path.IsReadOnly = true;
                name.IsReadOnly = true;
                description.IsReadOnly = true;
                Update.IsEnabled = false;
            }
        }

        private async void Delete_Drop(object sender, DragEventArgs e)
        {
            var msgDialog = DelItem.getParentId() == "-1" ?
                new Windows.UI.Popups.MessageDialog("确定删除该节点(包括其子节点)？") { Title = "删除提示" } :
                new Windows.UI.Popups.MessageDialog("确定删除该节点？") { Title = "删除提示" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { this.DeleteItem(); }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand => { this.CancelDelete(); }));
            await msgDialog.ShowAsync();
        }

        private void Delete_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.Caption = "Delete";
            e.DragUIOverride.IsContentVisible = false;
        }
    }
}
