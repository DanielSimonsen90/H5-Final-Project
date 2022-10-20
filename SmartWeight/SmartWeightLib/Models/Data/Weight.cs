namespace SmartWeightLib.Models.Data
{
    /// <summary>
    /// Weight class to represent a weight
    /// </summary>
    public class Weight : IDbItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public Weight() { }
        public Weight(string name)
        {
            Name = name;
        }
    }
}
