#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Studio.Core.CorePublic
File: MarketDataSettingsCache.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Studio.Core
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;

	using Ecng.Collections;
	using Ecng.Common;
	using Ecng.Serialization;

	using StockSharp.Localization;

	public class MarketDataSettingsCache : IPersistable
	{
		public ObservableCollection<MarketDataSettings> Settings { get; private set; }

		public MarketDataSettings NewSettingsItem { get; private set; }

		public event Action Changed;

		public MarketDataSettingsCache()
		{
			NewSettingsItem = new MarketDataSettings
			{
				Id = Guid.Empty,
				Path = LocalizedStrings.Str3229 + "..."
			};

			Settings = new ObservableCollection<MarketDataSettings> { NewSettingsItem };
			Settings.CollectionChanged += OnSettingsCollectionChanged;
		}

		private void OnSettingsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Changed.SafeInvoke();
		}

		public void Save()
		{
			Changed.SafeInvoke();
		}

		public void Load(SettingsStorage storage)
		{
			var settings = storage
				.GetValue<IEnumerable<SettingsStorage>>("Settings", new SettingsStorage[0])
				.Select(s => s.Load<MarketDataSettings>());

			Settings.Clear();
			Settings.Add(NewSettingsItem);
			Settings.AddRange(settings);
		}

		public void Save(SettingsStorage storage)
		{
			storage.SetValue("Settings", Settings.Where(i => i != NewSettingsItem).Select(s => s.Save()).ToArray());
		}
	}
}