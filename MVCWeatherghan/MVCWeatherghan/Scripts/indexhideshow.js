function hideAll() {
    $("#faqpage").hide();
    $("#colorscale").hide();
    $("#aboutpage").hide();
};

$(document).ready(function () {
    $("#faqtab").click(function(e) {
        hideAll();
        $('#faqpage').show("slide", { direction: "up" }, 400);
    });
    $("#faqtab").hover(function () {
        $(this).stop().css({
            "cursor": "pointer"
        })
    });
});
   
$(document).ready(function () {
    $("#colortab").click(function (e) {
        hideAll();
        $('#colorscale').show("slide", { direction: "up" }, 400);
    });
    $("#colortab").hover(function () {
        $(this).stop().css({
            "cursor": "pointer"
        })
    });
});

$(document).ready(function () {
    $("#abouttab").click(function (e) {
        hideAll();
        $('#aboutpage').show("slide", { direction: "up" }, 400);
    });
    $("#abouttab").hover(function () {
        $(this).stop().css({
            "cursor": "pointer"
        })
    });
});
    