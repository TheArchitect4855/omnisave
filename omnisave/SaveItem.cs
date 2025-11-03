using System.Reflection;

namespace OmniSave;

/// <summary>
/// A value to be saved.
/// </summary>
public readonly struct SaveItem : IComparable<SaveItem>
{
	/// <summary>
	/// A unique key for this SaveItem.
	/// </summary>
	public readonly string Key;

	/// <summary>
	/// The type of this <c>SaveItem</c>'s value.
	/// </summary>
	public readonly Type ValueType;

	private readonly MemberInfo _member;

	/// <summary>
	/// Creates a new <c>SaveItem</c> from the passed <c>MemberInfo</c>.
	/// </summary>
	/// <param name="member">The target item to save. This must be either a <c>FieldInfo</c> or a <c>PropertyInfo</c>.</param>
	/// <exception cref="ArgumentException">Thrown if <c>member</c> is not a <c>FieldInfo</c> or a <c>PropertyInfo</c>.</exception>
	public SaveItem(MemberInfo member)
	{
		if (member is FieldInfo field)
		{
			if (!field.IsStatic) throw new ArgumentException($"{nameof(member)} must be static");
			ValueType = field.FieldType;
		}
		else if (member is PropertyInfo property)
		{
			var getter = property.GetMethod;
			if (getter == null) throw new ArgumentException($"{nameof(property)} is missing getter");

			var setter = property.SetMethod;
			if (setter == null) throw new ArgumentException($"{nameof(property)} is missing setter");

			if (!getter.IsStatic || !setter.IsStatic) throw new ArgumentException($"{nameof(property)} must be static");

			ValueType = property.PropertyType;
		}
		else throw new ArgumentException($"{nameof(member)} is not a field or property");

		Key = $"{member.DeclaringType!.FullName}.{member.Name}";
		_member = member;
	}

	/// <returns>The value of this save item.</returns>.
	public object? Get()
	{
		if (_member is FieldInfo field) return field.GetValue(null);
		else if (_member is PropertyInfo property) return property.GetValue(null);
		else throw new NotImplementedException();
	}

	/// <param name="value">The value to set this save item to.</param>
	/// <exception cref="ArgumentException">Thrown if the passed value cannot be converted to the destination type.</exception>
	public void Set(object? value)
	{
		if (_member is FieldInfo field) field.SetValue(null, value);
		else if (_member is PropertyInfo property) property.SetValue(null, value);
		else throw new NotImplementedException();
	}

	/// <summary>
	/// Compares two <c>SaveItem</c>s by key.
	/// </summary>
	public int CompareTo(SaveItem other) => Key.CompareTo(other.Key);
}
