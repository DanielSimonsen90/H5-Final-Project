#nullable disable

using SmartWeightLib;

namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// Represents end user, that uses the product
    /// </summary>
    public class User : IDbItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

        public User() { }
        public User(string username, string password, params Measurement[] measurements)
        {
            Username = username;
            Password = password;
            Measurements = measurements.ToList();
        }
    }
}
