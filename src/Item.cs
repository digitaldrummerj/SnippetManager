using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetManager
{
	public class Item
	{
		public string FullText = "";
		public string ShortText = "";

		public Item() { }
		public Item(string fullText)
		{
			this.FullText = fullText;
		}

		// used by ListBox
		public override string ToString()
		{
			return this.ShortText;
		}

	}
}
