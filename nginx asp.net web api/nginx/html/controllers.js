'use strict';
/* Controllers */
var eonKioskControllers =
angular.module('eonKioskControllers', ['ngSanitize'] );

eonKioskControllers.controller('StartController',
['$scope', '$location', '$http', 'config','resetCookies',
function StartController($scope, $location, $http, config, resetCookies) 
{
   resetCookies();
   $scope.clinicName = config.clinicName;
   $scope.clickStart = function () 
   {
      console.log('qwerty');
      $http.get(config.apiUrl+'/Patients/Test')
        .success(function (data) {
            console.log(data);
          });
   };   
}]);