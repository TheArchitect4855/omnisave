namespace OmniSave.SaveAdapter;

/// <summary>
/// An interface for managing the actual saving and loading of data.
/// </summary>
public interface ISaveAdapter
{
	/// <summary>
	/// Read the save data from <c>stream</c>.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="saveItems">A list of items to load.</param>
	/// <remarks>
	/// It is guaranteed that <c>stream</c> is readable
	/// and that <c>saveItems</c> are lexicographically ordered by key.
	/// </remarks>
	public void Load(Stream stream, IReadOnlyList<SaveItem> saveItems);

	/// <summary>
	/// Read the save data from <c>stream</c> asynchronously.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="saveItems">A list of items to load.</param>
	/// <remarks>
	/// It is guaranteed that <c>stream</c> is readable
	/// and that <c>saveItems</c> are lexicographically ordered by key.
	/// </remarks>
	public Task LoadAsync(Stream stream, IReadOnlyList<SaveItem> saveItems);

	/// <summary>
	/// Write the save data to <c>stream</c>.
	/// </summary>
	/// <param name="stream">The stream to write to.</param>
	/// <param name="saveItems">A list of items to save.</param>
	/// <remarks>
	/// It is guaranteed that <c>stream</c> is writable
	/// and that <c>saveItems</c> are lexicographically ordered by key.
	/// </remarks>
	public void Save(Stream stream, IReadOnlyList<SaveItem> saveItems);

	/// <summary>
	/// Write the save data to <c>stream</c> asynchronously.
	/// </summary>
	/// <param name="stream">The stream to write to.</param>
	/// <param name="saveItems">A list of items to save.</param>
	/// <remarks>
	/// It is guaranteed that <c>stream</c> is writable
	/// and that <c>saveItems</c> are lexicographically ordered by key.
	/// </remarks>
	public Task SaveAsync(Stream stream, IReadOnlyList<SaveItem> saveItems);
}
