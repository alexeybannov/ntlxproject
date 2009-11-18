using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using CI.Debt.Domain;
using NHibernate;

namespace CI.Debt.DAO {

	/// <summary>
	/// Класс предназначен для импорта субъектов и классификаторов из базы
	/// данных Access в SQLite.
	/// </summary>
	class DbImporter {

		private ISession session;

		/// <summary>
		/// Создает экземпляр типа DbImporter.
		/// </summary>
		public DbImporter() {
			session = DebtDAO.GetSession();
		}

		/// <summary>
		/// Импорт субъектов бюджетного процесса.
		/// </summary>
		public void ImportSubjects() {
			using (var connection = CreateConnection())
			using (var select = connection.CreateCommand()) {
				connection.Open();
				select.CommandText = "select SUBJ_GUID, SUBJ_NAME, SUBJ_CODE, BUDG_NAME from SUBJECT.txt";
				using (var reader = select.ExecuteReader())
				using (var tx = session.BeginTransaction()) {
					session.Delete("from Subject");
					while (reader.Read()) {
						var subject = new Subject() {
							Name = reader[1] as string,
							Code = reader[2] as string,
							BudgetName = reader[3] as string
						};
						if (!string.IsNullOrEmpty(subject.Name)) subject.Name = subject.Name.Replace("#", "\"");
						session.Save(subject, Convert.ToInt64(reader[0]));
					}
					tx.Commit();
				}
			}
		}

		/// <summary>
		/// Импорт бюджетной классификации расходов.
		/// </summary>
		public void ImportClassifiers() {
			using (var connection = CreateConnection())
			using (var select = connection.CreateCommand()) {
				connection.Open();
				select.CommandText = "select clsf_guid, clsf_code, grp_name_01, grp_name_02, grp_name_03, grp_name_04, grp_name_05, grp_name_06, grp_name_07, grp_name_08, grp_name_09, grp_name_10, grp_name_11, grp_name_12 from CLSF.txt where clsf_type_id = '1'";
				using (var reader = select.ExecuteReader())
				using (var tx = session.BeginTransaction()) {
					session.Delete("from Classifier");
					while (reader.Read()) {
						var classifier = new Classifier() {
							Code = reader.GetString(1),
							GrpName01 = reader[2] as string,
							GrpName02 = reader[3] as string,
							GrpName03 = reader[4] as string,
							GrpName04 = reader[5] as string,
							GrpName05 = reader[6] as string,
							GrpName06 = reader[7] as string,
							GrpName07 = reader[8] as string,
							GrpName08 = reader[9] as string,
							GrpName09 = reader[10] as string,
							GrpName10 = reader[11] as string,
							GrpName11 = reader[12] as string,
							GrpName12 = reader[13] as string,
						};
						session.Save(classifier, Convert.ToInt64(reader[0]));
					}
					tx.Commit();
				}
			}
		}

		/// <summary>
		/// Создание соединения с базой данных.
		/// </summary>
		/// <returns>IDbConnection</returns>
		private IDbConnection CreateConnection() {
			var connSettings = ConfigurationManager.ConnectionStrings["importDb"];
			var provider = DbProviderFactories.GetFactory(connSettings.ProviderName);
			var connection = provider.CreateConnection();
			connection.ConnectionString = connSettings.ConnectionString;
			return connection;
		}
	}
}