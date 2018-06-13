using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.Room
{
    public enum GuestIdentity : byte
    {
        MS,
        MR,
        MRS
    }

    public enum Sex : byte
    {
        MALE,
        FEMALE
    }

    public struct Guest
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public GuestIdentity Identity { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birthday { get; set; }
        public string PreferredLanguage { get; set; }
        public string AreaFrom { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
}
