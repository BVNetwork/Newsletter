<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EPiServerGroupProvider.ascx.cs" Inherits="BVNetwork.EPiSendMail.Plugin.RecipientItemProviders.EPiServerGroupProvider" %>
<asp:Panel ID="Panel1" runat="server" DefaultButton="cmdAddEPiGroupEmailAddresses">
    <!-- Add Work Items from group in EPiServer-->
    <b>Select EPiServer group to add users as worker items:</b>
    <br />
    <asp:DropDownList runat="server" ID="dropListEPiGroups" enableviewstate="false" 
        style="margin-top: 0.7em;"/>
    <br />
    <asp:Button id="cmdAddEPiGroupEmailAddresses" runat="server" 
                Text="Add" style="margin-top: 1em;"
                OnClick="cmdAddEPiGroupEmailAddresses_Click" />
</asp:Panel>