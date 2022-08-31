
using System;
using System.Collections.Generic;

namespace IronRockUtils.Json
{
	public class JsonElement
	{
		// Type of the element.
		public JsonElementType type;

		// Value of the element, depends on the element type.
		public object value;

		// Constructors for each allowed type.
		public JsonElement(JsonElementType type)
		{
			switch (this.type = type)
			{
				case JsonElementType.OBJECT:
					this.value = new Dictionary<string, JsonElement> ();
					break;

				case JsonElementType.ARRAY:
					this.value = new List<JsonElement> ();
					break;

				case JsonElementType.STRING:
					this.value = null;
					break;

				case JsonElementType.NUMBER:
					this.value = 0;
					break;

				case JsonElementType.BOOLEAN:
					this.value = false;
					break;
			}
		}

		public JsonElement(Dictionary<string, JsonElement> value)
		{
			this.type = JsonElementType.OBJECT;
			this.value = value;
		}

		public JsonElement(List<JsonElement> value)
		{
			this.type = JsonElementType.ARRAY;
			this.value = value;
		}

		public JsonElement(string value)
		{
			this.type = JsonElementType.STRING;
			this.value = value;
		}

		public JsonElement(double value)
		{
			this.type = JsonElementType.NUMBER;
			this.value = value;
		}

		public JsonElement(bool value)
		{
			this.type = JsonElementType.BOOLEAN;
			this.value = value;
		}

		// Returns the length of the array-value (only if type is ARRAY).
		public int getLength ()
		{
			if (type != JsonElementType.ARRAY)
				throw new Exception ("JsonElementType is not ARRAY");

			return ((List<JsonElement>)value).Count;
		}

		// Returns the keys of the object-value (only if type is OBJECT).
		public Dictionary<string, JsonElement>.KeyCollection getKeys ()
		{
			if (type != JsonElementType.OBJECT)
				throw new Exception ("JsonElementType is not OBJECT");

			return ((Dictionary<string, JsonElement>)value).Keys;
		}

		// Returns or sets the element at the specified index of the array (only if type is ARRAY).
		public JsonElement this[int index]
		{
			get {
				if (type != JsonElementType.ARRAY)
					throw new Exception ("JsonElementType is not ARRAY");
	
				return ((List<JsonElement>)this.value)[index];
			}

			set {
				if (type != JsonElementType.ARRAY)
					throw new Exception ("JsonElementType is not ARRAY");
	
				((List<JsonElement>)this.value)[index] = value;
			}
		}

		// Returns true/false if the map contains a given key (only if type is OBJECT).
		public bool hasKey (string key)
		{
			if (type != JsonElementType.OBJECT)
				throw new Exception ("JsonElementType is not OBJECT");

			return ((Dictionary<string, JsonElement>)value).ContainsKey(key);
		}

		// Returns or sets the element with the specified key of the map (only if type is OBJECT).
		public JsonElement this[string key]
		{
			get {
				if (type != JsonElementType.OBJECT)
					throw new Exception ("JsonElementType is not OBJECT");
	
				return ((Dictionary<string, JsonElement>)this.value)[key];
			}

			set {
				if (type != JsonElementType.OBJECT)
					throw new Exception ("JsonElementType is not OBJECT");
	
				((Dictionary<string, JsonElement>)this.value)[key] = value;
			}
		}

		// Adds an element to the list. (Array types only).
		public JsonElement add (JsonElement item)
		{
			if (type != JsonElementType.ARRAY)
				throw new Exception ("JsonElementType is not ARRAY");

			((List<JsonElement>)value).Add(item);
			return this;
		}

		// Adds an element to the object. (Object types only).
		public JsonElement add (string name, JsonElement item)
		{
			if (type != JsonElementType.OBJECT)
				throw new Exception ("JsonElementType is not OBJECT");

			((Dictionary<string, JsonElement>)value).Add(name, item);
			return this;
		}

		// Returns the array-value of the element.
		public List<JsonElement> getArrayData ()
		{
			if (type != JsonElementType.ARRAY)
				throw new Exception ("JsonElementType is not ARRAY");

			return (List<JsonElement>)value;
		}

		// Returns the object-value of the element.
		public Dictionary<string, JsonElement> getObjectData ()
		{
			if (type != JsonElementType.OBJECT)
				throw new Exception ("JsonElementType is not OBJECT");

			return (Dictionary<string, JsonElement>)value;
		}

		// Returns the string value of the element.
		public string getString ()
		{
			if (type == JsonElementType.STRING)
				return (string)value;

			if (type == JsonElementType.NUMBER)
				return ((double)value).ToString();

			if (type == JsonElementType.BOOLEAN)
				return ((bool)value) ? "true" : "false";

			throw new Exception ("JsonElementType is not STRING/NUMBER/BOOLEAN");
		}

		// Sets the value of a string element.
		public void setString (string value)
		{
			if (type != JsonElementType.STRING)
				throw new Exception ("JsonElementType is not STRING");

			this.value = value;
		}

