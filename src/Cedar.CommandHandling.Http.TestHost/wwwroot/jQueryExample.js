$(function () {
    'use strict';

    var commandApi = new cedarJs.CommandApi({
        routePrefix: 'test/'
    });

    $('#btnAccepted').click(function() {
        sendCommand({
            commandId: '90D552BE-9259-4081-BEE0-A972D0AFAC8C',
            commandName: 'CommandThatIsAccepted',
            value: 'Data'
        });
    });

    $('#btnException').click(function () {
        sendCommand({
            commandId: '90D552BE-9259-4081-BEE0-A972D0AFAC8C',
            commandName: 'CommandThatThrowsProblemDetailsException'
        });
    });

    function sendCommand(command) {

        commandApi.execute(command).then(function (e) {
            $('#result').text('Command Is Accepted');
        }, function (e) {
            $('#result').text('Command Is Not Accepted');
            console.log(e.responseText);
        });
    }
});
