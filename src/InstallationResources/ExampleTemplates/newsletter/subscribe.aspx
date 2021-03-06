<%--
    Template: Subscribe to newsletters
        Create a page of this type to let visitors subscribe to one or more
        Newsletters. The Newsletter list will show all public recipient lists.
    
    Notes:
        AutoEventWireup has been turned off for the page, so you will have to
        wire your events manually if you add your own controls
        
        This template has no code behind file. It is not needed as all the logic
        is in the Bvn:NewsletterSubscribe user control. You do not need to recompile
        your project after adding this template.
        
        If you have a different masterpage file, or have renamed the content 
        placeholders, change the code below, or put the user control on one of
        your existing templates.
--%>
<%@ Page language="c#" Codebehind="subscribe.aspx.cs" 
         MasterPageFile="~/templates/public/MasterPages/MasterPage.master" 
         AutoEventWireup="false" 
         Inherits="EPiServer.TemplatePage" %>
<%@ Register TagPrefix="Bvn"        TagName="NewsletterSubscribe"   Src="~/bvn/sendmail/units/subscribe.ascx"%>
<%@ Register TagPrefix="public"     TagName="MainBody"              Src="~/Templates/Public/Units/Placeable/MainBody.ascx" %>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <%-- We need to bring in the main body too, as we're overwriting the MainBodyRegion --%>
    <div id="MainBody">
	    <public:MainBody runat="server" />
        <%--	
          This control will render the list of available public recipient lists.
          Users can only join public recipient lists
        --%>
	    <Bvn:NewsletterSubscribe runat="server" />
	    <EPiServer:Property PropertyName="MainBodyBottom" DisplayMissingMessage="false" runat="server" />
    </div>
</asp:Content>


