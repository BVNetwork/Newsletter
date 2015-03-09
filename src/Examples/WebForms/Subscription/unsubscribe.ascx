<%@ Control Language="c#" AutoEventWireup="false" Codebehind="unsubscribe.ascx.cs" Inherits="BVNetwork.EPiSendMail.Units.Unsubscribe" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.WebControls" Assembly="EPiServer" %>
<div>
	<episerver:translate text="/bvnetwork/sendmail/unsubscribe/email" runat="server" />&nbsp;
	<asp:textbox id="txtEmail" runat="server"></asp:textbox>
	<br />
	<br />
	<asp:panel id="pnlError" runat="server" visible="False" forecolor="red" font-bold="True" style="padding: 10px;">
		<asp:literal id="lblError" runat="server" enableviewstate="false"></asp:literal>
	</asp:panel>
	<asp:panel id="pnlMessage" runat="server" visible="False" forecolor="green" font-bold="True">
		<asp:literal id="lblMessage" runat="server" enableviewstate="false"></asp:literal>
	</asp:panel>
	<episerver:translate text="/bvnetwork/sendmail/unsubscribe/selectnewsletters" runat="server"/>
	<br>
	<asp:checkboxlist id="chkNewsLetterLists" runat="server" cssclass="normal"></asp:checkboxlist>
	
	<asp:button id="cmdUnsubscribe" runat="server" translate="/bvnetwork/sendmail/unsubscribe/commitbutton" text="Unsubscribe" />

	<asp:repeater runat="server" id="rptResult">
		<headertemplate>
			<br />
			<br />
			<b><episerver:translate text="/bvnetwork/sendmail/unsubscribe/resultlabel" runat="server"/></b>
			<br />
			<table cellpadding="0" cellspacing="0" border="0">
		</headertemplate>
		<itemtemplate>
			<tr>
			<td valign="top" style="padding-top: 4px; padding-bottom: 4px;">
			<img src="/newsletter/images/<%# (bool)DataBinder.Eval(Container.DataItem, "SubscriptionResult") ? "ok.gif" : "cancel.gif" %>" border="0" />
			</td>
			<td>&nbsp;</td>
			<td valign="top">
			<%# DataBinder.Eval(Container.DataItem, "RecipientListName")%>
			
			<div runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Message") != null %>'
				 style="margin-left: 15px; color: #808080;" id="Div1">
				 <%# DataBinder.Eval(Container.DataItem, "Message") %>
			</div>
			</td>
			</tr>
		</itemtemplate>
		<footertemplate>
			</table>
		</footertemplate>
	</asp:repeater>
</div>
