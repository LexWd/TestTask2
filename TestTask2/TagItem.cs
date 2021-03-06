﻿using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Globalization ;
using System.Runtime.CompilerServices ;

namespace TestTask2
{
    [SuppressMessage ( "ReSharper", "SwitchStatementMissingSomeCases" )]
    public sealed class TagItem : INotifyPropertyChanged
    {
        private string _name ;
        private string Name
        {
            get => _name;
            set {

                _name = value ; OnPropertyChanged(); } 
        }

        // Since an object can have one of four value types, 
        // it is easier to store the value itself in the string 
        private string _value;

        private TagType _valueType;

        // In the case of the root of the hierarchy, there is no maternal object
        private TagItem _parent;

        // The list of child objects will be empty until we add any object to it.
        // Based on this, we can initialize this list here.
        public  List<TagItem> Childs { get ; set ; }

        private string _fullPath ;
        public string FullPath
        {
            get => _fullPath;
            set { _fullPath = value; OnPropertyChanged(); }
        }
        private uint _level = 1;

        #region InterfaceImplementation

        // In an effort to avoid processing an entire array of children using a loop, we can use an interface that notifies of changes in the fields

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ParentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    ChangeFullPath();
                    break;
                case "FullPath":
                    ChangeFullPath();
                    break;
            }
        }

        #endregion

        #region Constuctors

        private void Base(string name, TagItem parent) //Base part of any constructor
        {
            Name = name;
            _parent = parent;
            if (_parent == null) return;
            _level = _parent._level + 1;
            _parent.PropertyChanged += ParentOnPropertyChanged;
        }

        // Constructor for objects with int type values
        public TagItem(string name, TagItem parent, int value)
        {
            _value = value.ToString();
            _valueType = TagType.Int;
            Childs = new List<TagItem>();
            Base(name, parent);
        }

        // Constructor for objects with double type values
        public TagItem(string name, TagItem parent, double value)
        {
            _value = value.ToString(CultureInfo.InvariantCulture);
            _valueType = TagType.Double;
            Base(name, parent);
        }

        // Constructor for objects with bool type values
        public TagItem(string name, TagItem parent, bool value)
        {
            _value = value.ToString();
            _valueType = TagType.Bool;
            Base(name, parent);
        }

        // Constructor for objects without value
        public TagItem(string name, TagItem parent)
        {
            _value = null;
            _valueType = TagType.None;

            Base(name, parent);
        }

        // Constructor for root objects
        public TagItem(string name)
        {
            _value = null;
            _valueType = TagType.None;
            Base(name, null);
        }

       

        public TagItem(string name, TagItem parent, string tempValue, string valueType,
            string tempFullPath, uint tempLevel)
        {
            Name = name;
            _value = tempValue;
            FullPath = tempFullPath;
            _level = tempLevel;
            Childs = new List<TagItem>();
            _parent = parent;

            try
            {
                _valueType = TagItemExtensions.ConvertStringToTagType(valueType);
            }
            catch (NotSupportedTagTypeException)
            {
                _valueType = TagType.None;
            }

            if (_parent != null)
                _parent.PropertyChanged += ParentOnPropertyChanged;
        }

        public TagItem(string name, TagItem parent, TagType type)
        {
            Name = name;
            _parent = parent;
            _parent?.AddChild(this);
            _valueType = type;
            Childs = new List<TagItem>();
            if (_parent != null) FullPath = _parent.FullPath + $".{name}";
            _level = parent.GetLevel() + 1;
            if (_parent != null) _parent.PropertyChanged += ParentOnPropertyChanged;
        }

        public TagItem ( string name, string type )
        {
            Name = name ;
           _valueType = TagItemExtensions.ConvertStringToTagType(type);
        }

        #endregion

        #region ChildsControll

        public void AddChild(TagItem newChild)
        {
            if ( Childs == null )
            {
                Childs = new List<TagItem> ();
            }
            Childs.Add(newChild);
            newChild._parent = this;
            newChild.ChangeFullPath();
         
        }

        // This method removes a descendant from its copy
        public void DeleteChild(TagItem child)
        {
            Childs.Remove(child);
        }

        // This method removes a child by name
        public void DeleteChild(string name)
        {
            Childs.Remove(FindChild(name));
        }

        // This method can find child of object by name and return reference to it
        public TagItem FindChild(string name)
        {
            return Childs.Find(item => item.Name == name);
        }

        public bool HaveChilds()
        {
            return Childs?.Count > 0;
        }

        public IEnumerable<TagItem> GetChilds()
        {
            return Childs;
        }

        #endregion

        #region FieldControll

        private TagType GetTypeOfValues()
        {
            return _valueType;
        }

        public  string GetTypeOfValue()
        {
            switch (GetTypeOfValues())
            {
                case TagType.Bool:
                    return "Bool" ;
                case TagType.Int:
                    return "Int";
                case TagType.Double:
                    return "Double";
                case TagType.None:
                    return "None";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object GetValue()
        {
            switch (_valueType)
            {
                case TagType.Bool:
                    return GetBoolValue();
                case TagType.Int:
                    return GetIntValue();
                case TagType.Double:
                    return GetDoubleValue();
                case TagType.None:
                    return GetNoneValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetIntValue()
        {
            return Convert.ToInt32(_value);
        }

        private bool GetBoolValue()
        {
            return Convert.ToBoolean(_value);
        }

        private double GetDoubleValue()
        {
            return Convert.ToDouble(_value);
        }

        private static string GetNoneValue()
        {
            return "";
        }

        public void SetValue(int newValue)
        {
            _value = newValue.ToString();
            _valueType = TagType.Int;
        }

        public void SetValue(bool newValue)
        {
            _value = newValue.ToString();
            _valueType = TagType.Bool;
        }

        public void SetValue(double newValue)
        {
            _value = newValue.ToString(CultureInfo.InvariantCulture);
            _valueType = TagType.Double;
        }

        public void SetValue()
        {
            _value = null;
            _valueType = TagType.None;
        }

        public uint GetLevel()
        {
            return _level;
        }

        public string GetFullPath()
        {
            return FullPath;
        }

        public string GetName()
        {
            return Name;
        }

        public TagItem GetParent()
        {
            return _parent;
        }

        #region Renaming

        // Object name change
        public void ReName(string newName)
        {
            Name = newName;
            ChangeFullPath();
            
        }

        // Change the full path to the object, taking into account the presence of the parent object
        private void ChangeFullPath()
        {
            if (_parent != null)
                FullPath = _parent.FullPath + $".{Name}";
            else
                FullPath = $"{Name}";
        }

        #endregion

        #endregion

        #region ConsoleOutput

        public void Write()
        {
            Console.WriteLine($@"
 Full path: {FullPath}
 Level: {_level}
 Value: {_value}
 ValueType: {_valueType}
");
            if (Childs == null || Childs.Count <= 0) return;
            foreach (var child in Childs) child.Write();
        }

        #endregion

        public override string ToString() { return $"{Name} "; }

        public TagItemModel Transform()
        {
            return new TagItemModel(this);

        }

        public static string GetTypeOfValue ( TagItem root )
        {
            switch (root._valueType)
            {
                case TagType.Bool :
                    return "Bool" ;
                case TagType.Int :
                    return "Int" ;
                case TagType.Double :
                    return "Double" ;
                case TagType.None :
                    return "None" ;
                default :
                    throw new NotSupportedTagTypeException() ;
            }
        }
    }
}