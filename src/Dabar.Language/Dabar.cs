//
//  Dabar.cs
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
using Dabar.Contracts;

namespace Dabar.Language
{
    public sealed class Dabar : InputLanguageBase
    {
        public Dabar()
            : base(new XSLTValidator("Dabar", null))
        {
        }

        #region implemented abstract members of InputLanguageBase

        public override void Dispose()
        {

        }

        #endregion
    }
}

