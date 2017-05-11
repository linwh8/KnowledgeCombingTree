using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnowledgeCombingTree.Models;
using System.Collections.ObjectModel;

namespace KnowledgeCombingTree.ViewModels
{
    class TreeNodesViewModel
    {
        // 储存一棵树，有一个根节点(root)，AllItems储存根的所有子节点
        // 树暂且只有两层

        public TreeNode root { set; get; }
        public ObservableCollection<TreeNode> AllItems { set; get; }

        public TreeNodesViewModel()
        {
            AllItems = new ObservableCollection<TreeNode>();
        }

        public void AddChild(TreeNode child)
        {
            AllItems.Add(child);
        }
    }
}
