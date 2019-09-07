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
using NUnit.Framework;

namespace OPath.TestSuite
{
	[TestFixture]
	public class OPathCoreTests
	{
		#region ExceptionThrower class

		private class ExceptionThrower
		{
			public object Property
			{
				get { throw new InvalidOperationException("Property exception"); }
			}

			public object Method()
			{
				throw new InvalidOperationException("Method exception");
			}
		}

		#endregion

		#region Private member variables

		private const bool TRUE_BOOL = true;
		private const bool FALSE_BOOL = false;

		private const int ZERO_INT = 0;
		private const int NON_ZERO_INT = 616;

		private const double ZERO_DOUBLE = 0;
		private const double NON_ZERO_DOUBLE = 6.16;

		private const string EMPTY_STRING = "";
		private const string NON_EMPTY_STRING = "Lucifer";

		private static readonly DateTime MIN_DATE_TIME = DateTime.MinValue;
		private static readonly DateTime MAX_DATE_TIME = DateTime.MaxValue;
		private static readonly DateTime NON_MIN_DATE_TIME = new DateTime(1969, 8, 31);

		private static readonly int[] EMPTY_INT_ARRAY = new int[] { };
		private static readonly int[] NON_EMPTY_INT_ARRAY = new int[] { 0, 1, 2, 3 };
		private static readonly int[][] NON_EMPTY_2D_INT_ARRAY =
			new int[][] { 
				new int[] { 00, 01, 02, 03 },
				new int[] { 10, 11, 12, 13 },
			};

		private static readonly List<int> EMPTY_INT_LIST = new List<int>();
		private static readonly List<int> NON_EMPTY_INT_LIST = new List<int>(new int[] { 0, 1, 2, 3 });

		private static readonly Dictionary<string, string> EMPTY_DICTIONARY;
		private static readonly Dictionary<string, string> NON_EMPTY_DICTIONARY;
		private static readonly Dictionary<string, List<string>> NON_EMPTY_LIST_DICTIONARY;

		private OPathDocument m_OPathDocument = new OPathDocument();

		static OPathCoreTests()
		{
			EMPTY_DICTIONARY = new Dictionary<string, string>();

			NON_EMPTY_DICTIONARY = new Dictionary<string, string>();
			NON_EMPTY_DICTIONARY.Add("Key1", "Value1");
			NON_EMPTY_DICTIONARY.Add("Key2", "Value2");
			NON_EMPTY_DICTIONARY.Add("Key3", "Value3");

			NON_EMPTY_LIST_DICTIONARY = new Dictionary<string, List<string>>();
			NON_EMPTY_LIST_DICTIONARY.Add("Key1", new List<string>(new string[] { "Value1" }));
			NON_EMPTY_LIST_DICTIONARY.Add("Key2", new List<string>(new string[] { "Value2" }));
			NON_EMPTY_LIST_DICTIONARY.Add("Key3", new List<string>(new string[] { "Value3" }));
		}

		#endregion

		#region SetUp/TearDown

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			m_OPathDocument.Add("TrueBool", TRUE_BOOL);
			m_OPathDocument.Add("FalseBool", FALSE_BOOL);

			m_OPathDocument.Add("ZeroInt", ZERO_INT);
			m_OPathDocument.Add("NonZeroInt", NON_ZERO_INT);

			m_OPathDocument.Add("ZeroDouble", ZERO_DOUBLE);
			m_OPathDocument.Add("NonZeroDouble", NON_ZERO_DOUBLE);

			m_OPathDocument.Add("EmptyString", EMPTY_STRING);
			m_OPathDocument.Add("NonEmptyString", NON_EMPTY_STRING);

			m_OPathDocument.Add("MinDateTime", MIN_DATE_TIME);
			m_OPathDocument.Add("MaxDateTime", MIN_DATE_TIME);
			m_OPathDocument.Add("NonMinDateTime", NON_MIN_DATE_TIME);

			m_OPathDocument.Add("EmptyIntArray", EMPTY_INT_ARRAY);
			m_OPathDocument.Add("NonEmptyIntArray", NON_EMPTY_INT_ARRAY);
			m_OPathDocument.Add("NonEmpty2dIntArray", NON_EMPTY_2D_INT_ARRAY);

			m_OPathDocument.Add("EmptyIntList", EMPTY_INT_LIST);
			m_OPathDocument.Add("NonEmptyIntList", NON_EMPTY_INT_LIST);

