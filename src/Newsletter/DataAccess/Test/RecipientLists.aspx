<%@ Page Language="c#" Inherits="System.Web.UI.Page" 
         MasterPageFile="~/modules/bvn/SendMail/Data/Test/test.master" AutoEventWireup="true" %>
<%@ Import Namespace="BVNetwork.EPiSendMail.DataAccess" %>

<script runat="server" type="text/C#">
    private void Page_Load(object sender, EventArgs e)
    {
    }

    private void Page_PreRender(object sender, EventArgs e)
    {
        BindRecipientList();
    }

    private void BindRecipientList()
    {
        RecipientLists recipLists = RecipientLists.ListAll();
        rptAllRecipLists.DataSource = recipLists;
        rptAllRecipLists.DataBind();
    }

    /// <summary>   
    /// Imports email addresses as work items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmdAddCsvEmailAddresses_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtAddCsvEmailAddressToRecipListId.Text))
        {
            lblAddCsvEmailAddressesResult.Text = "Missing Job Id";
            return;
        }

        if (string.IsNullOrEmpty(txtAddCsvEmailAddresses.Text))
        {
            lblAddCsvEmailAddressesResult.Text = "Address text box is empty. Nothing to add.";
            return;
        }
        
        // Need job first
        int recipListId = int.Parse(txtAddCsvEmailAddressToRecipListId.Text);
        RecipientList recipList = RecipientList.Load(recipListId);
        if (recipList == null)
        {
            lblAddCsvEmailAddressesResult.Text = "Recipient List with id '" + txtAddCsvEmailAddressToRecipListId.Text + "' was not found.";
            return;
        }

        string emailAddresses = txtAddCsvEmailAddresses.Text;
        string[] emailAddressArray = emailAddresses.Split(new char[] { ';', ',', ' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        
        System.Collections.Generic.List<string> invalidEmailAddresses;
        System.Collections.Generic.List<string> duplicateAddresses;

        // Time it
        System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();

        int imported = recipList.ImportEmailAddresses(emailAddressArray, out invalidEmailAddresses, out duplicateAddresses);
        
        tmr.Stop();

        // Output rejected addresses
        string rejectedAddressesMsg = "";
        if (invalidEmailAddresses.Count > 0)
        {
            string[] arrayForJoin = invalidEmailAddresses.ToArray();
            rejectedAddressesMsg = string.Format("<br />\nRejected addresses: {0}", string.Join(" | ", arrayForJoin));
        }

        lblAddCsvEmailAddressesResult.Text =
            string.Format("Imported {0} of {1} email addresses for recipient list in {2} ms. {3}", 
                imported.ToString(), (emailAddressArray.Length).ToString(),
                tmr.ElapsedMilliseconds.ToString(),
                rejectedAddressesMsg);
    }

    private void cmdRecipientListCommand_Click(object sender, CommandEventArgs e)
    {
        int recipListId = int.Parse(e.CommandArgument.ToString());
        txtAddCsvEmailAddressToRecipListId.Text = e.CommandArgument.ToString();
        RecipientList recipList = RecipientList.Load(recipListId);


        if (e.CommandName == "ShowEmailAddresses")
        {
            EmailAddresses items = recipList.EmailAddresses;
            rptEmailAddresses.DataSource = items;
            rptEmailAddresses.DataBind();
        }
        else if (e.CommandName == "RemoveEmailAddresses")
        {
            recipList.DeleteEmailAddressItems();
        }
    }
    
    /// <summary>
    ///  Add new recipient list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmdAddRecipList_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtAddRecipListType.Text))
        {
            lblAddRecipListResult.Text = "Missing List Type";
            return;
        }

        if (string.IsNullOrEmpty(txtAddRecipListName.Text))
        {
            lblAddRecipListResult.Text = "Missing List Name";
            return;
        }

        RecipientListType listType = (RecipientListType)int.Parse(txtAddRecipListType.Text);
        // Create it
        RecipientList recipList = new RecipientList(listType, txtAddRecipListName.Text, txtAddRecipListDescription.Text);

        recipList.Save();

        // Saved it
        lblAddRecipListResult.Text = "Added new Recipient List with id: " + recipList.Id.ToString();

        // Show it in the list
        BindRecipientList();
    }

    private void cmdEmailListCommand_Click(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "EditEmailAddresses")
        {
            if (string.IsNullOrEmpty(txtAddCsvEmailAddressToRecipListId.Text))
                return;

            int recipListId = int.Parse(txtAddCsvEmailAddressToRecipListId.Text);
            string email = e.CommandArgument.ToString();

            EmailAddress emailItem = EmailAddress.Load(recipListId, email);

            txtEditEmailAddressRecipId.Text = recipListId.ToString();
            txtEditEmailAddressEmail.Text = emailItem.Email;
            txtEditEmailAddressComment.Text = emailItem.Comment;
            txtEditEmailAddressAdded.Text = emailItem.Added.ToString();
            txtEditEmailAddressSource.Text = emailItem.Source.ToString();

        }
    }


    /// <summary>
    /// Save Email Address
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmdEditEmailAddress_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtEditEmailAddressRecipId.Text))
        {
            lblEditWorkItemResult.Text = "Missing recipient list id";
            return;
        }

        if (string.IsNullOrEmpty(txtEditEmailAddressEmail.Text))
        {
            lblEditWorkItemResult.Text = "Missing email address";
            return;
        }

        int recipListId = int.Parse(txtEditEmailAddressRecipId.Text);
        string email = txtEditEmailAddressEmail.Text;
        EmailAddressSource source = (EmailAddressSource)int.Parse(txtEditEmailAddressSource.Text);

        EmailAddress emailItem = EmailAddress.Load(recipListId, email);

        emailItem.Source = source;
        emailItem.Comment = txtEditEmailAddressComment.Text;
        emailItem.Save();

        lblEditWorkItemResult.Text = "Email item has been updated.";
    }
    
