using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Communication
{
    public class Serialisation
    {
        public static void sendMsg(Stream s, Message msg)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, msg);
        }

        public static Message rcvMsg(Stream s)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (Message)bf.Deserialize(s);
            }catch (System.IO.IOException)
            {
                Console.WriteLine("User disconnected");
                Thread.CurrentThread.Abort();
            }
            return null;
        }
    }
}
