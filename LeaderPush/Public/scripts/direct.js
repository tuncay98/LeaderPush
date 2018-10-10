

var dom = document.domain;
if (Notification.permission != "granted") {
    Notification.requestPermission().then(function (result) {
        if (result == "granted") {
            window.location.href = 'https://www.leaderpush.com/uploadingservice/index?url=' + dom;


        }
    });
}



