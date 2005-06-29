<%@ Page Language="vb" AutoEventWireup="false" Codebehind="default.aspx.vb" Inherits="WordNetASPNet.WebForm1"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>WordNetASPNet Online WordNet Dictionary Browser</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta name="description" content="Free online dictionary thesaurus with wildcard and scrabble search.">
		<meta name="keywords" content="free online dictionary thesaurus wordnet wordnetaspnet troy simpson freeware lexicon">
	</HEAD>
	<body>
		<H2>WordNet.Net ASP.Net Sample</H2>
		<form id="Form1" method="post" runat="server">
			<P>
				<asp:Label id="Label1" runat="server">Search Word:</asp:Label>
				<asp:TextBox id="TextBox1" runat="server"></asp:TextBox>
				<asp:Button id="Button6" runat="server" Text="Search"></asp:Button>
				<asp:Button id="Button1" runat="server" Text="Redisplay overview" Visible="False"></asp:Button><BR>
				<asp:CheckBox id="chkWordWrap" runat="server" Text="Wordwrap" AutoPostBack="True" Checked="True"></asp:CheckBox><BR>
				<asp:Label id="Label2" runat="server" Width="392px">Searches for </asp:Label>
				<asp:Label id="Label3" runat="server">Senses:</asp:Label>
				<asp:TextBox id="TextBox2" runat="server" Width="32px"></asp:TextBox><BR>
				<asp:Label id="lblNoun" runat="server" Visible="False" Width="40px" Height="16px">Noun:</asp:Label>
				<asp:DropDownList id="Noun" runat="server" Visible="False" Width="432px" AutoPostBack="True"></asp:DropDownList><BR>
				<asp:Label id="lblVerb" runat="server" Visible="False" Width="40px" Height="16px">Verb:</asp:Label>
				<asp:DropDownList id="Verb" runat="server" Width="432px" AutoPostBack="True" Visible="False"></asp:DropDownList><BR>
				<asp:Label id="lblAdj" runat="server" Visible="False" Width="40px" Height="16px">Adj:</asp:Label>
				<asp:DropDownList id="Adj" runat="server" Width="432px" AutoPostBack="True" Visible="False"></asp:DropDownList><BR>
				<asp:Label id="lblAdv" runat="server" Visible="False" Width="40px" Height="16px">Adv:</asp:Label>
				<asp:DropDownList id="Adv" runat="server" Width="432px" AutoPostBack="True" Visible="False"></asp:DropDownList></P>
			<P></P>
			<P></P>
			<P></P>
			<P>
				<asp:Label id="lblResult" runat="server" Width="100%" BackColor="#FFFFC0"></asp:Label>
				<asp:Label id="StatusBar1" runat="server" Width="100%" BackColor="#E0E0E0"></asp:Label></P>
			<!-- tickle
<p align="center">
<a href="http://www.jdoqocy.com/click-1668927-10362942" target="_blank" >
<img src="http://www.lduhtrp.net/image-1668927-10362942" width="300" height="250" alt="Free IQ Test" border="0" /></a>
</p>
-->
			<P align="justify">&nbsp;</P>
		</form>
	</body>
</HTML>
