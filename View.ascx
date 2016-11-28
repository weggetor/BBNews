<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.View" %>
<%@ Register TagPrefix="dnn" TagName="JavaScriptLibraryInclude" Src="~/admin/Skins/JavaScriptLibraryInclude.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:JavaScriptLibraryInclude runat="server" Name="AngularJS" />
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-route" />
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-sanitize" />
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-ng-progress"  />
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-utils-pagination" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/libraries/angular-ng-progress/01_00_07/ngProgress.min.css" />
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-ng-toast"/>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/libraries/angular-ng-toast/01_05_06/ngToast.min.css"/>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/libraries/angular-ng-toast/01_05_06/ngToast-animations.min.css"/>
<dnn:JavaScriptLibraryInclude runat="server" Name="angular-utils-pagination"/>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/BBNews/js/app.js" Priority="40"/>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/BBNews/js/Services/viewService.js" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/BBNews/js/Controller/viewController.js" Priority="100" />

<div id="bbNewsApp<%=ModuleId%>">
    <div ng-view>Loading...</div>
    <toast></toast>
</div>

<script>
    $( document ).ready(function() {
        var fileref = document.createElement('script');
        fileref.setAttribute("src", "/Resources/libraries/AngularJS/01_04_05/i18n/angular-locale_<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name%>.js");
        document.getElementsByTagName("head")[0].appendChild(fileref);
    });    
    angular.element(document).ready(function () {
        function init(appName, moduleId, apiPath) {
            var sf = $.ServicesFramework(moduleId);
            var httpHeaders = { "ModuleId": sf.getModuleId(), "TabId": sf.getTabId(), "RequestVerificationToken": sf.getAntiForgeryValue() };
            var localAppName = appName + moduleId;
            var application = angular.module(localAppName, [appName])
                .constant("serviceRoot", sf.getServiceRoot(apiPath))
                .constant("moduleProperties", '<%=ModuleProperties%>')
                .config(function($httpProvider,$routeProvider) {
                    
                    // Extend $httpProvider with serviceFramework headers
                    angular.extend($httpProvider.defaults.headers.common, httpHeaders);

                    jsFileLocation = '<%=ControlPath%>js/';

                    $routeProvider
                        .when("/", {templateUrl: jsFileLocation + "Templates/View.html", controller: "viewController", controllerAs: "vm"})
                        .otherwise({redirectTo: '/'});
                });
            return application;
        };

        var app = init("bbNewsApp", <%=ModuleId%>, "BBNews");
        var moduleContainer = document.getElementById("bbNewsApp<%=ModuleId%>");
        angular.bootstrap(moduleContainer, [app.name]);
    });
</script>