using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Xml ;

namespace TestTask2
{
    public class TagStorage
    {
        public TagItem TagRoot;
        private XmlDocument _storage;
        private readonly List<XmlNode> _tempList2 = new List<XmlNode>();
        public static bool Loaded = false;
        public TagStorage()
        {
            TagRoot = new TagItem("Root");
        }

        public void Load(string filename)
        {
            _storage = new XmlDocument();
            _storage.Load(filename);
            Write();
        }

        public void Save(TagItem root, XmlElement parent, string filePath)
        {
            _storage = new XmlDocument();
            var temp = _storage.CreateXmlDeclaration("1.0", "UTF-8", "");
            var tempRoot = _storage.CreateElement("tags");
            _storage.AppendChild(temp);
            _storage.AppendChild(tempRoot);
            CreateStructure(root, tempRoot);
            _storage.Save(filePath);
        }

        private void CreateStructure ( TagItem root, XmlNode parent )
        {
            var tag           = _storage.CreateElement ( "tag" ) ;
            var nameAttr      = _storage.CreateAttribute ( "name" ) ;
            var pathAttr      = _storage.CreateElement ( "FullPath" ) ;
            var levelAttr     = _storage.CreateElement ( "Level" ) ;
            var valueTypeAttr = _storage.CreateElement ( "ValueType" ) ;
            var valueAttr     = _storage.CreateElement ( "Value" ) ;

            var nameText      = _storage.CreateTextNode ( root.GetName () ) ;
            var pathText      = _storage.CreateTextNode ( root.GetFullPath () ) ;
            var levelText     = _storage.CreateTextNode ( root.GetLevel ().ToString () ) ;
            var valueTypeText = _storage.CreateTextNode ( TagItem.GetTypeOfValue ( root ) ) ;
            var valueText     = _storage.CreateTextNode ( root.GetValue ().ToString () ) ;

            nameAttr.AppendChild ( nameText ) ;
            pathAttr.AppendChild ( pathText ) ;
            levelAttr.AppendChild ( levelText ) ;
            valueAttr.AppendChild ( valueText ) ;
            valueTypeAttr.AppendChild ( valueTypeText ) ;
            tag.Attributes.Append ( nameAttr ) ;
            tag.AppendChild ( pathAttr ) ;
            tag.AppendChild ( levelAttr ) ;
            tag.AppendChild ( valueAttr ) ;
            tag.AppendChild ( valueTypeAttr ) ;

            if ( root.HaveChilds () )
            {
                var childElement = _storage.CreateElement ( "Childs" ) ;
                foreach ( var child in root.GetChilds () ) CreateStructure ( child, childElement ) ;
                tag.AppendChild ( childElement ) ;
            }

            if ( parent != null )
            {
                parent.AppendChild ( tag ) ;
            }
            else
            {
                _storage.AppendChild ( tag ) ;
            }
        }

        private void Write()
        {
            var xRoot = _storage.DocumentElement;
            if (xRoot == null) return ;
            TagRoot = CreateData(null, null);
        }


        private TagItem CreateData(TagItem parentItem, XmlNode nodeForItem)
        {
            if (nodeForItem == null)
            {
                nodeForItem = _storage.DocumentElement;
                nodeForItem = nodeForItem?.FirstChild;
            }
            else if (_tempList2.Contains(nodeForItem))
            {
                return null;
            }

            if (nodeForItem?.Attributes == null || nodeForItem.Name != "tag")
            {
                return null;
            }

            _tempList2.Add(nodeForItem);
            var     attr      = nodeForItem.Attributes.GetNamedItem("name");
            uint    tempLevel = 1;
            var     tempPath  = "";
            var     tempName  = attr.Value;
            var     tempValue = "";
            var     valueType = "";
            XmlNode childsNod = null;
            foreach (XmlNode childNode in nodeForItem.ChildNodes)
                switch (childNode.Name)
                {

                    case "Level":
                        tempLevel = uint.Parse(childNode.InnerText);
                        break;
                    case "FullPath":
                        tempPath = childNode.InnerText;
                        break;
                    case "Value":
                        tempValue = childNode.InnerText;
                        break;
                    case "ValueType":
                        valueType = childNode.InnerText;
                        break;
                    case "Childs":
                        childsNod = childNode;
                        break;
                    default:
                        Console.WriteLine(@"Incorrect parameter");
                        break;
                }
            var newItem = new TagItem(tempName, parentItem, tempValue, valueType, tempPath, tempLevel);

            if ( childsNod == null ) return newItem ;
            foreach (XmlNode child in childsNod.ChildNodes)
            {
                var tempItem = CreateData(newItem, child);
                if (tempItem != null) newItem.AddChild(tempItem);
            }
            return newItem;
        }

        public TagItem Search(string fullName)
        {
            var tempItem = TagRoot;
            var split = fullName.Trim().Split('.');

            return split.Aggregate(tempItem, (current, name) => current?.FindChild(name));
        }

        public TagItem SearchByFullPath ( string fullPath ) {return Search(fullPath.Remove (0,4)); }

        public void DeleteByModel(TagItemModel model)
        {
            var temp = model.TagParent ;
            var tagForDelete = temp.FindChild ( model.TagName ) ;
            DeleteTag(tagForDelete);
        }

        public static void DeleteTag(TagItem tagForDelete)
        {
            var parent = tagForDelete?.GetParent () ;
            parent?.DeleteChild ( tagForDelete ) ;
        }

        public List<TagItemModel> CreateDataTree(TagItem parent = null)
        {
            if (parent == null) parent = TagRoot;
            var tempList = new List<TagItemModel>();
            if ( !parent.HaveChilds () ) return tempList ;
            tempList.AddRange ( parent.GetChilds ().Select ( node => node.Transform () ) ) ;
            return tempList;
        }



    }
}