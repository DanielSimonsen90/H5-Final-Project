using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// Connection between user and weight, to bind weight's partial measurement into a full measurement
    /// </summary>
    public class Connection : IDbItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int WeightId { get; set; }
        [ForeignKey("WeightId")]
        public Weight Weight { get; set; }

        public Connection() { }
        public Connection(User user, Weight weight)
        {
            User = user;
            UserId = user.Id;
            Weight = weight;
            WeightId = weight.Id;
        }
    }
}
