<%@ Page Language="c#" Inherits="System.Web.UI.Page" AutoEventWireup="true" %>
<html>
<head>
    <title>Test Newsletter</title>
    <style type="text/css">
            body {
                font-family: Tahoma;
                font-size: 0.8em;
            }
            
            .container {
                border: 2px #eee solid;
                padding: 1em;
                padding-top: 0.6em;
                margin-top: 1em;
            }
            
            .container h2 {
                margin-top: -1.2em;
                font-size: 1.3em;
            }
            
            .container label {
                width: 7em;
            }
            
            .container table {
                border: solid 1px #eee;
                font-size: 1em;
            }
            
            .container td {
                border: solid 1px #eee;
            }
            
            .container th {
                text-align: left;
            }
        </style>
    <%-- The navigate script is needed by EPiServer to be able to
         navigate away from this page --%>
    <script type='text/javascript'>
		<!--
		function onNavigate(newPageLink)
		{
			return -1;
		}
		function onCommand(newCommand)
		{
			return -1;
		}
		// -->
	</script>
        
</head>
<body>
    <form runat="server">
        <a href="jobs.aspx">Jobs</a>
        <br />
        <a href="workitems.aspx">Worker Items</a>
        <br />
        <a href="RecipientLists.aspx">Recipient Lists</a>
    </form>
</body>
</html>
