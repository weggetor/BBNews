// https://code.ciphertrick.com/2015/08/31/server-side-pagination-in-angularjs/
(function () {
    "use strict";

    angular
        .module("bbNewsApp")
        .controller("viewController", viewController);

    viewController.$inject = ["$scope", "$routeParams", "$location", "$window", "$log", "$sce", "ngProgress", "ngToast", "viewService", "moduleProperties"];

    function viewController($scope, $routeParams, $location, $window, $log, $sce, ngProgress, ngToast, viewService, moduleProperties) {

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
        vm.allNews = [];
        vm.chunkedNews = [];
        vm.totalCount = 0;
        vm.searchText = "";
        vm.pageSize = parseInt(vm.settings.RowsPerPage) * parseInt(vm.settings.NewsInRow);
        vm.startDate = vm.settings.StartDate ? new Date(vm.settings.StartDate).toISOString().substring(0, 10) : "1753-01-02";
        vm.endDate = vm.settings.EndDate ? new Date(vm.settings.EndDate).toISOString().substring(0, 10) : "9999-12-31";
        vm.topN = vm.settings.TopN ? parseInt(vm.settings.TopN) : -1;
        vm.includeHidden = false;
        vm.bsClass = "col-md-" + 12 / parseInt(vm.settings.NewsInRow);

        if (vm.settings.View == "1") {
            var myEl = angular.element(document.querySelector('#bbnews-marquee'));
            if (vm.settings.MarqueeDirection) {
                switch (vm.settings.MarqueeDirection) {
                    case "1":
                        myEl.attr('direction', "right");
                        break;
                    case "2":
                        myEl.attr('direction', "up");
                        break;
                    case "3":
                        myEl.attr('direction', "down");
                        break;
                }
            }
            if (vm.settings.MarqueeScrollAmount)
                myEl.attr('scrollamount',vm.settings.MarqueeScrollAmount);
            if (vm.settings.MarqueeScrollDelay)
                myEl.attr('scrolldelay',vm.settings.MarqueeScrollDelay);
            if (vm.settings.MarqueeAlternate)
                myEl.attr('behaviour', 'alternate');
        }

        vm.getNews = getNews;
        vm.search = search;

        getNews(vm.PortalId, vm.settings.CategoryID, vm.topN, vm.startDate, vm.endDate, 1, vm.pageSize, vm.includeHidden, vm.searchText);

        function getNews(portalId, categoryId, topN, startDate, endDate, pageNum, pageSize, includeHidden, search) {
            ngProgress.color('red');
            ngProgress.start();
            viewService.getNews(portalId, categoryId, topN, startDate, endDate, pageNum, pageSize, includeHidden, search)
                .success(function (response) {
                    vm.chunkedNews = [];
                    vm.allNews = response.Data;
                    vm.totalCount = parseInt(response.TotalCount);
                    for (var i = 0; i < vm.allNews.length; i++) {
                        vm.allNews[i] = convertDateValues(vm.allNews[i]);
                    }
                    var groupSize = parseInt(vm.settings.NewsInRow);
                    var i, j, tempArray, chunk = groupSize;
                    for (i = 0, j = vm.allNews.length; i < j; i += chunk) {
                        tempArray = vm.allNews.slice(i, i + chunk);
                        vm.chunkedNews.push(tempArray);
                    }
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

        function search() {
            vm.getNews(vm.PortalId, vm.settings.CategoryID, vm.topN, vm.startDate, vm.endDate, 1, vm.pageSize, vm.includeHidden, vm.searchText);
        }
    }
})();
