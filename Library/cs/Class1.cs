/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System.Collections;
using System.Windows.Forms;
using Wnlib;

namespace WordNetClasses
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class WN
	{
        public static bool hasmatch = false; // determines whether morphs are considered

        public WN( string dictpath )
		{
			//Wnlib.WNDB(dictpath);
			WNCommon.path = dictpath;
		}

		public void OverviewFor( string t, string p, ref bool b, ref SearchSet obj, ArrayList list)
		{
			PartOfSpeech pos = Wnlib.PartOfSpeech.of(p);
			SearchSet ss = Wnlib.WNDB.is_defined(t,pos);
			MorphStr ms = new Wnlib.MorphStr(t,pos);
            bool checkmorphs = false;

			checkmorphs = AddSearchFor(t,pos, list); // do a search
			string m;

            if(checkmorphs)
                WN.hasmatch = true;

            if(! hasmatch) {
			    // loop through morphs (if there are any)
			    while ((m=ms.next())!=null)
				    if (m!=t) 
				    {
					    ss = ss+WNDB.is_defined(m,pos);
					    AddSearchFor(m,pos, list);
				    }
            }
			b = ss.NonEmpty;
			obj = ss;
		}

		bool AddSearchFor(string s,PartOfSpeech pos, ArrayList list)
		{
			Search se = new Search(s,false,pos,new SearchType(false,"OVERVIEW"),0);
            if(se.lexemes.Count > 0)
			    list.Add(se);

            if(se.lexemes.Count > 0)
                return true; // results were found
            else
                return false;
		}

	}
}
