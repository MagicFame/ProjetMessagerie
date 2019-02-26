using System;
using System.Net.Sockets;
using Communication;

namespace ClientIRC
{
    class Guest
    {

        private TcpClient comm = null;
        public Guest(TcpClient communi)
        {
            comm = communi;
            init();
        }
        public void init() //Interface visiteur avec les 2 choix possibles (Inscription ou Connexion)
        {
            int result;
            do
            {
                Console.WriteLine("----Invité----");
                Console.WriteLine("Choisissez une option : ");
                Console.WriteLine("1- Créer un compte");
                Console.WriteLine("2- Se connecter");
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result == 1)
                    {
                        createAccount();
                    }
                    else if (result == 2)
                    {
                        connect();
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
        public void createAccount() //Créer un compte
        {
            Console.WriteLine("Entrer votre pseudonyme :");
            string username = Console.ReadLine();
            Console.WriteLine("Entrer votre mot de passe :");
            string password = Console.ReadLine();
            Console.WriteLine("Confirmer votre mot de passe :");
            string passwordConfirmation = Console.ReadLine();
            if (string.Equals(password, passwordConfirmation) || !string.Equals(username, "") || !string.Equals(password, ""))
            {
                Console.WriteLine("En cours de création, veuillez patienter ...");
                Serialisation.sendMsg(comm.GetStream(), new Value(1));
                Serialisation.sendMsg(comm.GetStream(), new Account(username, password));
                Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
                if (v.Values == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Ce pseudo a déjà été utilisé");
                    init();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Compte créé avec succès");
                    init();
                }
            }
            else
            {
                Console.WriteLine("Erreur : les mots de passes sont différents ou il y a des champs vides");
                init();
            }
        }
        public void connect() // Se connecter
        {
            Console.WriteLine("Entrer votre pseudonyme :");
            string username = Console.ReadLine();
            Console.WriteLine("Entrer votre mot de passe :");
            string password = Console.ReadLine();
            if (string.Equals(username, "") || string.Equals(password, ""))
            {
                Console.WriteLine("Il y a un ou plusieurs champs vides");
            }
            else
            {
                Console.WriteLine("Connexion en cours, veuillez patienter ...");
                Serialisation.sendMsg(comm.GetStream(), new Value(2));
                Serialisation.sendMsg(comm.GetStream(), new Account(username, password));
                Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
                if (v.Values == 1)
                {
                    Console.Clear();
                    new User(comm, new Account(username, password));
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Ce compte n'existe pas");
                    init();
                }
            }

        }
    }
}
