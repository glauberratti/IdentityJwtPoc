namespace IdentityJwtPoc.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime LastChange { get; set; } = DateTime.Now;

        public bool HasChange(string lastChangeClaim)
        {
            if (String.IsNullOrEmpty(lastChangeClaim))
                return false;

            var ok = DateTime.TryParse(lastChangeClaim, out DateTime lastChange);

            if (!ok)
                return false;

            return LastChangeWithoutMs() > lastChange;
        }

        private DateTime LastChangeWithoutMs()
        {
            return new DateTime(LastChange.Year, LastChange.Month, LastChange.Day, LastChange.Hour, LastChange.Minute, LastChange.Second);
        }
    }
}
