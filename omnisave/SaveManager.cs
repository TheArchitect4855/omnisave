using System.Reflection;
using OmniSave.SaveAdapter;

namespace OmniSave;

/// <summary>
/// A wrapper around an <c>ISaveAdapter</c> to handle saving and loading of data from a stream.
/// </summary>
/// <remarks>
/// The <c>SaveManager</c> acquires a global lock when saving or loading data. Therefore, it is guaranteed
/// that there will never be saving or loading on different threads at the same time. This also
/// means that calling <c>Save*</c> or <c>Load*</c> methods from within an adapter will cause a
/// deadlock. (there is no reasonable scenario to do this anyways)
/// </remarks>
public class SaveManager
{
	private readonly ISaveAdapter _adapter;
	private readonly SaveItem[] _saveItems;

	public SaveManager(ISaveAdapter adapter) : this(adapter, Assembly.GetCallingAssembly()) { }

	public SaveManager(ISaveAdapter adapter, params Assembly[] assemblies)
	{
		_adapter = adapter;
		_saveItems = assemblies
			.SelectMany(assembly => assembly.DefinedTypes)
			.SelectMany(type => type.DeclaredFields.Cast<MemberInfo>().Concat(type.DeclaredProperties))
			.Where(member => member.GetCustomAttribute<SaveAttribute>() != null)
			.Select(member => new SaveItem(member))
			.ToArray();
		Array.Sort<SaveItem>(_saveItems);
	}

	/// <summary>
	/// Loads save data from a stream.
	/// </summary>
	/// <param name="stream">The stream to load from.</param>
	/// <exception cref="ArgumentException">Thrown if <c>stream</c> is not readable.</exception>
	public void Load(Stream stream)
	{
		if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} must be readable");
		if (stream.Length == 0) return; // Nothing to do if the save file is empty
		lock (s_lock) _adapter.Load(stream, _saveItems);
	}

	/// <summary>
	/// Loads save data from a stream, asynchronously.
	/// </summary>
	/// <param name="stream">The stream to load from.</param>
	/// <exception cref="ArgumentException">Thrown if <c>stream</c> is not readable.</exception>
	public Task LoadAsync(Stream stream)
	{
		if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} must be readable");
		if (stream.Length == 0) return Task.CompletedTask; // Nothing to do if the save file is empty
		lock (s_lock) return _adapter.LoadAsync(stream, _saveItems);
	}

	/// <summary>
	/// Resets all save data. This does NOT persist the newly-reset data. (call <c>Load*</c> to do so)
	/// </summary>
	public void Reset()
	{
		lock (s_lock) foreach (var item in _saveItems) item.Set(null);
	}

	/// <summary>
	/// Saves data to a stream.
	/// </summary>
	/// <param name="stream">The stream to save to.</param>
	/// <exception cref="ArgumentException">Thrown if <c>stream</c> is not writable.</exception>
	public void Save(Stream stream)
	{
		if (!stream.CanWrite) throw new ArgumentException($"{nameof(stream)} must be writable");
		lock (s_lock) _adapter.Save(stream, _saveItems);
	}

	/// <summary>
	/// Saves data to a stream, asynchronously.
	/// </summary>
	/// <param name="stream">The stream to save to.</param>
	/// <exception cref="ArgumentException">Thrown if <c>stream</c> is not writable.</exception>
	public Task SaveAsync(Stream stream)
	{
		if (!stream.CanWrite) throw new ArgumentException($"{nameof(stream)} must be writable");
		lock (s_lock) return _adapter.SaveAsync(stream, _saveItems);
	}

	private static readonly object s_lock = new();
}
