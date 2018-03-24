using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_one_project
{
    public class CountryData : IEquatable<CountryData>, IComparable<CountryData>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Link { get; set; }
        public int ZonesCount { get; set; }
        public IList<string> Zones { get; set; }
        public CountryData(string name)
        {
            Name = name;
            Zones = new List<string>();
        }
        public bool Equals(CountryData other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (Name).GetHashCode();
        }

        public int CompareTo(CountryData other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return 1;
            }
            return Name.CompareTo(other.Name);
        }

    }
}
