using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeCombingTree.Models
{
    public class TreeNode
    {
        private string id;
        // 以parent_id等于"-1"来标记根节点(创建根节点时指定该值为-1)
        private string parent_id;
        private long level;
        public string path;
        public string name;
        public string description;
        // 保存图片路径，若为空则保存为默认图片
        private string feature_id;

        public TreeNode(string p_id, long l, string p, string n, string d, string i)
        {
            id = Guid.NewGuid().ToString();
            parent_id = p_id;
            level = l;
            path = p;
            name = n;
            description = d;
            feature_id = (i == "" ? "default.png" : i);
        }

        public TreeNode(string _id, string p_id, long l, string p, string n, string d, string i)
        {
            id = _id;
            parent_id = p_id;
            level = l;
            path = p;
            name = n;
            description = d;
            feature_id = (i == "" ? "default.png" : i);
        }

        public string getId()
        {
            return id;
        }

        public void setParentId(string pid)
        {
            parent_id = pid;
        }

        public string getParentId()
        {
            return parent_id;
        }

        public void setLevel(long l)
        {
            level = l;
        }

        public long getLevel()
        {
            return level;
        }

        public void setPath(string p)
        {
            path = p;
        }

        public string getPath()
        {
            return path;
        }

        public void setName(string n)
        {
            name = n;
        }

        public string getName()
        {
            return name;
        }

        public void setDescription(string d)
        {
            description = d;
        }

        public string getDescription()
        {
            return description;
        }

        public void setFeature_id(string i)
        {
            feature_id = i;
        }

        public string getFeature_id()
        {
            return feature_id;
        }
    }
}
