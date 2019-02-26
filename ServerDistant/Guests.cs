using System;
using System.Net.Sockets;
using System.IO;
using Communication;
using System.Xml.Serialization;
using System.Threading;

namespace ServerDistant
{
    class Guests
    {
        private static Semaphore semLogin = new Semaphore(initialCount: 1, maximumCount: 1, name: "My Semaphore");
        private TcpClient comm = null;
        public Guests(TcpClient communi)
        {
            comm = communi;
            init();
        }
        public void init()
        {
            Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
            if (v.Values == 1) // Créer un compte
            {
                newAccount();
            }
            else if (v.Values == 2) // Se connecter
            {
                connexion();
            }
        }

        public void newAccount() // créé nouveau compte
        {
            Account acc = (Account)Serialisation.rcvMsg(comm.GetStream());
            Console.WriteLine("Info Received. Username : " + acc.Username + " and pass : " + acc.Password);
            semLogin.WaitOne();
            Users us = deserializeUsers();
            semLogin.Release();
            bool answer = us.addUsers(acc);
            if (answer) //Si le compte a été créé
            {
                semLogin.WaitOne();
                serialiseUsers(us);
                Serialisation.sendMsg(comm.GetStream(), new Value(1));
                init();
                semLogin.Release();
            }
            else
            {
                Serialisation.sendMsg(comm.GetStream(), new Value(0));
                init();
            }

        }
        public void connexion()
        {
            Account acc = (Account)Serialisation.rcvMsg(comm.GetStream());
            Console.WriteLine("Info Received. Username : " + acc.Username + " and pass : " + acc.Password);
            Users us = deserializeUsers();
            if (us.verifyData(acc)) //compte existant donc connecté
            {
                Serialisation.sendMsg(comm.GetStream(), new Value(1));
                new Connected(comm, acc);
            }
            else // compte inexistant
            {
                Serialisation.sendMsg(comm.GetStream(), new Value(0));
                init();
            }
        }
        public Users deserializeUsers()
        {
            Users us = null;
            string path = "account.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(Users));
            StreamReader reader = new StreamReader(path);
            us = (Users)serializer.Deserialize(reader);
            reader.Close();
            return us;
        }
        public void serialiseUsers(Users u)
        {

            XmlSerializer xs = new XmlSerializer(typeof(Users));
            using (StreamWriter wr = new StreamWriter("account.xml"))
            {
                xs.Serialize(wr, u);
            }
        }
    }
}
