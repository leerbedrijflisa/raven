/// <reference path="../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    var app = angular.module('raven', []);

    app.controller('ValidationController', ValidationController);
    app.controller('CheckController', CheckController);
    app.controller('SetController', SetController);
}