
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace IronRockUtils
{
	// Used to store configuration data. An encrypted file is used.
	public class Config
	{
		// Default filename for the configuration data file.
		public static string defFilename = "config.dat";

		// Internal filepath of the configuration data file.
		public string filepath;

		// Password used to encrypt/decrypt the input file.
		private string password;

		// Name value pair collection for the configuration fields.
		private NameValueCollection fields;

		// Constructs the config file specifying the filepath.
		public Config (string filepath, string password)
		{
			this.filepath = filepath;
			this.password = password;
			this.load();
		}

		// Constructs the config object using the default filename config.dat on the execution folder.
		public Config (string password)
		{
			this.filepath = AppDomain.CurrentDomain.BaseDirectory + defFilename;
			this.password = password;
			this.load();
		}

		// Constructs an empty config object.
		public Config ()
		{
			this.fields = new NameValueCollection ();
			this.filepath = null;
			this.password = null;
		}

		// Saves the contents of the configuration dataset to the output file.
		public bool save ()
		{
			if (this.filepath == null)
				return false;

			string data = "";

			for (int i = 0; i < this.fields.Count; i++)
				data += this.fields.GetKey(i) + "|\n|" + this.fields.Get(i) + "|\n|";

			data += "IRU2";

			try {
				File.WriteAllText(this.filepath, Security.EncryptStringAES(data, this.password));
			}
			catch (Exception e) {
				Log.write("Config (Error): " + e.Message);
				return false;
			}

			return true;
		}

		// Loads the contents of the configuration data file.
		public void load ()
		{
			this.fields = new NameValueCollection ();

			if (!File.Exists(this.filepath)) return;

			string data = System.IO.File.ReadAllText(this.filepath);
			if (data == null || data.Length == 0) return;

			data = Security.DecryptStringAES(data, this.password, null);
			if (data == null || data.Length == 0) return;

			string separator = "|\n|";

			if (!data.EndsWith("IRU!") && !data.EndsWith("IRU2"))
				return;

			if (data.EndsWith("IRU!"))
				separator = "\n";

			data = data.Substring(0, data.Length-4);

			string[] arr = data.Split(new string[] { separator }, StringSplitOptions.None);

			for (int i = 0; i+1 < arr.Length; i+=2)
			{
				this.fields.Set(arr[i], arr[i+1]);
			}
		}

		// Family of functions used to set a value of a field on the configuration data set. It will
		// overwrite the field if it already exists.

		public Config put (string name, string value)
		{
			if (name != null) this.fields.Set (name, value == null ? "" : value);
			return this;
		}

		public Config put (string name, int value) {
			return put (name, Convert.ToString(value));
		}

		public Config put (string name, float value) {
			return put (name, Convert.ToString(value));
		}

		public Config put (string name, bool value) {
			return put (name, value ? "1" : "0");
		}

		// Family of functions used to obtain the value of a field. If the field does not exist the
		// proper default value will be returned.

		public string get (string name)
		{
			if (name == null) return "";

			string value = this.fields.Get(name);
			return value == null ? "" : value;
		}

		public int getInt (string name)
		{
			string value = get (name);
			return value.Length == 0 ? 0 : Convert.ToInt32(value);
		}

		public float getFloat (string name)
		{
			string value = get (name);
			return value.Length == 0 ? 0 : (float)Convert.ToDouble(value);
		}

		public bool getBool (string name)
		{
			string value = get (name);
			return value.Length == 0 ? false : (Convert.ToInt32(value) != 0 ? true : false);
		}
	}
}