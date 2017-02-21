(function () {
    'use strict';

    var app = angular.module('demoApp', ['cedarjs']);

    app.config(function (commandApiProvider) {
        commandApiProvider.configure({
            routePrefix: 'test/'
        });
    });

    app.controller('DemoController', function ($scope, commandApi) {

        $scope.result = '';

        $scope.sendAcceptedCommand = function () {

            sendCommand({
                commandId: "90D552BE-9259-4081-BEE0-A972D0AFAC8C",
                commandName: "CommandThatIsAccepted",
                value: 'Data'
            });
        };

        $scope.sendRejectedCommand = function () {

            sendCommand({
                commandId: "90D552BE-9259-4081-BEE0-A972D0AFAC8C",
                commandName: "CommandThatThrowsProblemDetailsException"
            });
        };
        
        function sendCommand(command) {
            commandApi.execute(command)
               .then(function () {
                   $scope.result = 'Command Is Accepted';
               }, function (e) {
                   $scope.result = 'Command Is Not Accepted';
                   console.log(e);
               });
        }
    });
}());