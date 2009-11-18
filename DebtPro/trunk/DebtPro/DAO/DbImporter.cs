using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using CI.Debt.Domain;
using NHibernate;
using CgfManager = System.Configuration.ConfigurationManager;

namespace CI.Debt.DAO {

	/// <summary>
	/// Класс предназначен для импорта субъектов и классификаторов из базы
	/// данных Access в SQLite.
	/// </summary>
	class DbImporter {

		private ISession session;

		private ConnectionStringSettings accessConnectionString;

		/// <summary>
		/// Создает экземпляр типа DbImporter.
		/// </summary>
		public DbImporter() {
			this.session = DebtDAO.GetSession();
			this.accessConnectionString = CgfManager.ConnectionStrings["txtFiles"];
		}

		/// <summary>
		/// Импорт субъектов бюджетного процесса.
		/// </summary>
		public void ImportSubjects() {
			using (IDbConnection accessConnection = CreateAccessConnection()) {
				accessConnection.Open();
				using (IDbCommand select = accessConnection.CreateCommand()) {
					select.CommandText = "select SUBJ_GUID, SUBJ_NAME, SUBJ_CODE, BUDG_NAME from SUBJECT.txt";
					using (IDataReader reader = select.ExecuteReader()) {
						ITransaction tx = session.BeginTransaction();
						try {
							session.Delete("from Subject");
							while (reader.Read()) {
								Subject subject = new Subject() {
									Name = (!reader.IsDBNull(1) ? reader.GetString(1) : null),
									Code = (!reader.IsDBNull(2) ? reader.GetString(2) : null),
									BudgetName = (!reader.IsDBNull(3) ? reader.GetString(3) : null)
								};
								if (!string.IsNullOrEmpty(subject.Name)) subject.Name = subject.Name.Replace("#", "\"");
								session.Save(subject, Convert.ToInt64(reader[0]));
							}
							tx.Commit();
						}
						catch {
							tx.Rollback();
							session.Flush();
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// Импорт бюджетной классификации расходов.
		/// </summary>
		public void ImportClassifiers() {
			using (IDbConnection accessConnection = CreateAccessConnection()) {
				accessConnection.Open();
				using (IDbCommand select = accessConnection.CreateCommand()) {
					select.CommandText = "select clsf_guid, clsf_code, clsf_name, clsf_full_name, grp_name_01, grp_name_02, grp_name_03, grp_name_04, grp_name_05, grp_name_06, grp_name_07, grp_name_08, grp_name_09, grp_name_10, grp_name_11, grp_name_12 from CLSF.txt where clsf_type_id = '1'";
					using (IDataReader reader = select.ExecuteReader()) {
						ITransaction tx = session.BeginTransaction();
						try {
							session.Delete("from Classifier");
							while (reader.Read()) {
								Classifier classifier = new Classifier() {
									Code = reader.GetString(1),
									GrpName01 = (!reader.IsDBNull(4) ? reader.GetString(4) : null),
									GrpName02 = (!reader.IsDBNull(5) ? reader.GetString(5) : null),
									GrpName03 = (!reader.IsDBNull(6) ? reader.GetString(6) : null),
									GrpName04 = (!reader.IsDBNull(7) ? reader.GetString(7) : null),
									GrpName05 = (!reader.IsDBNull(8) ? reader.GetString(8) : null),
									GrpName06 = (!reader.IsDBNull(9) ? reader.GetString(9) : null),
									GrpName07 = (!reader.IsDBNull(10) ? reader.GetString(10) : null),
									GrpName08 = (!reader.IsDBNull(11) ? reader.GetString(11) : null),
									GrpName09 = (!reader.IsDBNull(12) ? reader.GetString(12) : null),
									GrpName10 = (!reader.IsDBNull(13) ? reader.GetString(13) : null),
									GrpName11 = (!reader.IsDBNull(14) ? reader.GetString(14) : null),
									GrpName12 = (!reader.IsDBNull(15) ? reader.GetString(15) : null)
								};
								session.Save(classifier, Convert.ToInt64(reader[0]));
							}
							tx.Commit();
						}
						catch {
							tx.Rollback();
							session.Flush();
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// Создание соединения с базой данных Access.
		/// </summary>
		/// <returns>IDbConnection</returns>
		private IDbConnection CreateAccessConnection() {
			IDbConnection connection = null;
			switch (accessConnectionString.ProviderName) {
				case "System.Data.OleDb":
					connection = new OleDbConnection();
					break;
				case "System.Data.Odbc":
					connection = new OdbcConnection();
					break;
				default:
					throw new ArgumentOutOfRangeException("Неизвестный тип провайдера базы данных Access");
			}
			connection.ConnectionString = accessConnectionString.ConnectionString;
			return connection;
		}
	}
}