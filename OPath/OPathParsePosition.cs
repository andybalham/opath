﻿/*
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

namespace OPath
{
	internal enum OPathParsePosition
	{
		InExpression = 1,
		InString = 2,
		InIdentifier = 3,
		InMember = 4,
		PreIndexerValue = 5,
		InStringKey = 6,
		InVariableOPath = 7,
		InMethodParameters = 8,
		InIntIndexer = 9,
		PostIndexerValue = 10,
	}
}
