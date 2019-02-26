using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Communication
{

    public interface Message
    {
        string ToString();
    }

    [Serializable]
    public class Account : Message
    {
        public string username, password;
        public Account()
        {

        }
        public Account(string user, string pass)
        {
            this.username = user;
            this.password = pass;
        }


        public string Username
        {
            get { return username; }
        }

        public string Password
        {
            get { return password; }
        }

        public override string ToString()
        {
            return "Le compte a pour identifiants : " + username + " , " + password + "." ;
        }

    }
    [Serializable]
    public class ListSerial : Message
    {
        public List<PrivateLetter> l = null;
        public ListSerial()
        {

        }
        public ListSerial(List<PrivateLetter> le)
        {
            this.l = le;
        }
        public override string ToString()
        {
            return "Voici la liste";
        }
        public List<PrivateLetter> getList()
        {
            return l;
        }
    }
    [Serializable]
    public class Topic : Message
    {
        public List<PrivateLetter> priv = new List<PrivateLetter>();
        public string nameTopic;
        public Topic()
        {

        }
        public Topic(string name, PrivateLetter pl)
        {
            this.nameTopic = name;
            addList(pl);
        }
        public List<PrivateLetter> getList()
        {
            return priv;
        }
        public void addList(PrivateLetter pl){
            priv.Add(pl);
        }
        public string getName()
        {
            return nameTopic;
        }
        public override string ToString()
        {
            return "Voici la liste";
        }
    }
    [Serializable]
    public class TopicList : Message
    {
        public List<Topic> mess = new List<Topic>();
        public TopicList()
        {

        }
        public TopicList(List<Topic> li)
        {
            this.mess = li;
        }
        public void addList(Topic l)
        {
            mess.Add(l);
        }
        public override string ToString()
        {
            return "Voici la liste";
        }
        public Topic getTopic(string name)
        {
            foreach(Topic top in mess)
            {
                if(string.Equals(top.getName(), name))
                {
                    return top;
                }
            }
            return null;
        }
        public List<Topic> getList()
        {
            return mess;
        }
    }
    

    [Serializable]
    public class PrivateLetter : Message
    {
        public string message;
        public string from;
        public string to;
        public PrivateLetter()
        {

        }
        public PrivateLetter(string msg, Account fr, String t)
        {
            this.message = msg;
            this.from = fr.Username;
            this.to = t;
        }
    }
    [Serializable]
    public class Value : Message
    {
        private int values;
        public int Values
        {
            get { return values; }
        }
        public Value(int val)
        {
            this.values = val;
        }
        public override string ToString()
        {
            return "" + values;
        }
    }


}
