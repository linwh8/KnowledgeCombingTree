using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeCombingTree.ViewModels
{
    class InternetPageViewModel
    {
        private ObservableCollection<Models.TreeNode> rootItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> RootItems { get { return this.rootItems; } set { this.rootItems = value; } }
        // 储存某一颗树的子节点
        private ObservableCollection<Models.TreeNode> childrenItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> ChildrenItems { get { return this.childrenItems; } set { this.childrenItems = value; } }
        // 选中的要进行编辑的节点
        private Models.TreeNode selectedItem = null;
        public Models.TreeNode SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public InternetPageViewModel()
        {

        }

        public void RemoveTreeNode(Models.TreeNode item)
        {
            if (item.getParentId() == "-1")
                this.rootItems.Remove(item);
            else
                this.childrenItems.Remove(item);
            // set selectedItem to null after remove
        }
    }
}
