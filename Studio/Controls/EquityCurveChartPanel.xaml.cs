#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Studio.Controls.ControlsPublic
File: EquityCurveChartPanel.xaml.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Studio.Controls
{
	using System.Collections.Generic;
	using System.Windows.Media;

	using Ecng.Configuration;
	using Ecng.ComponentModel;

	using StockSharp.Studio.Core.Commands;
	using StockSharp.Xaml.Charting;
	using StockSharp.Localization;

	[DisplayNameLoc(LocalizedStrings.PnLKey)]
	[DescriptionLoc(LocalizedStrings.Str3260Key)]
	[Icon("images/equity_24x24.png")]
	public partial class EquityCurveChartPanel
	{
		private readonly ICollection<EquityData> _totalPnL;
		private readonly ICollection<EquityData> _unrealizedPnL;
		private readonly ICollection<EquityData> _commission;

		public EquityCurveChartPanel()
		{
			InitializeComponent();

			_totalPnL = EquityChart.CreateCurve(LocalizedStrings.PnL, Colors.Green, Colors.Red, EquityCurveChartStyles.Area);
			_unrealizedPnL = EquityChart.CreateCurve(LocalizedStrings.PnLUnreal, Colors.Black);
			_commission = EquityChart.CreateCurve(LocalizedStrings.Str159, Colors.Red, EquityCurveChartStyles.DashedLine);

			var cmdSvc = ConfigManager.GetService<IStudioCommandService>();
			cmdSvc.Register<ResetedCommand>(this, false, cmd =>
			{
				_totalPnL.Clear();
				_unrealizedPnL.Clear();
				_commission.Clear();
			});
			cmdSvc.Register<PnLChangedCommand>(this, false, cmd =>
			{
				_totalPnL.Add(new EquityData { Time = cmd.Time, Value = cmd.TotalPnL });
				_unrealizedPnL.Add(new EquityData { Time = cmd.Time, Value = cmd.UnrealizedPnL });
				_commission.Add(new EquityData { Time = cmd.Time, Value = cmd.Commission ?? 0 });
			});
		}

		public override void Dispose()
		{
			var cmdSvc = ConfigManager.GetService<IStudioCommandService>();
			cmdSvc.UnRegister<ResetedCommand>(this);
			cmdSvc.UnRegister<PnLChangedCommand>(this);
		}
	}
}