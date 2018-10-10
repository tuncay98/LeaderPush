var Title = document.querySelector("#Title");
var Body = document.querySelector("#Body");
var Link = document.querySelector("#Link");
var Upload = document.querySelector("#Upload");
var Button = document.querySelector("#Button");

Button.disabled = true;

TitleText = false;
BodyText = false;
LinkText = false;
UploadFile = false;


Title.addEventListener("input", function () {

    if (this.value.length > 0) {

        TitleText = true;
        Check();

    } else {
        TitleText = false;
        Button.disabled = true;
    }

    if (this.value.length < 32) {
        $(".TitleText").text(this.value);
    }

});


Body.addEventListener("input", function () {

    if (this.value.length > 0) {
        
        BodyText = true;
        Check()
    } else {
        BodyText = false;
        Button.disabled = true;
    }

    if (this.value.length < 32) {
        $(".BodyText").text(this.value);
    }


});

Link.addEventListener("change", function () {
    this.value.toLowerCase();

    if (this.value.length > 0) {

        LinkText = true;
        Check()
    } else {
        LinkText = false;
        Button.disabled = true;
    }
    var ValueLink = this.value;
    var res = ValueLink.split(".");
    for (var i = 0; i < res.length; i++) {

        if (res[0] == "https://www" || res[0] == "www" || res[0] == "http://www" || res[0] == "http://" || res[0] == "https://") {
            this.value = "";
            for (var q = 1; q < res.length; q++) {

                this.value += res[q] + ".";

            }
            var count = this.value.length;
            this.value = this.value.substring(0, count - 1);
            return
        }
        
    }

});

$("#progress").hide();
$("#Upload").click(function () {
    $(".progress-bar").css('width', "0");
    $(".progress-bar").css('background-color', "#007bff");
    $("#progress").hide();

});

Upload.addEventListener("change", function () {
    $("#progress").show();

    $(".progress-bar").animate({ width: "+=100%" },1000).promise().done(function () {
        $(".progress-bar").css('background-color', "#44da00");
      
    });

    readURL(this);

    if (this.files.length > 0) {

        UploadFile = true;
        Check()
    } else {
        UploadFile = false;
        Button.disabled = true;
    }

});

function Check() {
    if (TitleText && BodyText && LinkText && UploadFile) {
        Button.disabled = false;
    }
}


function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('.iconImgPlace').attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

