#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Candles.Compression.Algo
File: ICandleBuilderSourceValue.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Candles.Compression
{
	using System;
	using System.Diagnostics;

	using StockSharp.BusinessEntities;
	using StockSharp.Messages;
	using Ecng.Collections;

	/// <summary>
	/// The interface that describes data of the <see cref="ICandleBuilder"/> source.
	/// </summary>
	public interface ICandleBuilderSourceValue
	{
		///// <summary>
		///// The instrument identifier by which data has been created.
		///// </summary>
		//SecurityId SecurityId { get; }

		/// <summary>
		/// The time of new data occurrence.
		/// </summary>
		DateTimeOffset Time { get; }

		/// <summary>
		/// Price.
		/// </summary>
		decimal Price { get; }

		/// <summary>
		/// Volume.
		/// </summary>
		decimal? Volume { get; }

		/// <summary>
		/// Order side.
		/// </summary>
		Sides? OrderDirection { get; }
	}

	/// <summary>
	/// The <see cref="ICandleBuilder"/> source data is created on basis of <see cref="Trade"/>.
	/// </summary>
	[DebuggerDisplay("{" + nameof(Trade) + "}")]
	public class TradeCandleBuilderSourceValue : ICandleBuilderSourceValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TradeCandleBuilderSourceValue"/>.
		/// </summary>
		/// <param name="trade">Tick trade.</param>
		public TradeCandleBuilderSourceValue(Trade trade)
		{
			Trade = trade;
		}

		/// <summary>
		/// Tick trade.
		/// </summary>
		public Trade Trade { get; }

		//SecurityId ICandleBuilderSourceValue.SecurityId => Trade.Security.ToSecurityId();

		DateTimeOffset ICandleBuilderSourceValue.Time => Trade.Time;

		decimal ICandleBuilderSourceValue.Price => Trade.Price;

		decimal? ICandleBuilderSourceValue.Volume => Trade.Volume == 0 ? (decimal?)null : Trade.Volume;

		Sides? ICandleBuilderSourceValue.OrderDirection => Trade.OrderDirection;
	}

	/// <summary>
	/// The <see cref="ICandleBuilder"/> source data is created on basis of <see cref="ExecutionMessage"/>.
	/// </summary>
	[DebuggerDisplay("{" + nameof(Tick) + "}")]
	public class TickCandleBuilderSourceValue : ICandleBuilderSourceValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TickCandleBuilderSourceValue"/>.
		/// </summary>
		/// <param name="tick">Tick trade.</param>
		public TickCandleBuilderSourceValue(ExecutionMessage tick)
		{
			if (tick == null)
				throw new ArgumentNullException(nameof(tick));

			Tick = tick;
		}

		/// <summary>
		/// Tick trade.
		/// </summary>
		public ExecutionMessage Tick { get; }

		//SecurityId ICandleBuilderSourceValue.SecurityId => Tick.SecurityId;

		DateTimeOffset ICandleBuilderSourceValue.Time => Tick.ServerTime;

		decimal ICandleBuilderSourceValue.Price => Tick.TradePrice ?? 0;

		decimal? ICandleBuilderSourceValue.Volume => Tick.TradeVolume;

		Sides? ICandleBuilderSourceValue.OrderDirection => Tick.OriginSide;
	}

	/// <summary>
	/// Types of candle depth based data.
	/// </summary>
	public enum DepthCandleSourceTypes
	{
		/// <summary>
		/// Best bid.
		/// </summary>
		BestBid,

		/// <summary>
		/// Best ask.
		/// </summary>
		BestAsk,

		/// <summary>
		/// Spread middle.
		/// </summary>
		Middle,
	}

	/// <summary>
	/// The <see cref="ICandleBuilder"/> source data is created on basis of <see cref="MarketDepth"/>.
	/// </summary>
	[DebuggerDisplay("{" + nameof(Depth) + "}")]
	public class DepthCandleBuilderSourceValue : ICandleBuilderSourceValue
	{
		private readonly decimal _price;
		private readonly decimal? _volume;

		/// <summary>
		/// Initializes a new instance of the <see cref="DepthCandleBuilderSourceValue"/>.
		/// </summary>
		/// <param name="depth">Market depth.</param>
		/// <param name="type">Type of candle depth based data.</param>
		public DepthCandleBuilderSourceValue(MarketDepth depth, DepthCandleSourceTypes type)
		{
			Depth = depth;
			Type = type;

			var pair = Depth.BestPair;

			if (pair != null)
			{
				switch (Type)
				{
					case DepthCandleSourceTypes.BestBid:
						var bid = pair.Bid;

						if (bid != null)
						{
							_price = bid.Price;
							_volume = bid.Volume;
						}

						break;
					case DepthCandleSourceTypes.BestAsk:
						var ask = pair.Ask;

						if (ask != null)
						{
							_price = ask.Price;
							_volume = ask.Volume;
						}

						break;
					case DepthCandleSourceTypes.Middle:
						_price = pair.MiddlePrice ?? 0;
						//_volume = pair.Bid.Volume;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Market depth.
		/// </summary>
		public MarketDepth Depth { get; }

		/// <summary>
		/// Type of candle depth based data.
		/// </summary>
		public DepthCandleSourceTypes Type { get; }

		//SecurityId ICandleBuilderSourceValue.SecurityId => Depth.Security.ToSecurityId();

		DateTimeOffset ICandleBuilderSourceValue.Time => Depth.LastChangeTime;

		decimal ICandleBuilderSourceValue.Price => _price;

		decimal? ICandleBuilderSourceValue.Volume => _volume;

		Sides? ICandleBuilderSourceValue.OrderDirection => null;
	}

	/// <summary>
	/// The <see cref="ICandleBuilder"/> source data is created on basis of <see cref="QuoteChangeMessage"/>.
	/// </summary>
	[DebuggerDisplay("{" + nameof(QuoteChange) + "}")]
	public class QuoteCandleBuilderSourceValue : ICandleBuilderSourceValue
	{
		private readonly decimal _price;
		private readonly decimal? _volume;

		/// <summary>
		/// Initializes a new instance of the <see cref="DepthCandleBuilderSourceValue"/>.
		/// </summary>
		/// <param name="message">Messages containing quotes.</param>
		/// <param name="type">Type of candle depth based data.</param>
		public QuoteCandleBuilderSourceValue(QuoteChangeMessage message, DepthCandleSourceTypes type)
		{
			QuoteChange = message;
			Type = type;

			switch (Type)
			{
				case DepthCandleSourceTypes.BestBid:
				{
					var bid = message.GetBestBid();

					if (bid != null)
					{
						_price = bid.Price;
						_volume = bid.Volume;
					}

					break;
				}

				case DepthCandleSourceTypes.BestAsk:
				{
					var ask = message.GetBestAsk();

					if (ask != null)
					{
						_price = ask.Price;
						_volume = ask.Volume;
					}

					break;
				}


				case DepthCandleSourceTypes.Middle:
				{
					var bid = message.GetBestBid();
					var ask = message.GetBestAsk();

					if (bid != null && ask != null)
					{
						_price = (ask.Price + bid.Price) / 2;
						//_volume = pair.Bid.Volume;	
					}

					break;
				}

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Messages containing quotes.
		/// </summary>
		public QuoteChangeMessage QuoteChange { get; }

		/// <summary>
		/// Type of candle depth based data.
		/// </summary>
		public DepthCandleSourceTypes Type { get; }

		//SecurityId ICandleBuilderSourceValue.SecurityId => QuoteChange.SecurityId;

		DateTimeOffset ICandleBuilderSourceValue.Time => QuoteChange.ServerTime;

		decimal ICandleBuilderSourceValue.Price => _price;

		decimal? ICandleBuilderSourceValue.Volume => _volume;

		Sides? ICandleBuilderSourceValue.OrderDirection => null;
	}

	/// <summary>
	/// Types of candle level1 based data.
	/// </summary>
	public enum Level1CandleSourceTypes
	{
		/// <summary>
		/// Best bid.
		/// </summary>
		BestBid,

		/// <summary>
		/// Best ask.
		/// </summary>
		BestAsk,

		/// <summary>
		/// Spread middle.
		/// </summary>
		Middle,

		/// <summary>
		/// Last trade price.
		/// </summary>
		LastTrade,
	}

	/// <summary>
	/// The <see cref="ICandleBuilder"/> source data is created on basis of <see cref="QuoteChangeMessage"/>.
	/// </summary>
	[DebuggerDisplay("{" + nameof(QuoteChange) + "}")]
	public class Level1ChangeCandleBuilderSourceValue : ICandleBuilderSourceValue
	{
		private readonly decimal _price;
		private readonly decimal? _volume;

		/// <summary>
		/// Initializes a new instance of the <see cref="Level1ChangeCandleBuilderSourceValue"/>.
		/// </summary>
		/// <param name="message">Messages containing changes.</param>
		/// <param name="type">Types of candle level1 based data.</param>
		public Level1ChangeCandleBuilderSourceValue(Level1ChangeMessage message, Level1CandleSourceTypes type)
		{
			Level1Change = message;
			Type = type;

			_volume = null;

			switch (Type)
			{
				case Level1CandleSourceTypes.BestBid:
				{
					var bid = GetValue(message, Level1Fields.BestBidPrice);

					if (bid != null)
						_price = bid.Value;

					break;
				}

				case Level1CandleSourceTypes.BestAsk:
				{
					var ask = GetValue(message, Level1Fields.BestAskPrice);

					if (ask != null)
						_price = ask.Value;

					break;
				}

				case Level1CandleSourceTypes.Middle:
				{
					var bid = GetValue(message, Level1Fields.BestBidPrice);
					var ask = GetValue(message, Level1Fields.BestAskPrice);

					if (bid != null && ask != null)
						_price = (ask.Value + bid.Value) / 2;

					break;
				}

				case Level1CandleSourceTypes.LastTrade:
				{
					var lastPrice = GetValue(message, Level1Fields.LastTradePrice);

					if (lastPrice != null)
						_price = lastPrice.Value;

					break;
				}

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private decimal? GetValue(Level1ChangeMessage message, Level1Fields field)
		{
			return (decimal?)message.Changes.TryGetValue(field);
		}

		/// <summary>
		/// Messages containing quotes.
		/// </summary>
		public Level1ChangeMessage Level1Change { get; }

		/// <summary>
		/// Types of candle level1 based data.
		/// </summary>
		public Level1CandleSourceTypes Type { get; }

		//SecurityId ICandleBuilderSourceValue.SecurityId => QuoteChange.SecurityId;

		DateTimeOffset ICandleBuilderSourceValue.Time => Level1Change.ServerTime;

		decimal ICandleBuilderSourceValue.Price => _price;

		decimal? ICandleBuilderSourceValue.Volume => _volume;

		Sides? ICandleBuilderSourceValue.OrderDirection => null;
	}
}