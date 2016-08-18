/*
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Metaphone;

namespace WordNet
{
    public partial class Wildcard
    {
		private int doWnGrep(Wnlib.PartOfSpeech pos)
		{
			if (radWildcard.Checked | radRegularExpression.Checked) {
				return WildCardRegex(pos);
			} else if (radSoundsLike.Checked) {
				return soundsLike(pos);
			} else {
				return AnagramScrabble(pos);
			}
			//ListBox1.Sorted = True
		}

		private int soundsLike(Wnlib.PartOfSpeech pos)
		{
			int i = 0;
			int j = 0;
			int x = 0;
			int deccount = 0;
			string tmpstr = "";

			// asc 201 specifies a soundslike
			// adding the trackbar value produces the strength of "sounds like" match
			// between 1 and 3
			//tmpstr = TextBox1.Text & TrackBar1.Value & Chr(201)
			tmpstr = TextBox1.Text + "3" + Strings.Chr(201);

			deccount += 1;

			Wnlib.Search se = null;
			ArrayList list = new ArrayList();

			// if searchscrabble is checked then search the official list
			if (chkSearchScrabble.Checked) {
				list = EnableDT_Classes.EnableDT_Search.wngrep(tmpstr, radScrabble.Checked);
			} else {
				se = new Wnlib.Search(tmpstr, false, pos, new Wnlib.SearchType(false, "WNGREP"), 0, new wildgrep());
				list = se.strings;
			}

			ArrayList tmplist = new ArrayList();

			// delete duplicates
			// ignore errors here = the list may be empty
			try {
				DataRow dr = default(DataRow);
				for (j = 0; j <= list.Count - 1; j++) {
					if (tmplist.Count >= 1) {
						if (tmplist.IndexOf(list[j]) == -1) {
							tmplist.Add(list[j]);
							// **********
							//                        ListBox1.Items.Add(list[i])
							dr = dataSet1.Tables[0].NewRow();

							dr["Word"] = list[j];
							if (chkSearchScrabble.Checked) {
								dr["Score"] = ScrabbleScore(list[j].ToString());
							}
							dataSet1.Tables[0].Rows.Add(dr);
							// **********
						}
					} else {
						tmplist.Add(list[j]);
						//                    ListBox1.Items.Add(list[i])
						dr = dataSet1.Tables[0].NewRow();

						dr["Word"] = list[j];
						if (chkSearchScrabble.Checked) {
							dr["Score"] = ScrabbleScore(list[j].ToString());
						}
						dataSet1.Tables[0].Rows.Add(dr);
					}
				}

				list = tmplist;
				dataGrid1.DataSource = dataSet1.Tables[0];
			} catch {
			}

			i = list.Count;

			return i;
		}

		private int WildCardRegex(Wnlib.PartOfSpeech pos)
		{
			int i = 0;
			int j = 0;
			int x = 0;
			int deccount = 0;
			string tmpstr = "";

			// TODO: for both anagram search and scrabble search, remove duplicates from the list
			// before processing a word, otherwise the character test is done twice needlessly

			// not a scrabble search, just find the anagram
			if (!radRegularExpression.Checked) {
				tmpstr = TextBox1.Text;
			} else {
				tmpstr = TextBox1.Text + Strings.Chr(200);
			}

			Wnlib.Search se = null;
			ArrayList list = new ArrayList();

			// if searchscrabble is checked then search the official list
			if (chkSearchScrabble.Checked) {
				list = EnableDT_Classes.EnableDT_Search.wngrep(tmpstr, radScrabble.Checked);
			} else {
				se = new Wnlib.Search(tmpstr, false, pos, new Wnlib.SearchType(false, "WNGREP"), 0, new wildgrep());
				list = se.strings;
			}

			ArrayList tmplist = new ArrayList();

			if (list.IndexOf("error") != -1) {
				return -1;
			}

			//delete(duplicates)
			// ignore errors here = the list may be empty
			try {
				DataRow dr = default(DataRow);
				for (j = 0; j <= list.Count - 1; j++) {
					if (tmplist.Count >= 1) {
						if (tmplist.IndexOf(list[j]) == -1) {
							tmplist.Add(list[j]);
							// ******
							//ListBox1.Items.Add(list[i])
							dr = dataSet1.Tables[0].NewRow();

							dr["Word"] = list[j];
							if (chkSearchScrabble.Checked) {
								dr["Score"] = ScrabbleScore(list[j].ToString());
							}
							dataSet1.Tables[0].Rows.Add(dr);
							// ******
						}
					} else {
						tmplist.Add(list[j]);
						// ******
						//                    ListBox1.Items.Add(list[i])
						dr = dataSet1.Tables[0].NewRow();

						dr["Word"] = list[i];
						if (chkSearchScrabble.Checked) {
							dr["Score"] = ScrabbleScore(list[i].ToString());
						}
						dataSet1.Tables[0].Rows.Add(dr);
						// ******
					}
				}

				list = tmplist;

				dataGrid1.DataSource = dataSet1.Tables[0];
			} catch {
			}

			//For j = 0 To list.Count - 1
			//    If CheckAnagram(list[i].ToString(), TextBox1.Text) Then
			//        ListBox1.Items.Add(list[i])
			//    End If
			//Next j

			i = list.Count;

			return i;
		}

		private int AnagramScrabble(Wnlib.PartOfSpeech pos)
		{
			int i = 0;
			int j = 0;
			int x = 0;
			int deccount = 0;
			string tmpstr = "";
			DataRow dr = default(DataRow);

			// TODO: for both anagram search and scrabble search, remove duplicates from the list
			// before processing a word, otherwise the character test is done twice needlessly

			// not a scrabble search, just find the anagram
			if (!radScrabble.Checked) {
				// repeat the string into character match blocks for the length of itself
				// eg. kilmo would be [kilmo][kilmo][kilmo][kilmo][kilmo]
				for (j = 1; j <= TextBox1.Text.Length; j++) {
					tmpstr += "[" + TextBox1.Text + "]";
				}

				//        Dim se As Wnlib.Search = New Wnlib.Search(TextBox1.Text, False, pos, New Wnlib.SearchType(False, "WNGREP"), 0)
				Wnlib.Search se = null;
				ArrayList list = new ArrayList();

				// if searchscrabble is checked then search the official list
				if (chkSearchScrabble.Checked) {
					list = EnableDT_Classes.EnableDT_Search.wngrep(tmpstr, radScrabble.Checked);
				} else {
					se = new Wnlib.Search(tmpstr, false, pos, new Wnlib.SearchType(false, "WNGREP"), 0, new wildgrep());
					list = se.strings;
				}

				ArrayList tmplist = new ArrayList();

				//delete(duplicates)
				// ignore errors here = the list may be empty
				try {
					for (j = 0; j <= list.Count - 1; j++) {
						if (tmplist.Count >= 1) {
							if (tmplist.IndexOf(list[j]) == -1) {
								tmplist.Add(list[j]);
								// ******
								if (CheckAnagram(list[j].ToString().ToString(), TextBox1.Text)) {
									//                                ListBox1.Items.Add(list[i])
									dr = dataSet1.Tables[0].NewRow();

									dr["Word"] = list[j];
									if (chkSearchScrabble.Checked) {
										dr["Score"] = ScrabbleScore(list[j].ToString());
									}
									dataSet1.Tables[0].Rows.Add(dr);
								}
								// ******
							}
						} else {
							tmplist.Add(list[j]);
							// ******
							if (CheckAnagram(list[j].ToString().ToString(), TextBox1.Text)) {
								//                            ListBox1.Items.Add(list[i])
								dr = dataSet1.Tables[0].NewRow();

								dr["Word"] = list[j];
								if (chkSearchScrabble.Checked) {
									dr["Score"] = ScrabbleScore(list[j].ToString());
								}
								dataSet1.Tables[0].Rows.Add(dr);
							}
						}
					}

					list = tmplist;
				} catch {
				}

				//For j = 0 To list.Count - 1
				//    If CheckAnagram(list[i].ToString(), TextBox1.Text) Then
				//        ListBox1.Items.Add(list[i])
				//    End If
				//Next j

				i = list.Count;
			// scrabble search
			} else {
				deccount = 0;
				tmpstr = "";

				// asc 200 specifies a regex
				tmpstr = "^[" + TextBox1.Text + "]+$" + Strings.Chr(200);

				//For x = 1 To TextBox1.Text.Length
				// repeat the string into character match blocks for the length of itself
				// eg. kilmo would be [kilmo][kilmo][kilmo][kilmo][kilmo]
				//For j = 1 To (TextBox1.Text.Length - deccount)
				//    tmpstr += "[" & TextBox1.Text & "]"
				//Next

				deccount += 1;

				Wnlib.Search se = null;
				ArrayList list = new ArrayList();

				// if searchscrabble is checked then search the official list
				if (chkSearchScrabble.Checked) {
					list = EnableDT_Classes.EnableDT_Search.wngrep(tmpstr, radScrabble.Checked);
				} else {
					se = new Wnlib.Search(tmpstr, false, pos, new Wnlib.SearchType(false, "WNGREP"), 0, new wildgrep());
					list = se.strings;
				}

				ArrayList tmplist = new ArrayList();

				// delete duplicates
				// ignore errors here = the list may be empty
				try {
					for (j = 0; j <= list.Count - 1; j++) {
						if (tmplist.Count >= 1) {
							if (tmplist.IndexOf(list[j]) == -1) {
								tmplist.Add(list[j]);
								// **********
								if (CheckAnagram(list[j].ToString().ToString(), TextBox1.Text)) {
									//ListBox1.Items.Add(list[i])
									dr = dataSet1.Tables[0].NewRow();

									dr["Word"] = list[j];
									if (chkSearchScrabble.Checked) {
										dr["Score"] = ScrabbleScore(list[j].ToString());
									}
									dataSet1.Tables[0].Rows.Add(dr);
								}
								// **********
							}
						} else {
							tmplist.Add(list[j]);
							// **********
							if (CheckAnagram(list[j].ToString(), TextBox1.Text)) {
								//                            ListBox1.Items.Add(list[i])
								dr = dataSet1.Tables[0].NewRow();

								dr["Word"] = list[j];
								if (chkSearchScrabble.Checked) {
									dr["Score"] = ScrabbleScore(list[j].ToString());
								}
								dataSet1.Tables[0].Rows.Add(dr);
							}
						}
					}

					list = tmplist;
				} catch {
				}

				i = list.Count;
				tmpstr = "";
				//Next
				//DataGrid1.TableStyles(0).GridColumnStyles(0).Width = 200
				//DataGrid1.TableStyles(0).GridColumnStyles(1).Width = 100
			}

			dataGrid1.DataSource = dataSet1.Tables[0];

			return i;
		}

// get the basic scrabble tile score for a word
		private int ScrabbleScore(string wrd)
		{
			char c = '\0';
			int retval = 0;

			foreach (char c_loopVariable in Strings.LCase(wrd)) {
				c = c_loopVariable;
				switch (c) {
					case 'a':
					case 'e':
					case 'i':
					case 'l':
					case 'n':
					case 'o':
					case 'r':
					case 's':
					case 't':
					case 'u':
						retval += 1;
						break;

					case 'd':
					case 'g':
						retval += 2;
						break;

					case 'b':
					case 'c':
					case 'm':
					case 'p':
						retval += 3;
						break;

					case 'f':
					case 'h':
					case 'v':
					case 'w':
					case 'y':
						retval += 4;
						break;

					case 'k':
						retval += 5;
						break;

					case 'j':
					case 'x':
						retval += 8;
						break;

					case 'q':
					case 'z':
						retval += 10;
						break;
				}
			}

			return retval;
		}

		// check that each character in the pattern occurs for the number of times
		// in the word that it appears in the pattern
		private bool CheckAnagram(string wrdtocheck, string patt)
		{
			int cc1 = 0;
			int cc2 = 0;
			string clist = null;
			char ctocheck = '\0';
			char tmpc = '\0';
			Array tmparr1 = wrdtocheck.ToCharArray();
			Array tmparr2 = patt.ToCharArray();

			Array.Sort(tmparr1);
			Array.Sort(tmparr2);

			foreach (char ctocheck_loopVariable in tmparr2) {
				ctocheck = ctocheck_loopVariable;
				// make sure we haven't already tested this character - no use doing twice
				if (!(Strings.InStr(clist, ctocheck.ToString()) > 0)) {
					// add the character to the list so it is not rechecked if repeated up further
					clist += ctocheck;

					// do a basic check to see if the character even exists in the string
					// doesn't apply to a scrabble search because characters can be left out
					if (Strings.InStr(wrdtocheck, ctocheck.ToString()) == 0 & !radScrabble.Checked) {
						return false;
					}

					// TODO: replace the foreaches with instr increments - maybe ship to a recurring function
					// count the number of times the character exists in the pattern
					cc1 = 0;

					foreach (char tmpc_loopVariable in tmparr2) {
						tmpc = tmpc_loopVariable;
						if (tmpc == ctocheck) {
							// increment the count of chars found
							cc1 += 1;
						}
					}

					cc2 = 0;

					foreach (char tmpc_loopVariable in tmparr1) {
						tmpc = tmpc_loopVariable;
						if (tmpc == ctocheck) {
							// increment the count of chars found
							cc2 += 1;
						}

						// if we exceed the count in the pattern then discard the test
						if (cc2 > cc1) {
							return false;
						}
					}

					// character count of each letter in the anagram must match the retrieved value
					// does not apply to scrabble, the character can appear less times
					if (cc2 != cc1 & !radScrabble.Checked) {
						return false;
					}
				}
			}

			return true;
		}

		private void TextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				e.Handled = true;
				btnSearchClick(null, null);
			}
		}

		//    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
		//        Try ' avoid null selection
		//            Caller.TextBox1.Text = ListBox1.SelectedItem.ToString
		//        Catch
		//        End Try
		//    End Sub

		private void Button3_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void Pos_Click(object sender, System.EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;

			//        ListBox1.Visible = False
			dataGrid1.DataSource = null;
			dataSet1 = new DataSet();
			// clear the dataset
			//            DataGrid1.PerformLayout()
			//            DataGrid1.Invalidate()
			//            DataGrid1.Visible = False
			//			Me.Refresh
			//            DataGrid1.Visible = True

			//			DataSet1.Clear
			// setup datagrid columns
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "Results";

			DataGridTextBoxColumn TextCol1 = new DataGridTextBoxColumn();
			TextCol1.MappingName = "Word";
			TextCol1.HeaderText = "Word";
			//	        TextCol1.Width = Me.Width - 42

			if (chkSearchScrabble.Checked) {
				DataGridTextBoxColumn TextCol2 = new DataGridTextBoxColumn();
				TextCol2 = new DataGridTextBoxColumn();
				TextCol2.MappingName = "Score";
				TextCol2.HeaderText = "Score";
				//            	TextCol2.Width = 60
				//	            TextCol1.Width = Me.Width - TextCol2.Width - 42
				tableStyle.GridColumnStyles.Add(TextCol1);
				tableStyle.GridColumnStyles.Add(TextCol2);
			} else {
				tableStyle.GridColumnStyles.Add(TextCol1);
			}

			DataTable resTable = default(DataTable);

			if (dataSet1.Tables.Count == 0) {
				resTable = dataSet1.Tables.Add("Results");
			}

			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(tableStyle);
			dataSet1.Tables[0].DefaultView.AllowDelete = false;
			dataSet1.Tables[0].DefaultView.AllowNew = false;
			dataSet1.Tables[0].DefaultView.AllowEdit = false;

			DataColumn myCol = resTable.Columns.Add("Word", Type.GetType("System.String"));
			if (chkSearchScrabble.Checked) {
				resTable.Columns.Add("Score", Type.GetType("System.Int32"));
			}

			//            ListBox1.Visible = False
			//Button4.Visible = True
			//        InputPanel1.Enabled = False

			DataGrid1Resize(null, null);

			Refresh();

            //        ListBox1.Items.Clear()
            if (mi.Text.ToLower() == "all")
            {
                nn = 0;
                nv = 0;
                na = 0;
                nr = 0;

                nn = doWnGrep(Wnlib.PartOfSpeech.of("noun"));
                if (nn != -1 & !(chkSearchScrabble.Checked))
                {
                    nv = doWnGrep(Wnlib.PartOfSpeech.of("verb"));
                    if (nv != -1)
                    {
                        na = doWnGrep(Wnlib.PartOfSpeech.of("adj"));
                        if (na != -1)
                        {
                            nr = doWnGrep(Wnlib.PartOfSpeech.of("adv"));
                        }
                    }
                }
            }
            else
            {
                doWnGrep(Wnlib.PartOfSpeech.of(mi.Text.ToLower()));
            }

			DataGrid1Resize(null, null);

			statusBar1.Text = "Searched produced " + dataSet1.Tables[0].Rows.Count + " result(s)";
			//        ListBox1.Visible = True
		}

		//Private Sub radSoundsLike_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radWildcard.CheckedChanged, radRegularExpression.CheckedChanged, radAnagram.CheckedChanged, radScrabble.CheckedChanged, radSoundsLike.CheckedChanged
		//    If radSoundsLike.Checked Then
		//        TrackBar1.Visible = True
		//    Else
		//        TrackBar1.Visible = False
		//    End If
		//End Sub

		private void DataGrid1CurrentCellChanged(System.Object sender, System.EventArgs e)
		{

			string x = "";

			x = dataGrid1[dataGrid1.CurrentRowIndex, 0].ToString();

			Caller.TextBox1.Text = x;
		}

        private void ChkSearchScrabbleCheckedChanged(System.Object sender, System.EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if (cb.Checked)
            {
                Button4.Visible = false;
                statusBar1.Text = "Scrabble Allowed Words may not have definitions";
            } else
            {
                if(nn > 0 || nv > 0 || na > 0 || nr > 0) {
                    Button4.Visible = true;
                    statusBar1.Text = "";
                }
            }

        }

        private void DataGrid1Resize(System.Object sender, System.EventArgs ne)
		{
            ArrayList al = new ArrayList();

			//Me.refresh
			if (dataGrid1.TableStyles.Count == 0) {
				return;
			}

            // purge duplicates
            bool purgeflag = true; // we need to recursively purge in case there are more than two instances of a result

            while (purgeflag)
            {
                purgeflag = false;

                for (int i = 0; i < dataSet1.Tables[0].Rows.Count; i++)
                {
                    DataRow dr1 = dataSet1.Tables[0].Rows[i];

                    for (int x = i + 1; x < dataSet1.Tables[0].Rows.Count; x++)
                    {
                        DataRow dr2 = dataSet1.Tables[0].Rows[x];

                        if (x > i)
                        {
                            if (dr1[0].ToString() == dr2[0].ToString())
                            {
                                purgeflag = true;
                                al.Add(x);
                            }
                        }
                    }
                }

                for (int i = al.Count - 1; i >= 0; i--)
                {
                    if ((int)al[i] <= dataSet1.Tables[0].Rows.Count - 1)
                        dataSet1.Tables[0].Rows[(int)al[i]].Delete();
                }
            }

            Application.DoEvents();

			if (dataGrid1.TableStyles[0].GridColumnStyles.Count == 1) {
				if (dataGrid1.VisibleRowCount < dataSet1.Tables[0].Rows.Count) {
					// is scrolling
					dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 60;
				} else {
					// is not scrolling
					dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 43;

					// After setting the size above, the bottom row may become 'not visible'
					if (dataGrid1.VisibleRowCount < dataSet1.Tables[0].Rows.Count) {
						dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 60;
					}
				}
			} else {
				if (dataGrid1.VisibleRowCount < dataSet1.Tables[0].Rows.Count) {
					// is scrolling
					dataGrid1.TableStyles[0].GridColumnStyles[1].Width = 60;
					dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 60 - 60;
				} else {
					// is not scrolling
					dataGrid1.TableStyles[0].GridColumnStyles[1].Width = 60;
					dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 60 - 43;

					// After setting the size above, the bottom row may become 'not visible'
					if (dataGrid1.VisibleRowCount < dataSet1.Tables[0].Rows.Count) {
						dataGrid1.TableStyles[0].GridColumnStyles[1].Width = 60;
						dataGrid1.TableStyles[0].GridColumnStyles[0].Width = this.Width - 60 - 60;
					}
				}
			}
		}

		private void Button4Click(System.Object sender, System.EventArgs e)
		{
			Button b = (Button)sender;
			ContextMenu cm = new ContextMenu();

            //if ((nall > 0))
            //{
                MenuItem mi1 = new MenuItem();
                mi1.Text = "All";
                mi1.Click += Pos_Click;
                cm.MenuItems.Add(mi1);
            //}

            if ((nn > 0)) {
				MenuItem mi = new MenuItem();
				mi.Text = "Noun";
				mi.Click += Pos_Click;
				cm.MenuItems.Add(mi);
			}

			if ((nv > 0)) {
				MenuItem mi = new MenuItem();
				mi.Text = "Verb";
				mi.Click += Pos_Click;
				cm.MenuItems.Add(mi);
			}

			if ((na > 0)) {
				MenuItem mi = new MenuItem();
				mi.Text = "Adj";
				mi.Click += Pos_Click;
				cm.MenuItems.Add(mi);
			}

			if ((nr > 0)) {
				MenuItem mi = new MenuItem();
				mi.Text = "Adv";
				mi.Click += Pos_Click;
				cm.MenuItems.Add(mi);
			}

			cm.Show(b.Parent, new Point(b.Left, b.Bottom));
		}

        private void btnSearchClick(object sender, EventArgs e)
        {
			try {
				dataGrid1.DataSource = null;
				dataSet1 = new DataSet();
				// clear the dataset
				//            DataGrid1.PerformLayout()
				//            DataGrid1.Invalidate()
				//            DataGrid1.Visible = False
				//			Me.Refresh
				//            DataGrid1.Visible = True

				//			DataSet1.Clear
				// setup datagrid columns
				DataGridTableStyle tableStyle = new DataGridTableStyle();
				tableStyle.MappingName = "Results";

				DataGridTextBoxColumn TextCol1 = new DataGridTextBoxColumn();
				TextCol1.MappingName = "Word";
				TextCol1.HeaderText = "Word";
				//	        TextCol1.Width = Me.Width - 42

				if (chkSearchScrabble.Checked) {
					DataGridTextBoxColumn TextCol2 = new DataGridTextBoxColumn();
					TextCol2 = new DataGridTextBoxColumn();
					TextCol2.MappingName = "Score";
					TextCol2.HeaderText = "Score";
					//            	TextCol2.Width = 60
					//	            TextCol1.Width = Me.Width - TextCol2.Width - 42
					tableStyle.GridColumnStyles.Add(TextCol1);
					tableStyle.GridColumnStyles.Add(TextCol2);
				} else {
					tableStyle.GridColumnStyles.Add(TextCol1);
				}

				DataTable resTable = default(DataTable);

				if (dataSet1.Tables.Count == 0) {
					resTable = dataSet1.Tables.Add("Results");
				}

				dataGrid1.TableStyles.Clear();
				dataGrid1.TableStyles.Add(tableStyle);
				dataSet1.Tables[0].DefaultView.AllowDelete = false;
				dataSet1.Tables[0].DefaultView.AllowNew = false;
				dataSet1.Tables[0].DefaultView.AllowEdit = false;

				DataColumn myCol = resTable.Columns.Add("Word", Type.GetType("System.String"));
				if (chkSearchScrabble.Checked) {
					resTable.Columns.Add("Score", Type.GetType("System.Int32"));
				}

				//            ListBox1.Visible = False
				//Button4.Visible = True
				//        InputPanel1.Enabled = False

				DataGrid1Resize(null, null);

				Refresh();
				//            ListBox1.Items.Clear()
				nn = 0;
				nv = 0;
				na = 0;
				nr = 0;

				nn = doWnGrep(Wnlib.PartOfSpeech.of("noun"));
				if (nn != -1 & !(chkSearchScrabble.Checked)) {
					nv = doWnGrep(Wnlib.PartOfSpeech.of("verb"));
					if (nv != -1) {
						na = doWnGrep(Wnlib.PartOfSpeech.of("adj"));
						if (na != -1) {
							nr = doWnGrep(Wnlib.PartOfSpeech.of("adv"));
						}
					}
				}
				//            ListBox1.Visible = True
				Refresh();

				if ((nn > 0 | nv > 0 | na > 0 | nr > 0) & (!chkSearchScrabble.Checked)) {
					Button4.Visible = true;
					Button4.Focus();
				} else {
					Button4.Visible = false;
				}

                DataGrid1Resize(null, null);

                statusBar1.Text = "Searched produced " + dataSet1.Tables[0].Rows.Count + " result(s)";
			} catch (Exception ex) {
				Interaction.MsgBox("Please forward the following error to troy@ebswift.com :" + Constants.vbCrLf + Constants.vbCrLf + ex.Message);
			}
		}

        private void Button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = TextBox1.Text;
            if ((saveFileDialog1.ShowDialog() == DialogResult.OK))
            {
                StreamWriter f = new StreamWriter(saveFileDialog1.FileName, false);
                DataTable dt = (DataTable)dataGrid1.DataSource;
                string s = "";

                foreach(DataRow dr in dt.Rows)
                {
                    foreach(var dc in dr.ItemArray)
                    {
                        s += dc.ToString() + '\t';
                    }
                    s += "\r\n";
                }
                f.Write(s);
                f.Close();
            }
        }
    }

    class wildgrep : Wnlib.CustomGrep
    {
        // setup search word
        private static Metaphone.ShortDoubleMetaphone mphone = new ShortDoubleMetaphone();
        // setup compare word
        private static ShortDoubleMetaphone mphonecompare = new ShortDoubleMetaphone();

        public override ArrayList wngrep(string wordPassed, Wnlib.PartOfSpeech pos)
        {
            ArrayList r = new ArrayList();
            StreamReader inputFile = Wnlib.WNDB.index(pos);
            inputFile.BaseStream.Seek(0L, SeekOrigin.Begin);
            inputFile.DiscardBufferedData();
            string word = wordPassed.Replace(" ", "_");
            string line;
            char c = (char)200; // regex / scrabble
            char c2 = (char)201; // soundslike
            bool regexflag = false;
            bool soundslikeflag = false;
            int soundslikestrength = 0;

            // if the string ends with chr 200 the search is a regular expression
            if (word.EndsWith(c.ToString()))
            {
                regexflag = true;
                word = word.Replace(c.ToString(), "");
            }

            if (word.EndsWith(c2.ToString()))
            {
                soundslikeflag = true;
                word = word.Replace(c2.ToString(), "");
                //soundslikestrength = Convert.ToInt16(Regex.Replace(word, "[[^a-zA-Z]", ""));
                soundslikestrength = Convert.ToInt16(Regex.Replace(word, "[[^a-zA-Z]", ""));
                word = word.Replace(Regex.Replace(word, "[[^a-zA-Z]", ""), "");
            }

            if (!regexflag && !soundslikeflag) // non-regex search
            {
                while ((line = inputFile.ReadLine()) != null)
                {
                    int lineLen = line.IndexOf(' ');
                    line = line.Substring(0, lineLen);
                    // TODO: change the .IndexOf to allow wildcards
                    //				if (line.IndexOf(word)>=0)
                    try
                    {
                        if (ebswift.ebString.vbLike(line, word))
                            r.Add(line.Replace("_", " "));
                    }
                    catch
                    {
                    }
                }
            }
            else if (regexflag)
            { // regex search
                Regex rg = new Regex("a", RegexOptions.IgnoreCase);

                try
                {
                    rg = new Regex(word, RegexOptions.IgnoreCase);
                }
                catch
                {
                    MessageBox.Show("Malformed regular expression.");
                    r.Add("error");
                    return r;
                }

                while ((line = inputFile.ReadLine()) != null)
                {
                    int lineLen = line.IndexOf(' ');
                    line = line.Substring(0, lineLen);
                    // TODO: change the .IndexOf to allow wildcards
                    //				if (line.IndexOf(word)>=0)
                    try
                    {
                        Match m;

                        m = rg.Match(line);

                        if (m.Success)
                            r.Add(line.Replace("_", " "));
                    }
                    catch
                    {
                    }
                }
            }
            else if (soundslikeflag) // sounds like search
            {
                mphonecompare.computeKeys(word);

                while ((line = inputFile.ReadLine()) != null)
                {
                    int lineLen = line.IndexOf(' ');
                    line = line.Substring(0, lineLen);
                    try
                    {
                        if (soundsLike(line)) //, word)) //, soundslikestrength))
                            r.Add(line.Replace("_", " "));
                    }
                    catch
                    {
                    }
                }
            }
            return r;
        }

        // does a sounds like comparison based on a strength level
        internal static bool soundsLike(string wordnetstr) //, string searchstr) //, int strength)
        {
            mphone.computeKeys(wordnetstr);

            if (mphonecompare.PrimaryShortKey == mphone.PrimaryShortKey)
            {
                //Primary-Primary match, that's level 3 (strongest)
                return true;
            }

            //						if((mphonecompare.PrimaryShortKey == mphone.PrimaryShortKey || mphonecompare.AlternateShortKey == mphone.PrimaryShortKey) || (mphonecompare.AlternateShortKey == nullpointer.Metaphone.ShortDoubleMetaphone.METAPHONE_INVALID_KEY && (mphonecompare.PrimaryShortKey == mphone.AlternateShortKey || mphonecompare.AlternateShortKey == mphone.AlternateShortKey))) 
            //							return true;
            //						else
            //							return false;

            //			switch(strength)
            //			{
            //				case 1: //Alternate-Alternate match, that's level 1 (minimal)
            //					if (mphone.AlternateKey != null &&
            //						mphonecompare.AlternateKey != null) 
            //					{
            //						if (mphone.AlternateKey == mphonecompare.AlternateKey)
            //						{
            //							return true;
            //						} 
            //					}
            //					break;
            //
            //				case 2:
            //					if (mphonecompare.AlternateKey != null) 
            //					{
            //						if (mphonecompare.AlternateKey == mphone.PrimaryKey) 
            //						{
            //							//Alternate-Primary match, that's level 2 (normal)
            //							return true;
            //						} 
            //					}
            //     
            //					if (mphone.AlternateKey != null) 
            //					{
            //						if (mphonecompare.PrimaryKey == mphone.AlternateKey)
            //						{
            //							//Primary-Alternate match, that's level 2 (normal)
            //							return true;
            //						} 
            //					}
            //					break;
            //
            //				case 3:
            //					if (mphonecompare.PrimaryKey == mphone.PrimaryKey)
            //					{
            //						//Primary-Primary match, that's level 3 (strongest)
            //						return true;
            //					}
            //					break;
            //			}

            return false;
        }
    }

}

