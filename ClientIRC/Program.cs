using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientIRC
{
    class Program
    {
        static void Main(string[] args)
        {
            int result;
            do
            {
                Console.Clear();
                Console.WriteLine("---Bievenue utilisateur.---");
                Console.WriteLine("Souhaitez-vous vous connecter au serveur ? \n 1- Connection au serveur \n 2- Quitter l'application");
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result == 2)
                    {
                        Environment.Exit(0);
                    }
                    else if (result == 1)
                    {
                        try
                        {
                            Client c1 = new Client("127.0.0.1", 8976);
                            c1.start();
                        }
                        catch (System.Net.Sockets.SocketException sse)
                        {
                            Console.WriteLine("Connexion impossible : serveur fermé. Raison : " + sse.StackTrace);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Erreur, choisissez 1 ou 2");
                    }
                }
                else
                {
                    Console.WriteLine("Erreur, essayer d'entrer une valeur attendue");
                }
            } while (result != 1 && result != 2);
            
        }
    }
}
