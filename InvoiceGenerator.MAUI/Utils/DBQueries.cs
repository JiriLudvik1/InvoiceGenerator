using Dapper;
using InvoiceGenerator.MAUI.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceGenerator.MAUI
{
  public class DBQueries
  {
    public string ConnectionString { get; }

    public DBQueries(string connectionString)
    {
      ConnectionString = connectionString;
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
      catch
      {
        return new List<Customer>();
      }
    }

    public int GetNextInvoiceNumber(string year)
    {
      try
      {
        using (var conn = new SqliteConnection(ConnectionString))
        {
          return conn.ExecuteScalar<int>("SELECT NextInvoiceNumber FROM InvoiceNumber WHERE Year = @year", new { year = year });
        }
      }
      catch
      {
        return -1;
      }
    }

    public bool IncrementInvoiceNumber(string year)
    {
      try
      {
        using (var conn = new SqliteConnection(ConnectionString))
        {
          conn.Execute("UPDATE InvoiceNumber SET NextInvoiceNumber = NextInvoiceNumber + 1 WHERE Year = @year", new {year = year});
        }

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
