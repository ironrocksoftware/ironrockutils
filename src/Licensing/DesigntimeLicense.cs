
using System;
using System.ComponentModel;

namespace IronRockUtils.Licensing
{
	public class DesigntimeLicense : License
	{
		private Type type;

		internal DesigntimeLicense(Type type)
		{
			if (type == null)
         		throw new NullReferenceException("The licensed type reference cannot be null.");

      		this.type = type;
		}

		public override string LicenseKey
		{
			get 
			{
				return type.GUID.ToString();
			}
		}

		public override void Dispose()
		{
		}
	}
}
