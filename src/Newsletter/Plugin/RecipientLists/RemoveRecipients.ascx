<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RemoveRecipients.ascx.cs" 
            Inherits="BVNetwork.EPiSendMail.Plugin.RemoveRecipients" %>
<h2>Remove Recipients</h2>

<%-- 
    Note! This list will be made into a dynamic one,
    using some sort of plug-in method to populate. If
    you want to add your own, please contact steve@episerver.no
    first, to get a status update, and some pointers
    about how to develop this.
--%>
Please choose a provider for removing recipients:
<br />
<asp:Panel ID="pnlImportProviders" runat="server" style="padding-left: 1.5em;">
    <ul>
        <li><asp:LinkButton Text="Remove addresses that are part of another list" CommandName="RecipientList" OnCommand="cmdRemoveRecipients_Click" runat="server"/></li>
        <li><asp:LinkButton Text="Remove addresses by manually specifying the addresses" CommandName="TextFilter" OnCommand="cmdRemoveRecipients_Click" runat="server"/></li>
    </ul>
</asp:Panel>
<div class="well">
    <asp:Panel ID="pnlProviderContainer" runat="Server">
        <%-- Provider will be shown here --%>
    </asp:Panel>
</div>
