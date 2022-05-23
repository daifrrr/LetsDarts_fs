
$(document).ready(() => {
    setAllCSS()
})

$(window).resize(() => {
    setAllCSS()
})

const setAllCSS = () => {
    setPlayerGroupHeight()
}

const setPlayerGroupHeight = () => {
    const outerPlayerInputGroup = $("#outer-player-input-group")
    const playerInputGroup = $("#player-input-group")
    const playerInputGroupTop = playerInputGroup.position().top
    const buttonBullOut = $("#btn-game-bull-out")
    const buttonBullOutMargin = parseInt(buttonBullOut.css('margin-top')) + parseInt(buttonBullOut.css('margin-bottom'))
    const buttonBullOutHeight = buttonBullOut.height()
    const avaiableHeight = $(window).height() - ( $("header").height() + $("footer").height() )
    const newHeight = avaiableHeight - (buttonBullOutMargin + buttonBullOutHeight + playerInputGroupTop)

    $(outerPlayerInputGroup).height( newHeight )

    /*
    if (playerInputGroup.height() < playerInputGroup.prop("scrollHeight"))
        console.log("Overflow" + playerInputGroup.height() + " – " + playerInputGroup.prop("scrollHeight"))
    else
        console.log("No Overflow: " + playerInputGroup.height() + " – " + playerInputGroup.prop("scrollHeight"))
    */

}

/*
const onReady = () => {

    const playerGroup = document.getElementById("player-input-group")
    const playerGroupHeight = getOffset(playerGroup).top

    console.log(playerGroupHeight)
}

function getOffset(el) {
    const rect = el.getBoundingClientRect();
    return {
        left: rect.left + window.scrollX,
        top: rect.top + window.scrollY
    };
}

(function() {
    onReady();
})();
 */


//const playerGroup = document.getElementById("player-input-group")
//const playerGroupHeight = playerGroup.clientTop

//console.log("Player Group Client Height: " + playerGroupHeight)



