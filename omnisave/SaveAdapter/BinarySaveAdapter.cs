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
		if (count <= 0) return;

		var saveItem = 0;
		var key = reader.ReadString();
		while (saveItem < saveItems.Count)
		{
			var cmp = key.CompareTo(saveItems[saveItem].Key);
			if (cmp < 0) throw new LoadException("A key exists in the save file that does not exist in the program. Saved items cannot be removed, as their type information is necessary to deserialize the save file.");
			if (cmp > 0)
			{
				// Key exists in program, but not in save file. This is fine.
				saveItem += 1;
				continue;
			}

			var value = BinarySerializer.Deserialize(saveItems[saveItem].ValueType, reader);
			saveItems[saveItem].Set(value);

			if (--count == 0) break;
			key = reader.ReadString();
		}
	}

	public Task LoadAsync(Stream stream, IReadOnlyList<SaveItem> saveItems) => Task.Run(() => Load(stream, saveItems));

	public void Save(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var writer = new BinaryWriter(stream);
		writer.Write(saveItems.Count);
		foreach (var item in saveItems)
		{
			writer.Write(item.Key);
			BinarySerializer.Serialize(item.Get(), writer);
		}
	}

	public Task SaveAsync(Stream stream, IReadOnlyList<SaveItem> saveItems) => Task.Run(() => Save(stream, saveItems));

	public class LoadException : Exception
	{
		public LoadException(string message) : base(message) { }
	}
}
