namespace OmniSave.SaveAdapter;

public class XorSaveAdapter(ISaveAdapter baseAdapter, byte[] key) : ISaveAdapter
{
	private readonly ISaveAdapter _baseAdapter = baseAdapter;
	private readonly byte[] _key = key;

	public void Load(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var xor = new XorStream(stream, _key);
		_baseAdapter.Load(xor, saveItems);
	}

	public Task LoadAsync(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var xor = new XorStream(stream, _key);
		return _baseAdapter.LoadAsync(xor, saveItems);
	}

	public void Save(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var xor = new XorStream(stream, _key);
		_baseAdapter.Save(xor, saveItems);
	}

	public Task SaveAsync(Stream stream, IReadOnlyList<SaveItem> saveItems)
	{
		var xor = new XorStream(stream, _key);
		return _baseAdapter.SaveAsync(xor, saveItems);
	}

	private class XorStream(Stream baseStream, byte[] key) : Stream
	{
		public override bool CanRead => BaseStream.CanRead;
		public override bool CanSeek => false;
		public override bool CanWrite => BaseStream.CanWrite;
		public override long Length => BaseStream.Length;
		public override long Position { get => BaseStream.Position; set => SetLength(value); }

		public readonly Stream BaseStream = baseStream;
		private readonly byte[] _key = key;
		private byte[] _writeBuffer = Array.Empty<byte>();
		private int _keyIndex = 0;

		public override void Flush() => BaseStream.Flush();

		public override int Read(byte[] buffer, int offset, int count)
		{
			var length = BaseStream.Read(buffer, offset, count);
			XorBuffer(buffer, offset, count);
			return length;
		}

		public override int ReadByte()
		{
			var i = BaseStream.ReadByte();
			if (i >= 0) i ^= _key[_keyIndex++ % _key.Length];
			return i;
		}

		public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException("seeking is not supported");
		public override void SetLength(long value) => BaseStream.SetLength(value);

		public override void Write(byte[] buffer, int offset, int count)
		{
			XorWrite(buffer, offset, count);
			BaseStream.Write(_writeBuffer, 0, count);
		}

		public override void WriteByte(byte value) => BaseStream.WriteByte((byte)(value % _key[_keyIndex++ % _key.Length]));

		private void XorBuffer(byte[] buffer, int offset, int count)
		{
			for (var i = offset; i < offset + count; i += 1) buffer[i] ^= _key[_keyIndex++ % _key.Length];
		}

		private void XorWrite(byte[] source, int offset, int count)
		{
			if (_writeBuffer.Length < count) _writeBuffer = new byte[count];
			for (var i = 0; i < count; i += 1) _writeBuffer[i] = (byte)(source[i + offset] ^ _key[_keyIndex++ % _key.Length]);
		}
	}
}
