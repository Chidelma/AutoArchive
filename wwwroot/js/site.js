// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

if (window.location.pathname == '/') {
    document.getElementById("bodyLayout").style.backgroundColor = "#303030";
}

if (window.location.pathname == '/Home/Privacy') {
    document.getElementById("mobAcctBtn").style.color = "white";
}

$('.dropdown-toggle').dropdown();

$(document).ready(function () {
    if ($(window).width() <= 1080) {
        if (!(window.location.pathname == '/Home/Privacy')) {
            $("#renderBody").removeClass("container");
        }

        $('.pb-3').css('margin-top', '100px');

        $('a').click(function () {
            $('.pb-3').hide();
            $('.demo').show();
        });

        $('.middle').hide();

        $('#cookieConsent').css('display', 'none');
    }

    // iPhone X
    if ($(window).height() == 768 && $(window).width() == 375) {
        $('#divHeader').css('margin-top', '35px');
        $('#mobFooter').css('height', '80px');
        $('.demo').css('height', '768px');
    }
});

$(window).on("beforeunload", function () {
    $('.yt-loader').show();
    if ($(window).width() <= 1080) {
        $('.pb-3').hide();
        $('.demo').show();
    }
});

$(document).ready(function () {
    if ($(window).width() > 1080) {
        $(".detailsRow").hover(function () {
            $(this).css("opacity", "0.3");
        }, function () {
            $(this).css("opacity", "1");
        });

        $(".middle").hover(function () {
            $(this).css("opacity", "1");
        }, function () {
            $(this).css("opacity", "0");
        });
    }
});

$(window).on('load', function () {
    $('.yt-loader').css("width", "100%");
    $('.yt-loader').fadeOut();
    if ($(window).width() <= 1080) {
        $('.demo').hide();
        $('.pb-3').show();
    }
    if (window.location.pathname.includes("signin-oidc?code=")) {
        window.location.href = "https://scarfbeta.azurewebsites.net/Home/Results";
    }
});

$('#mobSubBtn').click(function () {
    $('.collapse').hide();
    $('.pb-3').hide();
    $('.demo').show();
});

function iconColor(path) {
    if (path == "/Home/Results") {
        $('#mobHomeBtn').css("color", "white");
        $('#mobGarageBtn').css("color", "#7c7c7c");
        $('#analytikTable').hide();
    }

    if (window.location.pathname.includes("Garage")) {
        $('#mobGarageBtn').css("color", "white");
        $('#mobHomeBtn').css("color", "#7c7c7c");
        $('#analytikTable').hide();
    }

    if (window.location.pathname.includes("Analytic")) {
        $('#analytikTable').show();
        $('#mobAnaltikBtn').css("color", "white");
        $('#mobGarageBtn').css("color", "#7c7c7c");
        $('#mobHomeBtn').css("color", "#7c7c7c");
    }
}

$(document).ready(function () {
    $('#garageTable').click(function () {
        window.location.href = '/Garage/Index';
        $('#mobGarageBtn').css("color", "white");
        $('#mobHomeBtn').css("color", "#7c7c7c");

        $('.pb-3').hide();
        $('.demo').show();
    });

    $('#homeTable').click(function () {
        window.location.href = '/Home/Results';
        $('#mobHomeBtn').css("color", "white");
        $('#mobGarageBtn').css("color", "#7c7c7c");

        $('.pb-3').hide();
        $('.demo').show();
    });

    $('#backTable').click(function () {
        $('#mobHomeBtn').css("color", "#7c7c7c");
        $('#mobGarageBtn').css("color", "#7c7c7c");
        $('#mobBackBtn').css("color", "white");

        $('.pb-3').hide();
        $('.demo').show();
    });

    iconColor(window.location.pathname);
});

function toggleNav() {

    var element = document.getElementById('mySidepanel');
    var style = window.getComputedStyle(element);
    var width = style.getPropertyValue('width');

    if (width == '0px') {
        document.getElementById("mySidepanel").style.height = "100%";
        document.getElementById("mySidepanel").style.width = "100%";
        document.getElementById("mobAcctBtn").style.color = "white";
        document.getElementById("mobHomeBtn").style.color = "#7c7c7c";
        document.getElementById("mobGarageBtn").style.color = "#7c7c7c";
        document.getElementById("mobAnaltikBtn").style.color = "#7c7c7c";
    }
    else {
        document.getElementById("mySidepanel").style.height = "0";
        document.getElementById("mySidepanel").style.width = "0";
        document.getElementById("mobAcctBtn").style.color = "#7c7c7c";
        $('#mobAcctInfo').hide();
        $('#mobContactForm').hide();
        $('#mobUploadForm').hide();

        iconColor(window.location.pathname);
    }
}

$("#btnGroupDrop1").on("click", function (e) {
    e.preventDefault();
});


