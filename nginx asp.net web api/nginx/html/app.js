'use strict';
/* App Module */
var eonKioskApp = angular.module('eonKioskApp', [
    'ngRoute',
    'eonKioskControllers',
    'kioskBusinessServices'
]);

eonKioskApp.config(['$httpProvider', function ($httpProvider) {

    $httpProvider.defaults.useXDomain = true;
    delete $httpProvider.defaults.headers.common['X-Requested-With'];
}]);

eonKioskApp.constant('config', {
    appName: 'Patient Sign-in',
    appVersion: 1.0,
    apiUrl: 'http://localhost:26001/api',
    clinicName: 'Вызвать метод Web API',
    patientPortal: 'https://eonportal.com/index.php?c=Y2xpZW50X2lkJTNEMQ==',
    pleaseSeeReception: 'Please see reception',
    thankYou: "[Client chosen message]"
});


eonKioskApp.config(['$routeProvider', '$locationProvider', '$httpProvider',
function ($routeProvider, $locationProvider, $httpProvider) {
    $routeProvider.
    when('/', {
        templateUrl: 'start1.html',
        controller: 'StartController'
    });

    $httpProvider.defaults.useXDomain = true;
    $httpProvider.defaults.withCredentials = true;
    delete $httpProvider.defaults.headers.common["X-Requested-With"];
    
    $locationProvider.html5Mode(false).hashPrefix('!');
}]);