using OmniSave;
using OmniSave.SaveAdapter;

var saveManager = new SaveManager(new XorSaveAdapter(
	new BinarySaveAdapter(), new byte[] { 0x4B, 0x55, 0x52, 0x54, 0x20, 0x32, 0x30, 0x32, 0x35, 0x20, 0x31, 0x31, 0x20, 0x30, 0x32, 0x21 }
));
Console.WriteLine($"Counter: {Data.Counter}");

Console.WriteLine($"Loading save...");
using var file = File.Open("save.dat", FileMode.OpenOrCreate);
saveManager.Load(file);
Console.WriteLine($"Save loaded. Counter: {Data.Counter}");

Data.Counter += 1;
Console.WriteLine($"Saving counter: {Data.Counter}");
file.SetLength(0);
saveManager.Save(file);

public static class Data
{
	[Save] public static int Counter = 0;
}
