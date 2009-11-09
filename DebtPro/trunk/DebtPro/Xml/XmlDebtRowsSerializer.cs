using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CI.Debt.Domain;

namespace CI.Debt.Xml {

	static class XmlDebtRowsSerializer {

		public static void Serialize(IEnumerable<DebtRow> rows, string fileName) {
			var serializer = new XmlSerializer(typeof(XmlDebtRows));

			using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
				serializer.Serialize(writer, new XmlDebtRows(rows), new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
			}
		}

		public static IEnumerable<DebtRow> Deserialize(string fileName) {
			if (!File.Exists(fileName)) throw new FileNotFoundException("Файл не найден", fileName);

			var serializer = new XmlSerializer(typeof(XmlDebtRows));
			using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
				var xmlRows = ((XmlDebtRows)serializer.Deserialize(reader)).Rows;
				return Array.ConvertAll<XmlDebtRow, DebtRow>(xmlRows, r => r.Row);
			}
		}

		public static IEnumerable<XmlClsfRow> DeserializeClassifiers(string fileName) {
			if (!File.Exists(fileName)) throw new FileNotFoundException("Файл не найден", fileName);

			var serializer = new XmlSerializer(typeof(XmlClsfRows));
			using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
				return ((XmlClsfRows)serializer.Deserialize(reader)).Rows;
			}
		}
	}
}