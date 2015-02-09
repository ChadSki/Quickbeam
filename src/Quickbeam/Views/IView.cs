namespace Quickbeam.Views
{
	public interface IView
	{
        /// <summary>
        /// Try to close the current IView.
        /// </summary>
        /// <returns>Did we close successfully?</returns>
		bool Close();
	}
}
