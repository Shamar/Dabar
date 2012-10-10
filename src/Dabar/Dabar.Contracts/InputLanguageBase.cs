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
using System.Collections.Generic;
using System.Xml.XPath;
using System.Linq;

namespace Dabar.Contracts
{
    public abstract class InputLanguageBase : IDisposable
    {
        private readonly XmlSchema _schema;
        private readonly InputCodeValidatorBase[] _validators;

        protected InputLanguageBase(InputCodeValidatorBase[] validators)
        {
            if(null == validators || validators.Length == 0 || validators.Any(v => null == v))
                throw new ArgumentNullException("validators");
            var assembly = this.GetType().Assembly;
            var namespaceName = this.GetType().Namespace;
            var languageName = this.GetType().Name;
            using (var stream = assembly.GetManifestResourceStream(namespaceName + "." + languageName + ".Language.xsd"))
            using (var reader = XmlReader.Create(stream))
            {
                _schema = XmlSchema.Read(
                    reader, 
                    new ValidationEventHandler(languageValidationCallBack));
            }
            _validators = validators;
        }

        private void languageValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                string message = string.Format("The XSD validation of the language '{0}' produced a warning: {1}", this.GetType().Name, e.Message);
                throw new InvalidOperationException(message);
            } else if (e.Severity == XmlSeverityType.Error)
            {
                string message = string.Format("The XSD validation of the language '{0}' produced a warning: {1}", this.GetType().Name, e.Message);
                throw new InvalidOperationException(message);
            }
        }

        private void sourceValidationCallBack(string sourceFilePath, List<CompilationMessage> resultingMessages, object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                resultingMessages.Add(new Warning(sourceFilePath, "unknown", e.Message));
            } 
            else if (e.Severity == XmlSeverityType.Error)
            {
                resultingMessages.Add(new Error(sourceFilePath, "unknown", e.Message));
            }
        }

        public bool IsValid(string sourceFilePath, out IEnumerable<CompilationMessage> messages)
        {
            List<CompilationMessage> resultingMessages = new List<CompilationMessage>();

            XmlSchemaSet sc = new XmlSchemaSet();
            
            // Add the schema to the collection.
            sc.Add(_schema);
            
            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler((s, e) => sourceValidationCallBack(sourceFilePath, resultingMessages, s, e));
            XmlReader reader = XmlReader.Create(sourceFilePath, settings);
            XPathDocument document = new XPathDocument(reader);
            if (resultingMessages.TrueForAll(m => !(m is Error)))
            {
                for (int i = 0; i < _validators.Length; ++i)
                {
                    resultingMessages.AddRange(_validators [i].Validate(document));
                }
            }
            messages = resultingMessages.ToArray();
            return resultingMessages.TrueForAll(m => !(m is Error));
        }

        #region IDisposable implementation

        public abstract void Dispose();

        #endregion
    }
}

