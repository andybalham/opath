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
using NUnit.Framework;

namespace OPath.TestSuite
{
	[TestFixture]
	public class OPathParserTests
	{
		#region Tests

		[Test]
		public void Parse_StandardSourceObjects_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"true and false or null");

			Assert.AreEqual(@"$Var1 and $Var2 or $Var3", parseResult.GetXPath(), "parseResult.GetXPath()");

			Assert.AreEqual(3, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_NoVariables_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"""Variable's1"" = 'Variable""s2'");

			Assert.AreEqual(@"""Variable's1"" = 'Variable""s2'", parseResult.GetXPath(), "parseResult.GetXPath()");

			Assert.AreEqual(0, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_XPathOperators_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"not (((100 div 5 mod 2) > 1) or true ()) and false ()");

			Assert.AreEqual(@"not(((100 div 5 mod 2) > 1) or true()) and false()", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(0, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_XPathOperatorsMixedCase_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"Not (((100 Div 5 Mod 2) > 1) OR True ()) AND False () AND (String-Length ('AND') > 2)");

			Assert.AreEqual(@"not(((100 div 5 mod 2) > 1) or true()) and false() and (string-length('AND') > 2)", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(0, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_XPathFunctions_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"Custom:Processing-Date () > date_time:now ()");

			Assert.AreEqual(@"custom:processing-date() > date_time:now()", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(0, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_NonXPathOperators_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"(! (((100/5%2) > 1)||true())&false()==true())|(true()!=false())");

			Assert.AreEqual(@"(not(((100 div 5 mod 2) > 1) or true()) and false()=true()) or (true()!=false())", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(0, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");
		}

		[Test]
		public void Parse_SingleVariable_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.GetReference(), "variable1.GetReference()");
			Assert.AreEqual(0, variable1.OPathParts.Length, "variable1.OPathParts.Length");
		}

		[Test]
		public void Parse_DuplicateVariables_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 = Variable1");

			Assert.AreEqual("$Var1 = $Var1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.GetReference(), "variable1.GetReference()");
			Assert.AreEqual(0, variable1.OPathParts.Length, "variable1.OPathParts.Length");
		}

		[Test]
		public void Parse_MultipleVariables_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 = Variable2");

			Assert.AreEqual("$Var1 = $Var2", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(2, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.GetReference(), "variable1.GetReference()");
			Assert.AreEqual(0, variable1.OPathParts.Length, "variable1.OPathParts.Length");

			OPathVariable variable2 = GetVariable("Var2", parseResult.GetVariables());
			Assert.AreEqual("Variable2", variable2.GetReference(), "variable1.GetReference()");
			Assert.AreEqual(0, variable2.OPathParts.Length, "variable1.OPathParts.Length");
		}

		[Test]
		public void Parse_VariableWithProperty_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1.Property = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Property", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariablePropertyPart(variable1, 0, "Property");
		}

		[Test]
		public void Parse_VariableWithMultipleProperties_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1.Property1.Property2 = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Property1.Property2", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(2, variable1.OPathParts.Length, "variable1.OPathParts.Length");

			AssertVariablePropertyPart(variable1, 0, "Property1");
			AssertVariablePropertyPart(variable1, 1, "Property2");
		}

		[Test]
		public void Parse_VariableWithMultiplePropertiesWithWhitespace_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 . Property1 . Property2 = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());

			Assert.AreEqual("Variable1.Property1.Property2", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(2, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariablePropertyPart(variable1, 0, "Property1");
			AssertVariablePropertyPart(variable1, 1, "Property2");
		}

		[Test]
		public void Parse_VariableWithPropertyAtEnd_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("1 = Variable1.Property");

			Assert.AreEqual("1 = $Var1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());

			Assert.AreEqual("Variable1.Property", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariablePropertyPart(variable1, 0, "Property");
		}

		[Test]
		public void Parse_VariableWithPropertyAtEndWithWhitespace_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("1 = Variable1.Property ");

			Assert.AreEqual("1 = $Var1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Property", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariablePropertyPart(variable1, 0, "Property");
		}

		[Test]
		public void Parse_VariableWithEmptyMethod_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1.Method() = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Method()", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableMethodPart(variable1, 0, "Method");
		}

		[Test]
		public void Parse_VariableWithEmptyMethodWithWhitespace_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 . Method ( ) = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Method()", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableMethodPart(variable1, 0, "Method");
		}

