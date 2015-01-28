$(function () {
    'use strict';

    $('#btnAccepted').click(function () {
        sendCommand('Cedar.HttpCommandHandling.Example.JavaScript.CommandThatIsAccepted', { value: 'Data' });
    });

    $('#btnException').click(function () {
        sendCommand('Cedar.HttpCommandHandling.Example.JavaScript.CommandThatThrowsProblemDetailsException', {});
    });

    function sendCommand(commandType, commandData) {
        $.ajax({
            url: '/commands/90D552BE-9259-4081-BEE0-A972D0AFAC8C',
            type: 'PUT',
            contentType: 'application/vnd.' + commandType.toLowerCase() + '+json',
            accepts: 'application/problem+json',
            data: JSON.stringify(commandData)
        }).then(function (e) {
            $('#result').text('Command Is Accepted');
        }, function (e) {
            console.log($('#result').text(e.responseText));
        });
    }
});
