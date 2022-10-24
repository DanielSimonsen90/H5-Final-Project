#nullable enable

using SmartWeightApp.Models;

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
                if (_validator is not null && !_validator(value)) throw new InvalidDataException("Received value does not pass validation.");
                _value = value;

                if (_key.HasValue) LocalStorage.Set(_key.Value, value);
            }
		}

		private readonly Func<T?, bool>? _validator;
		private readonly StorageKeys? _key = default;

		public DataStore(T? initialValue, StorageKeys? key = default, Func<T?, bool>? validator = null)
		{
            _value = LocalStorage.Get<T>(StorageKeys.USER).Result ?? initialValue;
			_key = key;
            _validator = validator;
		}
	}
}
