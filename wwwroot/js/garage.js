$(document).ready(function () {
    $("#filterBtn").click(function () {
        $("#filter").toggle();
    });

    $("#mobFilterBtn").click(function () {
        $("#mobFilter").toggle();
    });
});

$(document).ready(function () {
    $('#makeKey').click(function () {
        if ($('#makeKey').css('color') == 'rgb(33, 37, 41)') {
            $('#makeRem').hide();
            $('#makeChk').show();

            $('#makeKey').css('color', '#dcdcdc');

            $('.makeVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#makeRem').show();
            $('#makeChk').hide();

            $('#makeKey').css('color', 'rgb(33, 37, 41)');

            $('.makeVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#yearKey').click(function () {
        if ($('#yearKey').css('color') == 'rgb(33, 37, 41)') {
            $('#yearRem').hide();
            $('#yearChk').show();

            $('#yearKey').css('color', '#dcdcdc');

            $('.yearVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#yearRem').show();
            $('#yearChk').hide();

            $('#yearKey').css('color', 'rgb(33, 37, 41)');

            $('.yearVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#vinKey').click(function () {
        if ($('#vinKey').css('color') == 'rgb(33, 37, 41)') {
            $('#vinRem').hide();
            $('#vinChk').show();

            $('#vinKey').css('color', '#dcdcdc');

            $('.vinVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#vinRem').show();
            $('#vinChk').hide();

            $('#vinKey').css('color', 'rgb(33, 37, 41)');

            $('.vinVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#cylKey').click(function () {
        if ($('#cylKey').css('color') == 'rgb(33, 37, 41)') {
            $('#cylRem').hide();
            $('#cylChk').show();

            $('#cylKey').css('color', '#dcdcdc');

            $('.cylVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#cylRem').show();
            $('#cylChk').hide();

            $('#cylKey').css('color', 'rgb(33, 37, 41)');

            $('.cylVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#statusKey').click(function () {
        if ($('#statusKey').css('color') == 'rgb(33, 37, 41)') {
            $('#statusRem').hide();
            $('#statusChk').show();

            $('#statusKey').css('color', '#dcdcdc');

            $('.statusVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#statusRem').show();
            $('#statusChk').hide();

            $('#statusKey').css('color', 'rgb(33, 37, 41)');

            $('.statusVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#mileKey').click(function () {
        if ($('#mileKey').css('color') == 'rgb(33, 37, 41)') {
            $('#mileRem').hide();
            $('#mileChk').show();

            $('#mileKey').css('color', '#dcdcdc');

            $('.mileVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#mileRem').show();
            $('#mileChk').hide();

            $('#mileKey').css('color', 'rgb(33, 37, 41)');

            $('.mileVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#dateKey').click(function () {
        if ($('#dateKey').css('color') == 'rgb(33, 37, 41)') {
            $('#dateRem').hide();
            $('#dateChk').show();

            $('#dateKey').css('color', '#dcdcdc');

            $('.dateVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#dateRem').show();
            $('#dateChk').hide();

            $('#dateKey').css('color', 'rgb(33, 37, 41)');

            $('.dateVal').each(function (idx, el) {
                $(el).css('color', 'rgb(33, 37, 41)');
            });
        }
    });

    $('#bidKey').click(function () {
        if ($('#bidKey').css('color') == 'rgb(33, 37, 41)') {
            $('#bidRem').hide();
            $('#bidChk').show();

            $('#bidKey').css('color', '#dcdcdc');

            $('.bidVal').each(function (idx, el) {
                $(el).css('color', '#dcdcdc');
            });
        }
        else {
            $('#bidRem').show();
            $('#bidChk').hide();

            $('#bidKey').css('color', 'rgb(33, 37, 41)');

            $('.bidVal').each(function (idx, el) {
                $(el).css('color', 'rgb(0, 128, 0)');
            });
        }
    });

});