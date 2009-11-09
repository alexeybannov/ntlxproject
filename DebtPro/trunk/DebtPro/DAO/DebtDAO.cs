using System;
using System.Collections.Generic;
using CI.Debt.Domain;
using CI.Debt.Impl;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Tool.hbm2ddl;

namespace CI.Debt.DAO {

	/// <summary>
	/// Статический тип слоя доступа и манипулирования данными.
	/// Основа слоя -- ORM NHibernate (http://wwww.nhibernate.org).
	/// В качестве базы данных используется встраиваемая база данных SQLite (http://www.sqlite.org).
	/// </summary>
	static class DebtDAO {

		/// <summary>
		/// Пустой классификатор по умолчанию 000.00 00.000 00 00.000.000.000:000 с идентификатором 1.
		/// </summary>
		public static Classifier EmptyClassifier {
			get {
				if (emptyClassifer == null) {
					lock (typeof(DebtDAO)) {
						if (emptyClassifer == null) {
							emptyClassifer = GetOrCreateEmptyClassifier();
						}
					}
				}
				return emptyClassifer;
			}
		}

		private static Classifier emptyClassifer;

		/// <summary>
		/// Текущая конфигурация NHibernate.
		/// </summary>
		public static Configuration Cfg {
			get;
			private set;
		}

		/// <summary>
		/// Сессия NHibatenate.
		/// </summary>
		public static ISession Session {
			get;
			private set;
		}

		/// <summary>
		/// Фабрика сессий NHibernate.
		/// </summary>
		public static ISessionFactory Factory {
			get;
			private set;
		}

		/// <summary>
		/// Создает тип DebtDAO.
		/// </summary>
		static DebtDAO() {
			Cfg = new Configuration();
			Cfg.Configure();

			Factory = Cfg.BuildSessionFactory();
			Session = Factory.OpenSession();
			Session.FlushMode = FlushMode.Commit;

			try {
				using (var alterCommand = Session.Connection.CreateCommand()) {
					alterCommand.CommandText = "alter table DebtSettings add column FilterBudget TEXT";
					alterCommand.ExecuteNonQuery();
				}
			}
			catch { }
		}

		/// <summary>
		/// Создает структуру базы данных в подключенной базе данных.
		/// </summary>
		public static void ReCreateSchema() {
			var schema = new SchemaExport(Cfg);
			schema.Create(true, true);
		}

		/// <summary>
		/// Получить полный список субъектов (упорядочен по имени субъекта).
		/// </summary>
		/// <returns>Список субъектов</returns>
		public static IList<Subject> GetSubjects() {
			return Session.CreateCriteria(typeof(Subject)).
				AddOrder(new Order("Name", true)).
				List<Subject>();
		}

		/// <summary>
		/// Получить субъект по идентификатору. Если субъект не найден функция возвращает null.
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns>Субъект</returns>
		public static Subject GetSubject(long id) {
			return GetObjectById<Subject>(id);
		}

		/// <summary>
		/// Получить субъект по коду. Если субъект не найден функция возвращает null.
		/// </summary>
		/// <param name="code">Код субъекта.</param>
		/// <returns></returns>
		public static Subject GetSubjectByCode(string code) {
			return Session.CreateCriteria(typeof(Subject)).
				Add(Expression.Eq("Code", code != null ? code.Trim() : string.Empty)).
				SetMaxResults(1).
				UniqueResult<Subject>();
		}

		public static string[] GetBudgets() {
			return new List<string>(
				Session.CreateCriteria(typeof(Subject)).
					SetProjection(Projections.GroupProperty("BudgetName")).
					AddOrder(Order.Asc("BudgetName")).
					List<string>()
				).ToArray();
		}

		/// <summary>
		/// Получить полный список классификаторов (упорядочен по коду).
		/// </summary>
		/// <returns>Список классификаторов</returns>
		public static IList<Classifier> GetClassifiers() {
			return Session.CreateCriteria(typeof(Classifier)).
				AddOrder(new Order("Code", true)).
				List<Classifier>();
		}

		/// <summary>
		/// Получить классификатор по идентификатору. 
		/// Если не найден функция возвращает null.
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns>Классификатор</returns>
		public static Classifier GetClassifier(long id) {
			return GetObjectById<Classifier>(id);
		}

		/// <summary>
		/// Найти классификатор по коду. Если классификатор не найден 
		/// функция генерирует исключение ClassifierNotFoundException.
		/// </summary>
		/// <param name="code">Код по маске и просто код</param>
		/// <returns>Классификатор</returns>
		public static Classifier FindClassifier(string code) {
			var clsf = Session.CreateCriteria(typeof(Classifier)).
				Add(Expression.Eq("Code", DebtUtil.RemoveMaskSymbols(code))).
				SetMaxResults(1).
				UniqueResult<Classifier>();

			if (clsf != null) return clsf;
			throw new ClassifierNotFoundException(string.Format("Классификатор с кодом {0} не обнаружен в справочнике.", code));
		}

		/// <summary>
		/// Найти классификатор, у которого код начинается с заданной части кода.
		/// Если классификатор не найден функция возвращает null.
		/// </summary>
		/// <param name="code">Часть кода по маске или просто часть кода</param>
		/// <returns>Классификатор</returns>
		public static Classifier FindNearestClassifier(string code) {
			return Session.CreateCriteria(typeof(Classifier)).
				Add(Expression.Like("Code", DebtUtil.RemoveMaskSymbols(code), MatchMode.Start)).
				AddOrder(new Order("Code", true)).
				SetMaxResults(1).
				UniqueResult<Classifier>();
		}

