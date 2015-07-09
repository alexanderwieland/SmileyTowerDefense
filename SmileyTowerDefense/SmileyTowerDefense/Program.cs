using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SmileyTowerDefense
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
           

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Form1 runningForm = new Form1();
            //Application.Run(runningForm);

            //Client client = runningForm.cClient;
           

            //if (runningForm.Singleplayer)
            //    client.Inizialize();
            //else
            //{

            //    //server = runningForm.sServer;
            //}
            
            //new Thread(()=>StartGame()).Start();
            //new Thread(() => StartGame()).Start();
           
        }

        static void StartGame()
        {
            Game game = new Game();
            game.Run();
        }
    }
#endif
}

