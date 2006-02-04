/*
 * Hungarian method matching for maximizing total weights of bipartite grapth
 * Author: Dao Ngoc Thanh , thanh.dao@gmx.net
 * Copyright (c) 2005 Dao Ngoc Thanh
*/

using System;

namespace WordsMatching
{
    /// <summary>
    /// SS
    /// </summary>
    /// 

    public class BipartiteMatcher 
    {
        private string[] _leftTokens, _rightTokens;
        private float[][] _costMatrix;
        private float[] _leftLabel, _rightLabel;
        private int[] _previous, _incoming, _outgoing;

        private bool[] _leftVisited, _rightVisited;
        int leftLen, rightLen;
        bool _errorOccured = false;
        const int ROUND_LEN = 3;

        public BipartiteMatcher(string[] left, string[] right, float[][] simMatrix)
        {
            if (left == null || right == null || simMatrix == null)
            {
                _errorOccured = true;
                return;
            }

            _leftTokens = left;
            _rightTokens = right;

            if (_leftTokens.Length > _rightTokens.Length)
            {
                _costMatrix = Transpose(simMatrix);

                string[] temp = _leftTokens;
                _leftTokens = _rightTokens;
                _rightTokens = temp;
            }
            else
                _costMatrix = (float[][])simMatrix.Clone();



            MyInit();

            Make_Matching();
        }

        public static float[][] Transpose(float[][] matrix)
        {
            if (matrix == null)
                return null;
            int m = matrix.Length;
            int n = matrix[0].Length;
            float[][] transMatrix = new float[n][];

            for (int i = 0; i < n; i++)
                transMatrix[i] = new float[m];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                    transMatrix[j][i] = matrix[i][j];
            }

            return transMatrix;
        }


        private void MyInit()
        {
            Initialize();

            _leftVisited = new bool[leftLen + 1];
            _rightVisited = new bool[rightLen + 1];
            _previous = new int[(leftLen + rightLen) + 2];

        }

        private void Initialize()
        {
            leftLen = _leftTokens.Length - 1;
            rightLen = _rightTokens.Length - 1;

            _leftLabel = new float[leftLen + 1];
            _rightLabel = new float[rightLen + 1];
            for (int i = 0; i < _leftLabel.Length; i++) _leftLabel[i] = 0;
            for (int i = 0; i < _rightLabel.Length; i++) _rightLabel[i] = 0;


            for (int i = 0; i <= leftLen; i++)
            {
                float maxLeft = float.MinValue;
                for (int j = 0; j <= rightLen; j++)
                {
                    _costMatrix[i][j] = (float)Math.Round(_costMatrix[i][j], 2);
                    if (_costMatrix[i][j] > maxLeft) maxLeft = _costMatrix[i][j];
                }

                _leftLabel[i] = maxLeft;
            }

        }

        private void Flush()
        {
            for (int i = 0; i < _previous.Length; i++) _previous[i] = -1;
            for (int i = 0; i < _leftVisited.Length; i++) _leftVisited[i] = false;
            for (int i = 0; i < _rightVisited.Length; i++) _rightVisited[i] = false;
        }

        bool stop = false;
        bool FindPath(int source)
        {
            Flush();
            stop = false;
            Walk(source);
            return stop;

        }

        void Increase_MapPairs(int prev_i, int prev_j)
        {
            int[] tempOut = (int[])_outgoing.Clone();
            int i, j, k;
            i = prev_i; j = prev_j;
            _outgoing[i] = j; _incoming[j] = i;
            if (_previous[i] != -1)
            {
                do
                {
                    j = tempOut[i];
                    k = _previous[i];
                    _outgoing[k] = j; _incoming[j] = k;
                    i = k;
                } while (_previous[i] != -1);
            }
        }


        void Walk(int i)
        {
            _leftVisited[i] = true;

            for (int j = 0; j <= rightLen; j++)
                if (stop) return;
                else
                {
                    float tmp = (float)Math.Round(_leftLabel[i] + _rightLabel[j], 2);
                    if (!_rightVisited[j] && (tmp == _costMatrix[i][j]))
                    {
                        if (_incoming[j] == -1)
                        {
                            stop = true;
                            Increase_MapPairs(i, j);
                            return;
                        }
                        else
                        {
                            int k = _incoming[j];
                            _rightVisited[j] = true;
                            _previous[k] = i;
                            Walk(k);
                        }
                    }
                }
        }

        float GetMinDeviation()
        {
            float min = float.MaxValue;

            for (int i = 0; i <= leftLen; i++)
                if (_leftVisited[i])
                {
                    for (int j = 0; j <= rightLen; j++)
                        if (!_rightVisited[j])
                        {
                            float tmp = (float)Math.Round(_leftLabel[i] + _rightLabel[j], 2);
                            if (tmp - _costMatrix[i][j] < min)
                                min = tmp - _costMatrix[i][j];
                        }
                }

            return min;
        }

        void Relabels()
        {
            float dev = GetMinDeviation();

            for (int k = 0; k <= leftLen; k++)
                if (_leftVisited[k])
                {
                    _leftLabel[k] -= dev;
                    _leftLabel[k] = (float)Math.Round(_leftLabel[k], 2);
                }
            for (int k = 0; k <= rightLen; k++)
                if (_rightVisited[k])
                {
                    _rightLabel[k] += dev;
                    _rightLabel[k] = (float)Math.Round(_rightLabel[k], 2);
                }
        }

        void Make_Matching()
        {
            _outgoing = new int[leftLen + 1];
            _incoming = new int[rightLen + 1];
            for (int i = 0; i < _outgoing.Length; i++) _outgoing[i] = -1;
            for (int i = 0; i < _incoming.Length; i++) _incoming[i] = -1;

            for (int k = 0; k <= leftLen; k++)
                if (_outgoing[k] == -1)
                {
                    bool found = false;
                    do
                    {
                        found = FindPath(k);
                        if (!found) Relabels();

                    } while (!found);
                }
        }


        public float[] GetMapPairWeights()
        {
            float[] selWeights = new float[leftLen + 1];
            for (int i = 0; i <= leftLen; i++)
                if (_outgoing[i] != -1)
                {
                    selWeights[i] = _costMatrix[i][_outgoing[i]];
                }

            return selWeights;
        }


        public int[] GetMapPairs()
        {
            return _outgoing;
        }

        public float GetScore()
        {
            float dis = 0;

            float maxLen = rightLen + 1;
            if (maxLen > 0)
                return dis / maxLen;
            else
                return 1.0F;
        }


        public float Score
        {
            get
            {
                if (_errorOccured) return 0;
                else
                    return GetScore();
            }
        }

    }
}