			m_OPathDocument.Add("EmptyDictionary", EMPTY_DICTIONARY);
			m_OPathDocument.Add("NonEmptyDictionary", NON_EMPTY_DICTIONARY);
			m_OPathDocument.Add("NonEmptyListDictionary", NON_EMPTY_LIST_DICTIONARY);

			m_OPathDocument.Add("NullObject", null);

			m_OPathDocument.Add("ExceptionThrower", new ExceptionThrower());
		}

		#endregion

		#region Tests

		[Test]
		public void Evaluate_BoolVariables_ExpectedResult()
		{
			AssertExpressionEquals(TRUE_BOOL, "TrueBool");
			AssertExpressionEquals(FALSE_BOOL, "FalseBool");

			AssertExpressionEquals(true, @"TrueBool = true()");
			AssertExpressionEquals(false, @"TrueBool = false()");

			AssertExpressionEquals(TRUE_BOOL, "boolean(TrueBool)");
			AssertExpressionEquals(FALSE_BOOL, "boolean(FalseBool)");

			AssertExpressionEquals(true, @"not(FalseBool)");
		}

		[Test]
		public void Evaluate_IntVariables_ExpectedResult()
		{
			AssertExpressionEquals(ZERO_INT, "ZeroInt");
			AssertExpressionEquals(NON_ZERO_INT, "NonZeroInt");

			AssertExpressionEquals(true, "ZeroInt = 0");
			AssertExpressionEquals(false, "ZeroInt = 1");

			AssertExpressionEquals(true, "ZeroInt < NonZeroInt");
			AssertExpressionEquals(false, "ZeroInt > NonZeroInt");
		}

		[Test]
		public void Evaluate_DoubleVariables_ExpectedResult()
		{
			AssertExpressionEquals(ZERO_DOUBLE, "ZeroDouble");
			AssertExpressionEquals(NON_ZERO_DOUBLE, "NonZeroDouble");

			AssertExpressionEquals(true, "NonZeroDouble = " + NON_ZERO_DOUBLE);
			AssertExpressionEquals(false, "NonZeroDouble = 0");

			AssertExpressionEquals(true, "ZeroDouble < NonZeroDouble");
			AssertExpressionEquals(false, "ZeroDouble > NonZeroDouble");
		}

		[Test]
		public void Evaluate_StringVariables_ExpectedResult()
		{
			AssertExpressionEquals(EMPTY_STRING, "EmptyString");
			AssertExpressionEquals(NON_EMPTY_STRING, "NonEmptyString");

			AssertExpressionEquals(true, @"NonEmptyString = """ + NON_EMPTY_STRING + @"""");
			AssertExpressionEquals(false, @"NonEmptyString = """"");
			AssertExpressionEquals(false, @"NonEmptyString = ""NonEmptyString""");
		}
		
		[Test]
		public void Evaluate_DateTimeVariables_ExpectedResult()
		{
			AssertExpressionEquals(MIN_DATE_TIME, "MinDateTime");
			AssertExpressionEquals(NON_MIN_DATE_TIME, "NonMinDateTime");

			AssertExpressionEquals(true, "NonMinDateTime = NonMinDateTime");
			AssertExpressionEquals(false, "MinDateTime = NonMinDateTime");

			AssertExpressionEquals(true, "MinDateTime < NonMinDateTime");
			AssertExpressionEquals(false, "MinDateTime > NonMinDateTime");

			AssertExpressionEquals(true, "MinDateTime = DateTimeMinValue");
			AssertExpressionEquals(true, "MaxDateTime = DateTimeMinValue");
			AssertExpressionEquals(true, "MinDateTime < DateTimeNow");
			AssertExpressionEquals(true, "DateTimeToday < DateTimeNow");
		}

		[Test]
		public void Evaluate_DateTimeCustomFunctions_ExpectedResult()
		{
			AssertExpressionEquals(true, "MinDateTime = date-time:min-value()");
			AssertExpressionEquals(true, "MaxDateTime = date-time:min-value()");
			AssertExpressionEquals(true, "MinDateTime < date-time:now()");
			AssertExpressionEquals(true, "date-time:today() < date-time:now()");

			AssertExpressionEquals(true, "date-time:add-days(date-time:now(), 1) > date-time:now()");
		}

