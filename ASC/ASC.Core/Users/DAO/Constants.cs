using System;

namespace ASC.Core.Users.DAO {

	/// <summary>
	/// Константы для DAO
	/// </summary>
	sealed class Constants {
	
		/// <summary>
		/// Кеш пользователей
		/// </summary>
		public const string UserCacheName = "ASC.Core.Users.UserCache";
		
		/// <summary>
		/// Кеш структуры групп пользователей
		/// </summary>
		public const string StructureCacheName = "ASC.Core.Users.StructureCache";
		
		/// <summary>
		/// Кеш действий и категорий действий
		/// </summary>
		public const string AzStructureCacheName = "ASC.Core.Users.AzStructureCache";

		/// <summary>
		/// Кеш с записями о правах доступа
		/// </summary>
		public const string AzAceCacheName = "ASC.Core.Users.AzAceCache";

		/// <summary>
		/// Кеш с записями авторизационной информации об объектах.
		/// </summary>
		public const string AzObjectInfoCacheName = "ASC.Core.Users.AzObjectInfoCacheName";
	}
}
