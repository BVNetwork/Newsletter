<%@ Page Language="c#" MasterPageFile="~/modules/bvn/SendMail/Data/Test/test.master"
         Inherits="System.Web.UI.Page" 
         AutoEventWireup="true" %>
<%@ Import Namespace="BVNetwork.EPiSendMail.DataAccess" %>
<%@ Import Namespace="System.Diagnostics" %>
<script runat="server" type="text/C#">
    private void Page_Load(object sender, EventArgs e)
    {
    }

    private void Page_PreRender(object sender, EventArgs e)
    {
        BindJobList();
    }

    private void BindJobList()
    {
        Jobs jobs = Jobs.ListAll();
        rptAllJobs.DataSource = jobs;
        rptAllJobs.DataBind();
    }

    /// <summary>
    /// Imports email addresses as work items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmdAddCsvEmailAddresses_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtAddCsvEmailAddressToJobId.Text))
        {
            lblAddCsvEmailAddressesResult.Text = "Missing Job Id";
            return;
        }

        if (string.IsNullOrEmpty(txtAddEmailWorkItems.Text))
        {
            lblAddCsvEmailAddressesResult.Text = "Address text box is empty. Nothing to add.";
            return;
        }
        
        // Need job first
        int jobId = int.Parse(txtAddCsvEmailAddressToJobId.Text);
        Job job = Job.Load(jobId);
        if (job == null)
        {
            lblAddCsvEmailAddressesResult.Text = "Job with id '" + txtAddCsvEmailAddressToJobId.Text + "' was not found.";
            return;
        }

        string emailAddresses = txtAddEmailWorkItems.Text;
        string[] emailAddressArray = emailAddresses.Split(new char[] { ';', ',', ' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);

        System.Collections.Generic.List<string> invalidEmailAddresses;
        // Time it
        System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
        int imported = job.ImportEmailAddresses(emailAddressArray, out invalidEmailAddresses);

        tmr.Stop();

        // Output rejected addresses
        string rejectedAddressesMsg = "";
        if (invalidEmailAddresses.Count > 0)
        {
            string[] arrayForJoin = invalidEmailAddresses.ToArray();
            rejectedAddressesMsg = string.Format("<br />\nRejected addresses: {0}", string.Join(" | ", arrayForJoin));
        }
        
        lblAddCsvEmailAddressesResult.Text =
            string.Format("Imported {0} of {1} email addresses as work items in {2} ms. {3}", 
                    imported.ToString(), (emailAddressArray.Length).ToString(),
                    tmr.ElapsedMilliseconds.ToString(),
                    rejectedAddressesMsg
                    );
    }

    private void cmdWorkItemsList_Click(object sender, CommandEventArgs e)
    {
        int jobId = int.Parse(e.CommandArgument.ToString());
        Job job = Job.Load(jobId);

        if (e.CommandName == "ShowWorkItems")
        {   
            
            JobWorkItems items = job.WorkItems;
            rptWorkItems.DataSource = items;
            rptWorkItems.DataBind();
            
            txtAddCsvEmailAddressToJobId.Text = e.CommandArgument.ToString();
        }
        else if (e.CommandName == "RemoveWorkItemsThatExistsInRecipientList")
        {
            // Remove all items from the work items list that exists
            // in a recipient list. Normally done with block lists
            if (string.IsNullOrEmpty(txtRecipientListId.Text))
                return;
            
            int recipListId = int.Parse(txtRecipientListId.Text);

            // Filter (with timer)
            Stopwatch tmr = Stopwatch.StartNew();
            int count = job.FilterOnRecipients(recipListId);
            tmr.Stop();

            lblWorkItemsList.Text = string.Format("Filtering {0} work items took {1} ms.",
                    count.ToString(), 
                    tmr.ElapsedMilliseconds.ToString());
        }
        else if (e.CommandName == "AddFromRecipientList")
        {
            // Add all items in the recipient list to the work items
            if (string.IsNullOrEmpty(txtRecipientListId.Text))
                return;

            int recipListId = int.Parse(txtRecipientListId.Text);

            // Filter (with timer)
            Stopwatch tmr = Stopwatch.StartNew();
            int count = job.AddWorkItemsFromRecipientList(recipListId);
            tmr.Stop();

            lblWorkItemsList.Text = string.Format("Adding {0} work items took {1} ms.", 
                    count.ToString(),
                    tmr.ElapsedMilliseconds.ToString());
        }
        else if (e.CommandName == "RemoveAllWorkItems")
        {
            // Delete all work items
            job.DeleteAllWorkItems();
            lblWorkItemsList.Text = "Removed all work items for job";
        }
        else if (e.CommandName == "GetWorkBatch")
        {
            Stopwatch tmr = Stopwatch.StartNew();
            JobWorkItems items = job.GetWorkItemsForProcessing();
            tmr.Stop();
            
            rptWorkItems.DataSource = items;
            rptWorkItems.DataBind();

            lblWorkItemsList.Text = "Got batch for processing in " + tmr.ElapsedMilliseconds.ToString() + " ms.";
        }
        
    }

    private void cmdLoadWorkItem_Click(object sender, CommandEventArgs e)
    {
        
    }

    private void cmdEditWorkItem_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtEditWorkItemJobId.Text))
        {
            lblEditWorkItemResult.Text = "Missing Job Id";
            return;
        }
        
        if(string.IsNullOrEmpty(txtEditWorkItemEmailAddress.Text))
        {
            lblEditWorkItemResult.Text = "Missing Email Address";
            return;
        }
        
        if(string.IsNullOrEmpty(txtEditWorkItemStatus.Text))
        {
            txtEditWorkItemStatus.Text = "Missing Status";
            return;
        }
        
        int id = int.Parse(txtEditWorkItemJobId.Text);
        JobWorkItem workItem = JobWorkItem.Load(id, txtEditWorkItemEmailAddress.Text);
        if (workItem == null)
        {
            lblEditWorkItemResult.Text = "Work Item with job id '" + txtEditWorkItemJobId.Text + "' and email address '" + txtEditWorkItemEmailAddress.Text + "' was not found.";
        }

        workItem.Status = (JobWorkStatus)int.Parse(txtEditWorkItemStatus.Text);

        if (string.IsNullOrEmpty(txtEditWorkItemInfo.Text) == false)
            workItem.Info = txtEditWorkItemInfo.Text;
        else
            workItem.Info = null;
        workItem.Save();

        // Saved it
        lblEditWorkItemResult.Text = "Work Item was saved.";
    }
    
