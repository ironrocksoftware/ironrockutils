
using System;
using System.ComponentModel;

namespace IronRockUtils.Licensing
{
	public class LicenseProvider : System.ComponentModel.LicenseProvider
	{
		public static Config licenseConfig = null;

		public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
		{
			if (context.UsageMode == LicenseUsageMode.Designtime)
				return new DesigntimeLicense (type);

			if (licenseConfig == null)
			{
				if (allowExceptions)
					throw new LicenseException (type, instance, "License provider configuration not properly set.");
				else
					return null;
			}

			string value = licenseConfig.get(type.GUID.ToString().ToLower());
			if (String.IsNullOrEmpty(value)) return null;

			return new RuntimeLicense (type, value);
		}
	}
}
