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
using System.Text;

namespace OPath
{
	internal class OPathParseResult
	{
		#region Member variables

		private StringBuilder xpathBuilder = new StringBuilder();
		private Dictionary<string, OPathVariable> variableSet = new Dictionary<string, OPathVariable>();

		private List<OPathPart> opathPartList = new List<OPathPart>();

		private StringBuilder identifierBuilder = new StringBuilder();
		private StringBuilder whitespaceBuilder = new StringBuilder();
		private StringBuilder memberBuilder = new StringBuilder();
		private StringBuilder stringKeyBuilder = new StringBuilder();
		private StringBuilder intIndexBuilder = new StringBuilder(); 

		#endregion

		#region Public properties
		
		public OPathParsePosition CurrentPosition { get; set; }

		public char CurrentStringDelimiter { get; set; }

		public string CurrentVariableName { get; set; }

		public int IndexerStartIndex { get; set; }

		public int MemberStartIndex { get; set; }

		public int StringStartIndex { get; set; }

		public int IdentifierStartIndex { get; set; }

		public bool HasWhitespace
		{
			get
			{
				return this.whitespaceBuilder.Length > 0;
			}
		}

		#endregion

		#region Constructors
		
		public OPathParseResult()
		{
			this.CurrentPosition = OPathParsePosition.InExpression;
			this.CurrentStringDelimiter = default(char);
		} 

		#endregion

		#region Public methods

		public void AppendWhitespaceCharToXPath()
		{
			this.xpathBuilder.Append(' ');
		}

		public void AppendToXPath(char xpathChar)
		{
			this.xpathBuilder.Append(xpathChar);
		}

		public void AppendToXPath(string xpathString)
		{
			this.xpathBuilder.Append(xpathString);
		}

		public void AppendToIdentifier(char identifierChar)
		{
			this.identifierBuilder.Append(identifierChar);
		}

		public void AppendToIdentifier(string identifierString)
		{
			this.identifierBuilder.Append(identifierString);
		}

		public void AppendToMember(char identifierChar)
		{
			this.memberBuilder.Append(identifierChar);
		}

		public void ResetMember()
		{
			this.memberBuilder = new StringBuilder();
		}

		public void ResetOPathPartList()
		{
			this.opathPartList = new List<OPathPart>();
		}

		public void ResetWhitespace()
		{
			this.whitespaceBuilder = new StringBuilder();
		}

		public void AppendCharToWhitespace()
		{
			this.whitespaceBuilder.Append(' ');
		}

		public string GetAndResetIdentifier()
		{
			string identifier = GetIdentifier();
			ResetIdentifier();
			return identifier;
		}

		public string GetIdentifier()
		{
			string identifier = this.identifierBuilder.ToString();
			return identifier;
		}

		public void ResetIdentifier()
		{
			this.identifierBuilder = new StringBuilder();
		}

		public string GetAndResetWhitespace()
		{
			string whitespace = this.whitespaceBuilder.ToString();
			ResetWhitespace();
			return whitespace;
		}

		public bool IsDuplicateVariable(string variableReference)
		{
			bool isDuplicateVariable = this.variableSet.ContainsKey(variableReference);
			return isDuplicateVariable;
		}

		public string GetVariableXPathName(string variableReference)
		{
			OPathVariable variable = this.variableSet[variableReference];
			string variableXPathName = variable.XPathName;
			return variableXPathName;
		}

		public string GetNewVariableXPathName()
		{
			string variableXPathName = "Var" + (this.variableSet.Count + 1);
			return variableXPathName;
		}

		public OPathPart[] GetOPathParts()
		{
			OPathPart[] opathParts = this.opathPartList.ToArray();
			return opathParts;
		}

		public string AddNewVariable()
		{
			string xpathName = GetNewVariableXPathName();

			OPathVariable variable = new OPathVariable(this.CurrentVariableName, GetOPathParts(), xpathName);

			this.variableSet.Add(variable.GetReference(), variable);

			return xpathName;
		}

		public void AppendVariableToXPath(string xpathVariableName)
		{
			AppendToXPath('$');
			AppendToXPath(xpathVariableName);
		}

		public void AddVariable(string variableName)
		{
			this.CurrentVariableName = variableName;
			AddVariable();
		}

		public void AddVariable()
		{
			StringBuilder variableReferenceBuilder = new StringBuilder(this.CurrentVariableName);

			foreach (OPathPart opathPart in opathPartList)
			{
				variableReferenceBuilder.Append(opathPart);
			}

			string variableReference = variableReferenceBuilder.ToString();

			string xpathName;
			if (IsDuplicateVariable(variableReference))
			{
				xpathName = GetVariableXPathName(variableReference);
			}
			else
			{
				xpathName = AddNewVariable();
			}

			AppendVariableToXPath(xpathName);

			ResetOPathPartList();
		}

		public void AddOPathPropertyPart()
		{
			this.opathPartList.Add(new OPathPropertyPart(this.memberBuilder.ToString()));
			this.memberBuilder = new StringBuilder();
		}

		public void AddOPathMethodPart()
		{
			this.opathPartList.Add(new OPathMethodPart(this.memberBuilder.ToString()));
			this.memberBuilder = new StringBuilder();
		}

		public void AppendToIntIndexer(char intIndexerChar)
		{
			this.intIndexBuilder.Append(intIndexerChar);
		}

		public void AppendToStringKey(char stringKeyChar)
		{
			this.stringKeyBuilder.Append(stringKeyChar);
		}

		public bool IsInIntIndexer()
		{
			bool isInIntIndexer = (this.intIndexBuilder.Length > 0);
			return isInIntIndexer;
		}

		public void AddOPathIntIndexerPart()
		{
			this.opathPartList.Add(new OPathIntIndexerPart(int.Parse(this.intIndexBuilder.ToString())));
			this.intIndexBuilder = new StringBuilder();
		}

		public void AddOPathStringKeyPart()
		{
			this.opathPartList.Add(
				new OPathStringKeyPart(this.stringKeyBuilder.ToString(), this.CurrentStringDelimiter));
			this.stringKeyBuilder = new StringBuilder();
		}

		public bool IsStringDelimiter(char opathChar)
		{
			bool isStringDelimiter = (opathChar == this.CurrentStringDelimiter);
			return isStringDelimiter;
		}

		public OPathVariable[] GetVariables()
		{
			List<OPathVariable> variableList = (new List<OPathVariable>(this.variableSet.Values));

			OPathVariable[] variables = variableList.ToArray();

			return variables;
		}

		public string GetXPath()
		{
			string xpath = this.xpathBuilder.ToString();
			return xpath;
		}

		public void AppendOperatorToXPath(string identifier)
		{
			string lowerCaseIdentifier = identifier.ToLower();
			this.xpathBuilder.Append(lowerCaseIdentifier);
		}

		public void AppendFunctionToXPath(string identifier)
		{
			string lowerCaseIdentifier = identifier.ToLower();
			this.xpathBuilder.Append(lowerCaseIdentifier);
		}

		public void AppendOperatorToXPath(char opathChar)
		{
			switch (opathChar)
			{
				case '&':
					this.xpathBuilder.Append(" and ");
					break;
				case '|':
					this.xpathBuilder.Append(" or ");
					break;
				case '/':
					this.xpathBuilder.Append(" div ");
					break;
				case '%':
					this.xpathBuilder.Append(" mod ");
					break;
				default:
					throw new OPathException("Unhandled operator char: " + opathChar);
			};
		}

		#endregion
	}
}
