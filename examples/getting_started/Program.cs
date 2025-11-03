using OmniSave;
using OmniSave.SaveAdapter;

var saveManager = new SaveManager(new BinarySaveAdapter());
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
