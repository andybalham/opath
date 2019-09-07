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
using System.Reflection;

namespace OPath
{
	internal class OPathPropertyPart : OPathPart
	{
		#region Public properties
		
		public string PropertyName { get; set; }

		#endregion

		#region Constructors

		public OPathPropertyPart(string propertyName)
		{
			this.PropertyName = propertyName;
		}

		#endregion

		#region Public methods

		public override object Evaluate(object sourceObject, string sourceOPath)
		{
			Type sourceObjectType = sourceObject.GetType();

			PropertyInfo property = GetCachedValueMember(sourceObjectType) as PropertyInfo;

			if (property == null)
			{
				throw new OPathException(string.Format(
					"{0} of type {1} does not have a {2} property",
					sourceOPath, sourceObjectType.FullName, this.PropertyName));
			}

			try
			{

				object propertyValue = property.GetValue(sourceObject, new object[] { });
				return propertyValue;

			}
			catch (TargetInvocationException targetInvocationException)
			{
				Exception ex = targetInvocationException.InnerException;

				throw new OPathException(string.Format(
					@"{0}.{1} threw exception: {2}",
					sourceOPath, this.PropertyName, ex.Message), ex);
			}
		}


		public override string ToString()
		{
			string toString = "." + this.PropertyName;
			return toString;
		}

		#endregion

		#region Protected methods

		protected override MemberInfo GetValueMember(Type sourceObjectType)
		{
			PropertyInfo property = sourceObjectType.GetProperty(this.PropertyName);
			return property;
		}

		#endregion
	}
}
