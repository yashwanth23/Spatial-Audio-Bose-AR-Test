using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// An editor-only cache of the distinct locale keys across all passed <see cref="LocaleData"/> instances.
	/// </summary>
	internal sealed class LocaleKeyCache
	{
		/// <summary>
		/// A collection of the distinct locale keys across all passed <see cref="LocaleData"/> instances.
		/// </summary>
		public List<string> UniqueKeys
		{
			get { return _uniqueKeys; }
		}

		private LocaleData[] _localeData;
		private List<string> _uniqueKeys;

		/// <summary>
		/// Initializes a <see cref="LocaleKeyCache"/> instance with zero <see cref="LocaleData"/> assigned;
		/// these must be instead passed to the <seealso cref="Set"/> method.
		/// </summary>
		public LocaleKeyCache()
		{
			_localeData = new LocaleData[0];
		}

		/// <summary>
		/// Initializes a <see cref="LocaleKeyCache"/> instance with an array of <see cref="LocaleData"/>
		/// instances.
		/// </summary>
		/// <param name="localeData"></param>
		public LocaleKeyCache(LocaleData[] localeData)
		{
			_localeData = localeData;

			AggregateAllDistinctKeys();
		}

		/// <summary>
		/// Sets an array of <see cref="LocaleData"/> instances and aggregates their distinct keys into a new
		/// internal cache.
		/// </summary>
		/// <param name="localeData"></param>
		public void Set(LocaleData[] localeData)
		{
			_localeData = localeData;

			AggregateAllDistinctKeys();
		}

		/// <summary>
		/// Aggregates distinct keys from all <see cref="LocaleData"/> instances into a single collection.
		/// </summary>
		private void AggregateAllDistinctKeys()
		{
			Assert.IsNotNull(_localeData);

			if (_uniqueKeys == null)
			{
				_uniqueKeys = new List<string>();
			}
			else
			{
				_uniqueKeys.Clear();
			}

			for (var i = 0; i < _localeData.Length; i++)
			{
				Assert.IsNotNull(_localeData[i]);

				var ld = _localeData[i];
				var localeKVPs = ld.LocaleKVPs;
				for (var j = 0; j < localeKVPs.Count; j++)
				{
					if (!_uniqueKeys.Contains(localeKVPs[j].key))
					{
						_uniqueKeys.Add(localeKVPs[j].key);
					}
				}
			}
		}
	}
}
