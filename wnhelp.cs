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

using System;
using Wnlib;

namespace Wnlib
{
	public class WNHelp 
	{
		public string help;
		public WNHelp(string s,PartOfSpeech p)
		{
			help = (string)p.help[s];
		}
		public WNHelp(SearchType s,PartOfSpeech p)
			: this((s.rec?"-":"")+s.ptp.mnemonic,p) {}
		public WNHelp(string s,string p) 
			: this(s,PartOfSpeech.of(p)) {}
		static WNHelp() 
		{
			PartOfSpeech noun = PartOfSpeech.of("noun");
			PartOfSpeech verb = PartOfSpeech.of("verb");
			PartOfSpeech adj = PartOfSpeech.of("adj");
			PartOfSpeech adv = PartOfSpeech.of("adv");
			string freqhelp =
				"Display familiarity and polysemy information for the search string. \n"+
				"The polysemy count is obtained from WordNet, and is the number of \n"+
				"senses represented in the database. \n";
			noun.help["FREQ"] = freqhelp;
			verb.help["FREQ"] = freqhelp;
			adj.help["FREQ"] = freqhelp;
			adv.help["FREQ"] = freqhelp;
			string grephelp =
				"Print all strings in the database which contain the search string \n"+
				"as an individual word, or as the first or last string in a word or \n"+
				"collocation. \n";
			noun.help["WNGREP"] = grephelp;
			verb.help["WNGREP"] = grephelp;
			adj.help["WNGREP"] = grephelp;
			adv.help["WNGREP"] = grephelp;
			noun.help["HYPERPTR"] =
				"Display synonyms and immediate `hypernyms' of synsets containing \n"+
				"the search string.  Synsets are ordered by frequency of occurrence.  \n"+
				"\n"+
				"Hypernym is the generic term used to designate a whole class of \n"+
				"specific instances.  Y is a hypernym of X if X is a (kind of) Y. \n"+
				"\n"+
				"Hypernym synsets are preceded by \"=>\". \n";
			noun.help["RELATIVES"] =
				"Display synonyms and immediate `hypernyms' of synsets containing \n"+
				"the search string.  Synsets are grouped by similarity of meaning. \n"+
				"\n"+
				"Hypernym is the generic term used to designate a whole class of \n"+
				"specific instances.  Y is a hypernym of X if X is a (kind of) Y. \n"+
				"\n"+
				"Hypernym synsets are preceded by \"=>\". \n";
			noun.help["ANTPTR"] =
				"Display synsets containing `direct anotnyms' of the search string.  \n"+
				"\n"+
				"Direct antonyms are a pair of words between which there is an \n"+
				"associative bond built up by co-occurrences. \n"+
				"\n"+
				"Antonym synsets are preceded by \"=>\". \n";
			noun.help["COORDS"] =
				"Display the coordinates (sisters) of the search string.  This search \n"+
				"prints the immediate `hypernym' for each synset which contains the \n"+
				"search string and the hypernym's immediate `hyponyms'. \n"+
				"\n"+
				"Hypernym is the generic term used to designate a whole class of \n"+
				"specific instances.  Y is a hypernym of X if X is a (kind of) Y. \n"+
				"\n"+
				"Hyponym is the generic term used to designate a member of a class. \n"+
				"X is a hyponym of Y if X is a (kind of) Y.  \n"+
				"\n"+
				"Coordinate words are words that have the same hypernym.\n"+
				"\n"+
				"Hypernym synsets are preceded by \"->\", and hyponym synsets are \n"+
				"preceded by \"=>\". \n";
			noun.help["-HYPERPTR"] =
				"Recursively display `hypernym' (superordinate) tree for the search \n"+
				"string. \n"+
				"\n"+
				"Hypernym is the generic term used to designate a whole class of \n"+
				"specific instances.  Y is a hypernym of X if X is a (kind of) Y. \n"+
				"\n"+
				"Hypernym synsets are preceded by \"=>\", and are indented from \n"+
				"the left according to their level in the hierarchy. \n";
			noun.help["HYPOPTR"] =
				"Display immediate `hyponyms' (subordinates) for the search string. \n"+
				"\n"+
				"Hyponym is the generic term used to designate a member of a class. \n"+
				"X is a hyponym of Y if X is a (kind of) Y.  \n"+
				"\n"+
				"Hyponym synsets are preceded by \"=>\". \n";
			noun.help["-HYPOPTR"] =
				"Display `hyponym' (subordinate) tree for the search string.  This is \n"+
				"a recursive search which finds the hyponyms of each hyponym.  \n"+
				"\n"+
				"Hyponym is the generic term used to designate a member of a class. \n"+
				"X is a hyponym of Y if X is a (kind of) Y.  \n"+
				"\n"+
				"Hyponym synsets are preceded by \"=>\", and are indented from the left \n"+
				"according to their level in the hierarchy. \n";
			noun.help["HOLONYM"] =
				"Display all `holonyms' of the search string. \n"+
				"\n"+
				"A holonym is the name of the whole of which the 'meronym' names a part. \n"+
				"Y is a holonym of X if X is a part of Y. \n"+
				"\n"+
				"A meronym is the name of a constituent part, the substance of, or a \n"+
				"member of something.  X is a meronym of Y if X is a part of Y. \n"+
				"\n"+
				"Holonym synsets are preceded with either the string \"MEMBER OF\", \n"+
				"\"PART OF\" or \"SUBSTANCE OF\" depending on the specific type of holonym. \n";
			noun.help["-HHOLONYM"] = 
				"Display `holonyms' for search string tree.  This is a recursive search \n"+
				"that prints all the holonyms of the search string and all of the \n"+
				"holonym's holonyms. \n"+
				"\n"+
				"A holonym is the name of the whole of which the `meronym' names a part. \n"+
				"Y is a holonym of X if X is a part of Y. \n"+
				"\n"+
				"A meronym is the name of a constituent part, the substance of, or a \n"+
				"member of something.  X is a meronym of Y if X is a part of Y. \n"+
				"\n"+
				"Holonym synsets are preceded with either the string \"MEMBER OF\", \n"+
				"\"PART OF\" or \"SUBSTANCE OF\" depending on the specific \n"+
				"type of holonym.  Synsets are indented from the left according to \n"+
				"their level in the hierarchy. \n";
			noun.help["MERONYM"] = 
				"Display all `meronyms' of the search string.  \n"+
				"\n"+
				"A meronym is the name of a constituent part, the substance of, or a \n"+
				"member of something.  X is a meronym of Y if X is a part of Y. \n"+
				"\n"+
				"A holonym is the name of the whole of which the `meronym' names a part. \n"+
				"Y is a holonym of X if X is a part of Y. \n"+
				"\n"+
				"Meronym synsets are preceded with either the string \"HAS MEMBER\", \n"+
				"\"HAS PART\" or \"HAS SUBSTANCE\" depending on the specific type of holonym. \n";
			noun.help["-HMERONYM"] =
				"Display `meronyms' for search string tree.  This is a recursive search \n"+
				"the prints all the meronyms of the search string and all of its \n"+
				"`hypernyms'.  \n"+
				"\n"+
				"A meronym is the name of a constituent part, the substance of, or a \n"+
				"member of something.  X is a meronym of Y if X is a part of Y. \n"+
				"\n"+
				"A holonym is the name of the whole of which the `meronym' names a part. \n"+
				"Y is a holonym of X if X is a part of Y. \n"+
				"\n"+
				"Hypernym is the generic term used to designate a whole class of \n"+
				"specific instances.  Y is a hypernym of X if X is a (kind of) Y. \n"+
				"\n"+
				"Meronym synsets are preceded with either the string \"HAS MEMBER\", \n"+
				"\"HAS PART\" or \"HAS SUBSTANCE\" depending on the specific type of \n"+
				"holonym.  Synsets are indented from the left according to their level \n"+
				"in the hierarchy. \n";
			noun.help["ATTRIBUTE"] =
				"Display adjectives for which search string is an attribute. \n";
			verb.help["HYPERPTR"] =
				"Display synonyms and immediate `troponyms' of synsets containing \n"+
				"the search string.  \n"+
				"\n"+
				"A troponym is a verb expressing a specific manner elaboration of another \n"+
				"verb.  X is a troponym of Y if to X is to Y in some manner. \n"+
				"\n"+
				"Troponym synsets are preceded by \"=>\". \n";
			verb.help["VERBGROUP"] =
				"Display synonyms and immediate `troponyms' of synsets containing \n"+
				"the search string.  Synsets are grouped by similarity of meaning. \n"+
				"\n"+
				"A troponym is a verb expressing a specific manner elaboration of another \n"+
				"verb.  X is a troponym of Y if to X is to Y in some manner. \n"+
				"\n"+
				"Troponym synsets are preceded by \"=>\". \n";
			verb.help["ANTPTR"] =
				"Display synsets containing `direct anotnyms' of the search string.  \n"+
				"\n"+
				"Direct antonyms are a pair of words between which there is an \n"+
				"associative bond built up by co-occurrences. \n"+
				"\n"+
				"Antonym synsets are preceded by \"=>\". \n";
			verb.help["-HYPERPTR"] =
				"Recursively display `hypernym' tree for the search string.  For verbs,\n"+
				"'hypernyms' are refered to as `troponyms'. \n"+
				"\n"+
				"A troponym is a verb expressing a specific manner elaboration of another \n"+
				"verb.  X is a troponym of Y if to X is to Y in some manner. \n"+
				"\n"+
				"Troponym synsets are preceded by \"=>\". \n"+
				"\n"+
				"Troponym synsets are preceded by \"=>\", and are indented from \n"+
				"the left according to their level in the hierarchy. \n";
			verb.help["-HYPOPTR"] =
				"Display `hyponym' tree for the search string.  This is \n"+
				"a recursive search which finds the hyponyms of each hyponym.  \n"+
				"\n"+
				"For verbs, hyponyms indicate particular ways to perform a function. \n"+
				"X is a hyponym of Y if to X is a particular way to Y. \n"+
				"\n"+
				"Hyponym synsets are preceded by \"=>\", and are indented from the left \n"+
				"according to their level in the hierarchy. \n";
			verb.help["ENTAILPTR"] =
				"Recursively display `entailment' relations of the search string.  \n"+
				"\n"+
				"The action represented by the verb X entails Y if X cannot be done \n"+
				"unless Y is, or has been, done. \n"+
				"\n"+
				"Entailment synsets are preceded by \"=>\", and are indented from the left \n"+
				"according to their level in the hierarchy. \n";
			verb.help["CAUSETO"] =
				"Recursively display `cause to' relations of the search string. \n"+
				"\n"+
				"The action represented by the verb X causes the action represented by \n"+
				"the verb Y. \n"+
				"\n"+
				"`Cause to' synsets are preceded by \"=>\", and are indented from the left \n"+
				"according to their level in the hierarch.  \n";
			verb.help["FRAMES"] =
				"Display applicable verb sentence `frames' for the search string. \n"+
				"\n"+
				"A frame is a sentence template illustrating the usage of a verb. \n"+
				"\n"+
				"Verb sentence frames are preceded with the string \"*>\" if a sentence \n"+
				"frame is acceptable for all of the words in the synset, and with \"=>\" \n"+
				"if a sentence frame is acceptable for the search string only.  \n";
			adj.help["SIMPTR"] =
				"Display synonyms and synsets related to synsets containing \n"+
				"the search string.  If the search string is in a `head synset' \n"+
				"the 'cluster's' `satellite synsets' are displayed.  If the search \n"+
				"string is in a satellite synset, its head synset is displayed. \n"+
				"If the search string is a `pertainym' the word or synset that it \n"+
				"pertains to is displayed. \n"+
				"\n"+
				"A cluster is a group of adjective synsets that are organized around \n"+
				"antonymous pairs or triplets.  An adjective cluster contains two or more \n"+
				"head synsets which contan antonyms.  Each head synset has one or more \n"+
				"satellite synsets. \n"+
				"\n"+
				"A head synset contains at least one word which has a `direct antonym' \n"+
				"in another head synset of the same cluster. \n"+
				"\n"+
				"A satellite synset represents a concept that is similar in meaning to \n"+
				"the concept represented by its head synset. \n"+
				"\n"+
				"Direct antonyms are a pair of words between which there is an \n"+
				"associative bond built up by co-occurrences. \n"+
				"\n"+
				"Direct antonyms are printed in parentheses following the adjective. \n"+
				"The position of an adjective in relation to the noun may be restricted \n"+
				"to the prenominal, postnominal or predicative position.  Where present \n"+
				"these restrictions are noted in parentheses. \n"+
				"\n"+
				"A pertainym is a relational adjective, usually defined by such phrases \n"+
				"as \"of or pertaining to\" and which does not have an antonym.  It pertains \n"+
				"to a noun or another pertainym. \n"+
				"\n"+
				"Senses contained in head synsets are displayed above the satellites, \n"+
				"which are indented and preceded by \"=>\".  Senses contained in \n"+
				"satellite synsets are displayed with the head synset below.  The head \n"+
				"synset is preceded by \"=>\". \n"+
				"\n"+
				"Pertainym senses display the word or synsets that the search string \n"+
				"pertains to. \n";
			adj.help["ANTPTR"] =
				"Display synsets containing antonyms of the search string. If the \n"+
				"search string is in a `head synset' the `direct antonym' is displayed \n"+
				"along with the head synset's `satellite synsets'.  If the search \n"+
				"string is in a satellite synset, its indirect antonym is displayed \n"+
				"via the head synset \n"+
				"\n"+
				"A head synset contains at least one word which has a `direct antonym' \n"+
				"in another head synset of the same cluster. \n"+
				"\n"+
				"A satellite synset represents a concept that is similar in meaning to \n"+
				"the concept represented by its head synset. \n"+
				"\n"+
				"Direct antonyms are a pair of words between which there is an \n"+
				"associative bond built up by co-occurrences. \n"+
				"\n"+
				"Direct antonyms are printed in parentheses following the adjective. \n"+
				"The position of an adjective in relation to the noun may be restricted \n"+
				"to the prenominal, postnominal or predicative position.  Where present \n"+
				"these restrictions are noted in parentheses. \n"+
				"\n"+
				"Senses contained in head synsets are displayed, followed by the \n"+
				"head synset containing the search string's direct antonym and its \n"+
				"similar synsets, which are indented and preceded by \"=>\".  Senses \n"+
				"contained in satellite synsets are displayed followed by the indirect \n"+
				"antonym via the satellite's head synset. \n";
			adj.help["ATTRIBUTE"] =
				"Display nouns which are attributes of search string. \n";
			adv.help["SIMPTR"] =
				"Display synonyms and synsets related to synsets containing \n"+
				"the search string.  If the search string is a `pertainym' the word \n"+
				"or synset that it pertains to is displayed. \n"+
				"\n"+
				"A pertainym is a relational adverb which is derived from an adjective. \n"+
				"\n"+
				"Pertainym senses display the word that the search string is derived from \n"+
				"and the adjective synset which contains the word.  If the adjective synset \n"+
				"is a satellite synset, its head synset is also displayed. \n";
			adv.help["ANTPTR"] =
				"Display synsets containing `direct antonyms' of the search string.  \n"+
				"\n"+
				"Direct antonyms are a pair of words between which there is an \n"+
				"associative bond built up by co-occurrences. \n"+
				"\n"+
				"Antonym synsets are preceded by \"=>\". \n";

		}
	}
}