using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;

namespace ClientIRC
{
    class User
    {
        private TcpClient comm = null;
        private Account acc = null;
        public User(TcpClient communi, Account c)
        {
            this.acc = c;
            this.comm = communi;
            init();
        }
        public void init()
        {
            int result;
            do
            {
                Console.WriteLine("----Bienvenue " + acc.Username + "!----");
                Console.WriteLine("Que souhaitez vous réaliser ?");
                Console.WriteLine("1 - Section messages privés");
                Console.WriteLine("2 - Section Topic");
                Console.WriteLine("3 - Se déconnecter");
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result == 1) //Consulter MP
                    {
                        privateMessages();

                    }
                    else if (result == 2) //Section Topic
                    {
                        topicSection();
                    }
                    else if (result == 3)
                    {
                        Serialisation.sendMsg(comm.GetStream(), new Value(3));
                        Console.Clear();
                        new Guest(comm);
                    }
                    else
                    {
                        Console.WriteLine("Erreur, choisissez 1, 2 ou 3");
                    }
                }
                else
                {
                    Console.WriteLine("Erreur, essayer d'entrer une valeur attendue");
                }
            } while (result != 1 && result != 2 && result != 3);
        }
        public void topicSection()
        {
            Console.Clear();
            Serialisation.sendMsg(comm.GetStream(), new Value(2));
            Console.WriteLine("----Section topic----");
            TopicList tl = (TopicList)Serialisation.rcvMsg(comm.GetStream());
            int j;
            Console.WriteLine("\n1 - Créer un topic\n");
            for (j = 0; j < tl.getList().Count; j++)
            {
                Console.WriteLine((j + 2) + (" - " + tl.getList().ElementAt(j).getName()));
            }
            Console.WriteLine("\n" + (j + 2) + " - Retour");
            int result5;
            do
            {
                if (int.TryParse(Console.ReadLine(), out result5))
                {
                    if (result5 == 1) //Créer un topic
                    {
                        createTopic();
                    }
                    else if (result5 == (j + 2)) //retour
                    {
                        Serialisation.sendMsg(comm.GetStream(), new Value(2));
                        init();
                    }
                    else if (result5 > 1 && result5 < j + 2)  // Lire le topic
                    {

                        readTopic(tl, result5);
                    }
                    else
                    {
                        Console.WriteLine("Erreur...");

                    }
                }
                else
                {
                    Console.WriteLine("Erreur...");

                }
            } while (result5 < 1 || result5 > (j + 2));

        }
        public void readTopic(TopicList tl, int result5)
        {
            Console.Clear();
            Console.WriteLine("----LECTURE TOPIC----");
            Console.WriteLine("Titre : " + tl.getList().ElementAt(result5 - 2).getName() + "\n");
            int i;
            for (i = 0; i < tl.getList().ElementAt(result5 - 2).getList().Count; i++) //Pour chaque message du topic
            {
                Console.WriteLine("Message " + (i + 1) + " de " + tl.getList().ElementAt(result5 - 2).getList().ElementAt(i).from);
                Console.WriteLine(tl.getList().ElementAt(result5 - 2).getList().ElementAt(i).message);
                Console.WriteLine("\n");
            }
            Console.WriteLine("1 - Répondre sur ce topic");
            Console.WriteLine("2 - Retour");
            int result6;
            if (int.TryParse(Console.ReadLine(), out result6))
            {
                if (result6 == 1) // Répondre
                {
                    Console.WriteLine("Que souhaitez-vous répondre ?");
                    string answer = Console.ReadLine();
                    Serialisation.sendMsg(comm.GetStream(), new Value(3));
                    Serialisation.sendMsg(comm.GetStream(), new PrivateLetter(answer, acc, tl.getList().ElementAt(result5 - 2).getName()));
                    topicSection();

                }
                else if (result6 == 2) //Retour
                {
                    Serialisation.sendMsg(comm.GetStream(), new Value(2));
                    topicSection();
                }
            }
            else
            {
                Console.WriteLine("Erreur");
            }
        }
        public void createTopic()
        {
            Console.Clear();
            Console.WriteLine("Quel nom souhaitez-vous donner au Topic ?");
            string name = Console.ReadLine();
            Console.WriteLine("Quel message souhaitez-vous écrire ?");
            string article = Console.ReadLine();
            Serialisation.sendMsg(comm.GetStream(), new Value(1));
            Serialisation.sendMsg(comm.GetStream(), new Topic(name, new PrivateLetter(article, this.acc, name)));
            topicSection();
        }
        public void privateMessages()
        {
            Console.Clear();
            int result1;
            do
            {
                Console.WriteLine("----Section Message Privés----");
                Console.WriteLine("1 - Afficher messages");
                Console.WriteLine("2 - Envoyer message");
                Console.WriteLine("3 - Retour");
                if (int.TryParse(Console.ReadLine(), out result1))
                {
                    if (result1 == 1) // Afficher les messages
                    {
                        checkMessage();
                    }
                    else if (result1 == 2) // Envoyer un message
                    {
                        sendMessage();
                    }
                    else if (result1 == 3) // Retour
                    {
                        init();
                    }
                    else
                    {
                        Console.WriteLine("Erreur");
                    }
                }
                else
                {
                    Console.WriteLine("Erreur...");
                }
            } while (result1 < 1|| result1 > 3);
        }
        public void checkMessage()
        {
            Console.Clear();
            Console.WriteLine("----Boite de réception----");
            Serialisation.sendMsg(comm.GetStream(), new Value(1)); //Section MP
            Serialisation.sendMsg(comm.GetStream(), new Value(1)); //Afficher MP
            Serialisation.sendMsg(comm.GetStream(), new Account(acc.Username, acc.Password));
            ListSerial l = (ListSerial)Serialisation.rcvMsg(comm.GetStream());
            List<String> expeditor = new List<string>();
            foreach (PrivateLetter priv in l.getList())
            {
                if (!String.Equals(priv.from, acc.Username)) // On récupère toutes les personnes qui nous ont écrit des messages sauf nous
                {
                    expeditor.Add(priv.from);
                }
                else // On récupère toute les personnes à qui on a écrit des messages
                {
                    expeditor.Add(priv.to);
                }
 
            }
            expeditor = expeditor.Distinct().ToList();
            int i, result4;
            for (i = 0; i < expeditor.Count; i++)
            {
                Console.WriteLine((i + 1) + " - " + expeditor.ElementAt(i));
            }
            Console.WriteLine(i+1 + " - Quitter");
            do
            {
                if (int.TryParse(Console.ReadLine(), out result4))
                {
                    if (result4 > 0 && result4 < i + 1)  //Lire la conv
                    {
                        conversation(expeditor.ElementAt(result4 - 1), l);
                    }
                    else if(result4 == i + 1)
                    {
                        privateMessages();
                    }
                    else
                    {
                        Console.WriteLine("Erreur... ");  
                    }
                }
                else
                {
                    Console.WriteLine("Erreur... Puase");
                    
                }
            } while (result4 < 0 && result4 > i + 1);


        }
        public void conversation(String exp, ListSerial la)
        {
            Console.Clear();
            Console.WriteLine("Message Privé avec " + exp);
            foreach (PrivateLetter privatespecifi in la.getList())
            {
                if(string.Equals(exp, acc.username)) //si l'utilisateur s'écrit à lui même
                {
                    if (String.Equals(privatespecifi.from, exp) && String.Equals(privatespecifi.to, exp))
                    {
                        Console.WriteLine("\nDe: " + privatespecifi.from);
                        Console.WriteLine("A : " + privatespecifi.to);
                        Console.WriteLine("\n" + privatespecifi.message);
                        Console.WriteLine("\n");
                    }
                }
                else //Sinon (cas normal)
                {
                    if (String.Equals(privatespecifi.from, exp) || String.Equals(privatespecifi.to, exp)) //On affiche tous les message ayant pour dest ou exp celui sélectionné
                    {
                        Console.WriteLine("\nDe: " + privatespecifi.from);
                        Console.WriteLine("A : " + privatespecifi.to);
                        Console.WriteLine("\n" + privatespecifi.message);
                        Console.WriteLine("\n");
                    }
                }    
            }
            Console.WriteLine("1 - Répondre");
            Console.WriteLine("2 - Retour");
            int result5;
            do
            {
                if (int.TryParse(Console.ReadLine(), out result5))
                {
                    if (result5 == 1) //Repondre
                    {
                        Console.WriteLine("Quel message souhaitez vous écrire ?");
                        String contenu = Console.ReadLine();
                        Serialisation.sendMsg(comm.GetStream(), new Value(1)); //Section MP
                        Serialisation.sendMsg(comm.GetStream(), new Value(2)); //Envoyer MP
                        Serialisation.sendMsg(comm.GetStream(), new PrivateLetter(contenu, this.acc, exp));
                        Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
                        if (v.Values == 1)
                        {
                            Console.WriteLine("Message envoyé avec succès");
                            System.Threading.Thread.Sleep(1000);
                            Console.Clear();
                            checkMessage();
                        }
                    }
                    else if (result5 == 2) //Retour
                    {
                        checkMessage();
                    }
                }
            } while (result5 != 1 && result5 != 2);

        }
        public void sendMessage()
        {
            int result2;
            do
            {
                Console.WriteLine("A qui souhaitez-vous envoyer un message ?");
                String to = Console.ReadLine();
                Console.WriteLine("Ecrivez votre message ci dessous :");
                String message = Console.ReadLine();
                Console.WriteLine("1 - Confirmez l'envoi ");
                Console.WriteLine("2 - Annuler");
                if (int.TryParse(Console.ReadLine(), out result2))
                {
                    if (result2 == 1)//envoyer
                    {
                        Serialisation.sendMsg(comm.GetStream(), new Value(1)); //Section MP
                        Serialisation.sendMsg(comm.GetStream(), new Value(2)); //Envoyer MP
                        Serialisation.sendMsg(comm.GetStream(), new PrivateLetter(message, this.acc, to));
                        Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
                        if (v.Values == 1)
                        {
                            Console.WriteLine("Message envoyé avec succès");
                            System.Threading.Thread.Sleep(1000);
                            Console.Clear();
                            privateMessages();
                        }
                    }
                    else if (result2 == 2) //annuler
                    {
                        init();
                    }
                    else
                    {
                        Console.WriteLine("Erreur");
                    }
                }
                else
                {
                    Console.WriteLine("Erreur...");
                }
            } while (result2 != 1 && result2 != 2);
        }
    }
}
