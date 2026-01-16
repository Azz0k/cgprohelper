using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Helper.Models
{
    public class EmailFields: IEquatable<EmailFields>
    {
        public string From { get; set; }
        public HashSet<string> To { get; set; } = [];
        public EmailFields(string from) 
        {
            From = from;
        }
        public override bool Equals(object? obj)
        => Equals(obj as EmailFields);
        public bool Equals(EmailFields? other)
        {
            if (other == null) return false;
            if (!String.Equals(this.From, other.From, StringComparison.OrdinalIgnoreCase)) return false;
            return this.To.SetEquals(other.To);

        }
        public override int GetHashCode()
        {
            int hash = From.ToLowerInvariant().GetHashCode();
            foreach (var email in To.OrderBy(e => e))
            {
                hash = HashCode.Combine(hash, email.ToLowerInvariant().GetHashCode());
            }
            return hash;
        }
    }
}
