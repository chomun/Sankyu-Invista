' Note: For instructions on enabling IIS6 or IIS7 classic mode, 
' visit http://go.microsoft.com/?LinkId=9394802
Imports System.Web.Http
Imports System.Web.Optimization

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    Sub Application_Start()
        AreaRegistration.RegisterAllAreas()

        WebApiConfig.Register(GlobalConfiguration.Configuration)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        log4net.Config.XmlConfigurator.Configure()
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf AppDomain_UnhandledException
    End Sub

    Private Sub AppDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)

        Dim logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        logger.Error(e.ExceptionObject.ToString())

    End Sub
    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)

        Dim DB As New INV3Entities
        Dim x As USER_MASTER
        Dim userName As String = CommonFunction.NothingToString(Session("CURRENT_USER_NAME"))
        Dim countryCode As String = CommonFunction.NothingToString(Session("CURRENT_COUNTRY"))
        x = DB.USER_MASTER.Where(Function(u) u.USER_ID = userName And u.COUNTRY_CD = countryCode).FirstOrDefault()
        If Not IsNothing(x) Then
            x.SESSION_ID = "LOGOFF"
            x.UPDATED_DT = DateTime.Now
            DB.SaveChanges()
        End If

    End Sub
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        If Session("CURRENT_OUTBOUND_ORDER") <> "" And Session("CURRENT_REV") <> "" Then
            CommonFunction.UnLockShippingOut(Session("CURRENT_OUTBOUND_ORDER"), Session("CURRENT_REV"), Session("CURRENT_COUNTRY"))
        End If
        Dim logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        logger.Error(Server.GetLastError())
    End Sub

End Class