		[Test]
		public void Evaluate_SingleVariableWithWhitespace_ExpectedResult()
		{
			AssertExpressionEquals(NON_EMPTY_STRING.Trim().Length, " NonEmptyString . Trim ( ) . Length");
			AssertExpressionEquals(NON_EMPTY_DICTIONARY["Key1"].Trim().Length, @" NonEmptyDictionary [ ""Key1"" ] . Trim ( ) . Length");
		}

		[Test]
		public void Evaluate_LogicOperators_ExpectedResult()
		{
			AssertExpressionEquals(true, "(0 < 1) or (0 > 1)");
			AssertExpressionEquals(false, "(0 < 1) and (0 > 1)");
		}

		[Test]
		public void Evaluate_BooleanFunctions_ExpectedResult()
		{
			AssertExpressionEquals(true, "true()");
			AssertExpressionEquals(false, "false()");
			AssertExpressionEquals(false, "boolean(0)");
			AssertExpressionEquals(true, "not(false())");
		}

		[Test]
		public void Evaluate_StringFunctions_ExpectedResult()
		{
			AssertExpressionEquals(true, @"concat(""Hello"", ""Dolly"") = ""HelloDolly""");
			AssertExpressionEquals(true, @"contains(""Hello"", ""Hell"")");
			AssertExpressionEquals(true, @"normalize-space(""Hello  Dolly"") = ""Hello Dolly""");
			AssertExpressionEquals(true, @"starts-with(""Hello"", ""Hell"")");
			AssertExpressionEquals(true, @"string(0) = ""0""");
			AssertExpressionEquals(true, @"string-length(""Hello"") = 5");
			AssertExpressionEquals(true, @"substring(""Hello"", 1, 4) = ""Hell""");
			AssertExpressionEquals(true, @"substring-after(""Hello"", ""H"") = ""ello""");
			AssertExpressionEquals(true, @"substring-before(""Hello"", ""o"") = ""Hell""");
			AssertExpressionEquals(true, @"translate(""Hello"", ""o"", ""O"") = ""HellO""");
		}

		[Test]
		public void Evaluate_NumberOperatorsAndFunctions_ExpectedResult()
		{
			AssertExpressionEquals(true, "10 div 5 = 2");
			AssertExpressionEquals(true, "10 mod 3 = 1");
			AssertExpressionEquals(true, "ceiling(3.14) = 4");
			AssertExpressionEquals(true, "floor(3.14) = 3");
			AssertExpressionEquals(true, @"number(""616"") = 616");
			AssertExpressionEquals(true, "round(5.5) = 6");
		}

		[Test]
		public void Evaluate_VariablesInDifferentPositions_ExpectedResult()
		{
			AssertExpressionEquals(true, "ZeroInt + NonZeroInt = NonZeroInt + ZeroInt");
		}

		[Test]
		public void Evaluate_ArrayIndexers_ExpectedResult()
		{
			AssertExpressionEquals(true, "NonEmptyIntArray[0] = 0");
			AssertExpressionEquals(true, "3 = NonEmptyIntArray[3]");
			AssertExpressionEquals(true, "NonEmpty2dIntArray[0][0] = 00");
			AssertExpressionEquals(true, "13 = NonEmpty2dIntArray[1][3]");

			AssertExpressionEquals(true, "NonEmptyIntArray [0] = 0");
			AssertExpressionEquals(true, "3 = NonEmptyIntArray [3]");
			AssertExpressionEquals(true, "NonEmpty2dIntArray [0] [0] = 00");
			AssertExpressionEquals(true, "13 = NonEmpty2dIntArray [1] [3]");

			// TODO: Allow [0, 0] syntax?
		}

		[Test]
		public void Evaluate_ListIndexers_ExpectedResult()
		{
			AssertExpressionEquals(true, "NonEmptyIntList[0] = 0");
			AssertExpressionEquals(true, "3 = NonEmptyIntList[3]");
		}

