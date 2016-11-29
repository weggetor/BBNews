// https://code.ciphertrick.com/2015/08/31/server-side-pagination-in-angularjs/
(function () {
    "use strict";

    angular
        .module("bbNewsApp")
        .controller("detailController", detailController);

    detailController.$inject = ["$scope", "$routeParams", "$location", "$window", "$log", "$sce", "ngProgress", "ngToast", "viewService", "moduleProperties"];

    function detailController($scope, $routeParams, $location, $window, $log, $sce, ngProgress, ngToast, viewService, moduleProperties) {

        var vm = this;
        
        vm.moduleProperties = JSON.parse(moduleProperties);
        vm.localize = vm.moduleProperties.Resources;
        vm.settings = vm.moduleProperties.Settings;
        vm.isEditable = vm.moduleProperties.IsEditable; // Is in edit Mode
        vm.editMode = vm.moduleProperties.EditMode; // has right to edit
        vm.isAdmin = vm.moduleProperties.IsAdmin;
        vm.moduleId = vm.moduleProperties.ModuleId;
        vm.PortalId = vm.moduleProperties.PortalId;
        vm.userId = vm.moduleProperties.UserId;
        vm.moduleDirectory = vm.moduleProperties.ModuleDirectory;
        vm.portalLanguages = vm.moduleProperties.PortalLanguages;
        vm.currentLanguage = vm.moduleProperties.CurrentLanguage;
        vm.news = {};
        vm.newsId = parseInt($routeParams.id);

        vm.getSingleNews = getSingleNews;

        getSingleNews(vm.newsId);

        function getSingleNews(newsId) {
            ngProgress.color('red');
            ngProgress.start();
            viewService.getSingleNews(newsId)
                .success(function (response) {
                    vm.news = convertDateValues(response);
                    ngProgress.complete();
                })
                .error(function(errData) {
                    $log.error(vm.localize.GetNews_DataError, errData);
                    ngProgress.complete();
                    ngToast.danger({ content: vm.localize.GetNews_DataError + ' ' + errData.Message });
                });
        }

        function convertDateValues(record) {
            if (record.PubDate)
                record.PubDate = new Date(record.PubDate);
            return record;
        }
    }
})();
