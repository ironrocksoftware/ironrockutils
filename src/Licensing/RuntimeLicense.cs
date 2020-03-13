
using System;
using System.ComponentModel;

namespace IronRockUtils.Licensing
{
	public class RuntimeLicense : License
	{
		private Type type;
		private string licenseKey;

		internal RuntimeLicense(Type type, string licenseKey)
		{
			if (type == null)
         		throw new NullReferenceException("The licensed type reference cannot be null.");

      		this.type = type;
      		this.licenseKey = licenseKey;
		}

		public override string LicenseKey
		{
			get 
			{
				return licenseKey;
			}
		}

		public override void Dispose()
		{
		}
	}
}
