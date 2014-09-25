/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    export class SetController {
        private validation;
        private submission;

        public static $inject = [
            '$scope',
            '$attrs',
            '$parse'
        ];

        constructor($scope, $attrs, $parse: ng.IParseService) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('CheckController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.submission = { 'Name': '', 'Code': '' };
        }

        keypress($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        }

        add() {
            // Take the data out of the field
            var setSubmission = this.submission;
            this.submission = { 'Name': '', 'Code': '' };

            // If the field was empty, nothing to do
            if (setSubmission.Name === '')
                return;

            var index = this.validation.submission.CheckSets.indexOf(setSubmission);

            // If not already in the list, add
            if (index === -1)
                this.validation.submission.CheckSets.push(setSubmission);
        }

        remove(setEntry) {
            var index = this.validation.submission.CheckSets.indexOf(setEntry);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.CheckSets.splice(index, 1);
        }
    }
}