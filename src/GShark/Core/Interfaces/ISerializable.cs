namespace GShark.Core.Interfaces
{
    public interface ISerializable<T>
    {
        public T FromJson(string s);
        public string ToJson();
    }
}