		[Test]
		public void Evaluate_DictionaryStringIndexers_ExpectedResult()
		{
			AssertExpressionEquals(true, @"NonEmptyDictionary[""Key1""] = ""Value1""");
			AssertExpressionEquals(true, @"""Value3"" = NonEmptyDictionary[""Key3""]");
		}

		[Test]
		public void Evaluate_EmptyMethod_ExpectedResult()
		{
			AssertExpressionEquals(true, "NonEmptyString.Trim() = NonEmptyString.Trim()");
		}

		[Test]
		public void Evaluate_MultipleMembers_ExpectedResult()
		{
			AssertExpressionEquals(true, "NonEmptyString.Trim().Length = NonEmptyString.Trim().Length");
			AssertExpressionEquals(true, @"NonEmptyDictionary[""Key1""].Trim().Length = NonEmptyDictionary[""Key1""].Trim().Length");
			AssertExpressionEquals(true, "NonEmptyIntList.ToArray()[0].ToString().Length = NonEmptyIntList.ToArray()[0].ToString().Length");
			AssertExpressionEquals(true, @"NonEmptyListDictionary[""Key1""][0].Trim().Length = NonEmptyListDictionary[""Key1""][0].Trim().Length");
		}

		[Test]
		public void Evaluate_NullSourceObjects_ExpectedResult()
		{
			bool actualValue = 
				(bool)OPathNavigator.Evaluate(null, OPathExpression.Compile("true()"));
			Assert.AreEqual(true, actualValue, "true()");
		}

		[Test]
		public void Evaluate_CustomKeywords_ExpectedResult()
		{
			AssertExpressionEquals(true, "10 / 5 = 2");
			AssertExpressionEquals(true, "10 % 5 = 0");
			AssertExpressionEquals("&|!/%", @"""&|!/%""");

			AssertExpressionEquals(true, "true() & true()");
			AssertExpressionEquals(true, "true() | false()");
			AssertExpressionEquals(true, "!(false())");
			AssertExpressionEquals(true, "(false() != true())");

			AssertExpressionEquals(true, "true() == true()");

			AssertExpressionEquals(true, "true");
			AssertExpressionEquals(false, "false");
			AssertExpressionEquals(true, "!(false)");
			AssertExpressionEquals(false, "true = false");
			AssertExpressionEquals(true, "!(false = true)");
			AssertExpressionEquals(true, "!(false) = true");
			AssertExpressionEquals(true, @"true and true");
			AssertExpressionEquals(false, @"false or false");
			AssertExpressionEquals(true, @"true and TrueBool");
			AssertExpressionEquals(false, @"false and TrueBool");
		}

		[Test]
		public void Evaluate_TestingForNull_ExpectedResult()
		{
			AssertExpressionEquals(true, "NullObject = null");
		}

        [Test]
        public void Evaluate_UnspecifiedObject_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"UnspecifiedObject"),
                () =>
                {
                    OPathNavigator.Evaluate(new OPathDocument(), OPathExpression.Compile("UnspecifiedObject = 0"));
                });
        }

        [Test]
        public void Evaluate_NonExistantProperty_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyString\.NonExistantProperty.*EmptyString.*System\.String.*NonExistantProperty"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument,
                        OPathExpression.Compile("EmptyString.NonExistantProperty = 0"));
                });
        }

        [Test]
        public void Evaluate_NonExistantEmptyMethod_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyString\.NonExistantEmptyMethod().*EmptyString.*System\.String.*NonExistantEmptyMethod"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument,
                        OPathExpression.Compile("EmptyString.NonExistantEmptyMethod() = 0"));
                });
        }

        [Test]
        public void Evaluate_NonExistantStringIndexer_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyString\[""StringKey""\].*EmptyString.*System\.String.*string indexer"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"EmptyString[""StringKey""] = 0"));
                });
        }

        [Test]
        public void Evaluate_NonExistantIntIndexer_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyString\[0\].*EmptyString.*System\.String.*int indexer"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"EmptyString[0] = 0"));
                });
        }

        [Test]
        public void Evaluate_ListIndexOutOfBounds_ExpectedExpectation()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyIntList\[616\].*EmptyIntList.*616.*index out of range"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile("EmptyIntList[616] = 0"));
                });
        }

        [Test]
        public void Evaluate_ArrayIndexOutOfBounds_ExpectedExpectation()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyIntArray\[616\].*EmptyIntArray.*616.*index out of range"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile("EmptyIntArray[616] = 0"));
                });
        }

        [Test]
        public void Evaluate_NonExistantStringIndex_ExpectedExpectation()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"EmptyDictionary\[""Lucifer""\].*EmptyDictionary.*Lucifer.*key not found"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"EmptyDictionary[""Lucifer""] = 0"));
                });
        }

        [Test]
        public void Evaluate_UnclosedString_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Unclosed string"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"NonEmptyString = X"""));
                });
        }

        [Test]
        public void Evaluate_UnclosedIndexer_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"Invalid character in indexer"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"NonEmptyIntList[0 = 0"));
                });
        }

        [Test]
        public void Evaluate_UnclosedBracket_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"\(true\(\) = false\(\).*has an invalid token"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"(true() = false()"));
                });
        }

        [Test]
        public void Evaluate_NullReferenceInObjectPath_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"NullObject.*null"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"NullObject.Property = null"));
                });
        }

        [Test]
		public void Evaluate_NullReferenceAsNull_ExpectedResult()
		{
			AssertExpressionEquals(true, @"NullObject.Property = null", OPathOptions.ReturnDefaultForNull);
		}

		[Test]
		public void Evaluate_NullReferenceAsDefault_ExpectedResult()
		{
			AssertExpressionEquals(true, @"NullObject.Property = 616", OPathOptions.ReturnDefaultForNull, 616);
		}

        [Test]
        public void Evaluate_SingleValuePropertyException_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"ExceptionThrower\.Property.*Property exception"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"ExceptionThrower.Property"));
                });
        }

        [Test]
        public void Evaluate_SingleValueMethodException_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"ExceptionThrower\.Method\(\).*Method exception"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"ExceptionThrower.Method()"));
                });
        }

        [Test]
		public void Evaluate_SingleValuePropertyExceptionAsDefault_ExpectedResult()
		{
			AssertExpressionEquals(616, @"ExceptionThrower.Property", OPathOptions.ReturnDefaultIfException, 616);
		}

		[Test]
		public void Evaluate_SingleValueMethodExceptionAsDefault_ExpectedResult()
		{
			AssertExpressionEquals(616, @"ExceptionThrower.Method()", OPathOptions.ReturnDefaultIfException, 616);
		}

		[Test]
		public void Evaluate_SingleValuePropertyExceptionAsNull_ExpectedResult()
		{
			AssertExpressionEquals(null, @"ExceptionThrower.Property", OPathOptions.ReturnDefaultIfException);
		}

		[Test]
		public void Evaluate_SingleValueMethodExceptionAsNull_ExpectedResult()
		{
			AssertExpressionEquals(null, @"ExceptionThrower.Method()", OPathOptions.ReturnDefaultIfException);
		}

        [Test]
        public void Evaluate_ExpressionPropertyException_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"ExceptionThrower\.Property.*Property exception"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"ExceptionThrower.Property = null"));
                });
        }

        [Test]
        public void Evaluate_ExpressionMethodException_ExpectedException()
        {
            Assert.Throws(Is.TypeOf<OPathException>()
                    .And.Message.Matches(@"ExceptionThrower\.Method\(\).*Method exception"),
                () =>
                {
                    OPathNavigator.Evaluate(m_OPathDocument, OPathExpression.Compile(@"ExceptionThrower.Method() = null"));
                });
        }

        [Test]
		public void Evaluate_ExpressionPropertyExceptionAsNull_ExpectedResult()
		{
			AssertExpressionEquals(true, @"ExceptionThrower.Property = null", OPathOptions.ReturnDefaultIfException);
		}

		[Test]
		public void Evaluate_ExpressionMethodExceptionAsNull_ExpectedResult()
		{
			AssertExpressionEquals(true, @"ExceptionThrower.Method() = null", OPathOptions.ReturnDefaultIfException);
		}

		[Test]
		public void Evaluate_ExpressionPropertyExceptionAsDefault_ExpectedResult()
		{
			AssertExpressionEquals(true, @"ExceptionThrower.Property = 616", OPathOptions.ReturnDefaultIfException, 616);
		}

		[Test]
		public void Evaluate_ExpressionMethodExceptionAsDefault_ExpectedResult()
		{
			AssertExpressionEquals(true, @"ExceptionThrower.Method() = 616", OPathOptions.ReturnDefaultIfException, 616);
		}

		#endregion

		#region Private methods

		private void AssertExpressionEquals(object expectedValue, string expression)
		{
			AssertExpressionEquals(expectedValue, expression, OPathOptions.Defaults);
		}


		private void AssertExpressionEquals(object expectedValue, string expression,
			OPathOptions opathOptions)
		{
			AssertExpressionEquals(expectedValue, expression, opathOptions, null);
		}


		private void AssertExpressionEquals(object expectedValue, string expression,
			OPathOptions opathOptions, object defaultValue)
		{
			OPathExpression opathExpression = OPathExpression.Compile(expression);

			object actualValue =
				OPathNavigator.Evaluate(m_OPathDocument, opathExpression, opathOptions, defaultValue, 
					(string message) => Console.WriteLine(message));

			Assert.AreEqual(expectedValue, actualValue, expression);
		}

		#endregion
	}
}
