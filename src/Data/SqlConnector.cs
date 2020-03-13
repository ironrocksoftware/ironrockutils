
using System;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;

namespace IronRockUtils.Data
{
	public class SqlConnector
	{
		// Connection to the database.
		private SqlConnection dbConn;

		// Connection string.
		private string connString;

		// Constructs a connector for the specified file.
		public SqlConnector (string connString)
		{
			this.dbConn = null;
			this.connString = connString;
		}

		// Returns a clone of the connector but closed.
		public SqlConnector clone()
		{
			return new SqlConnector (this.connString);
		}

		// Opens the connection.
		public bool open ()
		{
			if (dbConn != null) return true;

			try
			{
				dbConn = new SqlConnection (connString);
				dbConn.Open();

				return true;
			}
			catch (Exception e)
			{
				IronRockUtils.Log.write ("SqlConnector.open (Error): " + e.Message);
				return false;
			}
		}

		// Closes the connection.
		public void close ()
		{
			if (dbConn == null)
				return;

			dbConn.Close();
			dbConn = null;
		}

		// Escapes and adds quotes to a value.
		public static string sqlEscape (string value)
		{
			value = value.Replace("'", "''");
			return "'" + value + "'";
		}

		// Runs a query and returns an scalar.
		public string getScalar (string sql, params object[] args)
		{
			if (dbConn == null)
				return null;

			if (args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
					args[i] = SqlConnector.sqlEscape(args[i].ToString());
	
				sql = String.Format(sql, args);
			}

			SqlCommand command = new SqlCommand(sql, dbConn);

			object value = command.ExecuteScalar();
			return value == null ? null : Convert.ToString(value);
		}

		// Executes a statement and returns completion status.
		public SqlResult getRows (string sql, params object[] args)
		{
			if (dbConn == null)
				return null;

			if (args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
					args[i] = SqlConnector.sqlEscape(args[i].ToString());
	
				sql = String.Format(sql, args);
			}

			SqlCommand command = new SqlCommand (sql, dbConn);
			SqlDataReader reader = command.ExecuteReader ();

			return new SqlResult (reader);
		}

		// Executes a statement and returns completion status.
		public bool execStmt (string sql, params object[] args)
		{
			if (dbConn == null)
				return false;

			if (args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
					args[i] = SqlConnector.sqlEscape(args[i].ToString());
	
				sql = String.Format(sql, args);
			}

			try
			{
				SqlCommand command = new SqlCommand(sql, dbConn);
				command.ExecuteNonQuery();
				return true;
			}
			catch (Exception e)
			{
				//IronRockUtils.Log.write("SqlConnector: execStmt: " + e.Message + "\n" + sql);
				return false;
			}
		}
	}
}
