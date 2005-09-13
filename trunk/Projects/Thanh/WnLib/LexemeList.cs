using System;
using System.Collections;
using Wnlib;

namespace Wnlib
{
	/// <summary>
	/// 
	/// </summary>	
	public class LexemeList : CollectionBase
	{
		public LexemeList()
		{

		}

		~LexemeList()
		{

		}

		public int Add(Lexeme item)
		{
			
			return List.Add(item);
		}
		public void Insert(int index, Lexeme item)
		{
			List.Insert(index, item);
		}
		public void Remove(Lexeme item)
		{
			List.Remove(item);
		} 
		public bool Contains(Lexeme item)
		{
			return List.Contains(item);
		}
		public int IndexOf(Lexeme item)
		{
			return List.IndexOf(item);
		}
		public void CopyTo(Lexeme[] array, int index)
		{
			List.CopyTo(array, index);
			
		}
		
		public Lexeme this[int index]
		{
			get { return (Lexeme)List[index]; }
			set { List[index] = value; }
		}

		public virtual void Dispose()
		{
			
		}
	}
}
