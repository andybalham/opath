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

namespace OPath
{
	internal static class OPathParser
	{
		#region Public methods

		public static OPathParseResult Parse(string opath)
		{
			OPathParseResult parseResult = new OPathParseResult();

			char[] opathChars = opath.ToCharArray();

			for (int opathCharIndex = 0; opathCharIndex < opathChars.Length; opathCharIndex++)
			{
				char opathChar = opathChars[opathCharIndex];

				char nextOPathChar = default(char);
				if (opathCharIndex + 1 < opathChars.Length)
				{
					nextOPathChar = opathChars[opathCharIndex + 1];
				}

				switch (parseResult.CurrentPosition)
				{
					case OPathParsePosition.InExpression:
						HandleCurrentPositionInExpression(parseResult, opathChar, nextOPathChar, opathCharIndex);
						break;

					case OPathParsePosition.InString:
						HandleCurrentPositionInString(parseResult, opathChar);
						break;

					case OPathParsePosition.InIdentifier:
						HandleCurrentPositionInIdentifier(parseResult, opathChar, opathCharIndex);
						break;

					case OPathParsePosition.InMember:
						HandleCurrentPositionInMember(parseResult, opathChar);
						break;

					case OPathParsePosition.PreIndexerValue:
						HandleCurrentPositionPreIndexerValue(parseResult, opathChar, opathCharIndex);
						break;

					case OPathParsePosition.PostIndexerValue:
						HandleCurrentPositionPostIndexerValue(parseResult, opathChar);
						break;

					case OPathParsePosition.InIntIndexer:
						HandleCurrentPositionInIntIndexer(parseResult, opathChar);
						break;

					case OPathParsePosition.InStringKey:
						HandleCurrentPositionInStringKey(parseResult, opathChar);
						break;

					case OPathParsePosition.InVariableOPath:
						HandleCurrentPositionInVariableOPath(parseResult, opathChar);
						break;

					case OPathParsePosition.InMethodParameters:
						HandleCurrentPositionInMethodParameters(parseResult, opathChar);
						break;

					default:
						throw new OPathException("Unhandled current position in expression: " + parseResult.CurrentPosition);
				}
			}

			switch (parseResult.CurrentPosition)
			{
				case OPathParsePosition.InExpression:
					// Nothing to do as we will have nothing buffered
					break;

				case OPathParsePosition.InIdentifier:
					HandleFinalPositionInIdentifier(parseResult);
					break;

				case OPathParsePosition.InMember:
					HandleFinalPositionInMember(parseResult);
					break;

				case OPathParsePosition.InVariableOPath:
					HandleFinalPositionInVariableOPath(parseResult);
					break;

				case OPathParsePosition.InString:
					throw new OPathException("Unclosed string starting at position " + (parseResult.StringStartIndex + 1));

				case OPathParsePosition.PreIndexerValue:
				case OPathParsePosition.PostIndexerValue:
				case OPathParsePosition.InIntIndexer:
					throw new OPathException("Unclosed indexer starting at position " + (parseResult.IndexerStartIndex + 1));

				case OPathParsePosition.InStringKey:
					throw new OPathException("Unclosed string key starting at position " + (parseResult.StringStartIndex + 1));

				case OPathParsePosition.InMethodParameters:
					throw new OPathException("Unclosed method starting at position " + (parseResult.MemberStartIndex + 1));

				default:
					throw new OPathException("Unhandled current position at end of expression: " + parseResult.CurrentPosition);
			}

			return parseResult;
		}

		private static void HandleCurrentPositionInExpression(OPathParseResult parseResult, char opathChar, 
			char nextOPathChar, int opathCharIndex)
		{
			if (IsWhitespaceChar(opathChar))
			{
				parseResult.AppendWhitespaceCharToXPath();
			}
			else if (IsAlphabeticChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InIdentifier;
				parseResult.AppendToIdentifier(opathChar);
				parseResult.IdentifierStartIndex = opathCharIndex;
			}
			else if (IsStringDelimiterChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InString;
				parseResult.CurrentStringDelimiter = opathChar;
				parseResult.AppendToXPath(opathChar);
				parseResult.StringStartIndex = opathCharIndex;
			}
			else if (IsOperatorChar(opathChar))
			{
				if (nextOPathChar == opathChar)
				{
					// Ignore multiple operators, such as && and ||
				}
				else
				{
					parseResult.AppendOperatorToXPath(opathChar);
				}
			}
			else if (IsNotChar(opathChar))
			{
				if (IsEqualityChar(nextOPathChar))
				{
					// Allow != through
					parseResult.AppendToXPath(opathChar);
				}
				else
				{
					parseResult.CurrentPosition = OPathParsePosition.InIdentifier;
					parseResult.AppendToIdentifier("not");
					parseResult.IdentifierStartIndex = opathCharIndex;
				}
			}
			else if (IsEqualityChar(opathChar))
			{
				if (nextOPathChar == opathChar)
				{
					// Ignore multiple equalities
				}
				else
				{
					parseResult.AppendToXPath(opathChar);
				}
			}
			else
			{
				parseResult.AppendToXPath(opathChar);
			}
		}

