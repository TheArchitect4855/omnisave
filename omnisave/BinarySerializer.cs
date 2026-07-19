namespace OmniSave;

/// <summary>
/// A utility for serializing and deserializing objects to a binary stream.
/// </summary>
/// <remarks>
/// Supported types: <br />
/// - <c>bool</c> <br />
/// - <c>byte</c> <br />
/// - <c>sbyte</c> <br />
/// - <c>char</c> <br />
/// - <c>decimal</c> <br />
/// - <c>double</c> <br />
/// - <c>float</c> <br />
/// - <c>int</c> <br />
/// - <c>uint</c> <br />
/// - <c>long</c> <br />
/// - <c>ulong</c> <br />
/// - <c>short</c> <br />
/// - <c>ushort</c> <br />
/// - <c>string</c> <br />
/// - Arrays of supported types, up to any level of nesting (i.e. all of <c>int[]</c>, <c>float[][]</c>, and <c>string[][][]</c> are supported) <br />
/// </remarks>
public static class BinarySerializer
{
	/// <summary>
	/// Deserializes from <c>reader</c> into type <c>type</c>.
	///
	/// See <see cref="BinarySerializer" /> for a list of supported types.
	/// </summary>
	/// <param name="type">The target type to deserialize into.</param>
	/// <param name="reader">The stream to deserialize from.</param>
	/// <returns>The value deserialized from the stream.</returns>
	/// <exception cref="DeserializeException">Thrown if the requested type is not supported.</exception>
	public static object? Deserialize(Type type, BinaryReader reader)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return Deserialize(type.GenericTypeArguments[0], reader);

		var hasValue = reader.ReadBoolean();
		if (!hasValue) return null;

		if (type == typeof(bool)) return reader.ReadBoolean();
		else if (type == typeof(byte)) return reader.ReadByte();
		else if (type == typeof(sbyte)) return reader.ReadSByte();
		else if (type == typeof(char)) return reader.ReadChar();
		else if (type == typeof(decimal)) return reader.ReadDecimal();
		else if (type == typeof(double)) return reader.ReadDouble();
		else if (type == typeof(float)) return reader.ReadSingle();
		else if (type == typeof(int)) return reader.ReadInt32();
		else if (type == typeof(uint)) return reader.ReadUInt32();
		else if (type == typeof(long)) return reader.ReadInt64();
		else if (type == typeof(ulong)) return reader.ReadUInt64();
		else if (type == typeof(short)) return reader.ReadInt16();
		else if (type == typeof(ushort)) return reader.ReadUInt16();
		else if (type == typeof(string)) return reader.ReadString();
		else if (type.IsArray) return DeserializeArray(type, reader);
		else throw new DeserializeException(type);
	}

	/// <summary>
	/// Serializes <c>value</c> to <c>writer</c>.
	///
	/// See <see cref="BinarySerializer" /> for a list of supported types.
	/// </summary>
	/// <param name="value">The value to serialize.</param>
	/// <param name="writer">The stream to serialize to.</param>
	/// <exception cref="SerializeException">Thrown if the type of <c>value</c> is not supported.</exception>
	public static void Serialize(object? value, BinaryWriter writer)
	{
		writer.Write(value != null);
		if (value == null) return;

		if (value is bool) writer.Write((bool)value);
		else if (value is byte) writer.Write((byte)value);
		else if (value is sbyte) writer.Write((sbyte)value);
		else if (value is char) writer.Write((char)value);
		else if (value is decimal) writer.Write((decimal)value);
		else if (value is double) writer.Write((double)value);
		else if (value is float) writer.Write((float)value);
		else if (value is int) writer.Write((int)value);
		else if (value is uint) writer.Write((uint)value);
		else if (value is long) writer.Write((long)value);
		else if (value is ulong) writer.Write((ulong)value);
		else if (value is short) writer.Write((short)value);
		else if (value is ushort) writer.Write((ushort)value);
		else if (value is string) writer.Write((string)value);
		else if (value is Array) SerializeArray((Array)value, writer);
		else throw new SerializeException(value.GetType());
	}

	private static Array DeserializeArray(Type arrayType, BinaryReader reader)
	{
		var length = reader.ReadInt32();
		var array = (Array)Activator.CreateInstance(arrayType, length)!;
		var elementType = arrayType.GetElementType()!;
		for (var i = 0; i < length; i += 1) array.SetValue(Deserialize(elementType, reader), i);
		return array;
	}

	private static void SerializeArray(Array value, BinaryWriter writer)
	{
		writer.Write(value.Length);
		foreach (var i in value) Serialize(i, writer);
	}

	public class DeserializeException : InvalidOperationException
	{
		public DeserializeException(Type type) : base($"cannot deserialize {type}") { }
	}

	public class SerializeException : InvalidOperationException
	{
		public SerializeException(Type type) : base($"cannot serialize {type}") { }
	}
}
