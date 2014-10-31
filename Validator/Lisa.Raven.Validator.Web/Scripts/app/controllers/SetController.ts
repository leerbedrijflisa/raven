/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    export class SetController {
        private validation: ValidationController;

        public createName: string;

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
    }
}