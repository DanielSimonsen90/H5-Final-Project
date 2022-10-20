using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// Full measurement
    /// </summary>
    public class Measurement : PartialMeasurement
    {
        public int? UserId { get; set; }

        public Measurement() { }
        public Measurement(int userId, int weightId, double value, DateTime? date) :
            base(weightId, value, date)
        {
            UserId = userId;
        }
        public Measurement(PartialMeasurement partial, int? userId) :
            base(partial.WeightId, partial.Value, partial.Date)
        {
            UserId = userId;
        }
    }
}
