using System;
using System.Collections;
using System.Diagnostics;

namespace Wnlib
{
	public class BitSet // BitArray seems to be bad news, so here goes
	{
		int nbits;
		int size;
		int[] bits;

		public BitSet(int n) { 
			nbits = n; 
			size = (n+31)/32; 
			bits = new int[size];  
		}
		public BitSet(BitSet b) 
		{ 
			nbits = b.nbits;
			size = b.size;
			bits = (int[])b.bits.Clone();
		}
		public bool this[int n] 
		{
			//n 代表 PointerType個數, 因此嘗試改成32
			get { return (bits[n>>5]&(1<<(n&31)))!=0; }
			
			set { int bit = 1<<(n&31);
				if (value) 
					bits[n>>5] |= bit;
				else
					bits[n>>5] &= ~bit;
			}
		}
		public BitSet And (BitSet a)
		{
			Debug.Assert(nbits==a.nbits);
			BitSet r = new BitSet(nbits);
			for (int j=0;j<size;j++)
				r.bits[j] = bits[j]&a.bits[j];
			return r;
		}
		public BitSet Or (BitSet a)
		{
			Debug.Assert(nbits==a.nbits);
			BitSet r = new BitSet(nbits);
			for (int j=0;j<size;j++)
				r.bits[j] = bits[j]|a.bits[j];
			return r;
		}
		public int Card
		{ get {
			int r = 0;
			for (int i=0;i<nbits;i++)
				if (this[i])
					r++;
			  return r;
		  }	}
		public override bool Equals(object o)
		{
			BitSet a = (BitSet) o;
			Debug.Assert(nbits==a.nbits);
			for (int i=0;i<size;i++)
				if (bits[i]!=a.bits[i]) 
					return false;
			return true;
		}
		public override int GetHashCode()
		{
			int n = 0;
			for (int j=0;j<size;j++)
					n += bits[j];
			return n;
		}

		public void display()
		{
			display("");
		}
		public void display(string s)
		{
			Console.Write(s+" ");
			for (int i=0;i<nbits;i++)
				if ((bits[i>>5]&(1<<(i&31)))!=0)
					Console.Write("1");
				else
					Console.Write("0");
			Console.WriteLine();
		}
	}
}