		private static void HandleCurrentPositionInString(OPathParseResult parseResult, char opathChar)
		{
			if (parseResult.IsStringDelimiter(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InExpression;
			}

			parseResult.AppendToXPath(opathChar);
		}

		private static void HandleCurrentPositionInIdentifier(OPathParseResult parseResult, char opathChar,
			int opathCharIndex)
		{
			if (IsIdentifierChar(opathChar))
			{
				if (parseResult.HasWhitespace)
				{
					string identifier = parseResult.GetIdentifier();

					if (IsStandardSourceObjectIdentifier(identifier))
					{
						parseResult.ResetIdentifier();
						parseResult.AddVariable(identifier);
						parseResult.AppendWhitespaceCharToXPath();
					}
				}

				parseResult.AppendToIdentifier(opathChar);
			}
			else if (IsWhitespaceChar(opathChar))
			{
				string identifier = parseResult.GetIdentifier();

				if (IsXPathOperator(identifier))
				{
					parseResult.CurrentPosition = OPathParsePosition.InExpression;
					parseResult.ResetIdentifier();
					parseResult.AppendOperatorToXPath(identifier);
					parseResult.AppendWhitespaceCharToXPath();
				}
				else
				{
					parseResult.AppendCharToWhitespace();
				}
			}
			else if (IsOpeningBracket(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InExpression;
				string identifier = parseResult.GetAndResetIdentifier();
				parseResult.AppendFunctionToXPath(identifier);
				parseResult.AppendToXPath(opathChar);
				parseResult.ResetWhitespace();
			}
			else if (IsMemberPrefixChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InMember;
				parseResult.CurrentVariableName = parseResult.GetAndResetIdentifier();
				parseResult.MemberStartIndex = opathCharIndex;
				parseResult.ResetWhitespace();
			}
			else if (IsIndexerStartChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.PreIndexerValue;
				parseResult.CurrentVariableName = parseResult.GetAndResetIdentifier();
				parseResult.IndexerStartIndex = opathCharIndex;
				parseResult.ResetWhitespace();
			}
			else if (IsStringDelimiterChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InString;
				parseResult.CurrentStringDelimiter = opathChar;
				parseResult.AppendToXPath(opathChar);
				parseResult.StringStartIndex = opathCharIndex;
			}
			else
			{
				parseResult.CurrentPosition = OPathParsePosition.InExpression;

				string identifier = parseResult.GetAndResetIdentifier();

				if (IsXPathOperator(identifier))
				{
					parseResult.AppendOperatorToXPath(identifier);
				}
				else
				{
					parseResult.AddVariable(identifier);
				}

				parseResult.AppendToXPath(parseResult.GetAndResetWhitespace());
				parseResult.AppendToXPath(opathChar);
			}
		}

		private static void HandleCurrentPositionInMember(OPathParseResult parseResult, char opathChar)
		{
			if (IsMemberChar(opathChar))
			{
				parseResult.AppendToMember(opathChar);
				parseResult.ResetWhitespace();
			}
			else if (IsOpeningBracket(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InMethodParameters;
				parseResult.ResetWhitespace();
			}
			else if (IsMemberPrefixChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InMember;
				parseResult.AddOPathPropertyPart();
				parseResult.ResetWhitespace();
			}
			else if (IsIndexerStartChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.PreIndexerValue;
				parseResult.AddOPathPropertyPart();
				parseResult.ResetWhitespace();
			}
			else if (IsWhitespaceChar(opathChar))
			{
				parseResult.AppendCharToWhitespace();
			}
			else
			{
				parseResult.CurrentPosition = OPathParsePosition.InExpression;
				parseResult.AddOPathPropertyPart();
				parseResult.AddVariable();
				parseResult.AppendToXPath(parseResult.GetAndResetWhitespace());
				parseResult.AppendToXPath(opathChar);
			}
		}

		private static void HandleCurrentPositionPreIndexerValue(OPathParseResult parseResult, char opathChar,
			int opathCharIndex)
		{
			if (IsWhitespaceChar(opathChar))
			{
				// Ignore
			}
			else if (IsStringDelimiterChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InStringKey;
				parseResult.CurrentStringDelimiter = opathChar;
				parseResult.StringStartIndex = opathCharIndex;
			}
			else if (IsNumericStartChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InIntIndexer;
				parseResult.AppendToIntIndexer(opathChar);
			}
			else
			{
				throw new OPathException("Invalid character in indexer starting at position " + (parseResult.IndexerStartIndex + 1));
			}
		}

		private static void HandleCurrentPositionPostIndexerValue(OPathParseResult parseResult, char opathChar)
		{
			if (IsWhitespaceChar(opathChar))
			{
				// Ignore
			}
			else if (IsIndexerEndChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InVariableOPath;

				if (parseResult.IsInIntIndexer())
				{
					parseResult.AddOPathIntIndexerPart();
				}
				else
				{
					parseResult.AddOPathStringKeyPart();
				}
			}
			else
			{
				throw new OPathException("Invalid character in indexer starting at position " + (parseResult.IndexerStartIndex + 1));
			}
		}

		private static void HandleCurrentPositionInIntIndexer(OPathParseResult parseResult, char opathChar)
		{
			if (IsNumericChar(opathChar))
			{
				parseResult.AppendToIntIndexer(opathChar);
			}
			else if (IsIndexerEndChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InVariableOPath;
				parseResult.AddOPathIntIndexerPart();
			}
			else if (IsWhitespaceChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.PostIndexerValue;
			}
			else
			{
				throw new OPathException("Invalid character in indexer starting at position " + (parseResult.IndexerStartIndex + 1));
			}
		}

		private static void HandleCurrentPositionInStringKey(OPathParseResult parseResult, char opathChar)
		{
			if (parseResult.IsStringDelimiter(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.PostIndexerValue;
			}
			else
			{
				parseResult.AppendToStringKey(opathChar);
			}
		}

		private static void HandleCurrentPositionInVariableOPath(OPathParseResult parseResult, char opathChar)
		{
			if (IsMemberPrefixChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InMember;
				parseResult.ResetWhitespace();
			}
			else if (IsIndexerStartChar(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.PreIndexerValue;
				parseResult.ResetWhitespace();
			}
			else if (IsWhitespaceChar(opathChar))
			{
				parseResult.AppendCharToWhitespace();
			}
			else
			{
				parseResult.CurrentPosition = OPathParsePosition.InExpression;
				parseResult.AddVariable();
				parseResult.AppendToXPath(parseResult.GetAndResetWhitespace());
				parseResult.AppendToXPath(opathChar);
			}
		}

		private static void HandleCurrentPositionInMethodParameters(OPathParseResult parseResult, char opathChar)
		{
			if (IsClosingBracket(opathChar))
			{
				parseResult.CurrentPosition = OPathParsePosition.InVariableOPath;
				parseResult.AddOPathMethodPart();
			}
			else if (IsWhitespaceChar(opathChar))
			{
				// Ignore
			}
			else
			{
				throw new OPathException("Non-empty method starting at position " + (parseResult.MemberStartIndex + 1));
			}
		}

		private static void HandleFinalPositionInIdentifier(OPathParseResult parseResult)
		{
			string identifier = parseResult.GetAndResetIdentifier();

			if (IsXPathOperator(identifier))
			{
				parseResult.AppendOperatorToXPath(identifier);
			}
			else
			{
				parseResult.AddVariable(identifier);
			}
		}

		private static void HandleFinalPositionInMember(OPathParseResult parseResult)
		{
			parseResult.AddOPathPropertyPart();
			parseResult.AddVariable();
		}

		private static void HandleFinalPositionInVariableOPath(OPathParseResult parseResult)
		{
			parseResult.AddVariable();
		}

		private static bool IsOperatorChar(char opathChar)
		{
			bool isOperatorChar;

			switch (opathChar)
			{
				case '&':
				case '|':
				case '/':
				case '%':
					isOperatorChar = true;
					break;
				default:
					isOperatorChar = false;
					break;
			}

			return isOperatorChar;
		}

		private static bool IsNotChar(char opathChar)
		{
			bool isNotChar = (opathChar == '!');
			return isNotChar;
		}

		private static bool IsEqualityChar(char opathChar)
		{
			bool isEqualityChar = (opathChar == '=');
			return isEqualityChar;
		}

		private static bool IsIdentifierChar(char opathChar)
		{
			bool isIdentifierChar;

			if (IsAlphanumericChar(opathChar))
			{
				isIdentifierChar = true;
			}
			else
			{
				switch (opathChar)
				{
					case '_':
					case '-':
					case ':':
						isIdentifierChar = true;
						break;
					default:
						isIdentifierChar = false;
						break;
				};
			}

			return isIdentifierChar;
		}

		private static bool IsMemberChar(char opathChar)
		{
			bool isMemberChar;

			if (IsAlphanumericChar(opathChar))
			{
				isMemberChar = true;
			}
			else
			{
				switch (opathChar)
				{
					case '_':
						isMemberChar = true;
						break;
					default:
						isMemberChar = false;
						break;
				};
			}

			return isMemberChar;
		}

		private static bool IsMemberPrefixChar(char opathChar)
		{
			bool isMemberStartChar;

			switch (opathChar)
			{
				case '.':
					isMemberStartChar = true;
					break;
				default:
					isMemberStartChar = false;
					break;
			};

			return isMemberStartChar;
		}

		private static bool IsIndexerStartChar(char opathChar)
		{
			bool isIndexerStartChar = (opathChar == '[');
			return isIndexerStartChar;
		}

		private static bool IsIndexerEndChar(char opathChar)
		{
			bool isIndexerEndChar = (opathChar == ']');
			return isIndexerEndChar;
		}

		private static bool IsOpeningBracket(char opathChar)
		{
			bool isOpeningBracket = (opathChar == '(');
			return isOpeningBracket;
		}

		private static bool IsClosingBracket(char opathChar)
		{
			bool isOpeningBracket = (opathChar == ')');
			return isOpeningBracket;
		}

		private static bool IsStandardSourceObjectIdentifier(string identifier)
		{
			bool isStandardSourceObjectIdentifier;

			switch (identifier)
			{
				case "null":
				case "true":
				case "false":
					isStandardSourceObjectIdentifier = true;
					break;

				default:
					isStandardSourceObjectIdentifier = false;
					break;
			}

			return isStandardSourceObjectIdentifier;
		}

		private static bool IsXPathOperator(string identifier)
		{
			bool isXPathOperator;

			switch (identifier.ToLower())
			{
				// Logic operators
				case "and":
				case "or":

				// Number operators
				case "div":
				case "mod":

					isXPathOperator = true;
					break;

				default:

					isXPathOperator = false;
					break;
			}

			return isXPathOperator;
		}

		private static bool IsStringDelimiterChar(char opathChar)
		{
			bool isStringStartChar;

			switch (opathChar)
			{
				case '"':
				case '\'':
					isStringStartChar = true;
					break;
				default:
					isStringStartChar = false;
					break;
			};

			return isStringStartChar;
		}

		private static bool IsAlphanumericChar(char opathChar)
		{
			bool isAlphanumericChar = IsAlphabeticChar(opathChar) || IsNumericChar(opathChar);

			return isAlphanumericChar;
		}

		private static bool IsAlphabeticChar(char opathChar)
		{
			bool isAlphabeticChar = 
				(('A' <= opathChar) && (opathChar <= 'Z'))
				|| (('a' <= opathChar) && (opathChar <= 'z'));

			return isAlphabeticChar;
		}

		private static bool IsNumericChar(char opathChar)
		{
			bool isNumericChar = ('0' <= opathChar) && (opathChar <= '9');

			return isNumericChar;
		}

		private static bool IsNumericStartChar(char opathChar)
		{
			bool isNumericStartChar = IsNumericChar(opathChar) || (opathChar == '-');

			return isNumericStartChar;
		}

		private static bool IsWhitespaceChar(char opathChar)
		{
			bool isWhitespaceChar;

			switch (opathChar)
			{
				case ' ':
				case '\r':
				case '\n':
				case '\t':
					isWhitespaceChar = true;
					break;
				default:
					isWhitespaceChar = false;
					break;
			};

			return isWhitespaceChar;
		}

		#endregion
	}
}
