mainApp.factory('companies', function ($resource) {
    var webAPI = 'rest/Company/:id';
    return {
        get:function(){ return $resource(webAPI).query() },
        save: function (company) { return $resource(webAPI).save(company) },
        remove: function (id) { return $resource(webAPI, {id : id}).delete() }
        }
    });