		/// <summary>
		/// Запистаь в базу данных новый классфикатор.
		/// </summary>
		/// <param name="clsf">Новый классификатор.</param>
		public static void SaveClassifier(Classifier clsf) {
			using (var tx = Session.BeginTransaction()) {
				if (clsf.Id == default(long)) {
					Session.Save(clsf);
				}
				else {
					Session.Save(clsf, clsf.Id);
				}
				tx.Commit();
			}
		}

		/// <summary>
		/// Получить настройки приложения.
		/// </summary>
		/// <returns>Настройки</returns>
		public static DebtSettings GetSettings() {
			using (var tx = Session.BeginTransaction()) {
				var settings = Session.Get<DebtSettings>(1);
				if (settings == null) {
					settings = new DebtSettings();
					Session.Save(settings);
				}
				tx.Commit();
				return settings;
			}
		}

		/// <summary>
		/// Сохранить настройки приложения.
		/// </summary>
		/// <param name="settings"></param>
		public static void SaveSettings(DebtSettings settings) {
			if (settings == null) throw new ArgumentNullException("settings");

			using (var tx = Session.BeginTransaction()) {
				Session.Update(settings);
				tx.Commit();
			}
		}

		/// <summary>
		/// Создание классификатора по умолчанию.
		/// </summary>
		/// <returns>Классификатор по умолчанию</returns>
		private static Classifier GetOrCreateEmptyClassifier() {
			using (var tx = Session.BeginTransaction()) {
				var clsf = Session.Get<Classifier>((long)1);
				if (clsf == null) {
					clsf = new Classifier() {
						Code = new string('0', Classifier.CodeLenght),
						GrpName12 = "<Пустой классификатор>"
					};
					Session.Save(clsf, (long)1);
				}
				tx.Commit();
				return clsf;
			}
		}

		/// <summary>
		/// Получить список строк задолженностей.
		/// </summary>
		/// <param name="type">Тип задолженности</param>
		/// <param name="month">Месяц</param>
		/// <param name="year">Год</param>
		/// <returns>Список строк задолженностей</returns>
		public static IList<DebtRow> GetDebtRows(DebtType type, int month, int year) {
			return Session.CreateCriteria(typeof(DebtRow)).
				Add(Expression.Eq("DebtType", type)).
				Add(Expression.Eq("Month", month)).
				Add(Expression.Eq("Year", year)).
				List<DebtRow>();
		}

		/// <summary>
		/// Получить список строк задолженностей.
		/// </summary>
		/// <param name="type">Тип задолженности</param>
		/// <param name="month">Месяц</param>
		/// <param name="year">Год</param>
		/// <param name="budget">Бюджет</param>
		/// <returns>Список строк задолженностей</returns>
		public static IList<DebtRow> GetDebtRows(DebtType type, int month, int year, string budgetName) {
			return Session.CreateQuery("from DebtRow r where DebtType = ? and Month = ? and Year = ? and r.Subject.BudgetName = ?").
				SetInt32(0, (Int32)type).
				SetInt32(1, month).
				SetInt32(2, year).
				SetString(3, budgetName).
				List<DebtRow>();
		}

		/// <summary>
		/// Удалить строку задолженности.
		/// </summary>
		/// <param name="row">Строка задолженности</param>
		public static void RemoveDebtRow(DebtRow row) {
			using (var tx = Session.BeginTransaction()) {
				Session.Delete(row);
				tx.Commit();
			}
		}

		/// <summary>
		/// Сохранить существующую или вставить новую строку задолженности.
		/// </summary>
		/// <param name="row">Строка задолженности</param>
		public static void SaveOrUpdateDebtRow(DebtRow row) {
			using (var tx = Session.BeginTransaction()) {
				Session.SaveOrUpdate(row);
				tx.Commit();
			}
		}

		/// <summary>
		/// Копировать строки из одной задолженности в другую.
		/// </summary>
		/// <param name="fromDebtType">Тип задолженности откуда копировать.</param>
		/// <param name="fromMonth">Месяц задолженности откуда копировать.</param>
		/// <param name="fromYear">Год задолженности откуда копировать.</param>
		/// <param name="toDebtType">Тип задолженности куда копировать.</param>
		/// <param name="toMonth">Месяц задолженности куда копировать.</param>
		/// <param name="toYear">Год задолженности куда копировать.</param>
		public static void CopyDebtRows(
			DebtType fromDebtType, int fromMonth, int fromYear,
			DebtType toDebtType, int toMonth, int toYear) {

			var fromRows = GetDebtRows(fromDebtType, fromMonth, fromYear);
			using (var tx = Session.BeginTransaction()) {
				foreach (var fromRow in fromRows) {
					var row = (DebtRow)fromRow.Clone();
					row.DebtType = toDebtType;
					row.Month = toMonth;
					row.Year = toYear;

					Session.Save(row);
				}
				tx.Commit();
			}
		}

		private static T GetObjectById<T>(long id) {
			return Session.Get<T>(id);
		}
	}
}