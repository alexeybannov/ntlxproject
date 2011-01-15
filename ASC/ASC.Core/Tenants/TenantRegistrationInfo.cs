using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ASC.Core.Tenants
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "tenantRegistrationInfo")]
	public class TenantRegistrationInfo : IXmlSerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Address
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public List<string> TrustedDomains
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public CultureInfo Culture
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TimeZoneInfo TimeZoneInfo
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string FirstName
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string LastName
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Email
		{
			get;
			set;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public TenantRegistrationInfo()
		{
			TrustedDomains = new List<string>();
			Culture = CultureInfo.CurrentCulture;
			TimeZoneInfo = TimeZoneInfo.Local;
		}

		#region IXmlSerializable Members

		/// <inheritdoc/>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <inheritdoc/>
		public void ReadXml(XmlReader reader)
		{
			while (true)
			{
				if (!reader.Read()) break;

				if (reader.LocalName == "name") Name = reader.ReadString();
				else if (reader.LocalName == "address") Address = reader.ReadString();
				
				else if (reader.LocalName == "culture") Culture = CultureInfo.GetCultureInfo(reader.ReadString());
				else if (reader.LocalName == "timezone") TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(reader.ReadString());
				
				else if (reader.LocalName == "trustedDomain") TrustedDomains.Add(reader.ReadString());
				
				else if (reader.LocalName == "firstname") FirstName = reader.ReadString();
				else if (reader.LocalName == "lastname") LastName = reader.ReadString();
				else if (reader.LocalName == "email") Email = reader.ReadString();
				else if (reader.LocalName == "password") Password = reader.ReadString();
			}
		}

		/// <inheritdoc/>
		public void WriteXml(XmlWriter writer)
		{
			if (!string.IsNullOrEmpty(Name)) writer.WriteElementString("name", Name);
			if (!string.IsNullOrEmpty(Address)) writer.WriteElementString("address", Address);

			if (0 < TrustedDomains.Count)
			{
				writer.WriteStartElement("trustedDomains");
				TrustedDomains.ForEach(trustedDomain => writer.WriteElementString("trustedDomain", trustedDomain));
				writer.WriteEndElement();
			}

			if (Culture != null) writer.WriteElementString("culture", Culture.Name);
			if (TimeZoneInfo != null) writer.WriteElementString("timezone", TimeZoneInfo.Id);

			if (!string.IsNullOrEmpty(FirstName)) writer.WriteElementString("firstname", FirstName);
			if (!string.IsNullOrEmpty(LastName)) writer.WriteElementString("lastname", LastName);
			if (!string.IsNullOrEmpty(Email)) writer.WriteElementString("email", Email);
			if (!string.IsNullOrEmpty(Password)) writer.WriteElementString("password", Password);
		}

		#endregion
	}
}