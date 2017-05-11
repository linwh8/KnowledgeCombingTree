using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnowledgeCombingTree.Models;

namespace KnowledgeCombingTree.ViewModels
{
    class RootNodesViewModel
    {
        // 储存所有根节点的ObservableCollection

        public ObservableCollection<TreeNode> AllItems { set; get; }

        public RootNodesViewModel()
        {
            AllItems = new ObservableCollection<TreeNode>();
        }

        public void Add(TreeNode root)
        {
            AllItems.Add(root);
        }

        public void Remove(string id)
        {
            AllItems.Remove(AllItems.Where(x => x.getId() == id).FirstOrDefault());
        }
    }
}