		// Returns the numeric value of the element.
		public double getNumber ()
		{
			double tmp = 0;

			if (type == JsonElementType.NUMBER)
				return (double)value;
			
			if (type == JsonElementType.BOOLEAN)
				return (bool)value ? 1 : 0;

			if (type == JsonElementType.STRING)
			{
				double.TryParse((string)value, out tmp);
				return tmp;
			}

			throw new Exception ("JsonElementType is not NUMBER/STRING/BOOLEAN");
		}

		// Sets the value of a numeric element.
		public void setNumber (double value)
		{
			if (type != JsonElementType.NUMBER)
				throw new Exception ("JsonElementType is not NUMBER");

			this.value = value;
		}

		// Returns the boolean value of the element.
		public bool getBool ()
		{
			if (type != JsonElementType.BOOLEAN)
				throw new Exception ("JsonElementType is not BOOLEAN");

			return (bool)value;
		}

		// Sets the value of a boolean element.
		public void setBool (bool value)
		{
			if (type != JsonElementType.BOOLEAN)
				throw new Exception ("JsonElementType is not BOOLEAN");

			this.value = value;
		}

		// Returns the array-value of the element.
		public JsonElement getArray (string name)
		{
			if (!hasKey(name)) return null;
			
			JsonElement elem = this[name];
			if (elem.type != JsonElementType.ARRAY) return null;

			return elem;
		}

		// Returns the array-value of the element.
		public List<JsonElement> getArrayData (string name)
		{
			if (!hasKey(name)) return new List<JsonElement>();

			JsonElement elem = this[name];
			if (elem.type != JsonElementType.ARRAY) return null;

			return elem.getArrayData();
		}

		// Sets an empty array with the given name.
		public void setArray (string name)
		{
			if (!hasKey(name))
				add (name, new JsonElement (JsonElementType.ARRAY));
			else
				this[name] = new JsonElement (JsonElementType.ARRAY);
		}

		// Returns the object-value of the element.
		public JsonElement getObject (string name)
		{
			if (!hasKey(name)) return null;
			
			JsonElement elem = this[name];
			if (elem.type != JsonElementType.OBJECT) return null;

			return elem;
		}

		// Returns the string value of the element.
		public string getString (string name)
		{
			if (!hasKey(name))
				return null;

			return this[name].getString();
		}

		// Sets the string value of an element.
		public void setString (string name, string value)
		{
			if (!hasKey(name))
				add (name, new JsonElement (value));
			else
				this[name].setString(value);
		}

		// Returns the string value of the element or "def" if the value is not found.
		public string getString (string name, string def)
		{
			if (!hasKey(name))
				return def;

			return this[name].getString() ?? def;
		}

		// Returns the numeric value of the element.
		public double getNumber (string name)
		{
			if (!hasKey(name)) return 0;
			return this[name].getNumber();
		}

		// Sets the number value of an element.
		public void setNumber (string name, double value)
		{
			if (!hasKey(name))
				add (name, new JsonElement (value));
			else
				this[name].setNumber(value);
		}

		// Returns the numeric value of the element.
		public double getNumber (string name, double def)
		{
			if (!hasKey(name)) return def;
			return this[name].getNumber();
		}

		// Returns the boolean value of the element.
		public bool getBool (string name)
		{
			if (!hasKey(name)) return false;
			return this[name].getBool();
		}

		// Sets the boolean value of an element.
		public void setBool (string name, bool value)
		{
			if (!hasKey(name))
				add (name, new JsonElement (value));
			else
				this[name].setBool(value);
		}

		// Returns the boolean value of the element or def if the value was not found.
		public bool getBool (string name, bool def)
		{
			if (!hasKey(name)) return def;
			return this[name].getBool();
		}

		// Escapes a string.
		public static string escape (string value)
		{
			value = value.Replace("\\", "\\\\");
			value = value.Replace("\"", "\\\"");
			value = value.Replace("\r", "\\r");
			value = value.Replace("\n", "\\n");
			value = value.Replace("\t", "\\t");
			value = value.Replace("\b", "\\b");
			value = value.Replace("\v", "\\v");
			value = value.Replace("\f", "\\f");

			return value;
		}

		// Unescapes a string.
		public static string unescape (string value)
		{
			value = value.Replace("\\\\", "<@A!4>");

			value = value.Replace("\\f", "\f");
			value = value.Replace("\\v", "\v");
			value = value.Replace("\\b", "\b");
			value = value.Replace("\\t", "\t");
			value = value.Replace("\\n", "\n");
			value = value.Replace("\\r", "\r");
			value = value.Replace("\\\"", "\"");

			value = value.Replace("<@A!4>", "\\");

			return value;
		}

		// Converts the element to its JSON string representation.
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();

			switch (type)
			{
				case JsonElementType.OBJECT:
					sb.Append("{");

					bool first = true;

					foreach (string key in this.getKeys())
					{
						if (first == false) sb.Append(","); first = false;
						sb.Append("\"" + JsonElement.escape(key) + "\":");
						sb.Append(this[key].ToString());
					}

					sb.Append("}");
					break;

				case JsonElementType.ARRAY:
					sb.Append("[");

					int n = this.getLength();
					
					for (int i = 0; i < n; i++)
					{
						sb.Append(this[i].ToString());
						if (i != n-1) sb.Append(",");
					}

					sb.Append("]");
					break;

				case JsonElementType.STRING:
					sb.Append(value == null ? "null" : ("\"" + JsonElement.escape(getString()) + "\""));
					break;

				case JsonElementType.NUMBER:
					sb.Append(getNumber());
					break;

				case JsonElementType.BOOLEAN:
					sb.Append((getBool() ? "true" : "false"));
					break;
			}