(function checkIfMsgSent() {
    if (typeof localStorage !== 'undefined') {
        var isMsgSent = localStorage.getItem("messageSent");

        if (isMsgSent == "true") {
            $('#contactForm').show();
            $('#succMsg').show();
            toggleNav();
            $('#mobContactForm').toggle();
            $('#mobSuccMsg').show();
        }

        localStorage.removeItem('messageSent');
    } else {
        // localStorage not defined
        alert('Your browser in incompatible for some features in this website');
    }
})();

$('#cntSend').click(function (e) {

    if ($('#formSel').val() !== null && $('formSubj').val() !== "" && $('#formDesc').val() !== "") {
        if (typeof localStorage !== 'undefined') {

            localStorage.messageSent = "true";

        } else {
            // localStorage not defined
            alert('Your browser in incompatible for some features in this website');
        }
    }
    else {

        e.preventDefault();

        $('#failMsg').show();
    }
});

$('#mobSendBtn').click(function (e) {

    if ($('#mobFormSel').val() !== null && $('mobFormSubj').val() !== "" && $('#mobFormDesc').val() !== "") {
        if (typeof localStorage !== 'undefined') {

            localStorage.messageSent = "true";

        } else {
            // localStorage not defined
            alert('Your browser in incompatible for some features in this website');
        }
    }
    else {
        e.preventDefault();
        $('#mobFailMsg').show();
    }
});

(function checkIfFileUploaded() {
    if (typeof localStorage !== 'undefined') {
        var isFileUploaded = localStorage.getItem("fileUploaded"); // JSON.parse because we want a boolean

        if (isFileUploaded == "true") {
            $('#UploadForm').show();
            $('#succUpload').show();
            toggleNav();
            $('#mobUploadForm').toggle();
            $('#mobSuccUpload').show();
        }

        localStorage.removeItem('fileUploaded');
    } else {
        // localStorage not defined
        alert('Your browser in incompatible for some features in this website');
    }
})();

$('#submitUpload').click(function (e) {
    if ($('#formFileSel').val() !== null && document.getElementById("Files").files.length > 0) {

        //console.log("All Entries Valid");

        if (typeof localStorage !== 'undefined') {

            localStorage.fileUploaded = "true";

        } else {
            // localStorage not defined
            alert('Your browser in incompatible for some features in this website');
        }
    }
    else {
        //console.log("All Entries Invalid");
        e.preventDefault();

        $('#failUpload').show();
    }
});

$('#mobSubmitUpload').click(function (e) {
    if ($('#mobFormFileSel').val() !== null && document.getElementById("mobFiles").files.length > 0) {

        //console.log("All Entries Valid");
        if (typeof localStorage !== 'undefined') {

            localStorage.fileUploaded = "true";

        } else {
            // localStorage not defined
            alert('Your browser in incompatible for some features in this website');
        }
    }
    else {
        //console.log("All Entries Invalid");
        e.preventDefault();

        $('#mobFailUpload').show();
    }
});

// Modal Button For Account
$(document).ready(function () {
    $('#profBtn').click(function () {
        $('.dropdown-toggle').dropdown();
        $('#acctInfo').show();
    });

    $('#acctCnt').click(function () {
        $('#acctInfo').hide();
        $('#contactForm').show();
    });

    $('#acctClose').click(function (e) {
        e.preventDefault();
        $('#acctInfo').hide();
    });

    $('#contactBtn').click(function () {
        $('.dropdown-toggle').dropdown();
        $('#contactForm').show();
    });

    $('#cntClose').click(function (e) {
        e.preventDefault();
        $('#contactForm').hide();
        $('#failMsg').hide();
        $('#succMsg').hide();
    });

    $('#uploadBtn').click(function () {
        $('.dropdown-toggle').dropdown();
        $('#UploadForm').show();
    });

    $('#uploadClose').click(function (e) {
        e.preventDefault();
        $('#UploadForm').hide();
        $('#failUpload').hide();
        $('#succUpload').hide();
    });

    $('#logoutBtn').click(function () {
        window.location.href = '/Account/SignOut';
        return false;
    });

    $('#mobProfileBtn').click(function () {
        $('#mobContactForm').hide();
        $('#mobUploadForm').hide();
        $('#mobAcctInfo').toggle();
    });

    $('#mobContactBtn').click(function () {
        $('#mobAcctInfo').hide();
        $('#mobUploadForm').hide();
        $('#mobFailMsg').hide();
        $('#mobSuccMsg').hide();
        $('#mobContactForm').toggle();
    });

    $('#mobUploadBtn').click(function () {
        $('#mobAcctInfo').hide();
        $('#mobContactForm').hide();
        $('#mobSuccUpload').hide();
        $('#mobFailUpload').hide();
        $('#mobUploadForm').toggle();
    });

    $('#mobLogoutBtn').click(function () {
        window.location.href = '/Account/SignOut';
        return false;
    });
});