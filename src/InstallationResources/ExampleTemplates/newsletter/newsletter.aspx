<%--
    Template: Example Newsletter
        This is the content of the newsletter you will be sending as emails. The
        html will be screen-scraped by the Newsletter engine before sending the
        emails.
    
    Notes:
        AutoEventWireup has been turned off for the page, so you will have to
        wire your events manually if you add your own controls.
        
        EnableViewState has been turned off. We do NOT want viewstate in the emails
        sent.
        
        This template has no code behind file. You do not need to recompile
        your project after adding this template.
        
        If you want more than one newsletter layout, create new templates like
        this one, and configure the newsletter engine to recognize the page types
        as newsletters.
        
        Different email clients have different rules for parsing html. Some will
        remove all styles, some will remove or change html. Don't bother making
        clean markup, try to add fallbacks in case your styles are stripped by 
        duplicating the most imporant ones to the tags.
        
        Try to keep the newsletters as simple as possible. Pixel precision is futile
        and will break at some point.
        
        People might forward your newsletters. Make sure you test for that, using
        sensible fonts and colors for the text used when forwarding.
        
        The meta markup (asp.net server side stuff) has been written to minimize 
        empty lines in the start of the resulting html.

--%><%@ Page EnableViewState="False" 
         language="c#" 
         Codebehind="newsletter.aspx.cs" 
         AutoEventWireup="false" 
         Inherits="EPiServer.SimplePage" 
%><script runat="server" type="text/C#">
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // We're using databinding syntax in the template, so we need to DataBind
        this.DataBind();
    }
</script><%@
 Register TagPrefix="MailSender" Namespace="BVNetwork.EPiSendMail.WebControls" Assembly="BVNetwork.EPiSendMail" 
%><?xml version="1.0" encoding="utf-8"?>
<!doctype html public "-//w3c//dtd html 4.01 transitional//en" "http://www.w3.org/tr/html4/loose.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="no" lang="no">
	<head>
		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="content-language" content="en">
		<meta http-equiv="content-style-type" content="text/css" />
		<title>
			<%= CurrentPage.PageName %>
		</title>
		<%-- Make sure the style is indented. Outlook will remove any leading 
			 periods if they are the first on the line, making your classes useless --%>
		<style>
			#maincontentdiv {
				font-family: Tahoma, Verdana, Arial, Helvetica;
				font-size: 0.8em;
			}			
		</style>		
	</head>
	<body bgcolor="#FFFFFF" text="#000000" topmargin="10"><%--
		    We do not include the form tag here, as we don't want
		    it inside the email, and it will also remove the hidden
		    viewstate field (removing runat=server from the form
		    also do that - in case you need the form field.)
		--%><%-- 
		Center for IE --%>
		<div style="text-align: center" id="maincontentdiv">
			<div style="width: 500px; margin-left: auto; margin-right: auto; text-align: left">
				<h1 style="font-size: 2.1em; border-bottom: 0px solid #ccc;color:#000;background-color:#ffffff;"><%= CurrentPage["MailTitle"] == null ? CurrentPage["PageName"] : CurrentPage["MailTitle"]%></h1>
				<div
					visible='<%# CurrentPage["MainBody"] != null %>'
					id="divMainBody"
					runat="server"
					style="border-bottom: 0px solid #fff;"
					>
					<%# CurrentPage["MainBody"] %>
				</div>
				
				<table cellpadding="0" cellspacing="0" width="100%">
					<tr>
						<td align="center"
							style="background-color:#fafafa;font-size:0.7em">
							<%# CurrentPage["NewsLetterFooterContent"] %>					
						</td>
					</tr>
				</table>
			</div>
		</div>
	</body>
</html>
