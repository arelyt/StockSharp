#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Xaml.Xaml
File: ChangePasswordWindow.xaml.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Xaml
{
	using System;
	using System.Windows;

	using Ecng.Common;

	/// <summary>
	/// The window for changing password.
	/// </summary>
	partial class ChangePasswordWindow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChangePasswordWindow"/>.
		/// </summary>
		public ChangePasswordWindow() 
		{
			InitializeComponent();
		}

		/// <summary>
		/// Password change processing event.
		/// </summary>
		public Action Process;

		/// <summary>
		/// To refresh password change result.
		/// </summary>
		/// <param name="result">The result in the form of a text message.</param>
		public void UpdateResult(string result)
		{
			Result.Text = result;
			ChangePassword.IsEnabled = true;
		}

		/// <summary>
		/// Current password.
		/// </summary>
		public string CurrentPassword => CurrentPasswordCtrl.Password;

		/// <summary>
		/// New password.
		/// </summary>
		public string NewPassword => NewPasswordCtrl.Password;

		private void ChangePassword_OnClick(object sender, RoutedEventArgs e)
		{
			ChangePassword.IsEnabled = false;
			Process.SafeInvoke();
		}
	}
}
