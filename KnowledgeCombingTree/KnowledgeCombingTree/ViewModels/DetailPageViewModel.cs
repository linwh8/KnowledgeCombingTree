using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace KnowledgeCombingTree.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
<<<<<<< HEAD
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

=======
        private ObservableCollection<Models.FolderItem> allItems = new ObservableCollection<Models.FolderItem>();
        public ObservableCollection<Models.FolderItem> AllItems { get { return this.allItems; } }
        private Models.FolderItem selectedItem = default(Models.FolderItem);
        public Models.FolderItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        /*        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item
            this.allItems.Add(new Models.TodoItem("作业1", "我的作业1"));
            this.allItems.Add(new Models.TodoItem("作业2", "我的作业2"));
        }

        public void AddTodoItem(string title, string description)
        {
            this.allItems.Add(new Models.TodoItem(title, description));
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            this.allItems.Remove(this.selectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, DateTime date)
        {
            // DIY
            this.selectedItem.title = title;
            this.selectedItem.description = description;
            this.selectedItem.date = date;
            // set selectedItem to null after update
            this.selectedItem = null;
        }*/
        public DetailPageViewModel()
        {
           // if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
           // {
           //     Value = "Designtime value";
           // }
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
        }
        public void AddFolderItem(string folderName)
        {
            this.allItems.Add(new Models.FolderItem("", "", folderName, ""));
        }

        public void UpdateFolderItem(string title, string details, string folderPath, string url) {
            this.SelectedItem.Title = title;
            this.SelectedItem.Details = details;
            this.SelectedItem.FolderPath = folderPath;
            this.SelectedItem.URL = url;
            this.SelectedItem = null;
        }

        public void RemoveFolderItem(Models.FolderItem item)
        {
            // DIY
            this.allItems.Remove(item);
            // set selectedItem to null after remove
        }


        /*
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
<<<<<<< HEAD

        public void AddTreeNode(Models.TreeNode item)
        {
            this.rootItems.Add(item);
        }

        public void UpdateTreeNode(string name, string description, string image)
        {
            this.SelectedItem.setName(name);
            this.SelectedItem.setDescription(description);
            this.SelectedItem.setImage(image);
            this.SelectedItem = null;
        }
        public void UpdateTreeNodeWithPath(string path, string name, string description, string image)
        {
            this.SelectedItem.setPath(path);
            this.SelectedItem.setName(name);
            this.SelectedItem.setDescription(description);
            this.SelectedItem.setImage(image);
            this.SelectedItem = null;
        }

        public void RemoveTreeNode(Models.TreeNode item)
        {
            // DIY
            this.rootItems.Remove(item);
            // set selectedItem to null after remove
        }

=======
        */
>>>>>>> f4cc925bf303552f6d36d66a81677eaac8385bf8
    }
}

