
/* Compute similarity between two words 
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * $Update : 01 Feb 2006
 *  - Add Wu & Palmer similarity measure
 *  - Tested on the dataSet RG (Li 2003, et al )
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

    public class HierarchicalWordData
    {
        static readonly Opt IS_A_NOUN = Opt.at(11);
        static readonly Opt IS_A_VERB = Opt.at(35);

        public Hashtable Distance=new Hashtable ();
        Hashtable DepthMatrix = new Hashtable();
        Hashtable SynWord=new Hashtable();                        
        public MyWordInfo WordInfo;

        public HierarchicalWordData(MyWordInfo wordInfo)
        {
            this.WordInfo = wordInfo;
            Build_WordData();
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

        void Build_WordData()
        {
            Opt opt = GetSearchType(WordInfo.Pos);
            if (opt == null) return;

            Search se = new Search(WordInfo.Word, true, opt.pos, opt.sch, WordInfo.Sense);
            if (se.senses != null && se.senses.Count == 0 && se.morphs.Count > 0)
            {
                IDictionaryEnumerator getEnum = se.morphs.GetEnumerator();
                while (getEnum.MoveNext())
                {
                    string morphForm = (string)getEnum.Key;
                    if ((Wnlib.Search)getEnum.Value != null)
                    {
                        se = (Wnlib.Search)getEnum.Value;
                        if (se.senses != null && se.senses.Count > 0)
                        {
                            WordInfo.Word = morphForm;
                            break;
                        }
                    }
                }
            }
            
            if (se.senses != null)
                Traverse(se.senses, null, 1);
            Compute_DepthMatrix();
        }


        void Traverse(SynSetList synsets, SynSet prevSynset, int depth)
        {
            foreach (SynSet wsense in synsets)
            {                            
                Add_WordSenses(prevSynset, wsense, depth);

                if (wsense.senses != null)
                    Traverse(wsense.senses, wsense, depth + 1);
            }
            
        }

        void Add_WordSenses(SynSet prev, SynSet next, int depth)
        {
            SynWord[next.hereiam] = next.words[0].word;
            if (prev != null)
                DepthMatrix[GetKey(prev.hereiam , next.hereiam)] = 1;
            if (!Distance.ContainsKey(next.hereiam))
            { 
                Distance[next.hereiam] = depth; 
            }
            else
            {
                int bestDepth = (int)Distance[next.hereiam];

                if (bestDepth > depth)
                    Distance[next.hereiam] = depth;
            }            
            return;           
        }
        
        object GetKey(object i, object j)
        {            
            double encode_i = Math.Log10(Convert.ToDouble(i));
            double encode_j = Math.Log10(Convert.ToDouble(j));
            //return Convert.ToString(i) + "_" + Convert.ToString(j);             
            return encode_i * 1000.0d + encode_j; 
        }

        int _rootNode=int.MaxValue;
        void Compute_DepthMatrix()
        {
           IDictionaryEnumerator k = Distance.GetEnumerator();
           while (k.MoveNext())
           {
               object k_key = k.Key;
               if (_rootNode > (int)k_key) _rootNode = (int)k_key;

               IDictionaryEnumerator i = Distance.GetEnumerator();
               while (i.MoveNext())
               {
                   object i_key = i.Key;
                   IDictionaryEnumerator j = Distance.GetEnumerator();

                   while (j.MoveNext())
                   {
                       object j_key = j.Key;
                       if (i_key != j_key && j_key != k_key && i_key != k_key &&
                           DepthMatrix.ContainsKey(GetKey(i_key, k_key)) &&
                           DepthMatrix.ContainsKey(GetKey(k_key, j_key)))                            
                       {                           
                           if (!DepthMatrix.ContainsKey(GetKey(i_key, j_key))
                            || (int)DepthMatrix[GetKey(i_key, j_key)] > (int)DepthMatrix[GetKey(i_key, k_key)] + (int)DepthMatrix[GetKey(k_key, j_key)])
                           {
                               DepthMatrix[GetKey(i_key, j_key)] = (int)DepthMatrix[GetKey(i_key, k_key)] + (int)DepthMatrix[GetKey(k_key, j_key)];                               
                           }
                       }                       
                   }
               }
           }           
        }

        public int GetDistance(int key)
        {
            if (Distance.ContainsKey(key))            
               return (int)Distance[key];            
            else return -1;
        }

        public int GetDepth(int key)
        {
            int rootDepth =(int) DepthMatrix[GetKey(key, _rootNode)];
            return  rootDepth + 1;
        }

    }

    public class WordDistance
    {   

        public WordDistance()
        {
        }

        public long FindLeastCommonAncestor(HierarchicalWordData[] words, out int distance, out int lcaDepth, out int depth1, out int depth2)
        {            
            long LCA = -1;
            lcaDepth = -1;
            depth1 = -1;
            depth2 = -1;

            distance = int.MaxValue;
            int i=-1;
            while (++i < 1 && LCA == -1)
            {
                IDictionaryEnumerator trackEnum = words[1 - i].Distance.GetEnumerator();
                if (trackEnum == null) return -1;
                while (trackEnum.MoveNext())
                {
                    int commonAcestor = (int)trackEnum.Key;
                    if (words[i].Distance.ContainsKey(commonAcestor))
                    {
                        int dis_1 = words[i].GetDistance (commonAcestor);
                        int dis_2 = words[1 - i].GetDistance(commonAcestor);

                        int len = dis_1 + dis_2 - 1;
                        if (distance > len)
                        {
                            int lcaDepth_1 = words[i].GetDepth(commonAcestor);
                            int lcaDepth_2 = words[1 - i].GetDepth(commonAcestor);
                            lcaDepth = lcaDepth_1 + lcaDepth_2;
                            depth1 = dis_1 + lcaDepth_1 - 1;
                            depth2 = dis_2 + lcaDepth_2 - 1;
                            distance = len;                            
                            LCA = commonAcestor;
                        }
                    }
                }
            }

            return LCA;
        }

        public float GetSimilarity(HierarchicalWordData word1, HierarchicalWordData word2)
        {
            return GetSimilarity(word1, word2, 2);
        }

        public string MeasureToString(int measure)
        {
            switch (measure)
            {
                case 1: return "Shortest path Length";
                case 2: return "Wu & Palmer";
                case 3: return "Leacock & Chodorow";
                case 4: return "Li 2003 et. al ";                
                default:
                    return string.Empty;
            }
        }
        
        float GetSimilarity(HierarchicalWordData word1, HierarchicalWordData word2, int strategy)
        {
            if (word1.WordInfo.Pos != word2.WordInfo.Pos || word1.WordInfo.Pos == PartsOfSpeech.Unknown) return 0.0F;
            if (word1.WordInfo.Word == word2.WordInfo.Word) return 1.0F;

            int length, lcaDepth, depth1, depth2;
            FindLeastCommonAncestor(new HierarchicalWordData[2] { word1, word2 }, out length, out lcaDepth, out depth1, out depth2);

            if (length == int.MaxValue) return 0.0F;
            if (strategy == 1)//Path Length
            {

                if (length == 0) return 1.0F;
            	else                	
            	{
                    float tmp = 1.0F / (float)length;
                	return (float)Math.Round(tmp, 2);
            	}
            }
            else
                if (strategy == 2) //Wu & Palmer
                {
                    if (length == 0) return 1.0F;
                    else                        
                        {
                            float tmp = (float)(lcaDepth) / (float)(depth1 + depth2);
                            return (float)Math.Round(tmp, 2);
                        }

                }
            
            return 0;
        }


    }
}
