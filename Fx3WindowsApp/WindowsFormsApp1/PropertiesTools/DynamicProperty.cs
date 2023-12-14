using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImagerDebugSoftware.PropertiesTools
{
    public sealed class DynamicProperty : PropertyDescriptor
    {
        private readonly Type _type;
        private readonly bool _hasDefaultValue;
        private readonly object _defaultValue;
        private readonly PropertyDescriptor _existing;
        private readonly DynamicTypeDescriptor _descriptor;
        private Dictionary<Type, object> _editors;
        private bool? _readOnly;
        private bool? _browsable;
        private string _displayName;
        private string _description;
        private string _category;
        private List<Attribute> _attributes = new List<Attribute>();

        internal DynamicProperty(DynamicTypeDescriptor descriptor, Type type, object value, string name, Attribute[] attrs)
            : base(name, attrs)
        {
            _descriptor = descriptor;
            _type = type;
            Value = value;
            DefaultValueAttribute def = DynamicTypeDescriptor.GetAttribute<DefaultValueAttribute>(Attributes);
            if (def == null)
            {
                _hasDefaultValue = false;
            }
            else
            {
                _hasDefaultValue = true;
                _defaultValue = def.Value;
            }
            if (attrs != null)
            {
                foreach (Attribute att in attrs)
                {
                    _attributes.Add(att);
                }
            }
        }

        internal static Attribute[] GetAttributes(PropertyDescriptor existing)
        {
            List<Attribute> atts = new List<Attribute>();
            foreach (Attribute a in existing.Attributes)
            {
                atts.Add(a);
            }
            return atts.ToArray();
        }

        internal DynamicProperty(DynamicTypeDescriptor descriptor, PropertyDescriptor existing, object component)
            : this(descriptor, existing.PropertyType, existing.GetValue(component), existing.Name, GetAttributes(existing))
        {
            _existing = existing;
        }

        public void RemoveAttributesOfType<T>() where T : Attribute
        {
            List<Attribute> remove = new List<Attribute>();
            foreach (Attribute att in _attributes)
            {
                if (typeof(T).IsAssignableFrom(att.GetType()))
                {
                    remove.Add(att);
                }
            }

            foreach (Attribute att in remove)
            {
                _attributes.Remove(att);
            }
        }

        public IList<Attribute> AttributesList
        {
            get
            {
                return _attributes;
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(_attributes.ToArray());
            }
        }

        public object Value
        {
            get
            {
                return this.GetValue(this._descriptor.Component);
            }
            set
            {
                if (_existing != null)
                {
                    //Check if readonly is ture, if ture then set readonly as false. After set the value then set read
                    bool isReadonlyTemp = _existing.IsReadOnly;
                    SetIsReadOnly(false);

                    _existing.SetValue(this._descriptor.Component, value);

                    SetIsReadOnly(isReadonlyTemp);

                    _descriptor.OnValueChanged(this);
                    return;
                }

                _descriptor.OnValueChanged(this);
            }
        }

        public override bool CanResetValue(object component)
        {
            if (_existing != null)
                return _existing.CanResetValue(component);

            return _hasDefaultValue;
        }

        public override Type ComponentType
        {
            get
            {
                if (_existing != null)
                    return _existing.ComponentType;

                return typeof(object);
            }
        }

        public override object GetValue(object component)
        {
            if (_existing != null)
                return _existing.GetValue(component);

            return Value;
        }

        public string GetCategory()
        {
            string category = null;
            if (_category != null)
                category = (string)_category.Clone();
            else
                category = (string)base.Category.Clone();

            while (category != null && category.StartsWith("\t"))
                category = category.Substring(1);

            return category;
        }

        public void SetCategory(string category)
        {
            _category = category;
        }

        public override string Description
        {
            get
            {
                if (_description != null)
                    return _description;

                return base.Description;
            }
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public override string DisplayName
        {
            get
            {
                if (_displayName != null)
                    return _displayName;

                if (_existing != null)
                    return _existing.DisplayName;

                return base.DisplayName;
            }
        }

        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }

        public override bool IsBrowsable
        {
            get
            {
                if (_browsable.HasValue)
                    return _browsable.Value;

                if (_existing != null)
                    return _existing.IsBrowsable;

                return base.IsBrowsable;
            }
        }


        public void SetBrowsable(bool browsable)
        {
            _browsable = browsable;
            AttributeCollection attrs = this.Attributes;
            FieldInfo fld = typeof(BrowsableAttribute).GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[typeof(BrowsableAttribute)], browsable);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                if (_readOnly.HasValue)
                    return _readOnly.Value;

                if (_existing != null)
                    return _existing.IsReadOnly;

                ReadOnlyAttribute att = DynamicTypeDescriptor.GetAttribute<ReadOnlyAttribute>(Attributes);
                if (att == null)
                    return false;

                return att.IsReadOnly;
            }
        }

        public void SetIsReadOnly(bool readOnly)
        {
            _readOnly = readOnly;
            ReadOnlyAttribute readOnlyAttribute = _existing.Attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (readOnlyAttribute == null) return;
            FieldInfo fieldToChange = readOnlyAttribute.GetType().GetField("isReadOnly",
                                    System.Reflection.BindingFlags.NonPublic |
                                    System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(readOnlyAttribute, readOnly);
        }

        public void SetPropertyValue<T>(string propertyName, object propertyValue)
        {
            AttributeCollection attrs = this.Attributes;
            FieldInfo fld = typeof(T).GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[typeof(T)], propertyValue);
            }
        }

        public override Type PropertyType
        {
            get
            {
                if (_existing != null)
                    return _existing.PropertyType;

                return _type;
            }
        }

        public override void ResetValue(object component)
        {
            if (_existing != null)
            {
                _existing.ResetValue(component);

                _descriptor.OnValueChanged(this);
                return;
            }

            if (CanResetValue(component))
            {
                Value = _defaultValue;
                _descriptor.OnValueChanged(this);
            }
        }

        public override void SetValue(object component, object value)
        {
            Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            if (_existing != null)
                return _existing.ShouldSerializeValue(component);

            return false;
        }

        public override object GetEditor(Type editorBaseType)
        {
            if (editorBaseType == null)
                throw new ArgumentNullException("editorBaseType");

            if (_editors != null)
            {
                object type;
                if ((_editors.TryGetValue(editorBaseType, out type)) && (type != null))
                    return type;
            }
            return base.GetEditor(editorBaseType);
        }

        public void SetEditor(Type editorBaseType, object obj)
        {
            if (editorBaseType == null)
                throw new ArgumentNullException("editorBaseType");

            if (_editors == null)
            {
                if (obj == null)
                    return;

                _editors = new Dictionary<Type, object>();
            }
            if (obj == null)
            {
                _editors.Remove(editorBaseType);
            }
            else
            {
                _editors[editorBaseType] = obj;
            }
        }
    }

}

