<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="subscribe.ascx.cs"
    Inherits="BVNetwork.EPiSendMail.Units.Subscribe" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.WebControls" Assembly="EPiServer" %>
<div>
    <episerver:translate text="/bvnetwork/sendmail/subscribe/email" runat="server" />
    &nbsp;
    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox><br>
    <asp:Panel ID="pnlError" runat="server" Visible="False" ForeColor="red" Font-Bold="True">
        <asp:Literal ID="lblError" runat="server" EnableViewState="False"></asp:Literal>
    </asp:Panel>
    <asp:Panel ID="pnlMessage" runat="server" Visible="False" ForeColor="green" Font-Bold="True">
        <asp:Literal ID="lblMessage" runat="server" EnableViewState="False"></asp:Literal>
    </asp:Panel>
    <br />
    <br />
    <episerver:translate text="/bvnetwork/sendmail/subscribe/selectnewsletters" runat="server" />
    <asp:CheckBoxList ID="chkNewsLetterLists" runat="server" CssClass="normal">
    </asp:CheckBoxList>
    <asp:Button ID="cmdSubscribe" runat="server" translate="/bvnetwork/sendmail/subscribe/commitbutton"
        Text="Subscribe" />
    <asp:Repeater runat="server" ID="rptResult">
        <HeaderTemplate>
            <br />
            <br />
            <b>
                <episerver:translate text="/bvnetwork/sendmail/subscribe/resultlabel" runat="server" />
            </b>
            <br />
            <table cellpadding="0" cellspacing="0" border="0">
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td valign="top" style="padding-top: 4px; padding-bottom: 4px;">
                    <img src="/newsletter/images/<%# (bool)DataBinder.Eval(Container.DataItem, "SubscriptionResult") ? "ok.gif" : "cancel.gif" %>" border="0" />
                </td>
                <td>
                    &nbsp;
                </td>
                <td valign="top">
                    <%# DataBinder.Eval(Container.DataItem, "RecipientListName") %>
                    <div runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Message") != null %>'
                        style="margin-left: 15px; color: #808080;">
                        <%# DataBinder.Eval(Container.DataItem, "Message") %>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
