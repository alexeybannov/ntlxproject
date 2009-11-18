using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CI.Debt.DAO;
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
				var rows = new List<DebtRow>();
				foreach (var xmlRow in xmlRows) {
					var row = new DebtRow();
					row.Amount = xmlRow.Amount;
					row.Amount2 = xmlRow.Amount2;
					row.DebtType = (DebtType)xmlRow.DebtType;
					row.Month = xmlRow.Month;
					row.Year = xmlRow.Year;
					row.Subject = DebtDAO.GetSubjectByCode(xmlRow.SubjectCode);
					row.Classifier = DebtDAO.FindClassifier(xmlRow.ClassifierCode);
					if (row.Classifier == null) {
						row.Classifier = new Classifier(xmlRow.ClassifierId) {
							Code = xmlRow.ClassifierCode,
							GrpName12 = string.Format("<Классификатор импортирован из {0}>", Path.GetFileName(fileName))
						};
					}
					rows.Add(row);
				}
				return rows;
			}
		}
	}
}