using System;

namespace MtgConstructor
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class MainInit
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainLoop())
            {
                game.Run();
            }
        }
    }
}
