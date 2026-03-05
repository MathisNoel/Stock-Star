using System.Data; //Type DataTable stocké des tables
using Npgsql; //Commande SQL

namespace Stock_Star
{
    internal class GestionProduits
    {
        //On crée un champ qui va contenir le moyen de se connecter à la BDD
        private ConnectionBDD BDD=new ConnectionBDD();

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        public DataTable ChargerStock()
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using(NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL="SELECT nom"
            }
            return ; 
        }
    }
}
