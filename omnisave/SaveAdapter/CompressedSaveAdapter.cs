using System.IO.Compression;

namespace OmniSave.SaveAdapter;

/// <summary>
/// A save adapter that wraps another save adapter and compresses it using <see cref="GZipStream" />.
/// </summary>
public class CompressedSaveAdapter(ISaveAdapter inner, CompressionLevel compressionLevel) : ISaveAdapter
{
	public readonly CompressionLevel CompressionLevel = compressionLevel;
	private readonly ISaveAdapter _inner = inner;

	public void Load(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var compressed = new GZipStream(stream, CompressionMode.Decompress);
		_inner.Load(compressed, saveItems);
	}

	public Task LoadAsync(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var compressed = new GZipStream(stream, CompressionMode.Decompress);
		return _inner.LoadAsync(compressed, saveItems);
	}

	public void Save(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var compressed = new GZipStream(stream, CompressionLevel);
		_inner.Save(compressed, saveItems);
		compressed.Flush();
	}

	public async Task SaveAsync(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var compressed = new GZipStream(stream, CompressionLevel);
		await _inner.SaveAsync(stream, saveItems);
		await compressed.FlushAsync();
	}
}
