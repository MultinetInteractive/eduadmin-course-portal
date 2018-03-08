var resizeParentFrame = function () {
    var docHeight = jQuery("body").height();
    if (docHeight !== parentDocHeight) {
        parentDocHeight = docHeight;
        window.parent.postMessage(parentDocHeight, "*");
    }
};

var parentDocHeight = 0;

$(document).ready(function () {
    setInterval(resizeParentFrame, 250);
});