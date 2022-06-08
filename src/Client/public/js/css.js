
$(document).ready(() => {
    setAllCSS()
})

$(window).resize(() => {
    setAllCSS()
})

const setAllCSS = () => {
    setPlayerGroupHeight($("#outer-player-input-group-create"), $("#player-input-group-create"), $("#btn-game-bull-out"))
    //setPlayerGroupHeight($("#outer-player-input-group-sort"), $("#player-input-group-sort"), $("#btn-game-start"))
}

const setPlayerGroupHeight = (oPIG, pIG, btn) => {
    console.log("bin drin")
    const outerPlayerInputGroup = oPIG
    const playerInputGroupTop = pIG.position().top
    const buttonBullOut = btn
    const buttonBullOutMargin = parseInt(buttonBullOut.css('margin-top')) + parseInt(buttonBullOut.css('margin-bottom'))
    const buttonBullOutHeight = buttonBullOut.height()
    const avaiableHeight = $(window).height() - ( $("header").height() + $("footer").height() )
    const newHeight = avaiableHeight - (buttonBullOutMargin + buttonBullOutHeight + playerInputGroupTop)

    console.log('new height: ' + newHeight)

    $(outerPlayerInputGroup).height( newHeight )
}
