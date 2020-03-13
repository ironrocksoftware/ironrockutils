
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronRockUtils.Json
{
	public class Member
	{
		protected string name;
		protected string type;
		
		protected string refName;
		internal Class refValue;

		public Member (string name, string type)
		{
			this.name = name;
			this.type = type;

			this.refName = IsPrimitive(this.type) ? null : this.type;
			this.refValue = null;
		}

		public Member (string name, string type, string refName)
		{
			this.name = name;
			this.type = type;

			this.refName = refName;
			this.refValue = null;
		}

		public static bool IsPrimitive (string type)
		{
			return type != null ? (type == "Integer" || type == "Bool" || type == "Double" || type == "String" || type == "Class" || type == "Array") : true;
		}

		public string GetName ()
		{
			return this.name;
		}

		public string GetTypeName ()
		{
			return this.type;
		}

		public string GetRefName ()
		{
			return this.refName;
		}

		public Class GetRef ()
		{
			return this.refValue;
		}

		public virtual JsonElement ToJsonElement(object obj)
		{
			if (obj == null)
			{
				if (type == "Integer" || type == "Double")
					return new JsonElement ((double)0);

				if (type == "Bool")
					return new JsonElement ((bool)false);

				if (type == "String")
					return new JsonElement ("");

				if (type == "Array")
					return new JsonElement (JsonElementType.ARRAY);

				return new JsonElement (JsonElementType.OBJECT);
			}

			if (!IsPrimitive(type))
			{
				if (refValue == null)
					throw new Exception ("Reference value "+this.refName+" required by "+this.name+" not found.");

				return refValue.ToJsonElement(obj);
			}

			if (type == "Integer")
				return new JsonElement ((int)obj);

			if (type == "Double")
				return new JsonElement ((double)obj);

			if (type == "Bool")
				return new JsonElement ((bool)obj);

			if (type == "String")
				return new JsonElement (obj.ToString());

			// Array
			JsonElement elem = new JsonElement (JsonElementType.ARRAY);

			object[] data = (object[])obj;

			if (!IsPrimitive(refName) && refValue == null)
				throw new Exception ("Reference value "+this.refName+" required by "+this.name+" not found.");

			foreach (object value in data)
			{
				switch (refName)
				{
					case "Integer":
						elem.add(new JsonElement((int)value));
						break;

					case "Double":
						elem.add(new JsonElement((double)value));
						break;

					case "Bool":
						elem.add(new JsonElement((bool)value));
						break;

					case "String":
						elem.add(new JsonElement(value.ToString()));
						break;

					default:
						elem.add(refValue.ToJsonElement(value));
						break;
				}
			}

			return elem;
		}
	}

	public class Class : Member
	{
		protected List<Member> members;

		public Class(string name) : base(name, "Class")
		{
			this.members = new List<Member> ();
		}

		public Class(string name, string refName) : base(name, "Class", refName)
		{
			this.members = new List<Member> ();
		}

		public List<Member> GetMembers()
		{
			return this.members;
		}

		public Member Find (string name)
		{
			foreach (Member m in this.members)
			{
				if (m.GetName() == name)
					return m;
			}

			return null;
		}

		public bool AddMember (Member memb)
		{
			if (Find(memb.GetName()) != null)
				return false;

			members.Add(memb);
			return true;
		}

		public static object GetPropertyValue (object obj, string name, object def)
		{
			System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(name);
			if (propertyInfo != null) return propertyInfo.GetValue(obj, null) ?? def;

			System.Reflection.FieldInfo fieldInfo = obj.GetType().GetField(name);
			if (fieldInfo != null) return fieldInfo.GetValue(obj) ?? def;

			return def;
		}

		public override JsonElement ToJsonElement(object obj)
		{
			if (obj == null)
				return new JsonElement (JsonElementType.OBJECT);

			if (this.refName != null && this.refValue == null)
				throw new Exception ("Reference value "+this.refName+" required by "+this.name+" not found.");

			JsonElement elem;

			if (this.refValue == null)
				elem = new JsonElement (JsonElementType.OBJECT);
			else
				elem = this.refValue.ToJsonElement(obj);

			foreach (Member m in this.members)
			{
				elem.add (m.GetName(), m.ToJsonElement(GetPropertyValue(obj, m.GetName(), null)));
			}

			return elem;
		}
	}

	public class DataClasses
	{
		protected List<Class> list;
		
		public DataClasses()
		{
			this.list = new List<Class> ();
		}

		public List<Class> GetList()
		{
			return this.list;
		}

		public Class Find (string name)
		{
			foreach (Class cls in this.list)
			{
				if (cls.GetName() == name)
					return cls;
			}

			return null;
		}

		public bool AddClass (Class cls)
		{
			if (Find(cls.GetName()) != null)
				return false;

			this.list.Add(cls);
			return true;
		}

		public static DataClasses LoadFromDescriptor (string filename)
		{
			if (!File.Exists(filename))
				throw new Exception ("Descriptor file "+filename+" was not found.");

			DataClasses classes = new DataClasses ();

			Class curClass = null;
			int lineNo = 0;

			foreach (string inputLine in System.IO.File.ReadAllLines(filename))
			{
				string line = inputLine.Trim();
				lineNo++;

				if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//"))
					continue;

				string[] args = line.Split(':');
				
				for (int i = 0; i < args.Length; i++)
					args[i] = args[i].Trim();

				if (curClass == null)
				{
					string[] args2 = line.Split(' ');

					for (int i = 0; i < args2.Length; i++)
						args2[i] = args2[i].Trim();

					if (args2[0] == "End")
						break;

					if (args2[0] != "Class")
						throw new Exception ("Expected 'Class' got: " + line + " on line " + lineNo);

					if (args.Length == 2)
						curClass = new Class (args2[1], args[1].Trim());
					else
						curClass = new Class (args2[1]);

					if (!classes.AddClass(curClass))
						throw new Exception ("Class " + args2[1] + " already exists.");

					continue;
				}

				if (args[0] == "End")
				{
					curClass = null;
					continue;
				}

				if (args[1] == "Array")
				{
					if (args.Length != 3)
						throw new Exception ("Required type of array contents in line " + lineNo);

					if (!curClass.AddMember(new Member(args[0], "Array", args[2])))
						throw new Exception ("Field " + args[0] + " already exists in class "+curClass.GetName()+".");
				}
				else
				{
					if (!curClass.AddMember(new Member(args[0], args[1])))
						throw new Exception ("Field " + args[0] + " already exists in class "+curClass.GetName()+".");
				}
			}

			StringBuilder errors = new StringBuilder ();
			Class r;

			foreach (Class c in classes.GetList())
			{
				if (c.GetRefName() != null)
				{
					r = classes.Find(c.GetRefName());
					if (r == null)
					{
						errors.Append("Undefined class " + c.GetRefName() + " required by " + c.GetName() + "\n");
						continue;
					}

					c.refValue = r;
				}

				foreach (Member m in c.GetMembers())
				{
					if (m.GetTypeName() == "Array")
					{
						if (Member.IsPrimitive(m.GetRefName()))
							continue;

						r = classes.Find(m.GetRefName());
						if (r == null)
						{
							errors.Append("Undefined class " + m.GetRefName() + " referenced by " + c.GetName() + "." + m.GetName() + "\n");
							continue;
						}

						m.refValue = r;
						continue;
					}
					else if (m.GetRefName() != null)
					{
						r = classes.Find(m.GetRefName());
						if (r == null)
						{
							errors.Append("Undefined class " + m.GetRefName() + " referenced by " + c.GetName() + "." + m.GetName() + "\n");
							continue;
						}

						m.refValue = r;
						continue;
					}
				}
			}

			if (errors.Length != 0)
				throw new Exception (errors.ToString());

			return classes;
		}
	}
}
