
/* Compute similarity between two words without using the 
 * Part of speech tagger and Word sense disambiguation
 * Thanh.Dao@
 * 
 * 
 */
using System;
using System.Collections;
using System.Diagnostics;
using Wnlib;
using System.Text.RegularExpressions;

namespace WordsMatching
{
    /// <summary>
    /// Summary description for PathLengthMeasure.
    /// </summary>
    /// 

    public class WordHierarchical
    {
        static readonly Opt IS_A_NOUN = Opt.at(11);
        static readonly Opt IS_A_VERB = Opt.at(35);

        Hashtable Track=new Hashtable ();
        public int MaxDepth = -1;
        public MyWordInfo WordInfo;

        public WordHierarchical(MyWordInfo wordInfo)
        {
            this.WordInfo = wordInfo;
            GetWordInfo();
        }

        Opt GetSearchType(PartsOfSpeech pos)
        {
            Opt opt = null;
            switch (pos)
            {
                case Wnlib.PartsOfSpeech.Noun:
                    {
                        opt = IS_A_NOUN;
                        break;
                    }
                case Wnlib.PartsOfSpeech.Verb:
                    {
                        opt = IS_A_VERB;
                        break;
                    }
            };

            return opt;
        }

        void GetWordInfo()
        {
            Opt opt = GetSearchType(WordInfo.Pos);
            if (opt == null) return;

            Search se = new Search(WordInfo.Word, true, opt.pos, opt.sch, 0);

            if (se.senses != null && se.senses.Count == 0 && se.morphs.Count > 0)
            {
                IDictionaryEnumerator getEnum = se.morphs.GetEnumerator();

                while (getEnum.MoveNext())
                {
                    string rootForm = (string)getEnum.Key;

                    if ((Wnlib.Search)getEnum.Value != null)
                    {
                        se = (Wnlib.Search)getEnum.Value;
                        if (se.senses != null && se.senses.Count > 0)
                        {
                            WordInfo.Word = rootForm;
                            break;
                        }
                    }
                }
            }

            
            if (se.senses != null)
                Walk(se.senses, 1);
            
        }


        void Walk(SynSetList synsets,  int depth)
        {
            foreach (SynSet wsense in synsets)
            {
                Add_WordSenses(wsense, depth);
                if (wsense.senses != null )
                    Walk(wsense.senses,  depth + 1);
            }
            
        }


        void Add_WordSenses(SynSet ss, int depth)
        {
            foreach (Lexeme lex in ss.words)
            {
                string word = lex.word.Replace("_", " ");
                if (depth > MaxDepth) MaxDepth = depth;
                if (!Track.ContainsKey(word))               
                    Track[word] = depth;                
                else
                {
                    int oldDpt = (int)Track[word];

                    if (oldDpt > depth)
                        Track[word] = oldDpt;
                }
            }
           
        }


        public int GetDepth(string word)
        {
            if (Track.ContainsKey(word))
            {
                return (int)Track[word];
            }
            else return -1;
        }

        public int GetCommonAncestorDistance(WordHierarchical partner)
        {
            IDictionaryEnumerator getEnum = this.Track.GetEnumerator();
            int distance=int.MaxValue;
            while (getEnum.MoveNext())
            {
                string word = (string)getEnum.Key;

                int p_depth = partner.GetDepth(word);

                if (p_depth != -1)
                {
                    int depth = (int)getEnum.Value;
                    int len = depth + p_depth - 1;
                    if (len == 0) len = 1;
                    if (distance > len) distance = len;
                }
            }

            return distance;
        }

    }

    public class WordDistance
    {   

        public WordDistance()
        {
        }
        

        public float GetSimilarity(WordHierarchical word1, WordHierarchical word2)
        {
            if (word1.WordInfo.Pos != word2.WordInfo.Pos || word1.WordInfo.Pos == PartsOfSpeech.Unknown) return 0.0F;
            if (word1.WordInfo.Word == word2.WordInfo.Word) return 1.0F;

            int len_1 = word1.GetDepth(word2.WordInfo.Word);
            int len_2 = word2.GetDepth(word1.WordInfo.Word);

            int length=-1;
            length=len_1 != -1 ? len_1 : word1.GetCommonAncestorDistance(word2);

            if (length == -1)
                length=len_2 != -1 ? len_2 : word2.GetCommonAncestorDistance(word1);

            if (length == 0) return 1.0f;
            else
                if (length > 0)
            {                
                float tmp = 1.0F / length;
                return (float)Math.Round(tmp, 2);
            }
            else
                return 0;
        }


    }
}
