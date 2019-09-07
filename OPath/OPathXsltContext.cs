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
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OPath
{
	internal class OPathXsltContext : XsltContext
	{
		#region Public properties

		public IDictionary<string, object> Variables { get; private set; }

		public IDictionary<string, OPathCustomFunction> CustomFunctions { get; private set; }

		public OPathCustomTypeConverter CustomTypeConverter { get; private set; }

		#endregion

		#region Constructors

		public OPathXsltContext(IDictionary<string, object> variables, 
			IDictionary<string, OPathCustomFunction> customFunctions, 
			OPathCustomTypeConverter customTypeConverter)
		{
			if (variables == null)
			{
				this.Variables = new Dictionary<string, object>();
			}
			else
			{
				this.Variables = variables;
			}

			this.CustomFunctions = customFunctions;

			this.CustomTypeConverter = customTypeConverter;
		}

		#endregion

		#region XsltContext overrides

		/// <summary>
		/// When overridden in a derived class, compares the base Uniform Resource Identifiers (URIs) of two 
		/// documents based upon the order the documents were loaded by the XSLT processor (that is, the 
		/// <see cref="T:System.Xml.Xsl.XslTransform"/> class).
		/// </summary>
		/// <param name="baseUri">The base URI of the first document to compare.</param>
		/// <param name="nextbaseUri">The base URI of the second document to compare.</param>
		/// <returns>
		/// An integer value describing the relative order of the two base URIs: -1 if <paramref name="baseUri"/> occurs before <paramref name="nextbaseUri"/>; 0 if the two base URIs are identical; and 1 if <paramref name="baseUri"/> occurs after <paramref name="nextbaseUri"/>.
		/// </returns>
		public override int CompareDocument(string baseUri, string nextbaseUri)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When overridden in a derived class, evaluates whether to preserve white space nodes or strip them 
		/// for the given context.
		/// </summary>
		/// <param name="node">The white space node that is to be preserved or stripped in the current context.</param>
		/// <returns>
		/// Returns true if the white space is to be preserved or false if the white space is to be stripped.
		/// </returns>
		public override bool PreserveWhitespace(System.Xml.XPath.XPathNavigator node)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When overridden in a derived class, resolves a function reference and returns an 
		/// <see cref="T:System.Xml.Xsl.IXsltContextFunction"/> representing the function. The 
		/// <see cref="T:System.Xml.Xsl.IXsltContextFunction"/> is used at execution time to get the return 
		/// value of the function.
		/// </summary>
		/// <param name="prefix">The prefix of the function as it appears in the XPath expression.</param>
		/// <param name="name">The name of the function.</param>
		/// <param name="ArgTypes">An array of argument types for the function being resolved. This allows 
		/// you to select between methods with the same name (for example, overloaded methods).</param>
		/// <returns>
		/// An <see cref="T:System.Xml.Xsl.IXsltContextFunction"/> representing the function.
		/// </returns>
		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
		{
			IXsltContextFunction function = new OPathXsltContextFunction(prefix, name);
			return function;
		}

		/// <summary>
		/// When overridden in a derived class, resolves a variable reference and returns an 
		/// <see cref="T:System.Xml.Xsl.IXsltContextVariable"/> representing the variable.
		/// </summary>
		/// <param name="prefix">The prefix of the variable as it appears in the XPath expression.</param>
		/// <param name="name">The name of the variable.</param>
		/// <returns>
		/// An <see cref="T:System.Xml.Xsl.IXsltContextVariable"/> representing the variable at runtime.
		/// </returns>
		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			IXsltContextVariable variable = new OPathXsltContextVariable(name);
			return variable;
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether to include white space nodes 
		/// in the output.
		/// </summary>
		/// <value></value>
		/// <returns>true to check white space nodes in the source document for inclusion in the output; 
		/// false to not evaluate white space nodes. The default is true.
		/// </returns>
		public override bool Whitespace
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
