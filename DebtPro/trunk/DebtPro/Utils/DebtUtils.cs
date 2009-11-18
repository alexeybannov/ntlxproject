
namespace CI.Debt.Utils {

	static class DebtUtil {

		public static string GetMonthName(int month) {
			switch (month) {
				case 1: return "Январь";
				case 2: return "Февраль";
				case 3: return "Март";
				case 4: return "Апрель";
				case 5: return "Май";
				case 6: return "Июнь";
				case 7: return "Июль";
				case 8: return "Август";
				case 9: return "Сентябрь";
				case 10: return "Октябрь";
				case 11: return "Ноябрь";
				case 12: return "Декабрь";
			}
			return string.Empty;
		}		
	}
}
