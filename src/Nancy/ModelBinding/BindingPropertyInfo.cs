using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nancy.ModelBinding
{
	/// <summary>
	/// Represents a bindable member of a type, which can be a property or a field.
	/// </summary>
	public class BindingPropertyInfo
	{
		PropertyInfo _propertyInfo;
		FieldInfo _fieldInfo;

		/// <summary>
		/// Gets a reference to the MemberInfo that this BindingPropertyInfo represents. This can be a property or a field.
		/// </summary>
		public MemberInfo MemberInfo
		{
			get { return _propertyInfo ?? (MemberInfo)_fieldInfo; }
		}

		/// <summary>
		/// Gets the name of the property or field represented by this BindingPropertyInfo.
		/// </summary>
		public string Name
		{
			get { return this.MemberInfo.Name; }
		}

		/// <summary>
		/// Gets the data type of the property or field represented by this BindingPropertyInfo.
		/// </summary>
		public Type PropertyType
		{
			get
			{
				if (_propertyInfo != null)
					return _propertyInfo.PropertyType;
				else
					return _fieldInfo.FieldType;
			}
		}

		/// <summary>
		/// Constructs a BindingPropertyInfo instance for a property.
		/// </summary>
		/// <param name="propertyInfo">The bindable property to represent.</param>
		public BindingPropertyInfo(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException("propertyInfo");

			_propertyInfo = propertyInfo;
		}

		/// <summary>
		/// Constructs a BindingPropertyInfo instance for a field.
		/// </summary>
		/// <param name="fieldInfo">The bindable field to represent.</param>
		public BindingPropertyInfo(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
				throw new ArgumentNullException("fieldInfo");

			_fieldInfo = fieldInfo;
		}

		/// <summary>
		/// Gets the value from a specified object associated with the property or field represented by this BindingPropertyInfo.
		/// </summary>
		/// <param name="sourceObject">The object whose property or field should be retrieved.</param>
		/// <returns>The value for this BindingPropertyInfo's property or field in the specified object.</returns>
		public object GetValue(object sourceObject)
		{
			if (_propertyInfo != null)
				return _propertyInfo.GetValue(sourceObject, null);
			else
				return _fieldInfo.GetValue(sourceObject);
		}

		/// <summary>
		/// Sets the value from a specified object associated with the property or field represented by this BindingPropertyInfo.
		/// </summary>
		/// <param name="destinationObject">The object whose property or field should be assigned.</param>
		/// <param name="newValue">The value to assign in the specified object to this BindingPropertyInfo's property or field.</param>
		public void SetValue(object destinationObject, object newValue)
		{
			if (_propertyInfo != null)
				_propertyInfo.SetValue(destinationObject, newValue, null);
			else
				_fieldInfo.SetValue(destinationObject, newValue);
		}

		/// <summary>
		/// Returns an enumerable sequence of bindable properties for the specified type.
		/// </summary>
		/// <typeparam name="T">The type to enumerate.</typeparam>
		/// <returns>Bindable properties.</returns>
		public static IEnumerable<BindingPropertyInfo> Collect<T>()
		{
			return Collect(typeof(T));
		}

		/// <summary>
		/// Returns an enumerable sequence of bindable properties for the specified type.
		/// </summary>
		/// <param name="type">The type to enumerate.</param>
		/// <returns>Bindable properties.</returns>
		public static IEnumerable<BindingPropertyInfo> Collect(Type type)
		{
			var fromProperties = type
				.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite)
				.Where(property => !property.GetIndexParameters().Any())
				.Select(property => new BindingPropertyInfo(property));

			var fromFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => !f.IsInitOnly)
				.Select(field => new BindingPropertyInfo(field));

			return fromProperties.Concat(fromFields);
		}
	}
}
