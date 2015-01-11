using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using Future.Models;

namespace Future.Controllers.api
{
    public class ExampleOfRestfulApiController : ApiController
    {
        //This is an example for ApiController
        static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET api/example
        public IEnumerable<Example> Get()
        {
            var examples = new List<Example>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM examples", conn);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        examples.Add(new Example
                        {
                            Id = rdr.GetInt32(0),
                            Title = rdr.GetString(1),
                            Body = rdr.GetString(2)
                        });
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return examples;
        }

        // GET api/example/5
        public IEnumerable<Example> Get(int id)
        {
            var examples = new List<Example>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM examples WHERE Id=" + id, conn);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        examples.Add(new Example
                        {
                            Id = rdr.GetInt32(0),
                            Title = rdr.GetString(1),
                            Body = rdr.GetString(2)
                        });
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return examples;
        }

        // PUT api/example/5
        public string Post(Example example)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd =
                        new SqlCommand(
                            "INSERT INTO examples (Title, Body) VALUES ('" + example.Title + "', '" + example.Body +
                            "');", conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return "INSERT INTO examples (Name, Rating) VALUES ('" + example.Title + "', '" + example.Body + "');";
        }

        // DELETE api/example/5
        public void Delete(int id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM examples where id = " + id, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}