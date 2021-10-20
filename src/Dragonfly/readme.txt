------------------------------------------------
--------- Dragonfly SiteAuditor Readme ---------
------------------------------------------------

The Tools are accessed via Umbraco's WebApi path.
Try: /Umbraco/backoffice/Api/SiteAuditorApi/Help

If you'd like them to be accessible via the Back-office Dashboard as well, update the file '\Config\Dashboard.config' with this:

  <section alias="Dragonfly.SiteAuditor">
    <areas>
      <area>content</area>
    </areas>
    <tab caption="Site Auditor">
      <control>
        /App_Plugins/Dragonfly.SiteAuditor/Dashboard.html
      </control>
    </tab>
  </section>


*NOTE: You need to be logged-in to the Umbraco back-office in order to access the tools.

You can edit some of the tools' output via the Partial Views installed in ~/App_Plugins/Dragonfly.SiteAuditor/Views