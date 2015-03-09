<%@ Page Language="c#" MasterPageFile="~/modules/bvn/SendMail/Data/Test/test.master"
         Inherits="System.Web.UI.Page" AutoEventWireup="true" %>
<%@ Import Namespace="BVNetwork.EPiSendMail.DataAccess" %>
<script runat="server" type="text/C#">
    private void Page_Load(object sender, EventArgs e)
    {

    }

    private void cmdAddNewJob_Click(object sender, EventArgs e)
    {
        int pageId = 0;
        if (string.IsNullOrEmpty(txtNewJobPageId.Text) == false)
            pageId = int.Parse(txtNewJobPageId.Text);

        Job job = new Job(pageId, txtNewJobName.Text, txtNewJobDescription.Text);
        job.Save();

        lblNewJobResult.Text = "Added new job. It got id: " + job.Id;
    }

    private void Page_PreRender(object sender, EventArgs e)
    {
        BindJobList();
    }

    private void cmdDeleteById_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtLoadById.Text))
            return;

        int id = int.Parse(txtLoadById.Text);
        Job job = Job.Load(id);
        if (job == null)
        {
            lblLoadById.Text = "Job with id '" + txtLoadById.Text + "' was not found.";
            txtEditJobId.Text = "";
            txtEditJobName.Text = "";
            txtEditJobPageId.Text = "";
            txtEditJobDescription.Text = "";

            return;
        }
        
        // Delete it
        job.Delete();
        lblLoadById.Text = "Job was deleted";
    }
        
    private void cmdLoadById_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtLoadById.Text))
            return;

        int id = int.Parse(txtLoadById.Text);
        Job job = Job.Load(id);
        if (job == null)
        {
            lblLoadById.Text = "Job with id '" + txtLoadById.Text + "' was not found.";
            txtEditJobId.Text = "";
            txtEditJobName.Text = "";
            txtEditJobPageId.Text = "";
            txtEditJobDescription.Text = "";

            return;
        }

        lblLoadById.Text = job.ToString();
        lblLoadById.Text += "<br />";
        lblLoadById.Text += string.Format("NotStarted: {0} | Sending: {1} | Failed: {2} | Complete: {3}",
            job.GetWorkItemCountForStatus(JobWorkStatus.NotStarted).ToString(),
            job.GetWorkItemCountForStatus(JobWorkStatus.Sending).ToString(),
            job.GetWorkItemCountForStatus(JobWorkStatus.Failed).ToString(),
            job.GetWorkItemCountForStatus(JobWorkStatus.Complete).ToString()
            );
        
        txtEditJobId.Text = job.Id.ToString();
        txtEditJobName.Text = job.Name;
        txtEditJobPageId.Text = job.PageId.ToString();
        txtEditJobDescription.Text = job.Description;
    }

    private void cmdEditJob_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtEditJobId.Text))
            return;

        int id = int.Parse(txtEditJobId.Text);
        Job job = Job.Load(id);
        if (job == null)
        {
            lblEditJobResult.Text = "Job with id '" + txtEditJobId.Text + "' was not found.";
        }

        job.Name = txtEditJobName.Text;
        job.PageId = int.Parse(txtEditJobPageId.Text);
        job.Description = txtEditJobDescription.Text;
        job.Save();

        // Saved it, rebind it
        BindJobList();
        lblEditJobResult.Text = "Job with id '" + txtEditJobId.Text + "' was saved.";
    }

    private void BindJobList()
    {
        Jobs jobs = Jobs.ListAll();
        rptAllJobs.DataSource = jobs;
        rptAllJobs.DataBind();
    }
</script>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
        
        <h1>Newsletter Jobs Test Page</h1>
        <!-- Add new -->
        <asp:Panel CssClass="container" runat="server" DefaultButton="cmdAddNewJob">
            <h2>Add New Job</h2>
            <label>
                Name:
            </label>
            <asp:TextBox ID="txtNewJobName" runat="server" />
            <br />
            <label>
                Page ID:
            </label>
            <asp:TextBox ID="txtNewJobPageId" runat="server" />
            <br />
            <label>
                Description:
            </label>
            <asp:TextBox ID="txtNewJobDescription" runat="server" />
            <br />
            <label>
                &nbsp;</label>
            <asp:Button ID="cmdAddNewJob" runat="server" Text="Add Job" OnClick="cmdAddNewJob_Click" />
            <br />
            <asp:Label runat="server" ID="lblNewJobResult" />
        </asp:Panel>
        <!-- List all jobs -->
        <asp:Panel CssClass="container" runat="server">
            <h2>All Jobs</h2>
            <asp:Repeater ID="rptAllJobs" runat="server">
                <HeaderTemplate>
                    <table cellpadding="4" cellspacing="0" border="0">
                        <tr>
                            <th>
                                ID</td>
                            <th>
                                Name</td>
                            <th>
                                Page ID</td>
                            <th>
                                Description</td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Id") %>
                        </td>
                        <td>
                            <%# Eval("Name") %>
                        </td>
                        <td>
                            <%# Eval("PageId") %>
                        </td>
                        <td>
                            <%# Eval("Description") %>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
        <!--  Show or Delete Job by ID -->
        <asp:Panel CssClass="container" runat="server" DefaultButton="cmdLoadById">
            <h2>Show Job by ID</h2>
            <label>
                Job ID:</label>
            <asp:TextBox ID="txtLoadById" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="cmdLoadById" runat="server" OnClick="cmdLoadById_Click" Text="Load" />
            &nbsp;
            <asp:Button ID="cmdDeleteById" runat="server" OnClick="cmdDeleteById_Click" Text="Delete" />
            <br />
            <asp:Label ID="lblLoadById" runat="server" />
        </asp:Panel>
        <!-- Edit Job-->
        <asp:Panel CssClass="container" runat="server" DefaultButton="cmdEditJob">
            <h2>Edit Job</h2>
            <label>
                Job ID:
            </label>
            <asp:TextBox ID="txtEditJobId" runat="server" />
            <br />
            <label>
                Name:
            </label>
            <asp:TextBox ID="txtEditJobName" runat="server" />
            <br />
            <label>
                Page ID:
            </label>
            <asp:TextBox ID="txtEditJobPageId" runat="server" />
            <br />
            <label>
                Description:
            </label>
            <asp:TextBox ID="txtEditJobDescription" runat="server" />
            <br />
            <label>
                &nbsp;</label>
            <asp:Button ID="cmdEditJob" runat="server" Text="Edit Job" OnClick="cmdEditJob_Click" />
            <br />
            <asp:Label runat="server" ID="lblEditJobResult" />
        </asp:Panel>
</asp:Content>