
using System;
using System.Collections.Specialized;

namespace IronRockUtils.Data
{
	public class SqlRow
	{
		// Reference to the result set containing the row.
		public SqlResult result;

		// Values of the row.
		public object[] values;

		// Constructs the row.
		public SqlRow(SqlResult result)
		{
			this.result = result;
			this.values = new object[result.numFields()];
		}

		// Obtains a field given its index.
		public object this[int index]
		{
			get { return values[index]; }
			set { values[index] = value; }
		}

		// Obtains a field given its name.
		public object this[string name]
		{
			get { return values[result.fieldIndex(name)]; }
			set { values[result.fieldIndex(name)] = value; }
		}
	}
}
