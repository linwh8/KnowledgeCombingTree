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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace KnowledgeCombingTree.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InternetPage : Page
    {
        InternetPageViewModel ViewModel = new InternetPageViewModel();

        public InternetPage()
        {
            this.InitializeComponent();
        }

        /*------------------------------------- api ------------------------------------*/
        private void CreateRootNode()
        {
            ViewModel.SelectedItem = new Models.TreeNode("-1", -1, "", "", "", "");
        }

        private void CreateChildNode(string parent_id)
        {
            ViewModel.SelectedItem = new Models.TreeNode(parent_id, -1, "", "", "", "");
        }

        private void EditNode(TreeNode node)
        {
            ViewModel.SelectedItem = node;
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
        }

        private void CancelOperationsOnNode()
        {
            ViewModel.SelectedItem = null;
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
    }
}
