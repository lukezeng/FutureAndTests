mainApp.directive('greet', function () {
    return {
        template: '<h4>Greeting from {{from}} to {{to}}</h4>',
        controller: function ($scope, $element, $attrs) {
            $scope.from = $attrs.from;
            $scope.to = $attrs.greet;
        }
    }
});