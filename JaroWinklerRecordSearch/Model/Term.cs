using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public readonly struct Term : IEquatable<Term>
    {
        public static readonly Term None = new(-1, "", "");


        public int Id { get; }
        public string Orig { get; }
        public string Norm { get; }

        [JsonConstructor]
        public Term(int id, string orig, string norm)
        {
            Id = id;
            Orig = orig;
            Norm = norm;
        }

        public void Deconstruct(out int id, out string orig, out string norm)
        {
            id = Id;
            orig = Orig;
            norm = Norm;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public bool Equals(Term other)
        {
            return Id == other.Id && Orig == other.Orig && Norm == other.Norm;
        }

        public override bool Equals(object obj)
        {
            return obj is Term other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Orig != null ? Orig.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Norm != null ? Norm.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Term left, Term right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Term left, Term right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Orig}|{Norm} ({Id})";
        }
    }
}