namespace SampleDiagramPublic
{
	using System;

	using StockSharp.Xaml.Diagram;

	static class Extensions
	{
		public static string GetFileName(this CompositionDiagramElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			return element.TypeId.ToString().Replace("-", "_") + ".xml";
		}
	}
}