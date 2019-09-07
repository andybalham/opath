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
	public class ExampleTests
	{
		[Test]
		public void HelloOPathNETExample()
		{
			var expression = OPathExpression.Compile("MyVariable = 'OPath.NET'");

			var document = new OPathDocument();
			document.Add("MyVariable", "OPath.NET");

			var navigator = OPathNavigator.CreateNavigator(document);

			var result = navigator.Evaluate(expression);

			Console.WriteLine(result);
		}

		[Test]
		public void OPathOptionsExample()
		{
			var expression = OPathExpression.Compile("MyVariable.Length = 0");

			var document = new OPathDocument();
			document.Add("MyVariable", null);

			var navigator = OPathNavigator.CreateNavigator(document);

			try
			{
				var result = navigator.Evaluate(expression);

				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			try
			{
				var result = navigator.Evaluate(expression, OPathOptions.ReturnDefaultForNull);

				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			try
			{
				var result = navigator.Evaluate(expression, OPathOptions.ReturnDefaultForNull, 0);

				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[Test]
		public void LoggingExample()
		{
			var expression = OPathExpression.Compile("StringArray[0].Length > Int");

			var document = new OPathDocument();
			document.Add("StringArray", new string[] { "String" });
			document.Add("Int", 0);

			var navigator = OPathNavigator.CreateNavigator(document);

			var result =
				navigator.Evaluate(expression, OPathOptions.Defaults, null, 
					new OPathNavigator.ValueLogger(delegate(string message)
					{
						Console.WriteLine(message);
					}));

			Console.WriteLine(result);
		}

		[Test]
		public void CustomFunctionExample()
		{
			var expression = OPathExpression.Compile("my-prefix:my-function(String, Int)");

			var document = new OPathDocument();
			document.Add("String", "String" );
			document.Add("Int", 616);

			var navigator = OPathNavigator.CreateNavigator(document);

			navigator.RegisterCustomFunction("my-prefix", "my-function", 
				new OPathCustomFunction(delegate(object[] args) 
					{
						string[] stringArray = Array.ConvertAll<object, string>(args, delegate(object o) { return o + ""; });
						return string.Join(",", stringArray);
					}));

			var result = navigator.Evaluate(expression);

			Console.WriteLine(result);
		}
	}
}
