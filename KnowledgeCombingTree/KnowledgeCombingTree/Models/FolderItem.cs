using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace KnowledgeCombingTree.Models
{
    public class FolderItem
    {
        public FolderItem() { }
        public FolderItem(string title, string details, string folderPath, string url) {
            Title = title;
            Details = details;
            FolderPath = folderPath;
            URL = url;
        }
        public string Title;
        public string Details;
        public string FolderPath;
        public string URL;
        public BitmapImage Img { get; set; }
    }
}
