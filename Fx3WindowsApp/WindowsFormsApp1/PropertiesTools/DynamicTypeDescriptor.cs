using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImagerDebugSoftware.PropertiesTools
{
    public sealed class DynamicTypeDescriptor : ICustomTypeDescriptor, INotifyPropertyChanged
    {
        private Type _type;
        private AttributeCollection _attributes;
        private TypeConverter _typeConverter;
        private Dictionary<Type, object> _editors;
        private EventDescriptor _defaultEvent;
        private PropertyDescriptor _defaultProperty;
        private EventDescriptorCollection _events;

        public event PropertyChangedEventHandler PropertyChanged;

        private DynamicTypeDescriptor()
        {
        }

        ///<summary> 
        /// Init a DynamicTypeDescriptor object from Dictionary string
        ///</summary> 
        ///<param name="dynamicTypeDescriptor"></param> 
        ///<param name="initValue"></param> 
        ///<returns></returns> 
        public static DynamicTypeDescriptor FromDictionaryString(DynamicTypeDescriptor dynamicTypeDescriptor, string initValue)
        {
            Dictionary<string, string> initValueDictionary = initValue
                               .Split('|')
                               .Select(part => part.Split('='))
                               .Where(part => part.Length == 2)
                               .ToDictionary(sp => sp[0], sp => sp[1]);

            foreach (KeyValuePair<string, string> item in initValueDictionary)
            {
                string key = item.Key;
                string value = item.Value;
                DynamicProperty dynamicProperty = ((DynamicProperty)dynamicTypeDescriptor.Properties[key]);

                if (dynamicProperty == null) continue;

                if (dynamicProperty.PropertyType.IsEnum)
                {
                    dynamicProperty.Value = Enum.Parse(dynamicProperty.PropertyType, value);
                }
                else
                {
                    dynamicProperty.Value = Convert.ChangeType(value, dynamicProperty.PropertyType);
                }
            }

            return dynamicTypeDescriptor;
        }
        public DynamicTypeDescriptor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            _type = type;
            _typeConverter = TypeDescriptor.GetConverter(type);
            _defaultEvent = TypeDescriptor.GetDefaultEvent(type);
            _defaultProperty = TypeDescriptor.GetDefaultProperty(type);
            _events = TypeDescriptor.GetEvents(type);

            List<PropertyDescriptor> normalProperties = new List<PropertyDescriptor>();
            OriginalProperties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in OriginalProperties)
            {
                //if (!property.IsBrowsable)continue;
                normalProperties.Add(property);
            }
            Properties = new PropertyDescriptorCollection(normalProperties.ToArray());

            _attributes = TypeDescriptor.GetAttributes(type);

            _editors = new Dictionary<Type, object>();
            object editor = TypeDescriptor.GetEditor(type, typeof(UITypeEditor));
            if (editor != null)
            {
                _editors.Add(typeof(UITypeEditor), editor);
            }
            editor = TypeDescriptor.GetEditor(type, typeof(ComponentEditor));
            if (editor != null)
            {
                _editors.Add(typeof(ComponentEditor), editor);
            }
            editor = TypeDescriptor.GetEditor(type, typeof(InstanceCreationEditor));
            if (editor != null)
            {
                _editors.Add(typeof(InstanceCreationEditor), editor);
            }
        }

        public DynamicProperty GetDynamicProperty(string DynamicPropertyName)
        {
            return (DynamicProperty)this.Properties[DynamicPropertyName];
        }
        public T GetPropertyValue<T>(string name, T defaultValue)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (PropertyDescriptor pd in Properties)
            {
                if (pd.Name == name)
                {
                    try
                    {
                        return (T)Convert.ChangeType(pd.GetValue(Component), typeof(T));
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
            return defaultValue;
        }

        public void SetPropertyValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (PropertyDescriptor pd in Properties)
            {
                if (pd.Name == name)
                {
                    pd.SetValue(Component, value);
                    break;
                }
            }
        }
        internal void OnValueChanged(PropertyDescriptor prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop.Name));
        }

        internal static T GetAttribute<T>(AttributeCollection attributes) where T : Attribute
        {
            if (attributes == null)
                return null;

            foreach (Attribute att in attributes)
            {
                if (typeof(T).IsAssignableFrom(att.GetType()))
                    return (T)att;
            }
            return null;
        }


        public PropertyDescriptor AddProperty(Type type, string name, object value, string displayName, string description, string category, bool hasDefaultValue, object defaultValue, bool readOnly)
        {
            return AddProperty(type, name, value, displayName, description, category, hasDefaultValue, defaultValue, readOnly, null);
        }

        public PropertyDescriptor AddProperty(
            Type type,
            string name,
            object value,
            string displayName,
            string description,
            string category,
            bool hasDefaultValue,
            object defaultValue,
            bool readOnly,
            Type uiTypeEditor)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (name == null)
                throw new ArgumentNullException("name");

            List<Attribute> atts = new List<Attribute>();
            if (!string.IsNullOrEmpty(displayName))
            {
                atts.Add(new DisplayNameAttribute(displayName));
            }

            if (!string.IsNullOrEmpty(description))
            {
                atts.Add(new DescriptionAttribute(description));
            }

            if (!string.IsNullOrEmpty(category))
            {
                atts.Add(new CategoryAttribute(category));
            }

            if (hasDefaultValue)
            {
                atts.Add(new DefaultValueAttribute(defaultValue));
            }

            if (uiTypeEditor != null)
            {
                atts.Add(new EditorAttribute(uiTypeEditor, typeof(UITypeEditor)));
            }

            if (readOnly)
            {
                atts.Add(new ReadOnlyAttribute(true));
            }

            DynamicProperty property = new DynamicProperty(this, type, value, name, atts.ToArray());
            AddProperty(property);
            return property;
        }

        public void RemoveProperty(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            List<PropertyDescriptor> remove = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in Properties)
            {
                if (pd.Name == name)
                {
                    remove.Add(pd);
                }
            }

            foreach (PropertyDescriptor pd in remove)
            {
                Properties.Remove(pd);
            }
        }

        public void AddProperty(PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Properties.Add(property);
        }

        public override string ToString()
        {
            return base.ToString() + " (" + Component + ")";
        }

        public PropertyDescriptorCollection OriginalProperties { get; private set; }
        public PropertyDescriptorCollection Properties { get; private set; }

        public DynamicTypeDescriptor FromComponent(object component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (!_type.IsAssignableFrom(component.GetType()))
                throw new ArgumentException(null, "component");

            DynamicTypeDescriptor desc = new DynamicTypeDescriptor();
            desc._type = _type;
            desc.Component = component;

            // shallow copy on purpose
            desc._typeConverter = _typeConverter;
            desc._editors = _editors;
            desc._defaultEvent = _defaultEvent;
            desc._defaultProperty = _defaultProperty;
            desc._attributes = _attributes;
            desc._events = _events;
            desc.OriginalProperties = OriginalProperties;

            // track values
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in Properties)
            {
                DynamicProperty ap = new DynamicProperty(desc, pd, component);
                properties.Add(ap);
            }

            desc.Properties = new PropertyDescriptorCollection(properties.ToArray());
            return desc;
        }

        public object Component { get; private set; }
        public string ClassName { get; set; }
        public string ComponentName { get; set; }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return _attributes;
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            if (ClassName != null)
                return ClassName;

            if (Component != null)
                return Component.GetType().Name;

            if (_type != null)
                return _type.Name;

            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            if (ComponentName != null)
                return ComponentName;

            return Component != null ? Component.ToString() : null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return _typeConverter;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return _defaultEvent;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return _defaultProperty;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            object editor;
            if (_editors.TryGetValue(editorBaseType, out editor))
                return editor;

            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return _events;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return _events;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return Properties;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return Properties;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return Component;
        }
    }
}