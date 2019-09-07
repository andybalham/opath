/*
	Copyright (C) 2010 Andy Blackledge

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published 
	by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace OPath
{
	internal static class OPathDateTime
	{
		#region Member variables

		public const string FunctionPrefix = "date-time";

		private const string DATE_FORMAT = "yyyyMMddHHmmssfff";

		#endregion

		#region Public methods

		public static object Convert(object value)
		{
			object convertedValue = ((DateTime)value).ToString(DATE_FORMAT);
			return convertedValue;
		}

		public static object Now(object[] args)
		{
			return DateTime.Now;
		}

		public static object Today(object[] args)
		{
			return DateTime.Today;
		}

		public static object MinValue(object[] args)
		{
			return DateTime.MinValue;
		}

		public static object MaxValue(object[] args)
		{
			return DateTime.MaxValue;
		}

		public static object AddDays(object[] args)
		{
			// TODO: Check the input params and see how errors appear to the end user

			string dateString = args[0] + "";

			DateTime dateTime = ParseDateString(dateString);
			double days = double.Parse(args[1] + "");

			DateTime result = dateTime.AddDays(days);

			return result;
		} 

		#endregion

		#region Private methods
		
		private static DateTime ParseDateString(string dateString)
		{
			DateTime dateTime = DateTime.ParseExact(dateString, DATE_FORMAT, null);
			return dateTime;
		} 

		#endregion
	}
}
