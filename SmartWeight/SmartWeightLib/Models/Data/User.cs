#nullable disable

using SmartWeightLib;

namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// Represents end user, that uses the product
    /// </summary>
    public class User : Login, IDbItem
    {
        public int Id { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

        public User() : base(string.Empty, string.Empty) { }
        public User(string username, string password, params Measurement[] measurements) : base(username, password)
        {
            Username = username;
            Password = password;
            Measurements = measurements.ToList();
        }
    }
}
