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

using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace OPath
{
	public class OPathExpression
	{
		#region Public properties

		public string OPath { get; private set; }

		#endregion

		#region Internal properties

		internal OPathVariable[] Variables { get; set; }

		internal XPathExpression XPathExpression { get; set; }

		#endregion

		#region Factory methods

		public static OPathExpression Compile(string opath)
		{
			OPathParseResult parseResult = OPathParser.Parse(opath);

			string xpath = parseResult.GetXPath();
			OPathVariable[] opathVariables = parseResult.GetVariables();

			XPathExpression xpathExpression = null;
			try
			{
				xpathExpression = XPathExpression.Compile(xpath);
			}
			catch (XPathException ex)
			{
				throw new OPathException(string.Format(
					"An XPathException was thrown when compiling the expression '{0}': {1}",
					xpath, ex.Message), ex);
			}

			OPathExpression opathExpression = new OPathExpression(opath, opathVariables, xpathExpression);

			return opathExpression;
		}

		#endregion

		#region Constructors

		private OPathExpression(string opath, OPathVariable[] variables, XPathExpression xpathExpression)
		{
			this.OPath = opath;
			this.Variables = variables;
			this.XPathExpression = xpathExpression;
		}

		#endregion

		#region Internal methods

		internal bool IsSingleVariable()
		{
			const string SINGLE_VARIABLE_REGEX = @"
				^[ ]*  # Start with zero or more spaces
				\$     # Followed by a $
				\w+    # Then one or more alphanumeric characters or underscores
				[ ]*$  # Ending with zero or more spaces
				";

			bool isSingleVariable =
				(this.Variables.Length == 1)
				&& (Regex.IsMatch(
					this.XPathExpression.Expression, SINGLE_VARIABLE_REGEX, RegexOptions.IgnorePatternWhitespace));

			return isSingleVariable;
		}

		#endregion
	}
}
