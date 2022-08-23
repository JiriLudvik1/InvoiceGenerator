using Dapper;
using InvoiceGenerator.MAUI.Models;
using Microsoft.Data.Sqlite;

namespace InvoiceGenerator.MAUI
{
  public class DBQueries
  {
    public string ConnectionString { get; }

    public DBQueries(string connectionString)
    {
      ConnectionString = connectionString;
      //SQLitePCL.Batteries.Init();
    }

    public List<Customer> GetAllCustomers()
    {
      try
      {
        using (var conn = new SqliteConnection(ConnectionString))
        {
          string sql = "SELECT * FROM Customers";
          conn.Open();
          return conn.Query<Customer>(sql).ToList();
        }
      }
      catch (Exception ex)
      {
        return new List<Customer>();
      }

    }

  }
}
