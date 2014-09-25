/// <reference path="../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    var app = angular.module('raven', []);

    app.controller('ValidationController', ValidationController);
    app.controller('SetController', SetController);
    app.controller('CheckController', CheckController);
}