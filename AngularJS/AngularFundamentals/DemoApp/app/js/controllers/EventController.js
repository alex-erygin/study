/**
 * Created by alex on 07.01.16.
 */
'use strict';

eventsApp.controller('EventController',
    function EventController($scope){
        $scope.event = {
            name : "Angular Boot Camp",
            date : "1/1/2013",
            time : "10:30",
            location : {
                address : "Google Headquarters",
                city : "Mountain View",
                province : "CA"
            },
            imageUrl : "img/angularjs-logo.png"
        }
    }
);
