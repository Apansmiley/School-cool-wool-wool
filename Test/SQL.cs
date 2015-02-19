using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasTest
{
    class CSQL
    { 
       public bool start()
        {
            bool connected = false;
            Console.WriteLine("Trying to contact SQL server...");
            string connectionString = @"Data Source=SPO14S-07\SQLEXPRESS;Integrated Security=SSPI;Database=test;timeout=5";
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                Console.WriteLine("Connected");
                connected = true;
                //SqlDataReader reader = null;
                //command = new SqlCommand("SELECT * FROM KAKA", connection);
                //reader = command.ExecuteReader();

                //while (reader.Read())
                //{
                //    Console.WriteLine(reader["ProdNamn"]);
                //}

                connection.Close();
            }
            catch //(Exception ex)
            {
                connected = false;
                Console.WriteLine("Failed to connect to server.");
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
                if (command != null)
                    command.Dispose();
            }

            return connected;
        }
    }
}