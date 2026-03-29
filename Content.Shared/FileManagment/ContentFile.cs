namespace Content.Shared.FileManagment;

public readonly struct ContentFile
{
    public string Name { get; }
    public string Extension { get; }
    public string FullName => Name + "." + Extension;
    public byte[] Data { get; }
    public int Length => Data.Length;

    public ContentFile()
    {
        Name = "";
        Extension = "";
        Data = [];
    }

    public ContentFile(string name, string extension, byte[] data)
    {
        Name = name;
        Extension = extension;
        Data = data;
    }

    public ContentFile(string fullName, byte[] data)
    {
        var split = fullName.Split('.');
        Name = split[0];
        Extension = split[1];
        Data = data;
    }
}