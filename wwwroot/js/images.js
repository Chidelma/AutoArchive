$(document).ready(function () {
    // Get the modal
    var modal = document.getElementById('myModal');
    // Get the image and insert it inside the modal - use its "alt" text as a caption
    var img1 = document.getElementsByClassName("myImg");

    var img2 = document.getElementsByClassName("scndImg");

    var img3 = document.getElementsByClassName("thrdImg");

    var img4 = document.getElementsByClassName("frthImg");

    var img5 = document.getElementsByClassName("fithImg");

    var modalImg0 = document.getElementById("img00");

    var modalImg1 = document.getElementById("img01");

    var modalImg2 = document.getElementById("img02");

    var modalImg3 = document.getElementById("img03");

    var modalImg4 = document.getElementById("img04");

    var modalImg5 = document.getElementById("img05");

    $('.myImg').each(function (idx, el) {
        $(el).click(function () {

            modal.style.display = "block";

            $("body").addClass("modal-open");

            modalImg0.src = img1[idx].src;

            modalImg1.src = img1[idx].src;

            modalImg2.src = img2[idx].src;

            modalImg3.src = img3[idx].src;

            modalImg4.src = img4[idx].src;

            modalImg5.src = img5[idx].src;

        });
    });

    $('.modal-content').each(function (index, elem) {
        $(elem).click(function () {
            modalImg0.src = this.src;
        });
    });

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {

        $(modalImg0).attr('src', '');

        modal.style.display = "none";

        $("body").removeClass("modal-open");
    }
});