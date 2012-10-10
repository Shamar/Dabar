//
//  XSLTValidator.cs
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Resolvers;

namespace Dabar.Contracts
{
    public sealed class XSLTValidator : InputCodeValidatorBase
    {
        private readonly XslCompiledTransform _xslt;
        private readonly XsltArgumentList _xslArg;
        public XSLTValidator(string languageName, XsltArgumentList xsltArgumentList)
        {
            if(null == xsltArgumentList)
                throw new ArgumentNullException("xsltArgumentList");
            _xslArg = xsltArgumentList;
            _xslt = new XslCompiledTransform();
            
            var assembly = this.GetType().Assembly;
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
                        using (var stream = assembly.GetManifestResourceStream(resourceName))
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

        #region implemented abstract members of InputCodeValidatorBase

        public override IEnumerable<CompilationMessage> Validate(XPathDocument source)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

