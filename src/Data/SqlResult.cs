
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IronRockUtils.Data
{
	public class SqlResult : IEnumerable<SqlRow>
	{
		// List of rows.
		private List<SqlRow> rows;
		
		// Number of fields per row.
		private int fieldCount;

		// Name of fields in the result set.
		private Dictionary<string, int> fields;
		private string[] s_fields = null;

		// Constructs a result set from the specified data reader.
		public SqlResult(SqlDataReader reader)
		{
			this.rows = new List<SqlRow> ();

			this.fieldCount = reader.FieldCount;
			this.fields = new Dictionary<string, int> ();

			for (int i = 0; i < this.fieldCount; i++)
				this.fields.Add(reader.GetName(i), i);

			while (reader.Read())
			{
				SqlRow row = new SqlRow (this);

				for (int i = 0; i < this.fieldCount; i++)
					row[i] = reader.GetValue(i);

				rows.Add(row);
			}

			reader.Close();
		}

		// Constructs a result set from a field list.
		public SqlResult(string[] fields)
		{
			this.rows = new List<SqlRow> ();

			this.fieldCount = fields.Length;
			this.fields = new Dictionary<string, int> ();

			for (int i = 0; i < this.fieldCount; i++)
				this.fields.Add(fields[i], i);
		}

		// Converts the field names to uppercase.
		public void useUpperNames()
		{
			var temp = new Dictionary<string, int> ();

			foreach (KeyValuePair<string, int> value in this.fields)
				temp.Add(value.Key.ToUpper(), value.Value);

			this.fields = temp;
		}

		// Adds a row to the set and returns it.
		public SqlRow addRow ()
		{
			SqlRow row = new SqlRow (this);
			rows.Add(row);
			return row;
		}

		// Returns an enumerator for all rows.
		public IEnumerator<SqlRow> GetEnumerator()
		{
			return rows.GetEnumerator();
		}

		// Generic enumerator.
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
		    return this.GetEnumerator();
		}

		// Returns the number of rows in the result set.
		public int numRows ()
		{
			return rows.Count;
		}

		// Returns the number of fields per row.
		public int numFields ()
		{
			return fieldCount;
		}

		// Returns the index of a field name.
		public int fieldIndex (string name)
		{
			return fields.ContainsKey(name) ? fields[name] : -1;
		}
		
		// Returns the row at the specified index.
		public SqlRow this[int index]
		{
			get { return this.rows[index]; }
			set { this.rows[index] = value; }
		}

		// Returns the index of the first row with a column with the given value.
		public int indexOf (string name, string value)
		{
			int col = fieldIndex(name);

			for (int i = 0; i < this.rows.Count; i++)
			{
				if (this.rows[i][col].ToString() == value)
					return i;
			}

			return -1;
		}

		// Returns the name of a field.
		public string getField (int index)
		{
			return this.getFields()[index];
		}

		// Returns all field names as an array of strings.
		public string[] getFields ()
		{
			if (s_fields != null)
				return s_fields;

			string[] fields = new string[this.fields.Count];
			
			foreach (KeyValuePair<string, int> value in this.fields)
				fields[value.Value] = value.Key;

			return s_fields = fields;
		}
	}
}
