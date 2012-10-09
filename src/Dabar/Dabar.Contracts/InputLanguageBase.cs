//
//  InputLanguageBase.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2012 Giacomo Tesio
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using System.Xml.Resolvers;

namespace Dabar.Contracts
{
    public abstract class InputLanguageBase : IDisposable
    {
        private readonly XmlSchema _schema;
        private readonly XslCompiledTransform _xslt;
        private readonly XsltArgumentList _xslArg;
        protected InputLanguageBase()
        {
            var assembly = this.GetType().Assembly;
            var namespaceName = this.GetType().Namespace;
            var languageName = this.GetType().Name;
            using (var stream = assembly.GetManifestResourceStream(namespaceName + "." + languageName + ".Language.xsd"))
            using (var reader = XmlReader.Create(stream))
            {
                _schema = XmlSchema.Read(
                    reader, 
                    new ValidationEventHandler(languageValidationEventHandler));
            }
            _xslt = new XslCompiledTransform();

            var resolver = new XmlPreloadedResolver();

            string mainXSLT = null;
            string[] resourceNames = this.GetType().Assembly.GetManifestResourceNames();
            foreach(string resourceName in resourceNames)
            {
                if(resourceName.EndsWith(".xslt") && resourceName.Contains(languageName))
                {
                    string templateName = resourceName.Substring(resourceName.LastIndexOf(languageName) + languageName.Length);
                    if(templateName == ".xslt")
                    {
                        mainXSLT = resourceName;
                    }
                    else
                    {
                        using (var stream = assembly.GetManifestResourceStream(namespaceName + "." + languageName + ".Language.xsd"))
                        {
                            resolver.Add(resolver.ResolveUri(null, templateName), stream);
                        }
                    }
                }
            }
            using (var stream = assembly.GetManifestResourceStream(mainXSLT))
            using (var reader = XmlReader.Create(stream))
            {
                _xslt.Load(reader, null, resolver);
            }
        }

        protected InputLanguageBase(XsltArgumentList xsltArgumentList)
            : this()
        {
            if(null == xsltArgumentList)
                throw new ArgumentNullException("xsltArgumentList");
            _xslArg = xsltArgumentList;
        }

        private void languageValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                string message = string.Format("The XSD validation of the language '{0}' produced a warning: {1}", this.GetType().Name, e.Message);
                throw new InvalidOperationException(message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                string message = string.Format("The XSD validation of the language '{0}' produced a warning: {1}", this.GetType().Name, e.Message);
                throw new InvalidOperationException(message);
            }
        }

        public bool IsValid(string sourceFilePath)
        {
            return false;
        }

        #region IDisposable implementation

        public abstract void Dispose();

        #endregion
    }
}

