<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecipientProvider.ascx.cs" Inherits="BVNetwork.EPiSendMail.Plugin.RecipientItemProviders.RecipientProvider" %>

<b>Select a Recipient List to add to this newsletter:</b>
<br />
<asp:Repeater ID="rptAddFromRecipientLists" runat="server" enableviewstate="false">
    <HeaderTemplate>
        <table class=" table table-striped">
            <tr>
                <th>&nbsp;</th>
                <th>Name</th>
                <th>Type</th>
                <th>Count</th>
                <th>Description</th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>[<asp:LinkButton ID="cmdSelecteRecipList" runat="server" 
                                Text="Select" CommandArgument='<%# Eval("Id") %>' 
                                OnCommand="cmdSelect_Click"
                                />]</td>
            <td><%# Eval("Name") %></td>
            <td><%# Eval("ListType") %></td>
            <td><%# Eval("EmailAddressCount") %></td>
            <td><%# Eval("Description") %></td>
        </tr>        
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>