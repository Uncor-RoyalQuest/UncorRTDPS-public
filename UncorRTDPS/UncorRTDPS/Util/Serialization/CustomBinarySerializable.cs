using System.IO;

namespace UncorRTDPS.Util.Serialization
{
    public interface ICustomBinarySerializable
    {
        void ReadObject(BinaryReader binaryReader);
        void WriteObject(BinaryWriter binaryWriter);
    }
}
