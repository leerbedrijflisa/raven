/// <reference path="../typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

    var app = angular.module('raven', []);

    app.controller('ValidationController', Raven.ValidationController);
    app.controller('CheckController', Raven.CheckController);
    app.controller('SetController', Raven.SetController);
})(Raven || (Raven = {}));
//# sourceMappingURL=app.js.map
