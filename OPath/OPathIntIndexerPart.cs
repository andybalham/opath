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
			var sourceObjectType = sourceObject.GetType();

			var indexerMethod = GetCachedValueMember(sourceObjectType) as MethodInfo;

			if (indexerMethod == null)
			{
				throw new OPathException(
                    $"{sourceOPath} of type {sourceObjectType.FullName} does not have an int indexer");
			}

			// TODO: Get the last item if the index is negative

			try
			{

				var indexerValue = indexerMethod.Invoke(sourceObject, new object[] { this.IndexValue });
				return indexerValue;

			}
			catch (TargetInvocationException targetInvocationException)
			{
				var ex = targetInvocationException.InnerException;

                if (ex != null)
                {
				    if ((ex is ArgumentOutOfRangeException)
					    || (ex is IndexOutOfRangeException))
				    {
					    throw new OPathException($"{sourceOPath}[{this.IndexValue}] index out of range");
				    }
                }

                throw new OPathException($"{sourceOPath}[{this.IndexValue}] threw exception: {ex?.Message}", ex);
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
			var indexerMethodName = sourceObjectType.IsArray ? "GetValue" : "get_Item";

			var indexerMethod = sourceObjectType.GetMethod(indexerMethodName, new[] { typeof(int) });

			return indexerMethod;
		}

		#endregion
	}
}
