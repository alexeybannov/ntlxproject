using System;
using System.Collections.Generic;
using CI.Debt.Domain;
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

		private static Configuration configuration;

		private static ISession session;

		private static ISessionFactory factory;

		private static List<Subject> subjects;

		private static List<string> budgets;

		private static IDictionary<long, Classifier> classifiers;


		/// <summary>
		/// Инициализация слоя доступа к данным.
		/// </summary>
		public static void Initialize() {
			configuration = new Configuration();
			configuration.Configure();

			factory = configuration.BuildSessionFactory();
			session = factory.OpenSession(new DebtInterceptor());
			session.FlushMode = FlushMode.Commit;

			PatchDatabase();

			LoadCache();
		}

		/// <summary>
		/// Получить текущую сессию NHibernate.
		/// </summary>
		/// <returns>Текущая сессия NHibernate.</returns>
		public static ISession GetSession() {
			CheckInitialization();
			return session;
		}

		/// <summary>
		/// Создает структуру базы данных в подключенной базе данных.
		/// </summary>
		public static void ReCreateSchema() {
			CheckInitialization();
			var schema = new SchemaExport(configuration);
			schema.Create(true, true);
		}

		/// <summary>
		/// Получить полный список субъектов (упорядочен по имени субъекта).
		/// </summary>
		/// <returns>Список субъектов</returns>
		public static List<Subject> GetSubjects() {
			CheckInitialization();
			return new List<Subject>(subjects);
		}

		/// <summary>
		/// Получить субъект по идентификатору. Если субъект не найден функция возвращает null.
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns>Субъект</returns>
		public static Subject GetSubject(long id) {
			CheckInitialization();
			return GetObjectById<Subject>(id);
		}

		/// <summary>
		/// Получить субъект по коду. Если субъект не найден функция возвращает null.
		/// </summary>
		/// <param name="code">Код субъекта.</param>
		/// <returns></returns>
		public static Subject GetSubjectByCode(string code) {
			CheckInitialization();
			if (string.IsNullOrEmpty(code)) return null;

			foreach (var s in subjects) {
				if (s.Code == code) return s;
			}
			return null;
		}

		public static string[] GetBudgets() {
			CheckInitialization();
			return budgets.ToArray();
		}

		/// <summary>
		/// Получить полный список классификаторов (упорядочен по коду).
		/// </summary>
		/// <returns>Список классификаторов</returns>
		public static List<Classifier> GetClassifiers() {
			CheckInitialization();

			var list = new List<Classifier>(classifiers.Values);
			list.Sort((x, y) => string.Compare(x.Code, y.Code));
			return list;
		}

		/// <summary>
		/// Получить классификатор по идентификатору. 
		/// Если не найден функция возвращает null.
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns>Классификатор</returns>
		public static Classifier GetClassifier(long id) {
			CheckInitialization();
			return GetObjectById<Classifier>(id);
		}

		/// <summary>
		/// Найти классификатор по коду. Если классификатор не найден 
		/// функция генерирует исключение ClassifierNotFoundException.
		/// </summary>
		/// <param name="code">Код по маске и просто код</param>
		/// <returns>Классификатор</returns>
		public static Classifier FindClassifier(string code) {
			CheckInitialization();
			if (string.IsNullOrEmpty(code)) return null;

			var clearCode = Classifier.RemoveMaskSymbols(code);
			foreach (var c in classifiers.Values) {
				if (c.Code == clearCode) return c;
			}
			return null;
		}

		/// <summary>
		/// Найти классификатор, у которого код начинается с заданной части кода.
		/// Если классификатор не найден функция возвращает null.
		/// </summary>
		/// <param name="code">Часть кода по маске или просто часть кода</param>
		/// <returns>Классификатор</returns>
		public static Classifier FindNearestClassifier(string code) {
			CheckInitialization();			
			if (string.IsNullOrEmpty(code)) return null;

			var clearCode = Classifier.RemoveMaskSymbols(code);
			foreach (var c in classifiers.Values) {
				if (c.Code.StartsWith(clearCode)) return c;
			}
			return null;
		}

		/// <summary>
		/// Запистаь в базу данных новый классфикатор.
		/// </summary>
		/// <param name="clsf">Новый классификатор.</param>
		public static void SaveClassifier(Classifier clsf) {
			CheckInitialization();
			using (var tx = session.BeginTransaction()) {
				if (clsf.Id == default(long)) {
					session.Save(clsf);
				}
				else {
					session.Save(clsf, clsf.Id);
				}
				tx.Commit();
				classifiers[clsf.Id] = clsf;
			}
		}

		/// <summary>
		/// Получить настройки приложения.
		/// </summary>
		/// <returns>Настройки</returns>
		public static DebtSettings GetSettings() {
			CheckInitialization();
			using (var tx = session.BeginTransaction()) {
				var settings = session.Get<DebtSettings>(1);
				if (settings == null) {
					settings = new DebtSettings();
					session.Save(settings);
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
			CheckInitialization();
			if (settings == null) throw new ArgumentNullException("settings");

			using (var tx = session.BeginTransaction()) {
				session.Update(settings);
				tx.Commit();
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
			CheckInitialization();
			return session.CreateCriteria(typeof(DebtRow)).
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
			CheckInitialization();
			return session.CreateQuery("from DebtRow r where DebtType = ? and Month = ? and Year = ? and r.Subject.BudgetName = ?").
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
			CheckInitialization();
			using (var tx = session.BeginTransaction()) {
				session.Delete(row);
				tx.Commit();
			}
		}

		/// <summary>
		/// Сохранить существующую или вставить новую строку задолженности.
		/// </summary>
		/// <param name="row">Строка задолженности</param>
		public static void SaveOrUpdateDebtRow(DebtRow row) {
			CheckInitialization();
			using (var tx = session.BeginTransaction()) {
				session.SaveOrUpdate(row);
				tx.Commit();
			}
		}

		/// <summary>
		/// Сохранить существующие или вставить новие строки задолженности.
		/// </summary>
		/// <param name="row">Строка задолженности</param>
		public static void SaveOrUpdateDebtRows(IEnumerable<DebtRow> rows) {
			CheckInitialization();
			using (var tx = session.BeginTransaction()) {
				foreach (var r in rows) {
					session.SaveOrUpdate(r);
				}
				tx.Commit();
				foreach (var r in rows) {
					classifiers[r.Classifier.Id] = r.Classifier;
				}
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

			CheckInitialization();
			var fromRows = GetDebtRows(fromDebtType, fromMonth, fromYear);
			using (var tx = session.BeginTransaction()) {
				foreach (var fromRow in fromRows) {
					var row = (DebtRow)fromRow.Clone();
					row.DebtType = toDebtType;
					row.Month = toMonth;
					row.Year = toYear;

					session.Save(row);
				}
				tx.Commit();
			}
		}

		private static void PatchDatabase() {
			try {
				using (var alterCommand = session.Connection.CreateCommand()) {
					alterCommand.CommandText = "alter table DebtSettings add column FilterBudget TEXT";
					alterCommand.ExecuteNonQuery();
				}
			}
			catch { }
		}

		private static void CheckInitialization() {
			if (session == null) throw new InvalidOperationException("Объект не инициалицирован. Необходим вызов метода Initialize()");
		}

		private static void LoadCache() {
			subjects = new List<Subject>(session.CreateCriteria(typeof(Subject)).List<Subject>());
			subjects.Sort((x, y) => string.Compare(x.Name, y.Name));

			budgets = new List<string>();
			foreach (var s in subjects) {
				if (!budgets.Contains(s.BudgetName)) budgets.Add(s.BudgetName);
			}
			budgets.Sort();

			classifiers = new Dictionary<long, Classifier>(3000);
			foreach (var c in session.CreateCriteria(typeof(Classifier)).List<Classifier>()) {
				classifiers[c.Id] = c;
			}
			if (!classifiers.ContainsKey(Classifier.Empty.Id)) {
				SaveClassifier(Classifier.Empty);
				classifiers[Classifier.Empty.Id] = Classifier.Empty;
			}
		}

		private static T GetObjectById<T>(long id) {
			return session.Get<T>(id);
		}
	}
}