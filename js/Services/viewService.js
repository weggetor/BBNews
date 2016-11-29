(function () {
    "use strict";

    angular
        .module("bbNewsApp")
        .factory("viewService", viewService);

    viewService.$inject = ["$http", "serviceRoot"];
    
    function viewService($http, serviceRoot) {

        var service = {};

        service.getNews = getNews;
        service.getSingleNews = getSingleNews;

        function getNews(portalId, categoryId, topN, startDate, endDate, pageNum, pageSize, includeHidden ,search) {
            return $http.get(serviceRoot + "news/list?portalid=" + portalId +"&categoryid=" + categoryId + "&topn=" + topN + "&startdate=" + startDate + "&endDate=" + endDate + "&pagenum=" + pageNum + "&pagesize=" + pageSize + "&includehidden=" + includeHidden + "&search=" + search );
        };

        function getSingleNews(newsId) {
            return $http.get(serviceRoot +"news/get?newsId=" + newsId)
        }

        return service;
   }
})();