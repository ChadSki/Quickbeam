using System.Windows;
using System.Windows.Controls;

namespace Quickbeam.Metro.Controls.Custom
{
	public class MetroClosableTabControl : TabControl
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MetroClosableTabItem();
		}
	}
}
