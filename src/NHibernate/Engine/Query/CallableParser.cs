using System;
using System.Text.RegularExpressions;
using NHibernate.Hql.Classic;
using NHibernate.Util;
using NHibernate.SqlCommand;

namespace NHibernate.Engine.Query
{
	public static class CallableParser
	{
		private static readonly Regex functionNameFinder = new Regex(@"\{[\S\s]*call[\s]+([\w]+)[^\w]");
		private static readonly int NewLineLength = Environment.NewLine.Length;

		public static SqlString Parse(string sqlString)
		{
			bool isCallableSyntax = sqlString.IndexOf("{") == 0 &&
									sqlString.IndexOf("}") == (sqlString.Length - 1) &&
									sqlString.IndexOf("call") > 0;

			if (!isCallableSyntax)
				throw new ParserException("Expected callable syntax {? = call procedure_name[(?, ?, ...)]} but got: " + sqlString);


			Match functionMatch = functionNameFinder.Match(sqlString);

			if ((!functionMatch.Success) || (functionMatch.Groups.Count < 2))
				throw new HibernateException("Could not determine function name for callable SQL: " + sqlString);

			string function = functionMatch.Groups[1].Value;
			return new SqlString(function);
		}
	}
}