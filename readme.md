# Newsletter for EPiServer #
This is a port of the old EPiSendMail (BV Network Newsletter) module from EPiCode, rewritten to support EPiServer CMS and Commerce 7.5 and newer.

## Features ##
 * Create newsletters as pages in EPiServer
 * Has recipient lists to hold your emails
 * Example templates with responsive email design using Zurb Ink
 * Will inline CSS to help make the email look correct in all (major) email clients
 * Configurable sender system (supports SMTP Server, pick up directory and [Mailgun](http://www.mailgun.com))
 * Send newsletters to contacts in Commerce using filtered contact lists

### Additional Features when using Mailgun ###
 * Supports large amount of emails
 * Low cost per email (first 10.000 emails per month are free)
 * Supports tracking of delivery, open and clicks
 * Uses the Mailgun campaign feature to get a better overview of your newsletters

## Downloads ##
The installation is done through Visual Studio using the EPiServer Nuget Feed on http://nuget.episerver.com/ 

Main Newsletter Add-on:
```
Install-Package EPiCode.Newsletter
```

Commerce Integration:
```
Install-Package EPiCode.Newsletter.Commerce
```

MVC Examples for Alloy:
```
Install-Package EPiCode.Newsletter.Examples
```
> **Note!** The examples package has requirements to code and configuration in the Alloy MVC sample. It will most likely fail to compile if you include them in a custom built site.
   
## Example Newsletter Designs ##
The module will send a page in EPiServer CMS if it inherits the `NewsletterBase` base class. In addition to the Newsletter module itself, you also need a layout and design for the newsletter, that works across as many Email clients as possible. Download and test two example newsletters based on the [Zurb Ink](http://zurb.com/ink/) framework that has been made to work in the MVC version of the Alloy sample site.

## Installation ##
1. Select the [EPiServer Nuget Feed](http://nuget.episerver.com/feed/packages.svc/) in Visual Studio Nuget Package Manager Dialog or the Packet Manger Console
1. Install the EPiCode.Newsletter or EPiCode.Newsletter.Examples (for Alloy) package
1. Recompile your site (if you install the examples)

## Configuration ##
The module supports sending emails through different mail senders (also called a ''sender engine''):
1. Recommended: Mailgun (http://mailgun.com)
1. Built-in smtp support (default)
1. aspnetEmail component from [AdvancedIntellect](http://www.advancedintellect.com/product.aspx?smtp) (license purchase required). **Note!** This sender is not actively supported.

**Note!** We recommend using the Mailgun sender, as it has the most features and offloads the sending process to the Mailgun servers. The first 10 000 emails from Mailgun per month is free. In order to send more emails you need to register a credit card.

The aspnetEmail sender engine is included for backwards compatibility, but will eventually be removed from the module.

### Configuring SMTP ###
When installing the module, the default sender is the built-in SMTP client in .NET. In order to send emails, you need to configure the `mailSettings` in web.config:

Example using smtp server. There are more settings you can configure for password protected servers, ssl support etc.
```
#!xml
<system.net>
  <mailSettings>
    <smtp deliveryMethod="Network" from="noreply@my.domain.com">
      <network host="your.smtp.server"/>
    </smtp>
  </mailSettings>
</system.net>
```

You can also use a pick up directory if you have a SMTP server on your network that can access the pickup directory. The pickup directory delivery is preferable to network, as sending emails goes a lot faster, and you are less likely to get timeouts in the scheduled job.
```
#!xml
<system.net>
  <mailSettings>
    <smtp deliveryMethod="SpecifiedPickupDirectory" from="noreply@my.domain.com">
      <specifiedPickupDirectory pickupDirectoryLocation="c:\temp\email-pickup-directory" />
    </smtp>        
  </mailSettings>
</system.net>
```
**Note!** The `pickupDirectoryLocation` can be on a network share.

### Configuring Mailgun ###
Add the following keys to the `<appSettings>` section in web.config to use Mailgun:
```
#!text/xml
<add key="Newsletter.SenderType" value="BVNetwork.EPiSendMail.Library.MailSenderMailgun, BVNetwork.EPiSendMail" />
<add key="Mailgun.ApiKey"    value="your-mailgun-api-key-here" />
<add key="Mailgun.Domain"    value="your-mailgun-domain-here" />
<add key="Mailgun.PublicKey" value="your-mailgun-public-key-here" />
```

The Api keys can be found on the Mailgun Account home page: https://mailgun.com/cp

You also need to configure your mail sending domain for your Mailgun account, which is well described on your Mailgun account page. For testing purposes, you can use the sandbox domain that is created when you register your Mailgun account.

## Troubleshooting ##
If you get an error during startup like this: `Cannot add duplicate collection entry of type 'add' with unique key attribute 'name' set to 'ExtensionlessUrlHandler-Integrated-4.0'` please check your web.config and remove any duplicate `ExtensionlessUrlHandler-Integrated-4.0` handlers (under `system.webServer`)

### Missing Assembly Redirects ###
In some cases, there are missing assembly redirects for some of the `System.Net` and `System.Web` assemblies. Make sure you have these in your web.config:
```
#!xml
<assemblyBinding>
  ...
  <dependentAssembly>
    <assemblyIdentity name="System.Web.Http" publicKeyToken="31bf3856ad364e35" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-5.1.0.0" newVersion="5.1.0.0" />
  </dependentAssembly>
  <dependentAssembly>
    <assemblyIdentity name="System.Net.Http.Formatting" publicKeyToken="31bf3856ad364e35" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-5.1.0.0" newVersion="5.1.0.0" />
  </dependentAssembly>
  <dependentAssembly>
    <assemblyIdentity name="System.Web.Http.WebHost" publicKeyToken="31bf3856ad364e35" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-5.1.0.0" newVersion="5.1.0.0" />
  </dependentAssembly>
  <dependentAssembly>
    <assemblyIdentity name="System.Net.Http" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
  </dependentAssembly>
</assemblyBinding>

```
'''Important! ''' You might have never versions of these referenced in your project. Make sure you check the actual versions of the assemblies before you add the redirects.


## Creating Newsletters ##
For a good example on how to create a newsletter design and content, see the Examples package above. The Newsletter module recognizes Newsletter pages in your site when your page type inherits from the NewsletterBase class, or implements the IRegisterNewsletterDetailsView.

You can have many different newsletter designs, just inherit the base class, and they will be recognized.

When the HTML for the newsletter is created, all styles will be inlined with the html elements to make it more probable that the design will look good in all major email clients.

## Support ##
This module is open source and unsupported. Feel free to [register new issues](https://github.com/BVNetwork/Newsletter/issues/new) though.

## Requirements ##
Runtime:
 * EPiServer CMS 7.9 or newer
 * .NET 4.5

## Dependencies ##
 * PreMailer.Net (1.2.6 or newer)
 * Microsoft.AspNet.WebApi (5.1.2 or newer)
 * RestSharp (104.4.0 or newer)

## Supports ##
 * EPiServer Commerce 7.5 or newer for custom recipient lists

## Contributions by ##
BV Network AS
Departementenes sikkerhets- og serviceorganisasjon (DSS)
