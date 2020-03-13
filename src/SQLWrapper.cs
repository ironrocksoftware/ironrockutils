﻿
using System;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;

namespace IronRockUtils
{
	public class SQLWrapper
	{
		private Config config;
		private SqlConnection conn;
		private SqlCommand cmd;

		private string connectionStringFormat;

		// Builds the wrapper using the specified config data object.
		public SQLWrapper(Config config, int timeout)
		{
			this.config = config;
			this.conn = null;
			this.cmd = new SqlCommand ();

			setConnectionStringFormat ("server={0}; user id={1}; password={2}; database={3}; Connection Timeout=" + (timeout > 0 ? timeout : 300));
		}

		public void setConnectionStringFormat (string value)
		{
			connectionStringFormat = value;
		}

		// Returns true/false if the connection is open or closed respectively.
		public bool isOpen ()
		{
			return this.conn == null ? false : true;
		}

		// Returns the connection string obtained from the configuration object.
		public string connectionString ()
		{
			string cs = String.Format(this.connectionStringFormat, config.get("sqlServer"), config.get("sqlUsername"), config.get("sqlPassword"), config.get("sqlDatabase"));
			return cs;
		}

		// Creates a new SqlConnection.
		public SqlConnection createConnection()
		{
			return new SqlConnection (connectionString());
		}

		// Opens the connection and returns the completion status.
		public bool open ()
		{
			if (isOpen()) return true;
			close ();

			conn = createConnection();

			try {
				conn.Open();
			}
			catch (Exception e)
			{
				Log.write ("Error while opening connection: " + e.Message);
				conn = null;
				return false;
			}

			return true;
		}

		// Closes the connection (if it's open).
		public void close ()
		{
			if (!isOpen()) return;

			conn.Close();
			conn = null;
		}

		// Loads a small data set and returns a name-value collection.
		public ArrayList getNvcArray (string query)
		{
			ArrayList array = new ArrayList ();
			if (!open()) return array;

			this.cmd.Connection = this.conn;
			this.cmd.CommandText = query;
			this.cmd.CommandTimeout = 300;

			SqlDataReader reader = this.cmd.ExecuteReader();

			while (reader.Read())
			{
				NameValueCollection nvc = new NameValueCollection ();
				array.Add (nvc);

				for (int i = 0; i < reader.FieldCount; i++)
				{
					nvc.Set (reader.GetName(i), Convert.ToString (reader.GetValue(i)));
				}
			}

			reader.Close();

			return array;
		}

		// Loads a small data set and returns a dictionary collection.
		public List<Dictionary<string, object>> getNvoArray (string query)
		{
			List<Dictionary<string, object>> array = new List<Dictionary<string, object>> ();
			if (!open()) return array;

			this.cmd.Connection = this.conn;
			this.cmd.CommandText = query;
			this.cmd.CommandTimeout = 300;

			SqlDataReader reader = this.cmd.ExecuteReader();

			while (reader.Read())
			{
				Dictionary<string, object> nvo = new Dictionary<string, object> ();
				array.Add(nvo);

				for (int i = 0; i < reader.FieldCount; i++)
				{
					nvo.Add (reader.GetName(i), reader.GetValue(i));
				}
			}

			reader.Close();

			return array;
		}

		// Executes a SQL statement.
		public int execStmt (string query)
		{
			if (!open()) return 0;

			this.cmd.Connection = this.conn;
			this.cmd.CommandText = query;
			this.cmd.CommandTimeout = 300;

			return this.cmd.ExecuteNonQuery ();
		}

		// Executes a SQL statement and returns scalar.
		public string getScalar (string query)
		{
			if (!open()) return "";

			this.cmd.Connection = this.conn;
			this.cmd.CommandText = query;
			this.cmd.CommandTimeout = 300;

			string result = Convert.ToString(this.cmd.ExecuteScalar());
			return String.IsNullOrEmpty(result) ? "" : result;
		}
	}
}