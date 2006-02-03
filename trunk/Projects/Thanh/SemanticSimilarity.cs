/*
 Semantic between two phrasals/sentences
 (disregards PartOfSpeech tagging and WordSenseDisambiguation)
 Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
 Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;

namespace WordsMatching
{
	/// <summary>
	/// Measuring relationship between two given sentences
	/// </summary>
	public class SemanticSimilarity
	{
		private int[] _senses1, _senses2;
        float[,] _similarity;

		string[] _source, _target;
		private int m, n;

		public SemanticSimilarity()
		{
			StopWordsHandler stopword=new StopWordsHandler() ;
		}

        private MyWordInfo[] Disambiguate(string[] words)
        {
            if (words.Length == 0) return null;

            MyWordInfo[] wordInfos=new MyWordInfo [words.Length];
            
            for (int i = 0; i < words.Length; i++)
            {
                
                WnLexicon.WordInfo wordInfo = WnLexicon.Lexicon.FindWordInfo(words[i], true);

                if (wordInfo.partOfSpeech != Wnlib.PartsOfSpeech.Unknown)
                {
                    if (wordInfo.text != string.Empty)
                        words[i] = wordInfo.text;

                    Wnlib.PartsOfSpeech[] posEnum = (Wnlib.PartsOfSpeech[])Enum.GetValues(typeof(Wnlib.PartsOfSpeech));

                    for (int j = 0; j < posEnum.Length; j++)
                    {
                        if (wordInfo.senseCounts[j] > 0) // get the first part of speech
                        {
                            wordInfos[i] = new MyWordInfo(words[i], posEnum[j]);                             
                            break;
                        }
                    }
                }
            }

            WordSenseDisambiguator wsd = new WordSenseDisambiguator();
            wordInfos=wsd.Disambiguate(wordInfos);

            return wordInfos;
        }

        MyWordInfo[] _myWordsInfo1, _myWordsInfo2;

        //private void MyInitOld()
        //{
        //    _myWordsInfo1 = Disambiguate(_source);
        //    _myWordsInfo2 = Disambiguate(_target);

        //    m = _myWordsInfo1.Length; n = _myWordsInfo2.Length;
        //    _similarity =new float[m, n] ;

        //    for (int i=0; i < m; i++)
        //    {
        //        _myWordsInfo1[i].Sense = _myWordsInfo1[i].Sense < 0 ? 0 : _myWordsInfo1[i].Sense;                

        //        string word1 = _source[i];
        //        for (int j=0; j < n; j++)
        //        {
        //            _myWordsInfo2[i].Sense = _myWordsInfo2[i].Sense < 0 ? 0 : _myWordsInfo2[i].Sense;					

        //            string word2=_target[j];
        //            WordDistance distance = new WordDistance();
        //            float weight = distance.GetSimilarity(_myWordsInfo1[i], _myWordsInfo2[j]);					

        //            _similarity[i, j]=weight;					
        //        }
        //    }
        //}



        float[][] _simMatrix;
        private void MyInit()
        {
            m = _source.Length; n = _target.Length;

            _simMatrix = new float[m][];            
            _myWordsInfo1 = new MyWordInfo [m];
            _myWordsInfo2 = new MyWordInfo [n];
            
            Wnlib.PartsOfSpeech[] posEnum = (Wnlib.PartsOfSpeech[])Enum.GetValues(typeof(Wnlib.PartsOfSpeech));
        	HierarchicalWordData[][] Data1 = new HierarchicalWordData[m][];
        	HierarchicalWordData[][] Data2 = new HierarchicalWordData[n][];

            for (int i = 0; i < m; i++)             
            {
                _simMatrix[i] = new float[n];
            	Data1[i] = new HierarchicalWordData[posEnum.Length];
                for (int pos1 = 1; pos1 < posEnum.Length; pos1++)
                {
                    _myWordsInfo1[i] = new MyWordInfo(_source[i], posEnum[pos1]);

                    if (Data1[i][pos1] == null)
                    {
                    	Data1[i][pos1] = new HierarchicalWordData(_myWordsInfo1[i]);
                    }
   
                     _myWordsInfo1[i].Sense = 0;
                     for (int j = 0; j < n; j++)
                     {
                         if (Data2[j] == null)
                         	Data2[j] = new HierarchicalWordData[posEnum.Length];
                        float synDist = AcronymChecker.GetEditDistanceSimilarity(_source[i], _target[j]);

                        for (int pos2 = 1; pos2 < posEnum.Length ; pos2++)
                         {
                              _myWordsInfo2[j] = new MyWordInfo(_target[j], posEnum[pos2]);

                              if (Data2[j][pos2] == null)
                              {
                              	Data2[j][pos2] = new HierarchicalWordData(_myWordsInfo2[j]);
                              }

                              _myWordsInfo2[j].Sense = 0;
                              SimilarityMeasure wordDistance= new SimilarityMeasure();
                              float semDist = wordDistance.GetSimilarity(Data1[i][pos1], Data2[j][pos2]);                              
                              float weight=Math.Max (synDist, semDist);

                              if (_simMatrix[i][j] < weight)
                                  _simMatrix[i][j] = weight;
                                        
                          }
                      }
                  }                  
               }            

        }

        public float GetScore(string string1, string string2)		
		{			
			Tokeniser tok=new Tokeniser() ;
            tok.UseStemming = false;

			_source=tok.Partition(string1) ;
			_target=tok.Partition(string2) ;

			if (_source.Length == 0 || _target.Length == 0 )
				return 0F;
			
			MyInit();		
			HeuristicMatcher match=new HeuristicMatcher() ;
            //float score = HeuristicMatcher.ComputeSetSimilarity(_simMatrix, 2, 0.3F);
            float score = HeuristicMatcher.ComputeSetSimilarity(_simMatrix, 1);
			return score;	
		}
	}
}
