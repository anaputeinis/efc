// источник cs от 2022-11-09
// 2023-03-12 Сохранил историческое имя файла
namespace System.Data
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;

	/// <summary> Вспомогательный класс конвертации строки DataRow в объект </summary>
	public static class RowToObjExtensions
	{
		/// <summary> Создает объекты T и заполняет поля и свойства значениями из строк rows </summary>
		public static List<T> ToObjList<T> ( this IEnumerable<DataRow> rows )
			where T : class, new()
		{
			if ( rows == null )
				return new List<T> ();
			return FromRows<T> ( rows.ToArray () );
		}
		/// <summary> Создает объекты T и заполняет поля и свойства значениями из data.Tables[0] </summary>
		public static List<T> ToObjList<T> ( this DataSet data )
			where T : class, new()
		{
			if ( data == null || data.Tables.Count <= 0 )
				return new List<T> ();
			return FromRows<T> ( data.Tables[ 0 ].Select () );
		}
		/// <summary> Создает объекты T и заполняет поля и свойства значениями из таблицы table </summary>
		public static List<T> ToObjList<T> ( this DataTable table )
			where T : class, new()
		{
			if ( table == null )
				return new List<T> ();
			return FromRows<T> ( table.Select () );
		}
		/// <summary> Создает объект T и заполняет поля и свойства значениями из строки row </summary>
		public static T ToObj<T> ( this DataRow row )
			where T : class, new()
		{
			if ( row == null )
				return null;
			return FromRow<T> ( row );
		}
		/// <summary> Создает объект T и заполняет поля и свойства значениями из строки row </summary>
		private static T FromRow<T> ( DataRow row )
			where T : class, new()
		{
			if ( row == null )
				return null;
			return FromRows<T> ( Enumerable.Repeat ( row, 1 ).ToArray () ).First ();
		}
		/// <summary> Создает объекты T и заполняет поля и свойства значениями из строк rows </summary>
		private static List<T> FromRows<T> ( DataRow[] rows )
			where T : class, new()
		{
			List<T> result = new List<T> ( rows.Length );
			if ( rows.Length == 0 )
				return result;
			for ( int i = 0 ; i < rows.Length ; i++ )
				result.Add ( new T () );
			DataColumnCollection columns = rows[ 0 ].Table.Columns;
			// Получаем список полей/свойства
			IEnumerable<MemberInfo> memberList = Enumerable
				.Concat<MemberInfo> ( typeof ( T ).GetProperties (), typeof ( T ).GetFields () )
				.Where ( member => IncludeMember ( member ) );

			foreach ( MemberInfo member in memberList )
			{
				object[] values;
				if ( Attribute.IsDefined ( member, typeof ( RowContainerAttribute ) ) &&
					GetMemberType ( member ) == typeof ( DataRow ) )
				{
					values = rows;
				}
				else
				{
					DataColumn col = columns[ GetColName ( member ) ];
					Type memberType = GetMemberType ( member );
					// Извлекаем значения из строк по имени колонки
					values = PrepareValues ( col.DataType, memberType, rows.Select ( row => row[ col ] ) )
						.ToArray ();
				}
				for ( int i = 0 ; i < result.Count ; i++ )
					// Устанавливаем в свойство значение
					SetMemberValue ( member, result[ i ], values[ i ] );
			}
			return result;
		}
		/// <summary> Определяет нужно ли включать поле/свойство в выборку </summary>
		private static bool IncludeMember ( MemberInfo member )
		{
			// Не включаем поля/свойства с тегом игнорирования
			if ( Attribute.IsDefined ( member, typeof ( ColIgnoreAttribute ) ) )
				return false;
			// Не включаем свойства, в которых нельзя записать данные
			if ( member is PropertyInfo && !( member as PropertyInfo ).CanWrite )
				return false;
			// Не включаем статические свойства
			if ( member is PropertyInfo && ( member as PropertyInfo ).GetSetMethod ().IsStatic )
				return false;
			// Не включаем статические поля
			if ( member is FieldInfo && ( member as FieldInfo ).IsStatic )
				return false;
			// Включаем поля/свойства
			if ( member is PropertyInfo || member is FieldInfo )
				return true;
			// Не включаем остальное
			return false;
		}
		/// <summary> Определяет имя колонки для поля/свойства </summary>
		private static string GetColName ( MemberInfo member )
		{
			// если есть атрибут с именем колонки
			if ( Attribute.IsDefined ( member, typeof ( ColNameAttribute ) ) )
				return ( Attribute.GetCustomAttribute ( member, typeof ( ColNameAttribute ) ) as ColNameAttribute ).Name;
			// Если атрибута с именем колонки нету - используем имя свойства
			return member.Name;
		}
		/// <summary> Подготовка значения value для поля/свойства member (конвертация, смена типа если возможно) </summary>
		private static IEnumerable<object> PrepareValues ( Type source, Type target, IEnumerable<object> values )
		{
			values = values.Select ( item => Convert.IsDBNull ( item ) ? null : item );
			// Если Nullable то изменяем тип на тип аргумента
			if ( target.IsGenericType && target.GetGenericTypeDefinition () == typeof ( Nullable<> ) )
				target = target.GetGenericArguments ()[ 0 ];
			if ( source == target || target.IsAssignableFrom ( source ) )
				return values;
			if ( typeof ( IConvertible ).IsAssignableFrom ( target ) )
				return values.Select ( item => item == null ? null :
					Convert.ChangeType ( item, target, CultureInfo.InvariantCulture ) );
			return values;
		}
		/// <summary> Определяет тип поля/свойства member </summary>
		private static Type GetMemberType ( MemberInfo member )
		{
			PropertyInfo prp = member as PropertyInfo;
			if ( prp != null )
				return prp.PropertyType;
			FieldInfo fld = member as FieldInfo;
			if ( fld != null )
				return fld.FieldType;
			throw new Exception ( $"{member} не является свойством или полем." );
		}
		/// <summary> Устанавливает значение value в поле или свойство member объекта target </summary>
		private static void SetMemberValue ( MemberInfo member, object target, object value )
		{
			PropertyInfo prp = member as PropertyInfo;
			if ( prp != null )
			{
				prp.SetValue ( target, value, null );
				return;
			}
			FieldInfo fld = member as FieldInfo;
			if ( fld != null )
			{
				fld.SetValue ( target, value );
				return;
			}
			throw new Exception ( $"{member} не является свойством или полем." );
		}
	}
	/// <summary> Имя колонки DataTable для поля/свойства объекта </summary>
	[AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class ColNameAttribute : Attribute
	{
		public ColNameAttribute ( string name )
		{
			this.Name = name;
		}
		/// <summary> Имя колонки </summary>
		public string Name
		{
			get;
			set;
		}
	}
	/// <summary> Игнорирует поле/свойство (не пытается заполнить из DataTable) </summary>
	[AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class ColIgnoreAttribute : Attribute
	{
	}
	/// <summary> Поле/Свойство для хранения исходного DataRow </summary>
	[AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class RowContainerAttribute : Attribute
	{
	}
}
