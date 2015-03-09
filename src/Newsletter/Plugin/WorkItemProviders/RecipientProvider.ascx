<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecipientProvider.ascx.cs" Inherits="BVNetwork.EPiSendMail.Plugin.WorkItemProviders.RecipientProvider" %>

<b>Select a Recipient List:</b>
<br />
<asp:Repeater ID="rptAddFromRecipientLists" runat="server" enableviewstate="false">
    <HeaderTemplate>
        <div style="height: 300px !important; overflow: auto;">
        <table class="table table-striped">
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
            <td><asp:LinkButton ID="cmdSelecteRecipList" runat="server" 
                                Text="Add" CommandArgument='<%# Eval("Id") %>' 
                                OnCommand="cmdSelect_Click"
                                /></td>
            <td><%# Eval("Name") %></td>
            <td><%# Eval("ListType") %></td>
            <td><span class="badge"><%# Eval("EmailAddressCount") %></span></td>
            <td><%# Eval("Description") %></td>
        </tr>        
    </ItemTemplate>
    <FooterTemplate>
        </table>
        </div>
    </FooterTemplate>
</asp:Repeater>