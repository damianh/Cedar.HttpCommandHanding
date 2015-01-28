(function () {
    'use strict';

    var app = angular.module('dempApp', []);

    app.controller('DemoController', function ($scope, $http) {

        $scope.result = '';

        $scope.sendAcceptedCommand = function () {
            sendCommand('Cedar.HttpCommandHandling.Example.JavaScript.CommandThatIsAccepted', { value: 'Data' });
        };

        $scope.sendRejectedCommand = function () {
            sendCommand('Cedar.HttpCommandHandling.Example.JavaScript.CommandThatThrowsProblemDetailsException', {});
        };

        function sendCommand(commandType, commandData) {
            $http.put('/commands/90D552BE-9259-4081-BEE0-A972D0AFAC8C',
                commandData, {
                    headers: {
                        'content-type': 'application/vnd.' + commandType.toLowerCase() + '+json',
                        'Accept': 'application/problem+json'
                    }
                }).then(function () {
                    $scope.result = 'Command Is Accepted';
                }, function (e) {
                    $scope.result = e.data;
                });
        }
    });
}());