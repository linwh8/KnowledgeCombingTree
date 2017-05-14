using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace KnowledgeCombingTree.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        // 储存一棵树，有一个根节点(root)，rootItems储存根的所有子节点
        // 树暂且只有两层

        //private Models.TreeNode root { set; get; }
        //private ObservableCollection<Models.TreeNode> rootItems { set; get; }

        // 储存所有树的根节点
        private ObservableCollection<Models.TreeNode> rootItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> RootItems { get { return this.rootItems; } set { this.rootItems = value; } }
        // 储存某一颗树的子节点
        private ObservableCollection<Models.TreeNode> childrenItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> ChildrenItems { get { return this.childrenItems; } set { this.childrenItems = value; } }
        // 选中的要进行编辑的节点
        private Models.TreeNode selectedItem = null;
        public Models.TreeNode SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }

        }

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void AddTreeNode(Models.TreeNode item)
        {
            if (item.getLevel() == 0)
                this.rootItems.Add(item);
            else
                this.childrenItems.Add(item);
        }

        public void UpdateTreeNode(string name, string description)
        {
            this.SelectedItem.setName(name);
            this.SelectedItem.setDescription(description);
            this.SelectedItem = null;
        }
        public void UpdateTreeNodeWithPath(string path, string name, string description)
        {
            this.SelectedItem.setPath(path);
            this.SelectedItem.setName(name);
            this.SelectedItem.setDescription(description);
            this.SelectedItem = null;
        }

        public void RemoveTreeNode(Models.TreeNode item)
        {
            // DIY
            if (item.getLevel() == 0)
                this.rootItems.Remove(item);
            else
                this.childrenItems.Remove(item);
            // set selectedItem to null after remove
        }

    }
}

