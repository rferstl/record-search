using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct FieldPos : IEquatable<FieldPos>
    {
        public FieldEnum Field { get; }
        public int Pos { get; }

        [JsonConstructor]
        public FieldPos(FieldEnum field, int pos)
        {
            Field = field;
            Pos = pos;
        }

        public void Deconstruct(out FieldEnum field, out int pos)
        {
            field = Field;
            pos = Pos;
        }

        public bool Equals(FieldPos other) 
        {
            return Field == other.Field && Pos == other.Pos;
        }

        public override bool Equals(object obj)
        {
            return obj is FieldPos other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Field * 397) ^ Pos;
            }
        }

        public static bool operator ==(FieldPos left, FieldPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FieldPos left, FieldPos right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Field}.{Pos}";
        }
        public object ToDump()
        {
            return $"{Field}.{Pos}";
        }

    }
}