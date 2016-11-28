(function () {
    "use strict";

    angular.module("bbNewsApp", ["ngRoute", "ngSanitize", "ngProgress", "ngToast", "angularUtils.directives.dirPagination"])
    .config(['ngToastProvider', function (ngToastProvider) {
        ngToastProvider.configure({
            animation: 'fade',
            horizontalPosition: 'left'
        });
    }])
    .filter("trust", ['$sce', function ($sce) {
        return function (htmlCode) {
            return $sce.trustAsHtml(htmlCode);
        }
    }]);
})();