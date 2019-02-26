using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Communication;

namespace ServerDistant
{
    class Connected
    {
        private TcpClient comm = null;
        private Account a = null;
        private static Semaphore semMessage = new Semaphore(initialCount: 1, maximumCount: 1, name: "My Semaphore");
        private static Semaphore semTopic = new Semaphore(initialCount: 1, maximumCount: 1, name: "My Semaphore");
        public Connected(TcpClient communi, Account ac)
        {
            comm = communi;
            a = ac;
            init();
        }
        public void init()
        {
            Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
            if (v.Values == 1) // message privé
            {
                PrivateMessage();
            }
            else if (v.Values == 2) //topic
            {
                Topic();
            }
            else if(v.Values == 3) // se déconnecter
            {
                new Guests(comm);
            }
        }
        public void Topic()
        {
            // Tourne en rond
            semTopic.WaitOne();
            TopicList topicList  = deserializeTopic();
            semTopic.Release();
            Serialisation.sendMsg(comm.GetStream(), new TopicList(topicList.getList()));
            Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
            if(v.Values == 1) // créer un topic
            {
                Topic top = (Topic)Serialisation.rcvMsg(comm.GetStream());
                topicList.addList(top);
                serialiseTopics(topicList);
                init();
            }
            else if(v.Values == 2) // retour
            {
                init();
            }
            else if(v.Values == 3) //répondre à un topic
            {
                PrivateLetter p = (PrivateLetter)Serialisation.rcvMsg(comm.GetStream());
                Topic c = topicList.getTopic(p.to);
                c.addList(p);
                semTopic.WaitOne();
                serialiseTopics(topicList);
                semTopic.Release();
                init();
            }
        }
        public void PrivateMessage()
        {
            Value v = (Value)Serialisation.rcvMsg(comm.GetStream());
            if (v.Values == 1) // consulter message
            {
                semMessage.WaitOne();
                Messages ms = deserializeMessage();
                semMessage.Release();
                Account us = (Account)Serialisation.rcvMsg(comm.GetStream());
                List<PrivateLetter> l = ms.getSpecificMessages(us);
                Serialisation.sendMsg(comm.GetStream(), new ListSerial(l));
                init();
            }
            else if (v.Values == 2) //envoyer message
            {
                PrivateLetter pl = (PrivateLetter)Serialisation.rcvMsg(comm.GetStream());
                semMessage.WaitOne();
                Messages ms = deserializeMessage();
                if (ms.add(pl)) serialiseMessage(ms);
                semMessage.Release();
                Serialisation.sendMsg(comm.GetStream(), new Value(1));
                init();
            }
        }
        public void serialiseMessage(Messages m)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Messages));
            using (StreamWriter wr = new StreamWriter("privatemessage.xml"))
            {
                xs.Serialize(wr, m);
            }
        }
        public Messages deserializeMessage()
        {
            Messages ms = null;
            string path = "privatemessage.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(Messages));
            StreamReader reader = new StreamReader(path);
            ms = (Messages)serializer.Deserialize(reader);
            reader.Close();
            return ms;
        }
        public void serialiseTopics(TopicList m)
        {
            XmlSerializer xs = new XmlSerializer(typeof(TopicList));
            using (StreamWriter wr = new StreamWriter("topics.xml"))
            {
                xs.Serialize(wr, m);
            }
        }
        public TopicList deserializeTopic()
        {
            TopicList tp = null;
            string path = "topics.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(TopicList));
            StreamReader reader = new StreamReader(path);
            tp = (TopicList)serializer.Deserialize(reader);
            reader.Close();
            return tp;
        }

    }
}
