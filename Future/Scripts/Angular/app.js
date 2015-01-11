var mainApp = angular.module('restaurantApp', ["ngRoute", "ngResource", "ngAnimate","ui.bootstrap"]).
    config(function ($routeProvider, $locationProvider) {
    $routeProvider.
        when('/', {
            controller: 'profilePageCtrl',
            templateUrl: '/Templates/ProfilePage.html'
        }).
        when('/casual', {
            controller: 'profilePageCtrl',
            templateUrl: '/Templates/ProfilePage1.html'
        }).
        when('/topCompanies', {
            controller: 'topCompaniesCtrl',
            templateUrl: '/Templates/topCompanies.html'
        }).
        when('/contactUs', {
            controller: 'contactUsCtrl',
            templateUrl: '/Templates/contactUs.html'
        }).
        when('/angualrExamples', {
            controller: 'examplesCtrl',
            templateUrl: '/Templates/angualrExamples.html'
        }).
        when('/jasmine', {
            controller: 'jasmineCtrl',
            templateUrl: '/Jasmine/SpecRunner.html'
        });
    //$locationProvider.html5Mode(true);
});









