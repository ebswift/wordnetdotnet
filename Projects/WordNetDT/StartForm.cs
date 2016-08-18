using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using Microsoft.VisualBasic;
//Imports Wnlib
using WordNetClasses;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WordNet.wnb
{

	public partial class StartForm : System.Windows.Forms.Form
	{
        WordNetClasses.WN wnc = new WordNetClasses.WN("..\\..\\..\\..\\WordNet\\dict\\");

        [STAThread]
		public static void Main(string[] args)
		{
			//Try
			Application.EnableVisualStyles();
			Application.DoEvents();
			Application.Run(new StartForm());
			//Catch ex As Exception
			//    MessageBox.Show(ex.Message)
			//End Try
		}


		private void Form1_Load(System.Object sender, System.EventArgs e)
		{
			f3 = new AdvancedOptions();
			LoadAbout();
			LoadHistory();
		}

		private void LoadAbout()
		{
			// load the 'about' text file
			System.IO.StreamReader myFile = new System.IO.StreamReader(MyPath() + "\\Intro.htm");
			string mystring = myFile.ReadToEnd();

			myFile.Close();

			//            TextBox3.Text = mystring

			//mystring = Replace(mystring, vbCrLf, "<br>")
			//HtmlViewer1.Navigate("about:blank")
			//Application.DoEvents()
			//HtmlViewer1.Document.Write(mystring)
			showFeedback(mystring, false);
		}

		private string MyPath()
		{
			//get the app path
			string fullAppName = Assembly.GetExecutingAssembly().GetName().CodeBase;
			//This strips off the exe name
			string FullAppPath = Path.GetDirectoryName(fullAppName);

			FullAppPath = Strings.Mid(FullAppPath, Strings.Len("file:\\\\"));

			// following is only during testing
			#if (DEBUG == true)
			FullAppPath = Strings.Mid(FullAppPath, 1, Strings.InStrRev(FullAppPath, "\\"));
			#endif


			return FullAppPath;
		}

		//Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
		//    Dim t As String
		//    If TextBox1.Lines.Length > 1 Then
		//        t = TextBox1.Lines(0)
		//        Label2.Text = "Searches for " + t + ":"
		//        Label2.Visible = True
		//        Overview()
		//    End If
		//End Sub

		private void Overview()
		{
            //overview for 'search'
            string t = null;

			t = TextBox1.Text;
			Label2.Text = "Searches for " + t + ":";
			Label2.Visible = true;
			Button1.Visible = false;

			//            TextBox3.Text = ""
			//HtmlViewer1.Document.close()
			//TextBox3.Visible = False
			//TextBox1.Enabled = False
			//lblWait.Visible = True
			// Timer1.Enabled = True
			StatusBar1.Text = "Overview of " + t;
			Refresh();

			try {
				// sets the visibility of noun, verb, adj, adv when showing buttons for a word

				//Stop

				list = new ArrayList();

                bool b = true;
                wnc.OverviewFor(t, "noun", ref b, ref bobj2, list);
				Button2.Visible = b;

                b = true;
                wnc.OverviewFor(t, "verb", ref b, ref bobj3, list);
				Button3.Visible = b;

                b = true;
                wnc.OverviewFor(t, "adj", ref b, ref bobj4, list);
				Button4.Visible = b;

                b = true;
                wnc.OverviewFor(t, "adv", ref b, ref bobj5, list);
				Button5.Visible = b;

				TextBox1.Text = t;
				//btnSearch.Visible = False
				TextBox2.Text = "0";
				AddHistory(new wnHistory(Strings.Replace(t, " ", "_"), Wnlib.Opt.at(Wnlib.Opt.Count - 1), 0));
				//            history.Add(New wnHistory(t, Wnlib.Opt.at(Wnlib.Opt.Count - 1), 0))
			} catch (System.Exception ex) {
				MessageBox.Show(ex.Message);
			}

			FixDisplay();
		}

		private void DoSearch(Wnlib.Opt opt)
		{
			if (opt.sch.ptp.mnemonic == "OVERVIEW") {
				Overview();
				return;
			}

            //TextBox3.Text = ""
            //HtmlViewer1.Document.Window.Close();
            //HtmlViewer1.Navigate("about:blank");
            HtmlViewer1.Document.OpenNew(false);
            HtmlViewer1.Visible = false;
			//TextBox1.Enabled = False
			//lblWait.Visible = True
			//Timer1.Enabled = True
			Refresh();

			list = new ArrayList();
			Wnlib.Search se = new Wnlib.Search(TextBox1.Text, true, opt.pos, opt.sch, int.Parse(TextBox2.Text));
			int a = se.buf.IndexOf("\n");
			if ((a >= 0)) {
				if ((a == 0)) {
					se.buf = se.buf.Substring(a + 1);
					a = se.buf.IndexOf("\n");
				}
				StatusBar1.Text = se.buf.Substring(0, a);
				se.buf = se.buf.Substring(a + 1);
			}
			AddHistory(new wnHistory(Strings.Replace(TextBox1.Text, " ", "_"), opt, int.Parse(TextBox2.Text)));
			//        history.Add(New wnHistory(TextBox1.Text, opt, Integer.Parse(TextBox2.Text)))
			list.Add(se);
			if ((Wnlib.WNOpt.opt("-h").flag)) {
				help = new Wnlib.WNHelp(opt.sch, opt.pos).help;
			}
			FixDisplay();
		}

		public void CantFindDictionary()
		{
			Interaction.MsgBox("Error loading dictionary.  Click OK, then locate the WordNet dictionary.");

			//Menudictpath_Click(Nothing, Nothing)
		}

		ArrayList list = new ArrayList();

		string help = "";
		public void FixDisplay()
		{
			//Try
			pbobject = "";
			ShowResults();

			TextBox1.Focus();

			//            tb.useList(g, wnc.list, wnc.help, tmpstr)

			//Catch ex As Exception
			//MsgBox("FixDisplay: " & ex.Message)
			//End Try
		}

		//ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)
		private void ShowResults()
		{
			string tmpstr = "";

			//Try
			if (list.Count == 0) {
				showFeedback("Search for " + TextBox1.Text + " returned 0 results.", true);
				return;
			}

			//Dim g As Graphics = e.Graphics()
			// this exists only for the type comparison
			//Dim tmptbox As WNBTBox.TBox = New WNBTBox.TBox(Nothing, Nothing)
			Overview tmptbox = new Overview();

			// this type comparison should be fixed, the tmptbox is a waste of resources
			//If Not Object.ReferenceEquals(PictureBox1.Tag.GetType, tmptbox.GetType) Then
			if ((!object.ReferenceEquals(pbobject.GetType(), tmptbox.GetType()))) {
				//Dim tb As WNBTBox.TBox = New WNBTBox.TBox(5, 5)
				Overview tb = new Overview();
				//TextBox3.Text = ""
				tb.useList(list, help, ref tmpstr);
				if ((help != null) & !string.IsNullOrEmpty(help)) {
					tmpstr = "<p>" + Strings.Replace(help, "vbcrlf", "<br />") + "</p>" + tmpstr;
				}
				tmpstr = Strings.Replace(tmpstr, Constants.vbLf, Constants.vbCrLf);
				tmpstr = Strings.Replace(tmpstr, Constants.vbCrLf, "", 1, 1);
				tmpstr = Strings.Replace(tmpstr, "_", " ");
				showFeedback(tmpstr, true);

				//HtmlViewer1.Document.close()
				//HtmlViewer1.Document.Write(tmpstr)
				//objdoc.write(tmpstr)
				//TextBox3.Text = tmpstr
				if (string.IsNullOrEmpty(tmpstr) | tmpstr == "<font color='green'><br />" + Constants.vbCr + " " + TextBox1.Text + " has no senses </font>") {
					showFeedback("Search for " + TextBox1.Text + " returned 0 results.", true);
				}
				HtmlViewer1.Visible = true;
				//TextBox1.Enabled = True
				//HtmlViewer1.SelectionLength = 0
				//Timer1.Enabled = False
				pbobject = tb;
			}

			TextBox1.Focus();
			//Dim t As WNBTBox.TBox = pbobject 'CType(pbobject, WNBTBox.TBox)
			//t.paint(g, False)
			//Catch ex As Exception
			//MsgBox("ShowResults: " & ex.Message)
			//End Try
		}


		#region "History"
		// the history list
		public class wnHistory
		{
			public string word;
			public Wnlib.Opt opt;

			public int sn;
			public wnHistory(string w, Wnlib.Opt o, int s)
			{
				word = w;
				opt = o;
				sn = s;
			}
		}

		private void AddHistory(wnHistory his)
		{
			// adds a new item to history, and appends a menu item
			history.Add(his);

			MenuItem mi = mnuHistory;
			mi.MenuItems.Clear();
			int x = 0;
			int i = 0;

			if ((history.Count > maxhistory)) {
				x = history.Count - maxhistory;
			}
			if ((history.Count == 0)) {
				MenuItem mitem = new MenuItem();
				// (opt.label, AddressOf searchMenu_Click)
				mitem.Text = "<empty>";
				mi.MenuItems.Add(mitem);
				return;
			}

			for (i = x; i <= history.Count - 1; i++) {
				wnHistory h = (wnHistory)history[i];
				// + x)
				string t = h.opt.label.ToString() + " for " + Strings.Chr(34) + h.word + Strings.Chr(34);
				if ((h.sn > 0)) {
					t += " sense " + h.sn.ToString();
				}
				MenuItem mitem = new MenuItem();
				// (opt.label, AddressOf searchMenu_Click)
				mitem.Text = Strings.Replace(t, "_", " ");
				mi.MenuItems.Add(mitem);
				mitem.Click += History_Click;
			}
		}

		private void History_Click(object sender, System.EventArgs e)
		{
			// an item on the History menu was clicked
			MenuItem mi = (MenuItem)sender;
			int x = 0;
			int i = 0;

			if ((history.Count > maxhistory)) {
				x = history.Count - maxhistory;
			}

			wnHistory h = null;

			//        For i = x To mi.Parent.MenuItems.Count - 1
			for (i = 0; i <= mi.Parent.MenuItems.Count - 1; i++) {
				if (mi.Text == mi.Parent.MenuItems[i].Text) {
					h = (wnHistory)history[i + x];
				}
			}

			TextBox1.Text = Strings.Replace(h.word, "_", " ");
			TextBox2.Text = "" + h.sn.ToString();
			DoSearch(h.opt);
		}

		private void LoadHistory()
		{
			System.IO.StreamReader f = null;

			try {
				f = new System.IO.StreamReader(MyPath() + "\\history.txt");
			} catch (System.Exception ex) {
				return;
				// no history file to load
			}

			try {
				string s = null;

				while (1 == 1) {
					s = f.ReadLine();
					if (s == null) {
						break; // TODO: might not be correct. Was : Exit While
					}

					Wnlib.StrTok st = new Wnlib.StrTok(s);
					string w = st.next();
					int i = int.Parse(st.next());
					int sn = int.Parse(st.next());

					Wnlib.Opt opt = Wnlib.Opt.at(i);
					AddHistory(new wnHistory(Strings.Replace(w, " ", "_"), opt, sn));
					//                history.Add(New wnHistory(w, opt, sn))
				}
				f.Close();
			} catch (System.Exception ex) {
				Interaction.MsgBox(ex.Message);
			}

			if ((history.Count == 0)) {
				MenuItem mi = mnuHistory;
				MenuItem mitem = new MenuItem();
				// (opt.label, AddressOf searchMenu_Click)
				mitem.Text = "<empty>";
				mi.MenuItems.Add(mitem);
				return;
			}
		}

		private void SaveHistory()
		{
			try {
				System.IO.StreamWriter f = new System.IO.StreamWriter(MyPath() + "\\history.txt");
				int x = 0;
				int i = 0;

				if (history.Count > maxhistory) {
					x = history.Count - maxhistory;
				}

				for (i = x; i <= history.Count - 1; i++) {
					wnHistory h = (wnHistory)history[i];
					// + x)
					f.WriteLine(Strings.Replace(h.word, " ", "_") + " " + h.opt.id.ToString() + " " + h.sn.ToString());
				}
				f.Close();
			} catch {
			}
		}
		#endregion

		public Wnlib.SearchSet bobj2;
		public Wnlib.SearchSet bobj3;
		public Wnlib.SearchSet bobj4;

		public Wnlib.SearchSet bobj5;
		private void btnSearch_Click(System.Object sender, System.EventArgs e)
		{
			Overview();
			TextBox1.Focus();
		}

		private void TextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				e.Handled = true;
				btnSearch_Click(null, null);
			}
		}

		//Private Sub TextBox3_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox3.KeyDown
		//    e.Handled = True
		//End Sub


		ArrayList opts = null;
		private void searchMenu_Click(object sender, System.EventArgs e)
		{
			// one of the options for button2_click was selected
			MenuItem mi = (MenuItem)sender;
			Wnlib.Opt opt = null;
			//= opts(mi.MenuItems.IndexOf(mi))
			int i = 0;
			string tmpstr = null;

            //System.Windows.Forms.Application.DoEvents()
            //TextBox3.Text = ""
            //HtmlViewer1.Document.Window.Close();
            //HtmlViewer1.Navigate("about:blank");
            HtmlViewer1.Document.OpenNew(false);
            HtmlViewer1.Visible = false;
			//TextBox1.Enabled = False
			//lblWait.Visible = True
			//Timer1.Enabled = True
			tmpstr = mi.Text;
			tmpstr = Strings.Replace(tmpstr, "Syns", "Synonyms");
			tmpstr = Strings.Replace(tmpstr, " x ", " by ");
			tmpstr = Strings.Replace(tmpstr, "Freq:", "Frequency:");
			StatusBar1.Text = tmpstr;
			Refresh();

			for (i = 0; i <= mi.Parent.MenuItems.Count - 1; i++) {
				if (mi.Text == mi.Parent.MenuItems[i].Text) {
					opt = (Wnlib.Opt)opts[i];
				}
			}
			DoSearch(opt);
			Button1.Visible = true;

			//System.Windows.Forms.Application.DoEvents()
			HtmlViewer1.Visible = true;
			//TextBox1.Enabled = True
			//Timer1.Enabled = False
			Refresh();
		}

		private void MenuItem13_Click(System.Object sender, System.EventArgs e)
		{
            MenuItem mi = (MenuItem)sender;

            mi.Checked = !mi.Checked;

			showFeedback(rawFeedback, true);
		}

		private void MenuItem10_Click(System.Object sender, System.EventArgs e)
		{
            MenuItem mi = (MenuItem)sender;
            Wnlib.WNOpt.opt("-h").flag = mi.Checked == (!mi.Checked);
		}

		private void MenuItem11_Click(System.Object sender, System.EventArgs e)
		{
            MenuItem mi = (MenuItem)sender;
            mi.Checked = !mi.Checked;
			Wnlib.WNOpt.opt("-g").flag = !mi.Checked;
		}

		private void MenuItem12_Click(System.Object sender, System.EventArgs e)
		{
			// advanced options menu
			//f2.Caller = Me
			f3.ShowDialog();
		}

		private void MenuItem2_Click(System.Object sender, System.EventArgs e)
		{
			Wildcard wc = new Wildcard();

			wc.Caller = this;

			wc.ShowDialog();
		}

		private void StartForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveHistory();

			f3 = null;
			// terminate our advanced options form
		}

		private void Button1_Click(System.Object sender, System.EventArgs e)
		{
			Overview();
		}

		private void MenuItem9_Click(System.Object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void MenuItem8_Click(System.Object sender, System.EventArgs e)
		{
            //TextBox3.Text = ""
            //HtmlViewer1.Document.Window.Close();
            //HtmlViewer1.Navigate("about:blank");
            HtmlViewer1.Document.OpenNew(false);
            TextBox1.Text = "";
			Label2.Text = "";
			StatusBar1.Text = "WordNetDT";
			Button2.Visible = false;
			Button3.Visible = false;
			Button4.Visible = false;
			Button5.Visible = false;
			Button1.Visible = false;
			btnSearch.Visible = true;
		}

		private void MenuItem18_Click(System.Object sender, System.EventArgs e)
		{
			LoadAbout();
		}

		private void MenuItem5_Click(System.Object sender, System.EventArgs e)
		{
			System.IO.StreamReader myFile = new System.IO.StreamReader(MyPath() + "\\wnb.txt");
			string mystring = myFile.ReadToEnd();

			myFile.Close();

			//TextBox3.Text = mystring
			mystring = Strings.Replace(mystring, Constants.vbCrLf, "<br>");
			showFeedback(mystring, true);

			//HtmlViewer1.Document.close()
			//HtmlViewer1.Document.Write(mystring)
		}

		private void MenuItem14_Click(System.Object sender, System.EventArgs e)
		{
			System.IO.StreamReader myFile = new System.IO.StreamReader(MyPath() + "\\WordNetGlossary.txt");
			string mystring = myFile.ReadToEnd();

			myFile.Close();

			//TextBox3.Text = mystring
			mystring = Strings.Replace(mystring, Constants.vbCrLf, "<br>");
			showFeedback(mystring, true);

			//HtmlViewer1.Document.close()
			//HtmlViewer1.Document.Write(mystring)
		}

		private void MenuItem15_Click(System.Object sender, System.EventArgs e)
		{
			System.IO.StreamReader myFile = new System.IO.StreamReader(MyPath() + "\\LICENSE.txt");
			string mystring = myFile.ReadToEnd();

			myFile.Close();

			//TextBox3.Text = mystring
			mystring = Strings.Replace(mystring, Constants.vbCrLf, "<br>");
			showFeedback(mystring, true);

			//HtmlViewer1.Document.close()
			//HtmlViewer1.Document.Write(mystring)
		}

		private void MenuItem16_Click(System.Object sender, System.EventArgs e)
		{
			System.IO.StreamReader myFile = new System.IO.StreamReader(MyPath() + "\\WordNetDTLicense.txt");
			string mystring = myFile.ReadToEnd();

			myFile.Close();

			//TextBox3.Text = mystring
			mystring = Strings.Replace(mystring, Constants.vbCrLf, "<br>");
			showFeedback(mystring, true);

			//HtmlViewer1.Document.close()
			//HtmlViewer1.Document.Write(mystring)
		}

		private void MenuItem7_Click(System.Object sender, System.EventArgs e)
		{
			SaveFileDialog1.FileName = TextBox1.Text;
			if ((SaveFileDialog1.ShowDialog() == DialogResult.OK)) {
				StreamWriter f = new StreamWriter(SaveFileDialog1.FileName, false);

				f.Write(HtmlViewer1.Document.Body.InnerText);
				f.Close();
			}
		}

		private void showFeedback(string mystring, bool reformat)
		{
			if (reformat) {
				string headstyle = null;
				// define the size of the table used to counter word wrapping
				string nowraptbl = "<TABLE id=table1 width=10000 border=0><br><TBODY><br><TR><br><TD>";
				// define the closing of the above table
				string closetbl = "</TD></TR></TBODY></TABLE>";

				//body.innerHTML = "<html><body>hello world</body></html>"


				headstyle = "<style>" + Constants.vbCrLf + "<!--" + Constants.vbCrLf + "*\t{ font-family:'Verdana'; font-size:10pt }" + Constants.vbCrLf + ".Word   { color: #0000FF; font-weight:bold }" + Constants.vbCrLf + ".Word a { color: #0000FF; font-weight:bold; text-decoration: none }" + Constants.vbCrLf + ".Type        { font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080 }" + Constants.vbCrLf + ".Defn   { color: #000000 }" + Constants.vbCrLf + ".Defn a { color: #000000; text-decoration: none }" + Constants.vbCrLf + ".Quote   { color: #800000; font-style:italic }" + Constants.vbCrLf + ".Quote a { color: #800000; text-decoration: none }" + Constants.vbCrLf + "a \t\t{ text-decoration: none }" + Constants.vbCrLf + "-->" + Constants.vbCrLf + "</style>" + Constants.vbCrLf + Constants.vbCrLf;

				mystring = Strings.Replace(mystring, Constants.vbCrLf, "<br>");
                //HtmlViewer1.Document.Window.Close();
                //HtmlViewer1.Navigate("about:blank");
                HtmlViewer1.Document.OpenNew(false);
                HtmlViewer1.Document.Write(headstyle);

				// word wrapping is off, so place inside a very large table
				if (!MenuItem13.Checked) {
					HtmlViewer1.Document.Write(nowraptbl);
				}

				// write the output
				HtmlViewer1.Document.Write(mystring);

				rawFeedback = mystring;

				// if word wrapping is off then close our large table
				if (!MenuItem13.Checked) {
					HtmlViewer1.Document.Write(closetbl);
				}
			} else {
                //HtmlViewer1.Document.Window.Close();
                //HtmlViewer1.Navigate("about:blank");
                HtmlViewer1.Document.OpenNew(false);
                HtmlViewer1.Document.Write(mystring);
			}

			TextBox1.Focus();
		}
        
		private void HtmlViewer1_BeforeNavigate2(object sender, WebBrowserNavigatingEventArgs e) // AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2Event e)
		{
			string tmpstr = null;
            tmpstr = e.Url.ToString();
			tmpstr = Strings.Replace(tmpstr, "about:blank", "");
			if (string.IsNullOrEmpty(tmpstr)) {
				return;
			}
            e.Cancel = true;

			StringWriter myWriter = new StringWriter();
			// Decode the encoded string.

			TextBox1.Text = Strings.Replace(tmpstr, "%20", " ").Replace("about:", "");
			btnSearch_Click(null, null);
		}
        
		private void BtnAdvancedClick(System.Object sender, System.EventArgs e)
		{
			MenuItem2_Click(null, null);
		}

		private void Button2Click(System.Object sender, System.EventArgs e)
		{
			// handles noun, verb, adj, adverb click for context menu
			Button b = (Button)sender;
			Wnlib.SearchSet ss = null;
			//= bobj2
			string btext = b.Text;

			if (btext == "Adjective") {
				btext = "Adj";
			}

			switch (btext) {
				case "Noun":
					ss = (Wnlib.SearchSet)bobj2;

					break;
				case "Verb":
					ss = (Wnlib.SearchSet)bobj3;

					break;
				case "Adj":
					ss = (Wnlib.SearchSet)bobj4;

					break;
				case "Adverb":
					ss = (Wnlib.SearchSet)bobj5;
					break;
			}
			Wnlib.PartOfSpeech pos = Wnlib.PartOfSpeech.of(btext.ToLower());
			int i = 0;
			opts = new ArrayList();
			ContextMenu cm = new ContextMenu();
			ArrayList tmplst = new ArrayList();

			for (i = 0; i <= Wnlib.Opt.Count - 1; i++) {
                Wnlib.Opt opt = Wnlib.Opt.at(i);

				//Try ' TODO: fix problem with adjective menu
				if (ss[opt.sch.ptp.ident] & object.ReferenceEquals(opt.pos, pos)) {
					if (tmplst.IndexOf(opt.label) == -1 & opt.label != "Grep") {
						MenuItem mi = new MenuItem();
						// (opt.label, AddressOf searchMenu_Click)
						mi.Text = opt.label;
						mi.Click += searchMenu_Click;
						opts.Add(opt);
						cm.MenuItems.Add(mi);

						tmplst.Add(opt.label);
					}
				}
				//Catch
				//End Try
			}

			cm.Show(b.Parent, new System.Drawing.Point(b.Left, b.Bottom));
			//Point(b.Left, b.Bottom))
		}

		private void Button3Click(System.Object sender, System.EventArgs e)
		{
			Button2Click(sender, null);
		}

		private void Button4Click(System.Object sender, System.EventArgs e)
		{
			Button2Click(sender, null);
		}

		private void Button5Click(System.Object sender, System.EventArgs e)
		{
			Button2Click(sender, null);
		}

	}
}
