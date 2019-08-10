$(document).ready(function () {
    $("#commentBtn").click(function () {
        $("#comment").toggle();
        $("html, body").animate({ scrollTop: $(document).height() }, "slow");
    });

    $("#mobCommentBtn").click(function () {
        $("#mobComment").toggle();
        $("html, body").animate({ scrollTop: $(document).height() }, "slow");
    });
});
