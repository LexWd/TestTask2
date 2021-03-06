﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis ;
using System.Linq;
using System.Windows.Controls ;

namespace TestTask2
{
[Serializable]
[SuppressMessage ( "ReSharper", "SwitchStatementMissingSomeCases" )]
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
                    Value = tag.GetValue().ToString();
                    Tag = "Images/Bool.png";
                    break;
                case "Int":
                    Value = tag.GetValue().ToString();
                    Tag = "Images/Integer.png";
                    break;
                case "Double":
                    Value = tag.GetValue().ToString();
                    Tag = "Images/Double.png";
                    break;
                case "None":
                    Value = "";
                    Tag = "Images/None.png";
                    break;
            }

            if ( !tag.HaveChilds () ) return ;
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
            tagItem.GetChilds () ;
            tagItem.Childs = tempList ;
            return tagItem ;
        }

    }
}
