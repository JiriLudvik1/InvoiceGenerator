using Dapper;
using InvoiceGenerator.MAUI.Models;
using Microsoft.Data.Sqlite;

namespace InvoiceGenerator.MAUI
{
  public class DBQueries
  {
    private string connectionString { get; }

    public DBQueries(string fileLocation)
    { 
      this.connectionString = $"DataSource={fileLocation}";
    }

    public List<CustomerModel> GetAllCustomers()
    {
      try
      {
        using (var conn = new SqliteConnection(connectionString))
        {
          string sql = "SELECT * FROM Customers";
          conn.Open();
          return conn.Query<CustomerModel>(sql).ToList();
        }
      }
      catch
      {
        return new List<CustomerModel>();
      }
    }

    public int GetNextInvoiceNumber(string year)
    {
      try
      {
        using (var conn = new SqliteConnection(connectionString))
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
        using (var conn = new SqliteConnection(connectionString))
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
