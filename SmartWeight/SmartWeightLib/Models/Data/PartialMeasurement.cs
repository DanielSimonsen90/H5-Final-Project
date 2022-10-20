using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// A partial measurement sent by the weight, when it has been used.
    /// </summary>
    public class PartialMeasurement : IDbItem
    {
        public int Id { get; set; }
        public int WeightId { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }

        public PartialMeasurement() { }
        public PartialMeasurement(int weightId, double value, DateTime? date)
        {
            WeightId = weightId;
            Value = value;
            Date = date ?? DateTime.Now;
        }
    }
}