		[Test]
		public void Parse_VariableWithEmptyMethodAtEnd_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("1 = Variable1.Method()");

			Assert.AreEqual("1 = $Var1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1.Method()", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableMethodPart(variable1, 0, "Method");
		}

		[Test]
		public void Parse_VariableWithIntIndexer_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1[1] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.Name, "variable1.Name");
			Assert.AreEqual("[1]", variable1.GetOPath(), "variable1.GetOPath()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableIntIndexPart(variable1, 0, 1);
		}

		[Test]
		public void Parse_VariableWithNegativeIntIndexer_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1[-1] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.Name, "variable1.Name");
			Assert.AreEqual("[-1]", variable1.GetOPath(), "variable1.GetOPath()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableIntIndexPart(variable1, 0, -1);
		}

		[Test]
		public void Parse_VariableWithIntIndexersWithWhitespace_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse("Variable1 [ 1 ] [ 2 ] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual("Variable1", variable1.Name, "variable1.Name");
			Assert.AreEqual("[1][2]", variable1.GetOPath(), "variable1.GetOPath()");

			Assert.AreEqual(2, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableIntIndexPart(variable1, 0, 1);
			AssertVariableIntIndexPart(variable1, 1, 2);
		}

		[Test]
		public void Parse_VariableWithStringKeyDoubleQuotes_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"Variable1[""Key's""] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual(@"Variable1[""Key's""]", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableStringKeyPart(variable1, 0, @"Key's");
		}

		[Test]
		public void Parse_VariableWithStringKeySingleQuotes_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"Variable1['Key""s'] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual(@"Variable1['Key""s']", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableStringKeyPart(variable1, 0, @"Key""s");
		}

		[Test]
		public void Parse_VariableWithStringKeyWithWhitespace_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"Variable1[ ""Key"" ] = 1");

			Assert.AreEqual("$Var1 = 1", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual(@"Variable1[""Key""]", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(1, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableStringKeyPart(variable1, 0, @"Key");
		}

		[Test]
		public void Parse_VariableWithMultipleMembers_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(@"listDictionary[""Key1""][1].Trim().Length != 6");

			Assert.AreEqual("$Var1 != 6", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(1, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual(@"listDictionary[""Key1""][1].Trim().Length", variable1.GetReference(), "variable1.GetReference()");

			Assert.AreEqual(4, variable1.OPathParts.Length, "variable1.OPathParts.Length");
			AssertVariableStringKeyPart(variable1, 0, "Key1");
			AssertVariableIntIndexPart(variable1, 1, 1);
			AssertVariableMethodPart(variable1, 2, "Trim");
			AssertVariablePropertyPart(variable1, 3, "Length");
		}

		[Test]
		public void Parse_MultipleVariablesWithMultipleMembers_ExpectedResult()
		{
			OPathParseResult parseResult = OPathParser.Parse(
				@"(listDictionary [ ""Key1"" ] [ 0 ] . Trim() . Length * 2)"
				+ @" != "
				+ @"(listDictionary[""Key2""][0].Trim().Length + listDictionary[""Key3""][0].Trim().Length)");

			Assert.AreEqual("($Var1 * 2) != ($Var2 + $Var3)", parseResult.GetXPath(), "parseResult.GetXPath()");
			Assert.AreEqual(3, parseResult.GetVariables().Length, "parseResult.GetVariables().Length");

			OPathVariable variable1 = GetVariable("Var1", parseResult.GetVariables());
			Assert.AreEqual(@"listDictionary[""Key1""][0].Trim().Length", variable1.GetReference(), "variable1.GetReference()");
			Assert.AreEqual(4, variable1.OPathParts.Length, "variable1.OPathParts.Length");

			OPathVariable variable2 = GetVariable("Var2", parseResult.GetVariables());
			Assert.AreEqual(@"listDictionary[""Key2""][0].Trim().Length", variable2.GetReference(), "variable3.GetReference()");
			Assert.AreEqual(4, variable1.OPathParts.Length, "variable2.OPathParts.Length");

			OPathVariable variable3 = GetVariable("Var3", parseResult.GetVariables());
			Assert.AreEqual(@"listDictionary[""Key3""][0].Trim().Length", variable3.GetReference(), "variable3.GetReference()");
			Assert.AreEqual(4, variable1.OPathParts.Length, "variable3.OPathParts.Length");
		}

		[Test]
		public void Parse_UnclosedIndexerPreValue_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed indexer starting at position 11"),
                () =>
                {
                    OPathParser.Parse("IndexedVar[");
                });
        }

        [Test]
        public void Parse_UnclosedIndexerPostValue_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed indexer starting at position 11"),
                () =>
                {
                    OPathParser.Parse("IndexedVar['Key'");
                });
        }

        [Test]
        public void Parse_UnclosedIntIndexer_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed indexer starting at position 11"),
                () =>
                {
                    OPathParser.Parse("IndexedVar[616");
                });
        }

        [Test]
        public void Parse_InvalidCharInIndexerPreValue_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Invalid character in indexer starting at position 11"),
                () =>
                {
                    OPathParser.Parse("IndexedVar[.616]");
                });
        }

        [Test]
        public void Parse_InvalidCharInIndexerPostValue_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Invalid character in indexer starting at position 11"),
                () =>
                {
                    OPathParser.Parse("IndexedVar[6.16]");
                });
        }

        [Test]
        public void Parse_NonEmptyMethod_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Non-empty method starting at position 10"),
                () =>
                {
                    OPathParser.Parse("MethodVar.Method(616)");
                });
        }

        [Test]
        public void Parse_UnclosedMethod_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed method starting at position 10"),
                () =>
                {
                    OPathParser.Parse("MethodVar.Method(");
                });
        }

        [Test]
        public void Parse_UnclosedStringKey_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed string key starting at position 12"),
                () =>
                {
                    OPathParser.Parse("IndexedVar['Key");
                });
        }

        [Test]
        public void Parse_UnclosedString_ExpectedException()
		{
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed string starting at position 1"),
                () =>
                {
			        OPathParser.Parse("'String");
                });
		}

		#endregion

		#region Private methods

		private static OPathVariable GetVariable(string xpathName, OPathVariable[] variables)
		{
			OPathVariable variable =
				Array.Find(variables, delegate(OPathVariable v) { return v.XPathName == xpathName; });
			return variable;
		}

		private static void AssertVariableIntIndexPart(OPathVariable variable, int partIndex, int expectedIndex)
		{
			OPathIntIndexerPart part = variable.OPathParts[partIndex] as OPathIntIndexerPart;

			Assert.IsNotNull(part, "Part " + partIndex);

			Assert.AreEqual(expectedIndex, part.IndexValue, "part.IndexValue");
		}

		private static void AssertVariableStringKeyPart(OPathVariable variable, int partIndex, string expectedKeyValue)
		{
			OPathStringKeyPart part = variable.OPathParts[partIndex] as OPathStringKeyPart;

			Assert.IsNotNull(part, "Part " + partIndex);

			Assert.AreEqual(expectedKeyValue, part.KeyValue, "part.KeyValue");
		}

		private static void AssertVariablePropertyPart(OPathVariable variable, int partIndex, string expectedPropertyName)
		{
			OPathPropertyPart part = variable.OPathParts[partIndex] as OPathPropertyPart;

			Assert.IsNotNull(part, "Part " + partIndex);

			Assert.AreEqual(expectedPropertyName, part.PropertyName, "part.PropertyName");
		}

		private static void AssertVariableMethodPart(OPathVariable variable, int partIndex, string expectedMethodName)
		{
			OPathMethodPart part = variable.OPathParts[partIndex] as OPathMethodPart;

			Assert.IsNotNull(part, "Part " + partIndex);

			Assert.AreEqual(expectedMethodName, part.MethodName, "part.MethodName");
		}

		#endregion
	}
}
