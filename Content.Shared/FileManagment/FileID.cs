using Robust.Shared.Serialization;

namespace Content.Shared.FileManagment;

[Serializable, NetSerializable]
public readonly struct FileId: IEquatable<FileId>, IComparable<FileId>
{
    public readonly int Id;

    public FileId(int id)
    {
        Id = id;
    }

    public bool Equals(FileId other)
    {
        return Id == other.Id;
    }

    public int CompareTo(FileId other)
    {
        return Id.CompareTo(other.Id);
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}