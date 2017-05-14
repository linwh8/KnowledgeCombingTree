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

            ViewModel.RootItems = DbService.GetItemsByParentId("-1");
        }




        /*----------------------------------- api --------------------------------*/
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

                StorageApplicationPermissions.MostRecentlyUsedList.Add(target);
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        // ��folderpickerѡ��һ���ļ���(Ŀ¼)���Զ�ɨ����������Ŀ¼�������ӽڵ�������ݿ�
        // ���ҽ���ЩĿ¼ȫ����ӵ�featureAccessList�Ա��ܹ�����ЩĿ¼
        // �Զ�����һ����
        private async void CreateTreeAuto()
        {
            StorageFolder folder = await ChooseFolder();
            if (folder != null)
            {
                var feature_id = AddInFeatureAccessList(folder);
                // ѡ����һ��Ŀ¼������һ�����ڵ�
                string pid = CreateTree(folder.Path, folder.Name, feature_id);
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
            var feature_id = AddInFeatureAccessList(folder);
            string pid = CreateTree(folder.Path, folder.Name, feature_id);
        }

        // ���һ���ӽڵ㣺
        // 1. ѡ��һ��·��������һ���ڵ㣬��ʱdescriptionΪ""
        // 2. ����ViewModel.SelectedItemΪ���½��ڵ㣬���б༭
        // 3. �༭���֮���ύ�޸ģ���ViewModel.SelectedItem��������Ϊnull
        // @param: ���ڵ�
        private async void AddChildNode(TreeNode parent_node)
        {
            try
            {
                TreeNode new_node = await ChooseAndCreateChildNode(parent_node);
                ViewModel.SelectedItem = new_node;
                ViewModel.ChildrenItems.Add(new_node);
                UploadModification();
            }
            catch (Exception e)
            {
                var i = new MessageDialog("�����ӽڵ�ʧ��").ShowAsync();
            }
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

        /*-------------------------------some driver functions-----------------------------------*/

        // ����һ���ӽڵ�
        // @param: ���ڵ�
        // @return: �������ӽڵ�
        private async Task<TreeNode> ChooseAndCreateChildNode(TreeNode parent_node)
        {
            StorageFolder folder = await ChooseFolder();
            var feature_id = AddInFeatureAccessList(folder);
            TreeNode node = CreateAndGetChildNode(parent_node.getId(), folder.Path, folder.Name, feature_id);
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
        private string CreateTree(string path, string name, string feature_id)
        {
            TreeNode root = new TreeNode("-1", 0, path, name, "", feature_id);
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
                //Debug.WriteLine(subfolder.Name);

                var feature_id = AddInFeatureAccessList(subfolder);
                // �����ӽڵ�
                CreateChildNode(pid, folder.Path, subfolder.Name, feature_id);

                // To iterate over the files in each folder, uncomment the following lines. 
                // foreach(StorageFile file in await folder.GetFilesAsync())
                //    Debug.WriteLine(" " + file.Name);
            }
        }

        private void CreateChildNode(string pid, string path, string name, string feature_id)
        {
            TreeNode node = new TreeNode(pid, 1, path + "\\" + name, name, "", feature_id);
            DbService.AddItem(node);
        }

        private TreeNode CreateAndGetChildNode(string pid, string path, string name, string feature_id)
        {
            TreeNode node = new TreeNode(pid, 1, path, name, "", feature_id);
            DbService.AddItem(node);
            return node;
        }

        /*-----------------------------------featureAccessList, �ļ���Ȩ�����------------------------------------*/
        private string AddInFeatureAccessList(StorageFolder folder)
        {
            var feature_id = StorageApplicationPermissions.FutureAccessList.Add(folder);
            return feature_id;
        }

        private void RemoveFromFeatureAccessList(string feature_id)
        {
            try
            {
                StorageApplicationPermissions.FutureAccessList.Remove(feature_id);
            }
            catch (Exception e)
            {
                var i = new MessageDialog(e.ToString()).ShowAsync();
            }
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
                    var i = new MessageDialog("ֻ������ļ���").ShowAsync();
                }
                catch
                {
                    StorageFolder folder = items[0] as StorageFolder;
                    if (folder != null)
                    {
                        var feature_id = AddInFeatureAccessList(folder);
                        // ѡ����һ��Ŀ¼������һ�����ڵ�
                        string pid = CreateTree(folder.Path, folder.Name, feature_id);
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
        private async void DelBoder_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            var msgDialog = DelItem.getParentId() == "-1" ?
                            new Windows.UI.Popups.MessageDialog("ȷ��ɾ���ýڵ�(�������ӽڵ�)��") { Title = "ɾ����ʾ" } :
                            new Windows.UI.Popups.MessageDialog("ȷ��ɾ���ýڵ㣿") { Title = "ɾ����ʾ" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("ȷ��", uiCommand => { this.DeleteItem(); }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("ȡ��", uiCommand => { this.CancelDelete(); }));
            await msgDialog.ShowAsync();
        }

        private void DeleteItem()
        {
            if (DelItem.getId() == ViewModel.SelectedItem.getId())
            {
                // ���ɾ���Ľڵ���SelectedItem����ɾ��֮��Ӧ�ðѱ༭������
                InfoGrid.Visibility = Visibility.Collapsed;
            }

            if (DelItem.getParentId() == "-1")
            {
                if (ViewModel.ChildrenItems.Count > 0 && ViewModel.ChildrenItems.ElementAt(0).getParentId() == DelItem.getId())
                {
                    foreach (var item in ViewModel.ChildrenItems)
                    {
                        RemoveFromFeatureAccessList(item.getFeature_id());
                    }
                    ViewModel.ChildrenItems.Clear();
                }
                else
                {
                    ObservableCollection<TreeNode> delNodes = DbService.GetItemsByParentId(DelItem.getId());
                    foreach (var item in delNodes)
                    {
                        RemoveFromFeatureAccessList(item.getFeature_id());
                    }
                }
                DbService.DeleteChildrenItemsByParent_id(DelItem.getId());
            }
            RemoveFromFeatureAccessList(DelItem.getFeature_id());
            DbService.DeleteItem(DelItem.getId());
            ViewModel.RemoveTreeNode(DelItem);
            var i = new MessageDialog("ɾ���ɹ�").ShowAsync();
        }

        private void CancelDelete()
        {
            var i = new MessageDialog("ȡ���ɹ�").ShowAsync();
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
                    //if (file != null)
                    //{
                    //    TreeNode new_node = new TreeNode(ViewModel.SelectedItem.getId(), 1, file.Path + "\\" + file.Name, file.Name, "", "");
                    //    ViewModel.AddTreeNode(new_node);
                    //    DbService.AddItem(new_node);
                    //}
                    //else
                    //{
                    //    var i = new MessageDialog("ѡ���ļ�ʧ��").ShowAsync();
                    //}
                    var i = new MessageDialog("ֻ������ļ���").ShowAsync();
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
            //TreeNode clickedItem = (TreeNode)e.OriginalSource;
            //ViewModel.SelectedItem = clickedItem;
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
        }

        // ChildList��Item˫��������
        private void ChildList_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            TreeNode item = (TreeNode)ChildList.SelectedItems[0];
            OpenFolder(item.getFeature_id());
        }

        // RootList ��Item ����һ�������
        private void RootList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedItem == null) return;
            MenuFlyout rootFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "AddChild" };
            MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "DeleteAllChildren"};
            firstItem.Click += AddChild;
            secondItem.Click += DeleteChildrenItems;
            rootFlyout.Items.Add(firstItem);
            rootFlyout.Items.Add(secondItem);
            rootFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private async void DeleteChildrenItems(object sender, RoutedEventArgs e)
        {
            var msgDialog = new MessageDialog("ȷ��ɾ���ýڵ�������ӽڵ㣿") { Title = "ɾ����ʾ" };
            msgDialog.Commands.Add(new UICommand("ȷ��", uiCommand => { this.DeleteChildrenItems(); }));
            msgDialog.Commands.Add(new UICommand("ȡ��", uiCommand => { this.CancelDelete(); }));
            await msgDialog.ShowAsync();
        }

        private void DeleteChildrenItems()
        {
            // ����ֻ�е�����ڵ�֮����ܹ����Ҽ���ѡ��ɾ�������ӽڵ㣬���Դ�ʱViewModel.ChildrenItemsһ���Ǹýڵ���ӽڵ��б�
            foreach (var item in ViewModel.ChildrenItems)
            {
                RemoveFromFeatureAccessList(item.getFeature_id());
            }
            ViewModel.ChildrenItems.Clear();
            // ����parent id�����ݿ���ɾ���ӽڵ�
            DbService.DeleteChildrenItemsByParent_id(ViewModel.SelectedItem.getId());
            var i = new MessageDialog("ɾ���ɹ�").ShowAsync();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UploadModification();
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

