@using EPiServer.Editor
@using EPiServer.Shell.UI
@model $rootnamespace$.Models.Pages.NewsletterWithSidebarPage
@{
    Layout = null;
}

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width" />
    <style>
            @Html.Raw(File.ReadAllText(Server.MapPath("~/static/ink/ink.css")))
    </style>
    <style>
        table.facebook td {
            background: #3b5998;
            border-color: #2d4473;
        }

        table.facebook:hover td {
            background: #2d4473 !important;
        }

        table.twitter td {
            background: #00acee;
            border-color: #0087bb;
        }

        table.twitter:hover td {
            background: #0087bb !important;
        }

        table.google-plus td {
            background-color: #DB4A39;
            border-color: #CC0000;
        }

        table.google-plus:hover td {
            background: #CC0000 !important;
        }

        .template-label {
            color: #ffffff;
            font-weight: bold;
            font-size: 11px;
        }

        .callout .wrapper {
            padding-bottom: 20px;
        }

        .callout .panel {
            background: #ECF8FF;
            border-color: #b9e5ff;
        }

        .header {
            background: #999999;
        }

        .footer .wrapper {
            background: #ebebeb;
        }

        .footer h5 {
            padding-bottom: 10px;
        }

        table.columns .text-pad {
            padding-left: 10px;
            padding-right: 10px;
        }

        table.columns .left-text-pad {
            padding-left: 10px;
        }

        table.columns .right-text-pad {
            padding-right: 10px;
        }

        @@media only screen and (max-width: 600px) {

            table[class="body"] .right-text-pad {
                padding-left: 10px !important;
            }

            table[class="body"] .left-text-pad {
                padding-right: 10px !important;
            }
        }

        /* Edit mode only */
        .overflow-hidden {
            overflow: hidden !important;
        }

        body {
            background-color: #fff;
        }
        .body {
            background-color: #fff;
        }
    </style>
</head>
<body>
    <table class="body">
        <tr>
            <td class="center" align="center" valign="top">
                <center>
                    <table class="row header">
                        <tr>
                            <td class="center" align="center">
                                <center>
                                    <table class="container">
                                        <tr>
                                            <td class="wrapper last">
                                                <table class="twelve columns">
                                                    <tr>
                                                        <td class="six sub-columns">
                                                            @Html.PropertyFor(m => m.Logo, new { CssClass = "overflow-hidden" })
                                                        </td>
                                                        <td class="six sub-columns last" style="text-align:right; vertical-align:middle;">
                                                            <span class="template-label">@Html.PropertyFor(m => m.HeaderText)</span>
                                                        </td>
                                                        <td class="expander"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </center>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table class="container">
                        <tr>
                            <td>
                                <!-- content start -->
                                <table class="row">
                                    <tr>
                                        <td class="wrapper last">
                                            <table class="twelve columns">
                                                <tr>
                                                    <td>
                                                        <h1>@Html.PropertyFor(m => m.PageName)</h1>
                                                        @if (PageEditing.PageIsInEditMode || Model.Lead != null)
                                                        {
                                                            <p class="lead">@Html.PropertyFor(m => m.Lead)</p>
                                                        }

                                                        <img width="580" height="300" src="http://lorempixel.com/g/580/300/" />
                                                    </td>
                                                    <td class="expander"></td>
                                                </tr>
                                            </table>
                                            @if (PageEditing.PageIsInEditMode || Model.SubPanel != null) {
                                                <table class="twelve columns">
                                                    <tr>
                                                        <td class="panel">
                                                            <p>@Html.PropertyFor(m => m.SubPanel)</p>
                                                        </td>
                                                        <td class="expander"></td>
                                                    </tr>
                                                </table>
                                            }
                                        </td>
                                    </tr>
                                </table>
                                <br />  <!-- Break Tag for row -->
                                <table class="row">
                                    <tr>
                                        <td class="wrapper">
                                            <table class="six columns">
                                                <tr>
                                                    <td>
                                                        @if (PageEditing.PageIsInEditMode == false)
                                                        {
                                                            @Html.Raw("<p class=\"overflow-hidden\">")
                                                        }

                                                        @Html.PropertyFor(m => m.MainBody)

                                                        @if (PageEditing.PageIsInEditMode == false)
                                                        {
                                                            @Html.Raw("</p>")
                                                        }

                                                        @Html.PropertyFor(m => m.CallToActionButton)
                                                    </td>
                                                    <td class="expander"></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="wrapper last">
                                            <table class="six columns">
                                                <tr>
                                                    <td class="panel">
                                                        <h6>@Html.PropertyFor(m => m.LinkListTitle)</h6>
                                                        <div @Html.EditAttributes(m => m.LinkList) style="margin-top: 1em;">
                                                        @foreach (var linkItem in Model.LinkList ?? Enumerable.Empty<EPiServer.SpecializedProperties.LinkItem>())
                                                        {
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            @Html.ContentLink(linkItem)
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <hr />
                                                        }
                                                        </div>
                                                    </td>
                                                    <td class="expander"></td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table class="six columns">
                                                <tr>
                                                    <td class="panel" @Html.EditAttributes(m => m.ContactDetails)>
                                                        @Html.Partial("Blocks/NewsletterContactDetailsBlock", Model.ContactDetails)
                                                    </td>
                                                    <td class="expander"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <!-- Legal + Unsubscribe -->
                                <table class="row">
                                    <tr>
                                        <td class="wrapper last">
                                            <table class="twelve columns">
                                                <tr>
                                                    <td align="center">
                                                        <center>
                                                            <p style="text-align:center;"><a href="#">Terms</a> | <a href="#">Privacy</a> | <a href="http://localhost/unsub?id=%recipient%">Unsubscribe</a></p>
                                                        </center>
                                                    </td>
                                                    <td class="expander"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <!-- container end below -->
                            </td>
                        </tr>
                    </table>
                </center>
            </td>
        </tr>
    </table>

</body>
</html>
