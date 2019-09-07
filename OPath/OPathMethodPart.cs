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
	internal class OPathMethodPart : OPathPart
	{
		#region Public properties
		
		public string MethodName { get; private set; }

		#endregion

		#region Constructors

		public OPathMethodPart(string methodName)
		{
			this.MethodName = methodName;
		}

		#endregion

		#region Public methods

		public override object Evaluate(object sourceObject, string sourceOPath)
		{
			Type sourceObjectType = sourceObject.GetType();

			MethodInfo method = GetCachedValueMember(sourceObjectType) as MethodInfo;

			if (method == null)
			{
				throw new OPathException(string.Format(
					"{0} of type {1} does not have a {2}() method",
					sourceOPath, sourceObjectType.FullName, this.MethodName));
			}

			try
			{

				object methodReturnValue = method.Invoke(sourceObject, new object[] { });
				return methodReturnValue;

			}
			catch (TargetInvocationException targetInvocationException)
			{
				Exception ex = targetInvocationException.InnerException;

				throw new OPathException(string.Format(
					@"{0}.{1}() threw exception: {2}",
					sourceOPath, this.MethodName, ex.Message), ex);
			}
		}


		public override string ToString()
		{
			string toString = "." + this.MethodName + "()";
			return toString;
		}

		#endregion

		#region Protected methods

		protected override MemberInfo GetValueMember(Type sourceObjectType)
		{
			MethodInfo method = sourceObjectType.GetMethod(this.MethodName, new Type[] { });
			return method;
		}

		#endregion
	}
}
