/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Author:	Jeff Martin
 * Date:	6/07/2005
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson, Jeff Martin
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
 */

using System;
using System.Collections;

namespace WnLexicon
{
	/// <summary>This class contains information about the word</summary>
	public class WordInfo
	{
		public string text = "";
		public Wnlib.PartsOfSpeech partOfSpeech = Wnlib.PartsOfSpeech.Unknown;
		public int[] senseCounts = null;

		/// <summary>a sum of all the sense counts hints at the commonality of a word</summary>
		public int Strength
		{
			get
			{
				if( senseCounts == null ) return 0;
				int strength = 0;
				foreach( int i in senseCounts )
					strength += i;
				return strength;
			}
		}

		public static bool operator == ( WordInfo a, WordInfo b )
		{
			bool retval = a.partOfSpeech == b.partOfSpeech;
			retval = retval && ( (a.senseCounts == null) == (b.senseCounts == null) );
			if( a.senseCounts != null && b.senseCounts != null )
				retval = retval && a.senseCounts.Equals( b.senseCounts );
			return retval;
		}

		public static bool operator != ( WordInfo a, WordInfo b )
		{
			return !( a == b );
		}

		public override bool Equals( object obj )
		{
			if( obj is WordInfo )
				return this == (WordInfo)obj;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return
				partOfSpeech.GetHashCode() ^
				senseCounts.GetHashCode();
		}
	}

	/// <summary>This class is designed to provide a front-end for WordNet when the part
	/// of speech of a word is not know. It has functions to determine the part of speech
	/// for a word using simple searches and morphological operations. It is not 100% correct
	/// because WordNet was most likely not intended to be used this way. However, it is
	/// accurate enough for most applications.</summary>
	public class Lexicon
	{
		/*-------------
		 * Data Members
		 *-------------*/

		/// <summary>This gets used a lot, so I decided to cache it in static memory.</summary>
		private static Wnlib.PartsOfSpeech[] enums =
			(Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );

		/*--------
		 * Methods
		 *--------*/

		/// <summary>Finds the part of speech for a given single word</summary>
		/// <param name="word">the word</param>
		/// <param name="includeMorphs">include morphology?</param>
		/// <returns>a structure containing information about the word</returns>
		public static WordInfo FindWordInfo( string word, bool includeMorphs )
		{
			WordInfo wordinfo = lookupWord( word );

			// include morphology if nothing was found on the original word
			if( wordinfo.Strength == 0 && includeMorphs )
				wordinfo = lookupWordMorphs( word );

			return wordinfo;
		}

		private static WordInfo lookupWord( string word )
		{
			// OVERVIEW: For each part of speech, look for the word.
			//           Compare relative strengths of the synsets in each category
			//			 to determine the most probable part of speech.
			//
			// PROBLEM:  Word definitions are often context-based. It would be better
			//           to find a way to search in-context in stead of just singling
			//           out an individual word.
			//
			// SOLUTION: Modify FindPartOfSpeech to include a second argument, string
			//           context. The pass the entire sentence as the context for part
			//           of speech determination.
			//
			// PROBLEM:  That's difficult to do so I'm going to keep this simple for now.

			int maxCount = 0;
			WordInfo wordinfo = new WordInfo();
			wordinfo.partOfSpeech = Wnlib.PartsOfSpeech.Unknown;

			// for each part of speech...
			Wnlib.PartsOfSpeech[] enums = (Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );
			wordinfo.senseCounts = new int[enums.Length];
			for( int i=0; i<enums.Length; i++ )
			{
				// get a valid part of speech
				Wnlib.PartsOfSpeech pos = enums[i];
				if( pos == Wnlib.PartsOfSpeech.Unknown )
					continue;

				// get an index to a synset collection
				Wnlib.Index index = Wnlib.Index.lookup( word, Wnlib.PartOfSpeech.of( pos ) );

				// none found?
				if( index == null )
					continue;

				// does this part of speech have a higher sense count?
				wordinfo.senseCounts[i] = index.sense_cnt;
				if( wordinfo.senseCounts[i] > maxCount )
				{
					maxCount = wordinfo.senseCounts[i];
					wordinfo.partOfSpeech = pos;
				}
			}
	
			return wordinfo;
		}

		/// <summary>Perform a WordNet lookup on each of the word's morphs and return the strongest match</summary>
		/// <param name="word">the word</param>
		/// <returns>a structure containg information about the word</returns>
		private static WordInfo lookupWordMorphs( string word )
		{
			// OVERVIEW: This functions only gets called when the word was not found with
			//           an exact match. So, enumerate all the parts of speech, then enumerate
			//           all of the word's morphs in each category. Perform a lookup on each
			//           morph and save the morph/strength/part-of-speech data sets. Finally,
			//           loop over all the data sets and then pick the strongest one.

			ArrayList wordinfos = new ArrayList();

			// for each part of speech...
			for( int i=0; i<enums.Length; i++ )
			{
				// get a valid part of speech
				Wnlib.PartsOfSpeech pos = enums[i];
				if( pos == Wnlib.PartsOfSpeech.Unknown )
					continue;

				// generate morph list
				Wnlib.MorphStr morphs = new Wnlib.MorphStr( word, Wnlib.PartOfSpeech.of( pos ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					// get an index to a synset collection
					Wnlib.Index index = Wnlib.Index.lookup( morph, Wnlib.PartOfSpeech.of( pos ) );

					// none found?
					if( index == null )
						continue;

					// save the wordinfo
					WordInfo wordinfo = getMorphInfo( wordinfos, morph );
					wordinfo.senseCounts[i] = index.sense_cnt;
				}
			}

			// search the wordinfo list for the best match
			WordInfo bestWordInfo = new WordInfo();
			int maxStrength = 0;
			foreach( WordInfo wordinfo in wordinfos )
			{
				// for each part of speech...
				int maxSenseCount = 0;
				int strength = 0;
				for( int i=0; i<enums.Length; i++ )
				{
					// get a valid part of speech
					Wnlib.PartsOfSpeech pos = enums[i];
					if( pos == Wnlib.PartsOfSpeech.Unknown )
						continue;

					// determine part of speech and strength
					strength += wordinfo.senseCounts[i];
					if( wordinfo.senseCounts[i] > maxSenseCount )
					{
						maxSenseCount = wordinfo.senseCounts[i];
						wordinfo.partOfSpeech = pos;
					}			
				}

				// best match?
				if( strength > maxStrength )
				{
					maxStrength = strength;
					bestWordInfo = wordinfo;
				}
			}

			return bestWordInfo;
		}

		private static WordInfo getMorphInfo( ArrayList morphinfos, string morph )
		{
			// Attempt to find the morph string in the list.
			// NOTE: Since the list should never get very large, a selection search will work just fine
			foreach( WordInfo morphinfo in morphinfos )
				if( morphinfo.text == morph )
					return morphinfo;

			// if not found, create a new one
			WordInfo wordinfo = new WordInfo();
			wordinfo.text = morph;
			wordinfo.senseCounts = new int[enums.Length];
			return (WordInfo)morphinfos[morphinfos.Add( wordinfo )];
		}
	}
}
