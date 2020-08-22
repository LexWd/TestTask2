using System;
using System.Collections.Generic;
using System.Collections.ObjectModel ;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask1 ;
using System.Runtime.Serialization;
using System.Windows.Controls ;
using System.Windows.Media ;

namespace TestTask2
{
[Serializable()]
    public class TagItemModel:TreeViewItem
    {
        public string TagName { get ; set ; }
        public string FullPath { get ; set ; }
        public uint Level { get ; set ; }
        public string TagType { get ; set ; }
        public string Value { get ; set ; }
        public TagItem TagParent { get ; set ; }
        
        public List<TagItemModel> Childs { get ; set ; }
        
        public TagItemModel ( TagItem tag )
        {
            
            TagName = tag.GetName() ;
            Header = TagName;
            FullPath = tag.GetFullPath() ;
            Level = tag.GetLevel() ;
            TagParent = tag.GetParent () ;
            TagType = tag.GetTypeOfValue();
            switch (TagType)
            {
                case "Bool":
                    Value = tag.GetValue ().ToString () ;
                    Tag = "Images/Bool.png";
                    break ;
                case "Int":
                    Value = tag.GetValue().ToString();
                    Tag = "Images/Integer.png";
                    break ;
                case "Double":
                    Value = tag.GetValue().ToString();
                    Tag = "Images/Double.png";
                    break ;
                case "None":
                    Value = "";
                    Tag = "Images/None.png";
                    break ;
                default:
                    break;
            }
            if(tag.HaveChilds())
            foreach ( var item in tag.GetChilds() )
            {
                Items.Add ( item.Transform () ) ;
            }
        }

        public TagItemModel()
        {
            TagName = "Root" ;
            Header = TagName ;
            FullPath = "Root" ;
            Level = 1 ;
            TagType = "None" ;
            Tag = "Images/None.png";
            Value = "" ;
            TagParent = null ;
        }

        public override string ToString ( ) { return $"{Name}" ; }

        public TagItem Transform ( )
        {
            var tagItem = new TagItem(TagName,TagParent,Value,TagType,FullPath,Level);
            var tempList = (from TagItemModel model in Items select model.Transform ()).ToList () ;
            var childs = tagItem.GetChilds () ;
            tagItem._childs = tempList ;
            return tagItem ;
        } 

    }
}
