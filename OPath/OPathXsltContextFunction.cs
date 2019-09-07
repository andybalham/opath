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
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OPath
{
	internal class OPathXsltContextFunction : IXsltContextFunction
	{
		#region Public properties

		public string Prefix { get; private set; }

		public string Name { get; private set; }

		#endregion

		#region Constructors

		public OPathXsltContextFunction(string prefix, string name)
		{
			this.Prefix = prefix;
			this.Name = name;
		}

		#endregion

		#region IXsltContextFunction Members

		public XPathResultType[] ArgTypes
		{
			get { throw new NotImplementedException(); }
		}

		public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
		{
			OPathXsltContext opathXsltContext = (OPathXsltContext)xsltContext;

			IDictionary<string, OPathCustomFunction> customFunctions =
				opathXsltContext.CustomFunctions;

			string customFunctionKey = GetCustomFunctionKey(this.Prefix, this.Name);

			OPathCustomFunction customFunction;

			if (customFunctions.TryGetValue(customFunctionKey, out customFunction))
			{
				OPathCustomTypeConverter customTypeConverter = opathXsltContext.CustomTypeConverter;

				object result = customFunction(args);

				object convertedResult = customTypeConverter(result);

				return convertedResult;
			}
			else
			{
				throw new OPathException("Could not resolve the custom function " + customFunctionKey);
			}
		}

		public int Maxargs
		{
			get { throw new NotImplementedException(); }
		}

		public int Minargs
		{
			get { throw new NotImplementedException(); }
		}

		public XPathResultType ReturnType
		{
			get { return XPathResultType.Any; }
		}

		#endregion

		#region Public methods

		public static string GetCustomFunctionKey(string prefix, string name)
		{
			StringBuilder customFunctionKeyBuilder = new StringBuilder();

			if (!string.IsNullOrEmpty(prefix))
			{
				customFunctionKeyBuilder.Append(prefix);
				customFunctionKeyBuilder.Append(':');
			}

			customFunctionKeyBuilder.Append(name);

			string customFunctionKey = customFunctionKeyBuilder.ToString().ToLower();

			return customFunctionKey;
		}

		#endregion
	}
}
