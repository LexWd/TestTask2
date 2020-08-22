using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TestTask2;

namespace TestTask1
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
            

            bool temp = Write();

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

        private void CreateStructure(TagItem root, XmlElement parent)
        {
            XmlElement tag = _storage.CreateElement("tag");
            XmlAttribute nameAttr = _storage.CreateAttribute("name");
            XmlElement pathAttr = _storage.CreateElement("FullPath");
            XmlElement levelAttr = _storage.CreateElement("Level");
            XmlElement valueTypeAttr = _storage.CreateElement("ValueType");
            XmlElement valueAttr = _storage.CreateElement("Value");

            XmlText nameText = _storage.CreateTextNode(root.GetName());
            XmlText pathText = _storage.CreateTextNode(root.GetFullPath());
            XmlText levelText = _storage.CreateTextNode(root.GetLevel().ToString());
            XmlText valueTypeText = _storage.CreateTextNode(TagItem.GetTypeOfValue(root));
            XmlText valueText = _storage.CreateTextNode(root.GetValue().ToString());

            nameAttr.AppendChild(nameText);
            pathAttr.AppendChild(pathText);
            levelAttr.AppendChild(levelText);
            valueAttr.AppendChild(valueText);
            valueTypeAttr.AppendChild(valueTypeText);
            tag.Attributes.Append(nameAttr);
            tag.AppendChild(pathAttr);
            tag.AppendChild(levelAttr);
            tag.AppendChild(valueAttr);
            tag.AppendChild(valueTypeAttr);

            if (root.HaveChilds())
            {
                XmlElement childElement = _storage.CreateElement("Childs");
                foreach (TagItem child in root.GetChilds()) CreateStructure(child, childElement);
                tag.AppendChild(childElement);
            }

            if (parent != null)
                parent.AppendChild(tag);
            else
                _storage.AppendChild(tag);
        }

        private bool Write()
        {
            var xRoot = _storage.DocumentElement;
            if (xRoot == null) return false;
            TagRoot = CreateData(null, null);
            return TagRoot != null;
        }

        private static void WriteNodes(IEnumerable xRoot)
        {

            foreach (XmlNode xNode in xRoot)
            {
                if (xNode.Attributes == null || xNode.Attributes.Count <= 0) continue;
                XmlNode attr = xNode.Attributes.GetNamedItem("name");
                if (attr != null)
                    Console.WriteLine(attr.Value);
                foreach (XmlNode childNode in xNode.ChildNodes)
                    switch (childNode.Name)
                    {
                        case "Level":
                            Console.WriteLine($"Level: {childNode.InnerText}");
                            break;
                        case "FullPath":
                            Console.WriteLine($"Full Path: {childNode.InnerText}");
                            break;
                        case "Value":
                            Console.WriteLine($"Value: {childNode.InnerText}");
                            break;
                        case "ValueType":
                            Console.WriteLine($"ValueType: {childNode.InnerText}");
                            break;
                        case "Childs":
                            XmlElement node = (XmlElement)childNode;
                            Console.WriteLine();
                            WriteNodes(node);
                            break;
                        default:
                            Console.WriteLine("Incorrect parameter");
                            break;
                    }

                Console.WriteLine();
            }
        }


        private TagItem CreateData(TagItem parentItem, XmlNode nodeForItem)
        {
            if (nodeForItem == null)
            {
                nodeForItem = _storage.DocumentElement;
                nodeForItem = nodeForItem?.FirstChild;
            }
            else if (_tempList2.Contains(nodeForItem)) return null;

            if (nodeForItem?.Attributes == null || (nodeForItem.Name != "tag"))
            {
                return null;
            }
            else
            {
                _tempList2.Add(nodeForItem);
                XmlNode attr = nodeForItem.Attributes.GetNamedItem("name");
                uint tempLevel = 1;
                string tempPath = "";
                string tempName = attr.Value;
                string tempValue = "";
                string valueType = "";
                XmlNode childsNod = null;
                foreach (XmlNode childNode in nodeForItem.ChildNodes)
                {
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
                            Console.WriteLine("Incorrect parameter");
                            break;
                    }
                }
                TagItem newItem = new TagItem(tempName, parentItem, tempValue, valueType, tempPath, tempLevel);

                if (childsNod != null)
                {
                    foreach (XmlNode child in childsNod.ChildNodes)
                    {
                        TagItem tempItem = CreateData(newItem, child);
                        if (tempItem != null)
                        {
                            newItem.AddChild(tempItem);
                        }
                    }
                }

                return newItem;
            }
        }

        public TagItem Search(string fullName)
        {
            TagItem tempItem = TagRoot;
            string[] split = fullName.Trim().Split('.');

            return split?.Aggregate(tempItem, (current, name) => current?.FindChild(name));
        }

        public TagItem SearchByFullPath ( string fullPath ) {return Search(fullPath.Remove (0,4)); }

        public void DeleteByModel(TagItemModel model)
        {
            DeleteTag(model.TagParent);
        }

        public static void DeleteTag(TagItem tagForDelete)
        {
            var parent = tagForDelete?.GetParent () ;
            parent?.DeleteChild ( tagForDelete ) ;
        }

        public List<TagItemModel> CreateDataTree(TagItem parent = null)
        {
            if (parent == null)
            {
                parent = TagRoot;
            }
            var tempList = new List<TagItemModel>();
            if ( !parent.HaveChilds () ) return tempList ;
            tempList.AddRange ( parent.GetChilds ().Select ( node => node.Transform () ) ) ;
            return tempList;
        }



    }
}