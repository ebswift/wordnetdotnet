using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for FormList.
	/// </summary>
	public class FormList : CollectionBase
	{
		public FormList()
		{
			
		}

		public void Add(Form form)
		{
			if (this.Contains(form))
				throw new FormAlreadyExistsException(form);

			base.InnerList.Add(form);
		}

		public void AddRange(Form[] forms)
		{
			foreach(Form form in forms)
				this.Add(form);
		}

		public void Remove(Form form)
		{
			if (this.Contains(form))
				base.InnerList.Remove(form);
		}

		public bool Contains(Form form)
		{
			foreach(Form f in base.InnerList)
				if (f == form)
					return true;
			return false;
		}

		public Form this[int index]
		{
			get
			{
				return null;
			}
		}
	}

	public class FormAlreadyExistsException : Exception
	{
		protected Form _form;

		public FormAlreadyExistsException(Form form) : base(string.Format("The Form '{0}' already exists in the list.", form.Text))
		{
			_form = form;
		}

		public Form Form
		{
			get
			{
				return _form;
			}
		}
	}
}
