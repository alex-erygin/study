var kioskBusinessServices =angular.module('kioskBusinessServices', ['ngCookies']);


kioskBusinessServices.factory('saveChoiceToAnswers',
['$cookieStore', function($cookieStore) {
return function(choiceAnswer) {
    
    var choiceAnswers = $cookieStore.get('ChoiceAnswers');
    if (typeof choiceAnswers === 'undefined') {
        choiceAnswers= []; 
        
    }    
    choiceAnswers.push(choiceAnswer); 
    $cookieStore.put('ChoiceAnswers',choiceAnswers);
};
}]);

kioskBusinessServices.factory('getAllChoiceAnswers',
['$cookieStore', function( $cookieStore) {
return function() {
    
    var choiceAnswers = $cookieStore.get('ChoiceAnswers');
    if (typeof choiceAnswers === 'undefined') {
        choiceAnswers= []; 
    }    
    return choiceAnswers;
};
}]);


kioskBusinessServices.factory('savePatient',
['$cookieStore', function($cookieStore) {
return function(patient) {
    $cookieStore.put('Patient',patient);
};
}]);

kioskBusinessServices.factory('getPatient',
['$cookieStore', function( $cookieStore) {
return function() {   
    var patient = $cookieStore.get('Patient');
    return patient;
};
}]);

kioskBusinessServices.factory('savePatientSubjective',
['$cookieStore', function($cookieStore) {
return function(patientSubjective) {
    $cookieStore.put('PatientSubjective',patientSubjective);
};
}]);

kioskBusinessServices.factory('getPatientSubjective',
['$cookieStore', function( $cookieStore) {
return function() {   
    var patientSubjectiive = $cookieStore.get('PatientSubjective');
    return patientSubjectiive;
};
}]);


kioskBusinessServices.factory('savePatientReport',
['$cookieStore', function($cookieStore) {
return function(patientReport) {
    $cookieStore.put('PatientReport',patientReport);
};
}]);

kioskBusinessServices.factory('getPatientReport',
['$cookieStore', function( $cookieStore) {
return function() {   
    var patientReport = $cookieStore.get('PatientReport');
    return patientReport;
};
}]);

kioskBusinessServices.factory('resetCookies',
['$cookieStore', function($cookieStore) {
return function() {
$cookieStore.remove('ChoiceAnswers');
$cookieStore.remove('Patient');
$cookieStore.remove('Patient');
$cookieStore.remove('PatientSubjective');
return true;
};
}]);