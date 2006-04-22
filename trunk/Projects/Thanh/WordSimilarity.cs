
/* Compute similarity between two words (IS_A Taxonomy-based approach)
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * (c) Dao Ngoc Thanh, 2005
 * 
 * $Update : 01 Feb 2006
 *  - Add Wu & Palmer similarity measure
 *  - Tested on the dataSet RG (Li 2003, et al )
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
    /// This retrieves data of a word given partOfSpeech and wordSense
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
            switch (pos)
            {
                case Wnlib.PartsOfSpeech.Noun: return IS_A_NOUN;                        
                case Wnlib.PartsOfSpeech.Verb: return IS_A_VERB;                                            
            };

            return null;
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

            if (WordInfo.Pos == Wnlib.PartsOfSpeech.Verb)
            {
                int AA = 0;
            }

            if (se.senses != null)
                Walk(se.senses, null, 1);
            Compute_DepthMatrix();
        }


        void Walk(SynSetList synsets, SynSet fromSS, int depth)
        {
            foreach (SynSet wsense in synsets)
            {                            
                Add_WordSenses(fromSS, wsense, depth);

                if (wsense.senses != null)
                    Walk(wsense.senses, wsense, depth + 1);
            }
            
        }

        void Add_WordSenses(SynSet fromSS, SynSet toSS, int depth)
        {
            SynWord[toSS.hereiam] = toSS.words[0].word;
            if (fromSS != null)
                DepthMatrix[GetKey(fromSS.hereiam , toSS.hereiam)] = 1;
            if (!Distance.ContainsKey(toSS.hereiam))
            { 
                Distance[toSS.hereiam] = depth; 
            }
            else
            {
                int bestDepth = (int)Distance[toSS.hereiam];

                if (bestDepth > depth)
                    Distance[toSS.hereiam] = depth;
            }            
            return;           
        }
        
        /// <summary>
        /// Generate an unique key which denotes the encoded path of going from i upto j
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        object GetKey(object i, object j)
        {            
            double encode_i = Math.Log10(Convert.ToDouble(i));
            double encode_j = Math.Log10(Convert.ToDouble(j));
            //return Convert.ToString(i) + "_" + Convert.ToString(j);             
            return encode_i * 1000.0d + encode_j; 
        }

        //int _rootNode = int.MaxValue;
        int[] _rootNodes;
        void Compute_DepthMatrix()
        {
            ArrayList rootnodes = new ArrayList();           
            foreach (int k in Distance.Keys)            
            {                
                foreach (int i in Distance.Keys) 
                    if (i != k && DepthMatrix.ContainsKey(GetKey(i, k))) 
                {
                    foreach (int j in Distance.Keys) 
                        if (i != j && j != k && DepthMatrix.ContainsKey(GetKey(k, j)))
                    {                         
                        if (!DepthMatrix.ContainsKey(GetKey(i, j))
                             || (int)DepthMatrix[GetKey(i, j)] > (int)DepthMatrix[GetKey(i, k)] + (int)DepthMatrix[GetKey(k, j)])                            
                                DepthMatrix[GetKey(i, j)] = (int)DepthMatrix[GetKey(i, k)] + (int)DepthMatrix[GetKey(k, j)];
                            
                    }
                }
            }

            foreach (int i in Distance.Keys)               
              {
                  bool isRooted = true;
                  foreach (int j in Distance.Keys)
                      if (DepthMatrix.ContainsKey(GetKey(i, j)))
                      {
                          isRooted = false;
                      }

                  if (isRooted) rootnodes.Add(i);
              }

              _rootNodes = (int[])rootnodes.ToArray(typeof(int));
        }



        /// <summary>
        /// Return distance of the entrySynset and the synset "key"
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetDistance(int key)
        {
            if (Distance.ContainsKey(key))            
               return (int)Distance[key];            
            else return -1;
        }

        /// <summary>
        /// Return the depth of a node, which is the shortest path between the root of the 
        /// taxonomy and the synset "key"
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetDepth(int key)
        {
            
            int rootDepth = -2;
            foreach (int rootNode in _rootNodes)
            {
                if (key == rootNode) return 1;
                if (DepthMatrix.ContainsKey(GetKey(key, rootNode)))
                    rootDepth = (int)DepthMatrix[GetKey(key, rootNode)];
            }
            return  rootDepth + 1;
        }

    }
    /// <summary>
    /// Measuring similarity between Words
    /// </summary>
    public class WordSimilarity
    {   

        public WordSimilarity()
        {
        }
        /// <summary>
        /// Return the least common ancestor/subsummer of two words
        /// No unique "join root node" is at present in use. 
        /// </summary>
        /// <param name="words"></param>
        /// <param name="distance"></param>
        /// <param name="lcaDepth"></param>
        /// <param name="depth1"></param>
        /// <param name="depth2"></param>
        /// <returns></returns>
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

        public float GetSimilarity(string word1, string word2)
        {
            Wnlib.PartsOfSpeech[] POSEnum = (Wnlib.PartsOfSpeech[])Enum.GetValues(typeof(Wnlib.PartsOfSpeech));

            float minSim = float.MaxValue;
            for (int partOfSpeech = 1; partOfSpeech < POSEnum.Length; partOfSpeech++)
            {
                HierarchicalWordData data_1 = new HierarchicalWordData(new MyWordInfo(word1, POSEnum[partOfSpeech]));
                HierarchicalWordData data_2 = new HierarchicalWordData(new MyWordInfo(word2, POSEnum[partOfSpeech]));
                float sim=GetSimilarity(data_1, data_2 );
                if (minSim > sim) minSim = sim;
            }
            return minSim;
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
                case 4: return "Li 2003 et. al (IS_A + HAS_A)";                
                default:
                    return string.Empty;
            }
        }
        
        /// <summary>
        /// Return the similarity of two given words with a taxonomy.
        /// </summary>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        float GetSimilarity(HierarchicalWordData word1, HierarchicalWordData word2, int strategy)
        {
            if (word1.WordInfo.Pos != word2.WordInfo.Pos || word1.WordInfo.Pos == PartsOfSpeech.Unknown) return 0.0F;
            if (word1.WordInfo.Word == word2.WordInfo.Word) return 1.0F;

            int pathLength, lcaDepth, depth_1, depth_2;
            FindLeastCommonAncestor(new HierarchicalWordData[2] { word1, word2 }, out pathLength, out lcaDepth, out depth_1, out depth_2);
             
            if (pathLength == int.MaxValue) return 0.0F;
            float sim=0.0F;
            if (strategy == 1)//Path Length
            {

                if (pathLength == 0) return 1.0F;
            	else                	            	
                    sim=1.0F / (float)pathLength;
            }
            else
                if (strategy == 2) //Wu & Palmer
                {
                    if (pathLength == 0) return 1.0F;
                    else                                                
                        sim=(float)(lcaDepth) / (float)(depth_1 + depth_2);                            
                }

            return (float)Math.Round(sim, 2);
        }


    }
}