			return sb.ToString();
		}

		// Converts the element to its string value representation.
		public string ToValue()
		{
			string data = "";

			switch (type)
			{
				case JsonElementType.STRING:
					data = value == null ? "" : getString();
					break;

				case JsonElementType.NUMBER:
					data = getNumber().ToString();
					break;

				case JsonElementType.BOOLEAN:
					data = getBool() ? "true" : "false";
					break;

				default:
					data = this.ToString();
					break;
			}

			return data;
		}

		// Converts an string to a JSON-element.
		private static bool StartsWith (string str, int sindex, int eindex, string test)
		{
			int count = test.Length;
			int i = 0;

			while (sindex <= eindex && count > 0)
			{
				if (str[sindex] != test[i])
					break;

				count--;
				sindex++;
				i++;
			}

			return count == 0;
		}

		// Converts an string to a JSON-element.
		private static JsonElement fromString (string str, int sindex, int eindex, out int used, bool report)
		{
			int temp, sindex0 = sindex;

			while (sindex <= eindex && str[sindex] <= 32)
				sindex++;

			if (StartsWith(str, sindex, eindex, "null"))
			{
				used = sindex - sindex0 + 4;
				return new JsonElement(JsonElementType.STRING);
			}

			if (StartsWith(str, sindex, eindex, "false"))
			{
				used = sindex - sindex0 + 5;
				return new JsonElement(false);
			}

			if (StartsWith(str, sindex, eindex, "true"))
			{
				used = sindex - sindex0 + 4;
				return new JsonElement(true);
			}

			if (str[sindex] == '[')
			{
				var t = new JsonElement(JsonElementType.ARRAY);
				sindex++;

				if (str[sindex] == ']')
					sindex++;

				while (str[sindex-1] != ']')
				{
					t.add(JsonElement.fromString(str, sindex, eindex, out temp, false));
					sindex += temp;

					while (sindex <= eindex && str[sindex] <= 32)
						sindex++;

					if (str[sindex] != ',' && str[sindex] != ']')
						throw new Exception ("Invalid JSON format: Expected ',' or ']' after item.");

					sindex++;
				}

				used = sindex - sindex0;
				return t;
			}

			if (str[sindex] == '{')
			{
				var t = new JsonElement(JsonElementType.OBJECT);
				sindex++;

				if (str[sindex] == '}')
					sindex++;

				while (str[sindex-1] != '}')
				{
					var tmp = JsonElement.fromString(str, sindex, eindex, out temp, false);
					sindex += temp;

					if (tmp.type != JsonElementType.STRING)
						throw new Exception("Invalid JSON: Expected key name as string.");

					while (sindex <= eindex && str[sindex] <= 32)
						sindex++;

					if (str[sindex] != ':')
						throw new Exception ("Invalid JSON format: Expected ':' after key name.");

					sindex++;

					t[tmp.getString()] = JsonElement.fromString(str, sindex, eindex, out temp, false);
					sindex += temp;

					while (sindex <= eindex && str[sindex] <= 32)
						sindex++;

					if (str[sindex] != ',' && str[sindex] != '}')
						throw new Exception ("Invalid JSON format: Expected ',' or '}' after property (Offset "+(sindex)+").");

					sindex++;
				}

				used = sindex - sindex0;
				return t;
			}

			if (str[sindex] == '"')
			{
				sindex++;

				int tmp = sindex;

				for (; sindex <= eindex; sindex++)
				{
					if (str[sindex] == '\\')
					{
						sindex++;
						continue;
					}

					if (str[sindex] == '"')
					{
						sindex++;
						break;
					}
				}

				used = sindex - sindex0;
				return new JsonElement(JsonElement.unescape(str.Substring(tmp, sindex-tmp-1)));
			}

			int j1 = str.IndexOf(",", sindex);
			int j2 = str.IndexOf("}", sindex);
			int j3 = str.IndexOf("]", sindex);
			int j = j1;

			if (j == -1 || (j2 != -1 && j2 < j1))
				j = j2;

			if (j == -1 || (j3 != -1 && j3 < j))
				j = j3;

			if (j == -1)
			{
				used = sindex - sindex0 + str.Length;
				return new JsonElement(double.Parse(str.Substring(sindex)));
			}
			else
			{
				used = sindex - sindex0 + (j - sindex);
				return new JsonElement(double.Parse(str.Substring(sindex, j-sindex)));
			}
		}

		// Converts an string to a JSON-element.
		public static JsonElement fromString (string str)
		{
			int temp;
			return String.IsNullOrEmpty(str) ? new JsonElement(JsonElementType.OBJECT) : fromString(str, 0, str.Length-1, out temp, true);
		}
	}
}
