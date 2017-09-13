// states
const FRONT = "FRONT";
const BACK = "BACK";

var state = FRONT;

$(function() {
    console.log("Udenad!");
    
    $(document).keypress(function (event) {
        if (isEnter(event)) {
            if(state === FRONT) {
                showBackSide();
            }
            else {
                saveScore();
            }
        }
    });
});

function showFrontSide() {
    window.location.reload(true);
}

function showBackSide() {
    $("#status-front").attr("hidden", "hidden");
    $("#status-back").removeAttr("hidden");
    $("#backside").removeAttr("hidden");
    $("#score-3").focus();
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