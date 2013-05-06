using System;

namespace BubbleShooter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BubbleShooter game = new BubbleShooter())
            {
                game.Run();
            }
        }
    }
#endif
}

