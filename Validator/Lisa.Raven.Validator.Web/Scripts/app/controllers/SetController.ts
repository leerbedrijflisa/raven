/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    export class SetController {
        private validation: ValidationController;

        public createName: string;
        public addCode: string;

        public static $inject = [
            '$scope',
            '$attrs',
            '$parse',
            '$http'
        ];

        constructor($scope, $attrs, $parse: ng.IParseService, private $http) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('SetController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.createName = '';
        }

        public createKeypress($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;
            
            $event.preventDefault();
            this.create();
        }

        public create() {
            // Take the data out of the field
            var name = this.createName;
            this.createName = '';

            // If the field was empty, nothing to do
            if (name === '')
                return;

            // Send out set to the server
            var newSet = {
                Name: name,
                Checks: this.validation.submission.Checks,
                Locked: false
            };
            this.$http.post("http://localhost:14512/api/v1/sets/create", newSet)
                .success((data, status) => {
                    // Add it to the list
                    this.validation.submission.Sets.unshift(data);

                    // Empty the check list
                    this.validation.submission.Checks = [];
                })
                .error((data, status) => {
                    alert('Could not create set: ' + data);
                });
        }

        public addKeypress($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        }

        public add() {
            // Take the data out of the field
            var code = this.addCode;
            this.addCode = '';

            // If the field was empty, nothing to do
            if (code === '')
                return;

            // Make sure it's not already in the list
            var index = this.validation.submission.Sets
                .map(c => c.Code)
                .indexOf(code);
            if (index !== -1) return;

            // Get the set from the server
            this.$http.get("http://localhost:14512/api/v1/sets/get/" + code)
                .success((data, status) => {
                    // Add it to the list
                    this.validation.submission.Sets.unshift(data);
                })
                .error((data, status) => {
                    alert('Could not get set: ' + data);
                });
        }

        public remove(setObj) {
            var index = this.validation.submission.Sets.indexOf(setObj);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.Sets.splice(index, 1);
        }
    }
}