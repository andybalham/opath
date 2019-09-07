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
using System.Collections.Generic;
using System.Reflection;

namespace OPath
{
	internal class OPathStringKeyPart : OPathPart
	{
		#region Public properties
		
		public string KeyValue { get; set; }

		public char Delimiter { get; set; }

		#endregion

		#region Constructors

		public OPathStringKeyPart(string keyValue, char delimiter)
		{
			this.KeyValue = keyValue;
			this.Delimiter = delimiter;
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
					"{0} of type {1} does not have a string indexer", sourceOPath, sourceObjectType.FullName));
			}

			try
			{

				object indexerValue = indexerMethod.Invoke(sourceObject, new object[] { this.KeyValue });
				return indexerValue;

			}
			catch (TargetInvocationException targetInvocationException)
			{
				Exception ex = targetInvocationException.InnerException;

				if (ex is KeyNotFoundException)
				{
					throw new OPathException(string.Format(
						@"{0}[""{1}""] key not found", sourceOPath, this.KeyValue));
				}

				throw new OPathException(string.Format(
					@"{0}[""{1}""] threw exception: {2}",
					sourceOPath, this.KeyValue, ex.Message), ex);
			}
		}


		public override string ToString()
		{
			string toString = "[" + this.Delimiter + this.KeyValue + this.Delimiter + "]";
			return toString;
		}

		#endregion	
	
		#region Protected methods

		protected override MemberInfo GetValueMember(Type sourceObjectType)
		{
			MethodInfo indexerMethod = sourceObjectType.GetMethod("get_Item", new Type[] { typeof(string) });
			return indexerMethod;
		}

		#endregion	
	}
}
