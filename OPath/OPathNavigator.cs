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
using System.Xml;
using System.Xml.XPath;

namespace OPath
{
	public class OPathNavigator
	{
		#region Public delegates

		public delegate void ValueLogger(string message);

		#endregion

		#region Member variables

		private const string NULL_MESSAGE_STRING = "<null>";

		private Dictionary<Type, OPathCustomTypeConverter> m_CustomerTypeConverters =
			new Dictionary<Type, OPathCustomTypeConverter>();

		private Dictionary<string, OPathCustomFunction> m_CustomFunctions = 
			new Dictionary<string, OPathCustomFunction>();

		private OPathDocument m_OPathDocument;

		#endregion

		#region Factory methods

		public static OPathNavigator CreateNavigator(OPathDocument opathDocument)
		{
			OPathNavigator opathNavigator = new OPathNavigator(opathDocument);
			return opathNavigator;
		}

		#endregion

		#region Constructors

		private OPathNavigator(OPathDocument opathDocument)
		{
			RegisterCustomTypeConverters();

			RegisterCustomFunctions();

			m_OPathDocument = opathDocument;
		}

		#endregion

		#region Public methods

		public static object Evaluate(OPathDocument opathDocument, OPathExpression opathExpression)
		{
			OPathNavigator targetNavigator = CreateNavigator(opathDocument);

			object opathValue = targetNavigator.Evaluate(opathExpression, OPathOptions.Defaults);

			return opathValue;
		}


		public static object Evaluate(OPathDocument opathDocument, OPathExpression opathExpression,
			OPathOptions opathOptions)
		{
			OPathNavigator targetNavigator = CreateNavigator(opathDocument);

			object opathValue = targetNavigator.Evaluate(opathExpression, opathOptions, null);

			return opathValue;
		}


		public static object Evaluate(OPathDocument opathDocument, OPathExpression opathExpression,
			OPathOptions opathOptions, object defaultValue)
		{
			OPathNavigator targetNavigator = CreateNavigator(opathDocument);

			object opathValue = targetNavigator.Evaluate(opathExpression, opathOptions, defaultValue, null);

			return opathValue;
		}


		public static object Evaluate(OPathDocument opathDocument, OPathExpression opathExpression,
			OPathOptions opathOptions, object defaultValue, ValueLogger valueLogger)
		{
			OPathNavigator targetNavigator = CreateNavigator(opathDocument);

			object opathValue = 
				targetNavigator.Evaluate(opathExpression, opathOptions, defaultValue, valueLogger);

			return opathValue;
		}


		public object Evaluate(OPathExpression opathExpression)
		{
			object opathValue = Evaluate(opathExpression, OPathOptions.Defaults);

			return opathValue;
		}


		public object Evaluate(OPathExpression opathExpression, OPathOptions opathOptions)
		{
			object opathValue = Evaluate(opathExpression, opathOptions, null);

			return opathValue;
		}


		public object Evaluate(OPathExpression opathExpression, OPathOptions opathOptions,
			object defaultValue)
		{
			object opathValue = Evaluate(opathExpression, opathOptions, defaultValue, null);

			return opathValue;
		}


		public object Evaluate(OPathExpression opathExpression, OPathOptions opathOptions,
			object defaultValue, ValueLogger valueLogger)
		{
			object evaluatedExpression;

			if (m_OPathDocument == null)
			{
				m_OPathDocument = new OPathDocument();
			}

			if (opathExpression.IsSingleVariable())
			{
				evaluatedExpression = 
					EvaluateSingleVariable(opathExpression, opathOptions, defaultValue, valueLogger);
			}
			else
			{
				evaluatedExpression = 
					EvaluateOPathExpression(opathExpression, opathOptions, defaultValue, valueLogger);
			}

			return evaluatedExpression;
		}

		/// <summary>
		/// Register the custom type converter specified, overriding any previous converter for that type.
		/// </summary>
		/// <param name="customType">Type of the custom.</param>
		/// <param name="customTypeConverter">The custom type converter.</param>
		public void RegisterCustomTypeConverter(Type customType, OPathCustomTypeConverter customTypeConverter)
		{
			m_CustomerTypeConverters[customType] = customTypeConverter;
		}

