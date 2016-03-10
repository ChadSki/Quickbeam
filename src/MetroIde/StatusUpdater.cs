namespace MetroIde
{
    public static class StatusUpdater
    {
        /// <summary>
        ///     Update the status of the application.
        /// </summary>
        /// <param name="update">The new status</param>
        public static void Update(string update)
        {
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.UpdateStatusText(update);
        }
    }
}