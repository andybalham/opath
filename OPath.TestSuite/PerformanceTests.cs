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
using OPath.TestSuite.Utils;

namespace OPath.TestSuite
{
	//[TestFixture]
	public class PerformanceTests
	{
		private const int EVALUATION_COUNT = 1000;

		private const string EXPRESSION =
			//@"listDictionary.Count > 0";
			//@"listDictionary[""Key1""] != null";
			@"listDictionary[""Key1""][0].Length > 0";
			//@"(listDictionary[""Key1""][0].Trim().Length * 2)"
			//+ @" != "
			//+ @"(listDictionary[""Key2""][0].Trim().Length + listDictionary[""Key3""][0].Trim().Length)";

		private Dictionary<string, List<string>> m_ListDictionary = new Dictionary<string, List<string>>();

		[SetUp]
		public void SetUp()
		{
			m_ListDictionary = new Dictionary<string, List<string>>();
			m_ListDictionary.Add("Key1", new List<string>(new string[] { "Value1" }));
			m_ListDictionary.Add("Key2", new List<string>(new string[] { "Value2" }));
			m_ListDictionary.Add("Key3", new List<string>(new string[] { "Value3" }));
		}

		[Test]
		public void TestNativePerformance()
		{
			HiPerfTimer nativeTimer = new HiPerfTimer();
			nativeTimer.Start();

			for (int i = 0; i < EVALUATION_COUNT; i++)
			{
				bool result =
					//m_ListDictionary.Count > 0;
					//m_ListDictionary["Key1"] != null;
					m_ListDictionary["Key1"][0].Length == 6;
					//(m_ListDictionary["Key1"][0].Trim().Length * 2)
					//    == (m_ListDictionary["Key2"][0].Trim().Length + m_ListDictionary["Key3"][0].Trim().Length);
			}

			nativeTimer.Stop();

			Console.WriteLine("Native performance {0} evaluations in {1:0.000} millis",
				EVALUATION_COUNT, (nativeTimer.Duration * 1000));
		}

		[Test]
		public void TestOPathPerformance()
		{
			HiPerfTimer rawTimer = new HiPerfTimer();
			rawTimer.Start();

			for (int i = 0; i < EVALUATION_COUNT; i++)
			{
				OPathExpression opathExpression = OPathExpression.Compile(EXPRESSION);

				OPathDocument opathDocument = new OPathDocument();
				opathDocument.Add("listDictionary", m_ListDictionary);

				OPathNavigator opathNavigator = OPathNavigator.CreateNavigator(opathDocument);

				opathNavigator.Evaluate(opathExpression);
			}

			rawTimer.Stop();

			Console.WriteLine("Uncompiled OPath performance {0} evaluations in {1:0.000} millis",
				EVALUATION_COUNT, (rawTimer.Duration * 1000));

			HiPerfTimer cachedTimer = new HiPerfTimer();
			cachedTimer.Start();

			OPathExpression cachedOPathExpression = OPathExpression.Compile(EXPRESSION);

			for (int i = 0; i < EVALUATION_COUNT; i++)
			{
				OPathDocument opathDocument = new OPathDocument();
				opathDocument.Add("listDictionary", m_ListDictionary);

				OPathNavigator opathNavigator = OPathNavigator.CreateNavigator(opathDocument);

				opathNavigator.Evaluate(cachedOPathExpression);
			}

			cachedTimer.Stop();

			Console.WriteLine("Compiled OPath performance {0} evaluations in {1:0.000} millis",
				EVALUATION_COUNT, (cachedTimer.Duration * 1000));
		}

		[Test]
		public void TestParsing()
		{
			HiPerfTimer handRolledTimer = new HiPerfTimer();
			handRolledTimer.Start();

			for (int i = 0; i < EVALUATION_COUNT; i++)
			{
				OPathParser.Parse(EXPRESSION);
			}

			handRolledTimer.Stop();

			Console.WriteLine("Hand-rolled parsing did {0} in {1:0.000} millis",
				EVALUATION_COUNT, (handRolledTimer.Duration * 1000));
		}
	}
}
