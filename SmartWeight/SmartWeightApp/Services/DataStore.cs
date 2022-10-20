#nullable enable

namespace SmartWeightApp.Services
{
    public class DataStore<T>
    {
		private T? _value;
		public T? Value
		{
			get => _value;
			set 
			{
                if (!Validator(value)) throw new InvalidDataException("Received value does not pass validation.");
                _value = value;
			}
		}

		private readonly Func<T?, bool> Validator;

		public DataStore(T? initialValue, Func<T?, bool> validator)
		{
            _value = initialValue;
            Validator = validator;
		}
	}
}
