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
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OPath
{
	internal class OPathXsltContextVariable : IXsltContextVariable
	{
		#region Public properties

		public string Name { get; private set; }

		#endregion

		#region Constructors

		public OPathXsltContextVariable(string name)
		{
			this.Name = name;
		}

		#endregion

		#region IXsltContextVariable Members

		public object Evaluate(XsltContext xsltContext)
		{
			OPathXsltContext opathXsltContext = (OPathXsltContext)xsltContext;

			IDictionary<string, object> variables = opathXsltContext.Variables;
			OPathCustomTypeConverter customTypeConverter = opathXsltContext.CustomTypeConverter;

			object value;
			if (!variables.TryGetValue(this.Name, out value))
			{
				throw new OPathException(
					string.Format("Variable {0} was not found in the OPathXsltContext.", this.Name));
			}

			object convertedValue = customTypeConverter(value);

			return convertedValue;
		}

		public bool IsLocal
		{
			get { return false; }
		}

		public bool IsParam
		{
			get { return false; }
		}

		public XPathResultType VariableType
		{
			get { return XPathResultType.Any; }
		}

		#endregion
	}
}
