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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OPath
{
	public class OPathDocument : Dictionary<string, object>
	{
		public OPathDocument() 
			: base()
		{

		}

		public OPathDocument(int capacity)
			: base(capacity)
		{

		}

		public OPathDocument(IEqualityComparer<string> comparer)
			: base(comparer)
		{

		}

		public OPathDocument(IDictionary<string, object> dictionary)
			: base(dictionary)
		{

		}

		public OPathDocument(int capacity, IEqualityComparer<string> comparer)
			: base(capacity, comparer)
		{

		}

		public OPathDocument(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}

		public OPathDocument(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
			: base(dictionary, comparer)
		{

		}

		// TODO: Add a constructor that takes an object[] params parameter???
	}
}
