using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace JaroWinklerRecordSearch.Model
{
    [PublicAPI]
    public struct ScdPerson
    {
        public static readonly ScdPerson None = new(-1, "", "", "", "", "", "");

        public int Id { get; }
        public string Gid { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FirstNameNat { get; }
        public string LastNameNat { get; }
        public string OrgCode { get; }

        [JsonConstructor]
        public ScdPerson(int id, string gid, string firstName, string lastName, string firstNameNat, string lastNameNat, string orgCode)
        {
            Id = id;
            Gid = gid;
            FirstName = firstName;
            LastName = lastName;
            FirstNameNat = firstNameNat;
            LastNameNat = lastNameNat;
            OrgCode = orgCode;
        }

        public (int id, string gid, string gn, string sn, string gnn, string snn) ToTuple() =>
            (Id, Gid, FirstName, LastName, FirstNameNat, LastNameNat);
    }
}