</script>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <h1>Newsletter Work Items Test Page</h1>
    <!-- List all jobs -->
    <asp:Panel CssClass="container" runat="server" ID="pnlListAllJobs">
        <h2>All Jobs</h2>
        <asp:Repeater ID="rptAllJobs" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="0" border="0">
                    <tr>
                        <th>
                            ID</th>
                        <th>
                            Name</th>
                        <th>
                            Page ID</th>
                        <th>Counts</th>
                        <th>
                            Description</th>
                        <th>&nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td valign="top">
                        <%# Eval("Id") %>
                    </td>
                    <td valign="top">
                        <%# Eval("Name") %>
                    </td>
                    <td valign="top">
                        <%# Eval("PageId") %>
                    </td>
                    <td valign="top">
                        NotStarted: <%# ((Job)Container.DataItem).GetWorkItemCountForStatus(JobWorkStatus.NotStarted).ToString() %>
                        <br />
                        Sending: <%# ((Job)Container.DataItem).GetWorkItemCountForStatus(JobWorkStatus.Sending).ToString()%>
                        <br />
                        Failed: <%# ((Job)Container.DataItem).GetWorkItemCountForStatus(JobWorkStatus.Failed).ToString()%>
                        <br />
                        Complete: <%# ((Job)Container.DataItem).GetWorkItemCountForStatus(JobWorkStatus.Complete).ToString()%>
                    </td>
                    <td valign="top">
                        <%# Eval("Description") %>
                    </td>
                    <td valign="top">
                        <asp:LinkButton CommandName="ShowWorkItems" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdWorkItemsList_Click"
                                        Text="Show Work Items" />
                   <br />
                        <asp:LinkButton CommandName="RemoveAllWorkItems" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdWorkItemsList_Click"
                                        Text="Remove Items" />
                   <br />
                   
                        <asp:LinkButton CommandName="RemoveWorkItemsThatExistsInRecipientList" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdWorkItemsList_Click"
                                        Text="Filter from Recipients" />
                   <br />
                    
                        <asp:LinkButton CommandName="AddFromRecipientList" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdWorkItemsList_Click"
                                        Text="Add from Recipients" />
                    <br />
                        <asp:LinkButton CommandName="GetWorkBatch" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        runat="server"
                                        OnCommand="cmdWorkItemsList_Click"
                                        Text="Get Processing Batch" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        Recipient List Id:&nbsp;
        <asp:TextBox ID="txtRecipientListId" runat="server" />
        <br />
        <asp:Label ID="lblWorkItemsList" runat="server" />
    </asp:Panel>
    
    <!-- Edit Work Item-->
    <asp:Panel CssClass="container" runat="server" DefaultButton="cmdEditWorkItem" ID="pnlEditWorkItem">
        <h2>Edit Work Item</h2>
        <label>
            Job ID:
        </label>
        <asp:TextBox ID="txtEditWorkItemJobId" runat="server" />
        <br />
        <label>
            Email Address:
        </label>
        <asp:TextBox ID="txtEditWorkItemEmailAddress" runat="server" />
        <br />
        <label>
            Status:
        </label>
        <asp:TextBox ID="txtEditWorkItemStatus" runat="server" />
        <br />
        <label>
            Info:
        </label>
        <asp:TextBox ID="txtEditWorkItemInfo" runat="server" />
        <br />
        <label>
            &nbsp;</label>
        <asp:Button ID="cmdEditWorkItem" runat="server" Text="Submit" OnClick="cmdEditWorkItem_Click" />
        <br />
        <asp:Label runat="server" ID="lblEditWorkItemResult" />
    </asp:Panel>

    <!-- List Work Items for a given job-->
    <asp:Panel CssClass="container" runat="server" id="pnlWorkItemsForJob">
        <h2>Work Items for Job</h2>
        <asp:Repeater ID="rptWorkItems" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="0" border="0">
                    <tr>
                        <th>
                            Email</th>
                        <th>
                            Status</th>
                        <th>
                            Info</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("EmailAddress")%></td>
                    <td><%# Eval("Status")%></td>
                    <td><%# Eval("Info")%></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <!-- Add Work Items from csv text-->
    <asp:Panel CssClass="container" runat="server" ID="pnlAddEmailAddresses">
        <h2>Add Email addresses</h2>
        Enter the email addresses you want to add as worker items as a comma separated string:
        <br />
        <asp:TextBox Rows="5" Width="100%" runat="server" TextMode="MultiLine" ID="txtAddEmailWorkItems" />
        
        <label style="width: 10em;">Add to Job with ID:</label>
        <asp:TextBox ID="txtAddCsvEmailAddressToJobId" runat="server" />
        <br /><br />
        <asp:Button id="cmdAddCsvEmailAddresses" runat="server" Text="Add Email Addresses" OnClick="cmdAddCsvEmailAddresses_Click" />
        <br />
        <asp:Label ID="lblAddCsvEmailAddressesResult" runat="server" />
    </asp:Panel>
</asp:Content>