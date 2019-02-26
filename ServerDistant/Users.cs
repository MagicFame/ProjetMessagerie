using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Communication;
namespace ServerDistant
{
    [XmlInclude(typeof(Account))]
    [Serializable]

    public class Users
    {
        public List<Account> usersDB = new List<Account>();
        public bool addUsers(Account ac) //Ajouter utilisateur
        {
            if (verifyUsername(ac)) //Compte disponible
            {
                Console.WriteLine("Compte créé");
                usersDB.Add(ac);
                return true;
            }
            else //Pseudo déjà utilisé
            {
                Console.WriteLine("Le compte existe déjà");
                return false;
            }

        }
        public List<Account> getUsers()
        {
            return usersDB;
        }
        public bool verifyUsername(Account ac) //Fonction de vérification de donnée - Si le pseudo est déjà dans la BDD
        {
            foreach (Account accou in usersDB)
            {
                if (string.Equals(ac.Username, accou.Username))
                {
                    return false;
                }
            }
            return true;
        }

        public bool verifyData(Account ac) // Fonction de vérification de donnée - Si un compte avec le pseudo et le mot de passe existe
        {
            foreach (Account accou in usersDB)
            {
                if (string.Equals(ac.Username, accou.Username) && string.Equals(ac.Password, accou.Password))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
