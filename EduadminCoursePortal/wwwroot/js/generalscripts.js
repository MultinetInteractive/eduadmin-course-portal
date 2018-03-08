function centerWindow(e) {
    e.sender.center();
}

var Pager = function (panelSelector, pageSize) {
    this.selector = panelSelector;
    this.allElements = $(panelSelector);
    this.pageSize = pageSize;
    this.currPage = 0;
    this.numberOfPages = Math.ceil(this.allElements.length / pageSize);

    var that = this;

    var hideAllPages = function() {
        $(that.selector).hide();
    };

    this.ShowPage = function() {
        hideAllPages();
        var from = that.currPage * that.pageSize;
        var to = from + that.pageSize;
        var toShow = that.allElements.slice(from, to);
        $(toShow).show();
    };

    this.PreviousPage = function() {
        if (that.currPage === 0) {
            return;
        }
        that.currPage--;
        that.ShowPage();
    };

    this.NextPage = function() {
        if (that.currPage === that.numberOfPages - 1) {
            return;
        }
        that.currPage++;
        that.ShowPage();
    };
};

function ShowAllPanels(idtype) {
    for (var i = 0; i < allElements.length; i++) {
        $(allElements[i]).closest("[id^='" + idtype + "']").show;
    }
}

function HideAllPanels(idtype) {
    for (var i = 0; i < allElements.length; i++) {
        $(allElements[i]).closest("[id^='" + idtype + "']").hide();
    }
}

function ShowPages(idtype) {
    HideAllPanels(idtype);
    var pageItems = allElements.slice(currPage * 20, 20 + currPage * 20);
    for (var i = 0; i < pageItems.length; i++) {
        $(pageItems[i]).closest("[id^='" + idtype + "]").show();
    }
    $(".pageOf").html("Sida " + (currPage + 1) + "/" + numberOfPages);
}

function NextPage(idtype) {
    if (currPage === numberOfPages - 1) {
        return;
    }
    currPage++;
    ShowPages(idtype);
}

function PreviousPage(idtype) {
    if (currPage === 0) {
        return;
    }
    currPage--;
    ShowPages(idtype);
}

function saveQuestionCallback(data) {
    if (data.Error === undefined) {
        toastr.success("Tillägg har sparats");
    } else {
        toastr.warning("Tillägg kunde inte sparas");
    }
}

function clearToastr() {
    toastr.remove();
}