</script>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <h1>Newsletter Recipient Lists Test Page</h1>
    <!-- List all Recipient Lists -->
    <asp:Panel CssClass="container" runat="server">
        <h2>All Recipient Lists</h2>
        <asp:Repeater ID="rptAllRecipLists" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="0" border="0">
                    <tr>
                        <th>
                            ID</th>
                        <th>
                            Type</th>
                        <th>
                            Name</th>
                        <th>
                            Count</th>
                        <th>
                            Created</th>
                        <th>
                            Description</th>
                        <th>&nbsp;</th>
                        <th>&nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("Id") %>
                    </td>
                    <td>
                        <%# Eval("ListType") %>
                    </td>
                    <td>
                        <%# Eval("Name") %>
                    </td>
                    <td>
                        <%# Eval("EmailAddressCount")%>
                    </td>
                    <td>
                        <%# Eval("Created") %>
                    </td>
                    <td>
                        <%# Eval("Description") %>
                    </td>
                    <td>
                        <asp:LinkButton CommandName="ShowEmailAddresses" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdRecipientListCommand_Click"
                                        Text="Show Email Addresses" />
                    </td>
                    <td>
                        <asp:LinkButton CommandName="RemoveEmailAddresses" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdRecipientListCommand_Click"
                                        Text="Remove Email Addresses" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>


    <!-- Add Recipient List -->
    <asp:Panel CssClass="container" runat="server" DefaultButton="cmdEditEmailAddress">
        <h2>Add Recipient List</h2>
        <label>Recipient List Type:</label>
        <asp:TextBox ID="txtAddRecipListType" runat="server" />
        <br />
        <label>Name:</label>
        <asp:TextBox ID="txtAddRecipListName" runat="server" />
        <br />
        <label>Description:</label>
        <asp:TextBox ID="txtAddRecipListDescription" runat="server" />
        <br />
        <asp:Button ID="cmdAddRecipList" runat="server" Text="Submit" OnClick="cmdAddRecipList_Click" />
        <br />
        <asp:Label runat="server" ID="lblAddRecipListResult" />
    </asp:Panel>


    <!-- Edit Email Address Entry -->
    <asp:Panel CssClass="container" runat="server" DefaultButton="cmdEditEmailAddress">
        <h2>Edit Email Address Entry</h2>
        <label>
            Recipient List Id:
        </label>
        <asp:TextBox ID="txtEditEmailAddressRecipId" runat="server" />
        <br />
        <label>
            Email Address:
        </label>
        <asp:TextBox ID="txtEditEmailAddressEmail" runat="server" />
        <br />
        <label>
            Comment:
        </label>
        <asp:TextBox ID="txtEditEmailAddressComment" runat="server" />
        <br />
        <label>
            Created:
        </label>
        <asp:TextBox ID="txtEditEmailAddressAdded" runat="server" />
        <br />
        <label>
            Source:
        </label>
        <asp:TextBox ID="txtEditEmailAddressSource" runat="server" />
        <br />
        <label>
            &nbsp;</label>
        <asp:Button ID="cmdEditEmailAddress" runat="server" Text="Submit" OnClick="cmdEditEmailAddress_Click" />
        <br />
        <asp:Label runat="server" ID="lblEditWorkItemResult" />
    </asp:Panel>

    <!-- List Email Addresses for a given Recipient List -->
    <asp:Panel CssClass="container" runat="server">
        <h2>List Email Addresses for a Recipient List</h2>
        <asp:Repeater ID="rptEmailAddresses" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="0" border="0">
                    <tr>
                        <th>
                            Email</th>
                        <th>
                            Source</th>
                        <th>
                            Added</th>
                        <th>
                            Comment</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:LinkButton CommandName="EditEmailAddresses" 
                                        CommandArgument='<%# Eval("Email") %>'
                                        runat="server"
                                        OnCommand="cmdEmailListCommand_Click"
                                        Text='<%# Eval("Email")%>' /> 
                    </td>
                    <td><%# Eval("Source")%></td>
                    <td><%# Eval("Added")%></td>
                    <td><%# Eval("Comment")%></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <!-- Add Email Addresses from csv text-->
    <asp:Panel CssClass="container" runat="server">
        <h2>Add Email addresses</h2>
        Enter the email addresses you want to add to Recipient List as a comma separated string:
        <br />
        <asp:TextBox Rows="5" Width="100%" runat="server" TextMode="MultiLine" ID="txtAddCsvEmailAddresses" />
        
        <label style="width: 10em;">Add to Job with ID:</label>
        <asp:TextBox ID="txtAddCsvEmailAddressToRecipListId" runat="server" />
        <br /><br />
        <asp:Button id="cmdAddCsvEmailAddresses" runat="server" Text="Add Email Addresses" OnClick="cmdAddCsvEmailAddresses_Click" />
        <br />
        <asp:Label ID="lblAddCsvEmailAddressesResult" runat="server" />
    </asp:Panel>
</asp:Content>