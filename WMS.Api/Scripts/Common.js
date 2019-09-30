function login() {
    var username = $('#username').val();
    var password = $('#password').val();
    if ((username === '' || username === null || username === undefined) &&
        (password === '' || password === null || password === undefined)) {
        var bOBJuser = $('#user');
        if (bOBJuser.is(":visible")) {
            bOBJuser.hide();
        }
        else {
            bOBJuser.show();
        }

        var bOBJpass = $('#pass');
        if (bOBJpass.is(":visible")) {
            bOBJpass.hide();
        }
        else {
            bOBJpass.show();
        }
        $('#username').focus();
    }
    else if (username === '' || username === null || username === undefined) {
        var bOBJuser = $('#user');
        if (bOBJuser.is(":visible")) {
            bOBJuser.hide();
        }
        else {
            bOBJuser.show();
        }
        $('#username').focus();
    }
    else if (password === '' || password === null || password === undefined) {
        var bOBJpass = $('#pass');
        if (bOBJpass.is(":visible")) {
            bOBJpass.hide();
        }
        else {
            bOBJpass.show();
        }
        $('#password').focus();
        $("#input:text").focus();
    }
    else {
        $.ajax({
            type: 'GET',
            url: '/Account/GetUser?',
            data: { UserName: username, Password: password },
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            cache: true,
            success: function (data) {
                if (data !== null && data !== undefined && data === true) {
                    window.location.replace('/Help');
                }
                else {
                    window.location.replace('/Home');
                }
            },
            error: function () {

            }
        });
    }
}

function RemoveUser(user) {
    var bOBJuser = $('#user');
    if (user.value !== undefined && user.value !== null && user.value !== '') {
        bOBJuser.hide();
    }
    else {
        bOBJuser.show();
    }
}

function RemovePass(pass) {
    var bOBJpass = $('#pass');
    if (pass.value !== undefined && pass.value !== null && pass.value !== '') {
        bOBJpass.hide();
    }
    else {
        bOBJpass.show();
    }
}