		/// <summary>
		/// Register the custom function specified, overriding any previous functions with the same details.
		/// </summary>
		/// <param name="prefix">The function prefix.</param>
		/// <param name="name">The function name.</param>
		/// <param name="customFunction">The custom function implementation.</param>
		public void RegisterCustomFunction(string prefix, string name, OPathCustomFunction customFunction)
		{
			string customFunctionKey = OPathXsltContextFunction.GetCustomFunctionKey(prefix, name);

			m_CustomFunctions[customFunctionKey] = customFunction;
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Gets the string used to represent null.
		/// </summary>
		/// <returns>The string used to represent null</returns>
		protected virtual string GetNullString()
		{
			return "__null__";
		}

		/// <summary>
		/// Registers the custom type converters supported by this navigator.
		/// </summary>
		protected virtual void RegisterCustomTypeConverters()
		{
			// Add a custom type converter to allow the comparison of DateTimes
			RegisterCustomTypeConverter(typeof(DateTime), OPathDateTime.Convert);
		}

		/// <summary>
		/// Registers the custom functions supported by this navigator.
		/// </summary>
		protected virtual void RegisterCustomFunctions()
		{
			// Add custom date functions
			RegisterCustomFunction(OPathDateTime.FunctionPrefix, "now", OPathDateTime.Now);
			RegisterCustomFunction(OPathDateTime.FunctionPrefix, "today", OPathDateTime.Today);
			RegisterCustomFunction(OPathDateTime.FunctionPrefix, "min-value", OPathDateTime.MinValue);
			RegisterCustomFunction(OPathDateTime.FunctionPrefix, "max-value", OPathDateTime.MaxValue);
			RegisterCustomFunction(OPathDateTime.FunctionPrefix, "add-days", OPathDateTime.AddDays);
		}

		#endregion

		#region Private methods

		private static bool IsReturnDefaultForNullOptionSet(OPathOptions opathOptions)
		{
			bool isAnyNullEqualsNullOptionSet =
				(opathOptions & OPathOptions.ReturnDefaultForNull)
					== OPathOptions.ReturnDefaultForNull;

			return isAnyNullEqualsNullOptionSet;
		}


		private static bool IsReturnDefaultIfExceptionOptionSet(OPathOptions opathOptions)
		{
			bool isAnyExceptionEqualsNullOptionSet =
				(opathOptions & OPathOptions.ReturnDefaultIfException)
					== OPathOptions.ReturnDefaultIfException;

			return isAnyExceptionEqualsNullOptionSet;
		}


		private static string GetDefaultForNullLogMessage(OPathVariable opathVariable, string evaluatedOPath,
			object defaultValue)
		{
			string defaultForNullLogMessage =
				string.Format("{0} interpreted as '{1}' ({2} evaluated to '{3}')",
					opathVariable.GetReference(), defaultValue ?? NULL_MESSAGE_STRING, evaluatedOPath, NULL_MESSAGE_STRING);

			return defaultForNullLogMessage;
		}


		private static string GetDefaultIfExceptionLogMessage(OPathVariable opathVariable, string evaluatedOPath,
			OPathPart opathPart, object defaultValue, Exception exception)
		{
			string exceptionMessage = exception.Message;
			Type exceptionType = exception.GetType();

			if ((exception is OPathException)
				&& (exception.InnerException != null))
			{
				exceptionMessage = exception.InnerException.Message;
				exceptionType = exception.InnerException.GetType();
			}

			string defaultIfExceptionLogMessage =
				string.Format("{0} interpreted as '{1}' ({2}{3} threw {4}: {5})",
					opathVariable.GetReference(), defaultValue ?? NULL_MESSAGE_STRING, evaluatedOPath, opathPart, 
					exceptionType.Name, exceptionMessage);

			return defaultIfExceptionLogMessage;
		}


		private object EvaluateSingleVariable(OPathExpression opathExpression, OPathOptions opathOptions,
			object defaultValue, ValueLogger valueLogger)
		{
			OPathVariable opathVariable = opathExpression.Variables[0];

			object sourceObject = GetSourceObject(opathVariable);

			object variableValue =
				EvaluateVariableReference(
					sourceObject, opathVariable, opathOptions, defaultValue, valueLogger, opathExpression.OPath);

			return variableValue;
		}


		private object EvaluateOPathExpression(OPathExpression opathExpression, OPathOptions opathOptions,
			object defaultValue, ValueLogger valueLogger)
		{
			XPathNavigator variableValueNavigator = (new XmlDocument()).CreateNavigator();

			// Clone the XPath expression so we can set a context on it in a thread-safe manner
			XPathExpression clonedXPathExpression = opathExpression.XPathExpression.Clone();

			IDictionary<string, object> variableValueDictionary = 
				GetVariableValueDictionary(opathExpression, opathOptions, defaultValue, valueLogger);

			clonedXPathExpression.SetContext(
				new OPathXsltContext(variableValueDictionary, m_CustomFunctions, GetXPathValue));

			object evaluatedExpression = variableValueNavigator.Evaluate(clonedXPathExpression);

			return evaluatedExpression;
		}


		private IDictionary<string, object> GetVariableValueDictionary(OPathExpression opathExpression,
			OPathOptions opathOptions, object defaultValue, ValueLogger valueLogger)
		{
			IDictionary<string, object> variableValueDictionary = new Dictionary<string, object>();

			foreach (OPathVariable opathVariable in opathExpression.Variables)
			{
				object sourceObject = GetSourceObject(opathVariable);

				object variableValue = null;
				try
				{

					variableValue =
						EvaluateVariableReference(
							sourceObject, opathVariable, opathOptions, defaultValue, valueLogger,
							opathExpression.OPath);

				}
				catch (Exception ex)
				{
					throw new OPathException(string.Format(
						"Exception occurred evaluating variable reference {0}: {1}",
						opathVariable.GetReference(), ex.Message), ex);
				}

				variableValueDictionary.Add(opathVariable.XPathName, variableValue);
			}

			return variableValueDictionary;
		}


		private object GetSourceObject(OPathVariable opathVariable)
		{
			object sourceObject;

			if (m_OPathDocument.ContainsKey(opathVariable.Name))
			{
				sourceObject = m_OPathDocument[opathVariable.Name];
			}
			else
			{
				object standardSourceObject;
				if (GetStandardSourceObject(opathVariable.Name, out standardSourceObject))
				{
					sourceObject = standardSourceObject;
				}
				else
				{
					throw new OPathException(string.Format(
						"The expression contains variable '{0}', but no source object with that name was supplied.",
						opathVariable.Name));
				}
			}

			return sourceObject;
		}


		private static bool GetStandardSourceObject(string variableName, out object standardSourceObject)
		{
			bool isStandardSourceObject = true;

			switch (variableName)
			{
				case "null":
					standardSourceObject = null;
					break;

				case "DateTimeMinValue":
					standardSourceObject = DateTime.MinValue;
					break;

				case "DateTimeMaxValue":
					standardSourceObject = DateTime.MaxValue;
					break;

				case "DateTimeNow":
					standardSourceObject = DateTime.Now;
					break;

				case "DateTimeToday":
					standardSourceObject = DateTime.Today;
					break;

				case "true":
					standardSourceObject = true;
					break;

				case "false":
					standardSourceObject = false;
					break;

				default:
					isStandardSourceObject = false;
					standardSourceObject = null;
					break;
			}

			return isStandardSourceObject;
		}


		private object GetXPathValue(object objectValue)
		{
			object xpathObjectValue;

			if (objectValue == null)
			{
				// Use a specific value for null, so we can check for it.
				xpathObjectValue = GetNullString();
			}
			else if (objectValue is bool)
			{
				// Don't do any special conversion for these types
				xpathObjectValue = objectValue;
			}
			else
			{
				OPathCustomTypeConverter customTypeConverter;
				if (m_CustomerTypeConverters.TryGetValue(objectValue.GetType(), out customTypeConverter))
				{
					xpathObjectValue = customTypeConverter(objectValue);
				}
				else
				{
					xpathObjectValue = objectValue.ToString();
				}
			}

			return xpathObjectValue;
		}


		private object EvaluateVariableReference(object sourceObject, OPathVariable opathVariable,
			OPathOptions opathOptions, object defaultValue, ValueLogger valueLogger, string opath)
		{
			object opathValue = sourceObject;
			string evaluatedOPath = opathVariable.Name;

			foreach (OPathPart opathPart in opathVariable.OPathParts)
			{
				if (opathValue == null)
				{
					if (IsReturnDefaultForNullOptionSet(opathOptions))
					{
						if (valueLogger != null)
						{
							string defaultForNullLogMessage =
								GetDefaultForNullLogMessage(opathVariable, evaluatedOPath, defaultValue);
							valueLogger(defaultForNullLogMessage);
						}

						opathValue = defaultValue;
						break;
					}
					else
					{
						throw new OPathException(
							string.Format("{0} evaluated to '{1}'.", evaluatedOPath, NULL_MESSAGE_STRING));
					}
				}

				try
				{

					opathValue = opathPart.Evaluate(opathValue, evaluatedOPath);

					evaluatedOPath += opathPart;

				}
				catch (Exception ex)
				{
					if (IsReturnDefaultIfExceptionOptionSet(opathOptions))
					{
						if (valueLogger != null)
						{
							string defaultIfExceptionLogMessage =
								GetDefaultIfExceptionLogMessage(
									opathVariable, evaluatedOPath, opathPart, defaultValue, ex);
							valueLogger(defaultIfExceptionLogMessage);
						}

						opathValue = defaultValue;
						break;
					}
					else
					{
						throw;
					}
				}
			}

			if (valueLogger != null) valueLogger(
				string.Format("{0} evaluated to '{1}'", opathVariable.GetReference(), opathValue ?? NULL_MESSAGE_STRING));

			return opathValue;
		}

		#endregion
	}
}
