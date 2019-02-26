using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
namespace ServerDistant
{
    [Serializable]
    public class Messages
    {
        public List<PrivateLetter> mess = new List<PrivateLetter>();

        public bool add(PrivateLetter ms)
        {
            mess.Add(ms);
            return true;
        }
        public List<PrivateLetter> getSpecificMessages(Account a) // Algorithme qui parcourt la liste de lettres mess afin de trouver uniquement les messages pour l'utilisateur a
        {

            List<PrivateLetter> returnedList = new List<PrivateLetter>();
            foreach (PrivateLetter pv in mess)
            {
                if (String.Equals(pv.to, a.Username) || String.Equals(pv.from, a.Username)){
                    returnedList.Add(pv);
                }
            }
            return returnedList;
        }

    }
}
