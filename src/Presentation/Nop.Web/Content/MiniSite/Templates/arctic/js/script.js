function switchDisplay(id) {
    if ($(id).css("display") == "none") {$(id).css("display", "block");} else {$(id).css("display", "none");}
}

$(function() {
    $( ".tabCatalog" ).tabs();
});

// slider for index
$(document).ready(function() {
    $(".goods").diyslider({
        width: "524px", // width of the slider
        height: "200px", // height of the slider
        display: 4, // number of slides you want it to display at once
        loop: true // disable looping on slides
    }); // this is all you need!

    // use buttons to change slide
    $("#go-left").bind("click", function(){
        $(".goods").diyslider("move", "back");
        return false;
    });
    $("#go-right").bind("click", function(){
        $(".goods").diyslider("move", "forth");
        return false;
    });

    $(".goodsView").diyslider({
        width: "524px", // width of the slider
        height: "200px", // height of the slider
        display: 4, // number of slides you want it to display at once
        loop: true // disable looping on slides
    }); // this is all you need!

    // use buttons to change slide
    $("#go-left_view").bind("click", function(){
        $(".goodsView").diyslider("move", "back");
        return false;
    });
    $("#go-right_view").bind("click", function(){
        $(".goodsView").diyslider("move", "forth");
        return false;
    });

})
