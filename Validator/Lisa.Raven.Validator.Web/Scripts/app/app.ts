/// <reference path="../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    var app = angular.module('raven', []);

    app.controller('ValidationController', ValidationController);
    app.controller('AddCheckController', AddCheckController);
}