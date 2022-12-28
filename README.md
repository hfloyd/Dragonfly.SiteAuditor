# Dragonfly.SiteAuditor #

A collection of tools to extract data about an Umbraco 7 site created by [Heather Floyd](https://www.HeatherFloyd.com).

[![Dragonfly Website](https://img.shields.io/badge/Dragonfly-Website-A84492)](https://DragonflyLibraries.com/umbraco-packages/site-auditor/) [![Umbraco Marketplace](https://img.shields.io/badge/Umbraco-Marketplace-3544B1?logo=Umbraco&logoColor=white)](https://marketplace.umbraco.com/package/Dragonfly.SiteAuditor) [![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.SiteAuditor)](https://www.nuget.org/packages/Dragonfly.SiteAuditor/) [![GitHub](https://img.shields.io/badge/GitHub-Sourcecode-blue?logo=github)](https://github.com/hfloyd/Dragonfly.SiteAuditor)

## Versions ##
This package is designed to work with Umbraco 7. [View all available versions](https://DragonflyLibraries.com/umbraco-packages/site-auditor/#Versions).

## Installation ##

[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.SiteAuditor)](https://www.nuget.org/packages/Dragonfly.SiteAuditor/)


```
PM>   Install-Package Dragonfly.SiteAuditor

```


## Usage ##
The Tools are accessed via Umbraco's WebApi path.

Try: 

http://Yoursite.com/Umbraco/backoffice/Api/SiteAuditorApi/Help

*NOTE: You need to be logged-in to the Umbraco back-office in order to access the tools.

You can edit some of the tools' output via the Partial Views installed in /Views/Partials/SiteAuditor
