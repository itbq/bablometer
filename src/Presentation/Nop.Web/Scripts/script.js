
function switchDisplay(id) {
    if ($(id).css("display") == "none") { $(id).css("display", "block"); } else { $(id).css("display", "none"); }
}

$(function () {
    var _tabs = $(".tabCatalog");
    if (_tabs != null && _tabs.length > 0) _tabs.tabs({
        activate: function (event, ui) {
            $(ui.newPanel.selector + ' .group-block').each(function () {
                var index = $(this).attr('id').indexOf('group');
                var number = parseInt($(this).attr('id').substring(index + 5));
                var width = $(this).width();
                $(this).css("left", (-width / 2 + number * 14 + 4) + 'px');
            });
          }
        });
});

// slider for index
$(document).ready(function () {

    _goods = $(".goods");
    if (_goods != null && _goods.length > 0) {
        $(".goods").diyslider({
            width: "524px", // width of the slider
            height: "208px", // height of the slider
            display: 4, // number of slides you want it to display at once
            loop: true // disable looping on slides
        }); // this is all you need!

        // use buttons to change slide
        $("#go-left").bind("click", function () {
            $(".goods").diyslider("move", "back");
            return false;
        });
        $("#go-right").bind("click", function () {
            $(".goods").diyslider("move", "forth");
            return false;
        });
    }
    _goodsView = $(".goodsView");
    if (_goodsView != null && _goodsView.length > 0) {
        $(".goodsView").diyslider({
            width: "524px", // width of the slider
            height: "208px", // height of the slider
            display: 4, // number of slides you want it to display at once
            loop: true // disable looping on slides
        }); // this is all you need!

        // use buttons to change slide
        $("#go-left_view").bind("click", function () {
            $(".goodsView").diyslider("move", "back");
            return false;
        });
        $("#go-right_view").bind("click", function () {
            $(".goodsView").diyslider("move", "forth");
            return false;
        });
    }

})
