// states
var FRONT = "FRONT";
var BACK = "BACK";

var state = FRONT;

$(function() {
    console.log("Udenad!");
    
    $(document).keypress(function (event) {
        if (isEnter(event)) {
            if ($("#notes").is(":focus")) {
                $("input[name=score]:checked").focus();
            }
            else if(state === FRONT) {
                showBackSide();
            }
            else {
                saveScore();
            }
        }
    });
    
    $("#notes").focusout(function () {
        var word = $("#word").text();
        var notes = $("#notes").text();
        $.post("notes", { word: word, notes: notes }, function (data) {
            console.log(data);
        }, "json");
    });
});

function showFrontSide() {
    window.location.reload(true);
}

function showBackSide() {
    $("#status-front").attr("hidden", "hidden");
    $("#status-back").removeAttr("hidden");
    $("#backside").removeAttr("hidden");
    $("#score-incorrect").focus();
    state = BACK;
}

function saveScore() {
    var word = $("#word").text();
    var score = $("input[name=score]:checked").val();
    
    $.post("score", { word: word, score: score }, function (data) {
        console.log(data);
    }, "json").done(showFrontSide());
}

function isEnter(event) {
    return event.charCode === 13;
}