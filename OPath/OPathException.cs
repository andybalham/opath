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
using System.Runtime.Serialization;

namespace OPath
{
	[Serializable]
	public class OPathException : SystemException
	{
		#region Public constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="OPathException"/> class.
		/// </summary>
		public OPathException()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OPathException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public OPathException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OPathException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public OPathException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion

		#region Protected constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="OPathException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected OPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
