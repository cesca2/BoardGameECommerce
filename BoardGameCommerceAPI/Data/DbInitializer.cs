using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
// id, name, yearpublished ,rank, bayesaverage, usersrated
public class DbInitializer
{
    public static void Initialize(SqliteConnection connection, bool reInitialize = true)
    {
        if (reInitialize)
        {
            var lst = new List<string> { "products", "sales", "customers", "sales_products" };

            foreach (string table in lst)
            {
                var reinitCommand = connection.CreateCommand();
                reinitCommand.CommandText = $"DROP TABLE IF EXISTS {table};";

                reinitCommand.ExecuteNonQuery();
            }

        }
        var command = connection.CreateCommand();
        command.CommandText = @"
              CREATE TABLE IF NOT EXISTS products (
                id TEXT NOT NULL PRIMARY KEY ,
                name TEXT NOT NULL,
                yearpublished INTEGER NOT NULL, 
                rank REAL NOT NULL,
                price REAL 
            );";

        command.ExecuteNonQuery();

        var command2 = connection.CreateCommand();
        command2.CommandText = @"
              CREATE TABLE IF NOT EXISTS customers (
                id TEXT NOT NULL PRIMARY KEY ,
                name TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE
            );";

        command2.ExecuteNonQuery();

        var command3 = connection.CreateCommand();
        command3.CommandText = @"
              CREATE TABLE IF NOT EXISTS sales (
                id TEXT NOT NULL PRIMARY KEY ,
                customer_id INTEGERNOT NULL,
                FOREIGN KEY (customer_id) 
                    REFERENCES customers (id) 
            );";

        command3.ExecuteNonQuery();

        var command4 = connection.CreateCommand();
        command4.CommandText = @"
              CREATE TABLE IF NOT EXISTS sales_products (
                id TEXT NOT NULL PRIMARY KEY ,
                product_id TEXT NOT NULL,
                sale_id TEXT NOT NULL,
                FOREIGN KEY (sale_id) 
                    REFERENCES sales (id),
                FOREIGN KEY (product_id)
                    REFERENCES products (id)
            );";

        command4.ExecuteNonQuery();

        using (var transaction = connection.BeginTransaction())
        {

            if (reInitialize)
            {
                var path = "boardgames_ranks.csv";
                var insert_command = connection.CreateCommand();
                insert_command.CommandText =
                     @"
                INSERT INTO products(id, name, yearpublished, rank)
                VALUES 
                ( $Id,
                  $Name, 
                  $Year, 
                  $Rank)
                ;
                ";


                using (TextFieldParser csvParser = new TextFieldParser(path))
                {

                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;
                    csvParser.CommentTokens = ["#"];
                    
                    // skip titles line
                    csvParser.ReadLine();

                    int i = 0;

                    while (!csvParser.EndOfData & i < 100) // remove limit later
                    {
                        i++;
                        string[] fields = csvParser.ReadFields();
                        insert_command.Parameters.Clear();
                        insert_command.Parameters.AddWithValue("$Id", Guid.NewGuid());
                        insert_command.Parameters.AddWithValue("$Name", fields[1]);
                        insert_command.Parameters.AddWithValue("$Year", int.Parse(fields[2]));
                        insert_command.Parameters.AddWithValue("$Rank", float.Parse(fields[3]));
                        insert_command.ExecuteNonQuery();

                    }
                }

                transaction.Commit();

            }
        }
    }
}