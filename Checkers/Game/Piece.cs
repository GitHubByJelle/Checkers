using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PieceC
{
    [Serializable]
    internal class Piece
    {
        public int id;
        int dir;

        public Piece(int id)
        {
            this.id = id;
            if (id == 1) this.dir = -1; // UP
            else if (id == 2) this.dir = 1; // DOWN
        }

        public int getId() { return this.id; }

        public int getDir() { return this.dir; }
    }

    public static class ExtensionMethods
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
