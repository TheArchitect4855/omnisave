namespace OmniSave.SaveAdapter;

/// <summary>
/// A simple save adapter that wraps <see cref="BinarySerializer" />.
/// </summary>
public class BinarySaveAdapter : ISaveAdapter
{
	public void Load(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var reader = new BinaryReader(stream);
		var count = reader.ReadInt32();

		var saveItemIndex = 0;
		for (var i = 0; i < count && saveItemIndex < saveItems.Count; i += 1)
		{
			var key = reader.ReadString();
			var size = reader.ReadUInt32();

			var cmp = key.CompareTo(saveItems[saveItemIndex].Key);
			while (cmp > 0 && saveItemIndex < saveItems.Count)
			{
				// Key exists in program, but not in save file.
				saveItemIndex += 1;
				cmp = key.CompareTo(saveItems[saveItemIndex].Key);
			}

			if (saveItemIndex >= saveItems.Count) break;

			if (cmp < 0)
			{
				// Key exists in save file, but not in program.
				reader.BaseStream.Seek(size, SeekOrigin.Current);
				continue;
			}

			var saveItem = saveItems[saveItemIndex];
			var value = BinarySerializer.Deserialize(saveItem.ValueType, reader);
			saveItem.Set(value);
			saveItemIndex += 1;
		}
	}

	public Task LoadAsync(Stream stream, IReadOnlyList<SaveItem> saveItems) => Task.Run(() => Load(stream, saveItems));

	public void Save(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var streamWriter = new BinaryWriter(stream);
		streamWriter.Write(saveItems.Count);

		var buffer = new MemoryStream();
		var bufferWriter = new BinaryWriter(buffer);
		foreach (var item in saveItems)
		{
			buffer.SetLength(0);
			BinarySerializer.Serialize(item.Get(), bufferWriter);

			var size = (uint)buffer.Position;
			streamWriter.Write(item.Key);
			streamWriter.Write(size);

			buffer.Position = 0;
			buffer.CopyTo(streamWriter.BaseStream);
		}
	}

	public Task SaveAsync(Stream stream, IReadOnlyList<SaveItem> saveItems) => Task.Run(() => Save(stream, saveItems));
}
