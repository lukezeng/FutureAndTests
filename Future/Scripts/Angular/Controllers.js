mainApp.controller('navigationCtrl', function ($scope, $location) {
    $scope.isActive = function (route) {
        return route === $location.path();
    };
});

mainApp.controller('profilePageCtrl', function ($scope, $http) {

});

mainApp.controller('contactUsCtrl', function ($scope, $http) {

});

mainApp.controller('jasmineCtrl', function ($scope, $http) {

});



mainApp.controller('topCompaniesCtrl', ['$scope', '$http', 'companies', function ($scope, $http, companies) {

    $scope.company = {};
    $scope.companies = companies.get();
    $scope.companiesPerPage = 900;
    $scope.currentPage = 0;

    $scope.Save = function (company) {
        companies.save(company).$promise.then(
            function () {
                $scope.companies.push(company);
            },
            function () { alert("NONO");});
        $scope.company = {};
    }

    $scope.DeleteCompany = function (id) {
        companies.remove(id).$promise.then(
            function () {
                $scope.companies = companies.get();
            });
    }



    $scope.prevPage = function () {
        if ($scope.currentPage > 0) {
            $scope.currentPage--;
        }
    };

    $scope.prevPageDisabled = function () {
        return $scope.currentPage === 0 ? "disabled" : "";
    };

    $scope.pageCount = function () {
        return Math.ceil($scope.companies.length / $scope.companiesPerPage) - 1;
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.pageCount()) {
            $scope.currentPage++;
        }
    };

    $scope.nextPageDisabled = function () {
        return $scope.currentPage === $scope.pageCount() ? "disabled" : "";
    };

    $scope.setPage = function (n) {
        $scope.currentPage = n;
    };

    $scope.range = function () {
        var rangeSize = 4;
        var ret = [];
        var start;

        start = $scope.currentPage;
        if (start > $scope.pageCount() - rangeSize) {
            start = $scope.pageCount() - rangeSize + 1;
        }

        for (var i = start; i < start + rangeSize; i++) {
            if (i - 1 < $scope.pageCount() && i >= 0) { ret.push(i); }
        }
        return ret;
    };


}]);


mainApp.controller('examplesCtrl', function ($scope, $http) {

    $scope.alerts = [
      { type: 'danger', msg: 'Oh snap! Change a few things up and try submitting again.' },
      { type: 'success', msg: 'Well done! You successfully read this important alert message.' }
    ];

    $scope.addAlert = function () {
        $scope.alerts.push({ msg: 'Another alert!' });
    };

    $scope.closeAlert = function (index) {
        $scope.alerts.splice(index, 1);
    };

    $scope.sortOrder = 'lastName';
    $scope.textToShow = "here is the text to show";
    $scope.nameList = ['Luke', 'Kaixin', 'Jianmin'];
    $scope.addName = function () {
        $scope.nameList.push($scope.nameToAdd);
        $scope.nameToAdd = '';
    }
    $scope.UserList = [
        { firstName: 'Luke', lastName: 'Zeng' },
        { firstName: 'Kaixin', lastName: 'Li' },
        { firstName: 'Jianmin', lastName: 'Chen' }
    ];
    $scope.addUser = function () {
        $scope.UserList.push({ firstName: $scope.userFirstName, lastName: $scope.userLastName });
        $scope.userFirstName = '';
        $scope.userLastName = '';
    }


    $scope.items = [];
    $scope.addItems = function () {
        for (i = 10; i--;) {
            $scope.items.push({ title: "item" + i, });
        }
    }
    $scope.clearItems = function () {
        $scope.items = [];
    }
    $scope.removeItem = function (index) {
        $scope.items.splice(index, 1);

    }

});

mainApp.controller('companyCtrl', function ($scope, $routeParams) {

    $scope.companyName = $routeParams.companyName;


});


mainApp.controller('unknownPageCtrl', function ($scope, $routeParams) {

    $scope.unknownString = $routeParams.unknown;


});


function AlertDemoCtrl($scope) {


}