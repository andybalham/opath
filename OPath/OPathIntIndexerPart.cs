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
	internal class OPathIntIndexerPart : OPathPart
	{
		#region Public properties
		
		public int IndexValue { get; set; }

		#endregion

		#region Constructors

		public OPathIntIndexerPart(int indexValue)
		{
			this.IndexValue = indexValue;
		}

		#endregion

		#region Public methods

		public override object Evaluate(object sourceObject, string sourceOPath)
		{
			Type sourceObjectType = sourceObject.GetType();

			MethodInfo indexerMethod = GetCachedValueMember(sourceObjectType) as MethodInfo;

			if (indexerMethod == null)
			{
				throw new OPathException(string.Format(
					"{0} of type {1} does not have an int indexer", sourceOPath, sourceObjectType.FullName));
			}

			// TODO: Get the last item if the index is negative

			try
			{

				object indexerValue = indexerMethod.Invoke(sourceObject, new object[] { this.IndexValue });
				return indexerValue;

			}
			catch (TargetInvocationException targetInvocationException)
			{
				Exception ex = targetInvocationException.InnerException;

				if ((ex is ArgumentOutOfRangeException)
					|| (ex is IndexOutOfRangeException))
				{
					throw new OPathException(string.Format(
						"{0}[{1}] index out of range", sourceOPath, this.IndexValue));
				}

				throw new OPathException(string.Format(
					"{0}[{1}] threw exception: {2}", sourceOPath, this.IndexValue, ex.Message), ex);
			}
		}


		public override string ToString()
		{
			string toString = "[" + this.IndexValue + "]";
			return toString;
		}

		#endregion	

		#region Protected methods

		protected override MemberInfo GetValueMember(Type sourceObjectType)
		{
			string indexerMethodName = sourceObjectType.IsArray ? "GetValue" : "get_Item";

			MethodInfo indexerMethod = sourceObjectType.GetMethod(indexerMethodName, new Type[] { typeof(int) });

			return indexerMethod;
		}

		#endregion
	}
}
