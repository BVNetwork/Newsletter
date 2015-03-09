<%@ Page Language="C#" AutoEventWireup="false" enableviewstate="false" CodeBehind="NewsletterPageTemplate.aspx.cs" Inherits="BVNetwork.EPiSendMail.Views.Pages.NewsletterPageTemplate" %>
<?xml version="1.0" encoding="utf-8"?>
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
		<style type="text/css">
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