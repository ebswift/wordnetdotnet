/* Relatedness Search Helper
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * (c) Dao Ngoc Thanh, 2005
 */

using System;
using Wnlib;

namespace WordsMatching
{
    class Relatedness
    {
        static Tokeniser tokenize = new Tokeniser();

        static string[] GetAllDefinitionTokens(Search se)
        {
            string rels = "";
            if (se.senses[0].senses != null)
                foreach (SynSet ss in se.senses[0].senses)
                {
                    foreach (Lexeme ww in ss.words)
                        rels += " " + ww.word;
                    rels += ss.defn;
                }

            string[] toks = tokenize.Partition(rels);
            return toks;
        }

        static string[] GetSynsetDefinition(SynSet sense)
        {
            if (sense == null) return null;
            string gloss = sense.defn;
            //			if (gloss.IndexOf(";") != -1)
            //				gloss=gloss.Substring(0, gloss.IndexOf(";")) ;
            foreach (Lexeme word in sense.words)            
                gloss += " " + word.word;
            
            string[] toks = tokenize.Partition(gloss);
            return toks;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Return list of option for searching relatedness correspond to pos
        /// E.g hypo, hyper of noun
        ///     tropo of verb.
        /// </returns>
        public static Opt[] GetRelatedness(PartsOfSpeech pos)
        {
			switch (pos)
			{
					case Wnlib.PartsOfSpeech.Noun:
					{
                        Opt[] NOUN_RELATEDNESS = new Opt[] { Opt.at(8), //hyper
												  Opt.at(14), //holo
												  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  };

						return  NOUN_RELATEDNESS;						
					}
					case Wnlib.PartsOfSpeech.Verb:
					{
                        Opt[] VERB_RELATEDNESS = new Opt[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  };
                        return VERB_RELATEDNESS;						
    				}
					case Wnlib.PartsOfSpeech.Adj:
					{
                        Opt[] ADJECTIVE_RELATEDNESS = new Opt[] {
													   Opt.at(0)												  
												   };

                        return ADJECTIVE_RELATEDNESS;
					}
					case Wnlib.PartsOfSpeech.Adv:
					{
                        Opt[] ADVEB_RELATEDNESS = new Opt[] {
												       Opt.at(48)												  
											   };
                        return ADVEB_RELATEDNESS;
					}				

			};

            return null; 
        }

        /// <summary>
        /// This function is to retrieve all relatedness information of given word, which
        /// will be used for the WSD task or a lesk relatedness measurement.
        /// </summary>
        /// <param name="word"> entry word</param>
        /// <param name="senseCount"> total sense of this word</param>
        /// <param name="relatednessTypes"> searching for relatedness that is specific partOfSpeech of given word</param>
        /// <returns>Return a three dimensions array:
        /// 1. SenseIndex.
        /// 2. Kind of relatedness. e.g : Hypernymy, Holonymy
        /// 3. Tokens list.
        /// </returns>
        public static string[][][] GetAllRelatednessData(string word, int senseCount, Opt[] relatednessTypes)
        {
            if (relatednessTypes == null) return null;
            string[][][] matrix = new string[senseCount][][];
            for (int i = 0; i < senseCount; i++)
            {
                matrix[i] = GetRelatednessGlosses(word, i + 1, relatednessTypes);
            }

            return matrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="senseIndex"></param>
        /// <param name="relatenessSearchTypes"></param>
        /// <returns></returns>
        public static string[][] GetRelatednessGlosses(string word, int senseNumber, Opt[] relatednessTypes)
        {
            string[][] relations = new string[relatednessTypes.Length + 1][];

            for (int i = 0; i < relatednessTypes.Length; i++)
            {
                Opt relateness = relatednessTypes[i];
                Search se = new Search(word, true, relateness.pos, relateness.sch, senseNumber);//								
                if (se.senses != null && se.senses.Count > 0)
                {
                    if (relations[0] == null)
                        relations[0] = GetSynsetDefinition(se.senses[0]);
                    if (se.senses[0].senses != null)
                        relations[i + 1] = GetAllDefinitionTokens(se);

                }
                else relations[i + 1] = null;
            }

            return relations;
        }


    }
}
