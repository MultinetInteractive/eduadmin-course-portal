var BookingValidationHelper = {
    GetInputIdSuffix: function (element) {
        var $element = $(element);
        var id = $element.attr("id");
        if (id === undefined || id === null) {
            return id;
        }

        var seperatorIndex = id.lastIndexOf("_");
        if (seperatorIndex < 0) {
            return id;
        }
        return id.substring(seperatorIndex + 1);
    }
};

var BookingBook = {
    UnexpectedBookingError: "",
    UnexpectedBookingErrorReloadPage: "",
    BookingFailedReloadPage: "",
    Init: function(unexpectedBookingError, bookingFailedReloadPage, unexpectedBookingErrorReloadPage) {
        this.UnexpectedBookingError = unexpectedBookingError;
        this.BookingFailedReloadPage = bookingFailedReloadPage;
        this.UnexpectedBookingErrorReloadPage = unexpectedBookingErrorReloadPage;
    },

    ClearCustomErrors: function() {
        $("#ulCustomErrors").empty();
        $("#spnCustomErrors").text("");
	},

	Success: function (data) {
        if (!data) {
            $("#spnCustomErrors").text(BookingBook.UnexpectedBookingError);
        } else {
			if (data.success === true) {
                window.location.href = data.successRedirectUrl;
            } else {
				try {
					console.log(data);
                    BookingBook._ShowServerErrors(data.errors);
				} catch (e) {
                    $("#spnCustomErrors").text(BookingBook.BookingFailedReloadPage);
                }
                $("#btnBook").show();
                $("#loading").hide();
            }
        }
    },

	Failure: function (data) {
        $("#spnCustomErrors").text(BookingBook.UnexpectedBookingErrorReloadPage);
        $("#btnBook").show();
        $("#loading").hide();
    },

    _ShowServerErrors: function(serverErrors) {
        var $form = $("#bookform");
        var $summary = $form.find("[data-valmsg-summary=true]")
            .addClass("validation-summary-errors")
            .removeClass("validation-summary-valid");
        var $ul = $summary.find("ul").empty();
        if ($ul.length === 0) {
            $ul = $("#ulCustomErrors").empty();
        }

        var error = {};

        for (var key in serverErrors) {
            if (key !== "") {
                error[key] = serverErrors[key].join("<br />");
            } else {
                $.each(serverErrors[key],
                    function() {
                        $("<li />").html(this).appendTo($ul);
                    });
            }
        }
        $form.validate().showErrors(error);
    }
};

var BookingParticipants = {
    AdditionalParticipantTarget: "#additionalParticipantsTarget",
    ParticipantsUrl: "",

    init: function(participantsurl) {
		this.ParticipantsUrl = participantsurl;
    },

    LoadContactDataIntoParticipant: function(participantIndex) {
        var participantWrapper = $(".single-participant-wrapper")[participantIndex];
        $(participantWrapper).attr("customerContact", true);

        $("#customerContactWrapper input").each(function() {
            var idSuffix = BookingValidationHelper.GetInputIdSuffix(this);
            if (idSuffix) {
                var $partElem = $("#Participants_" + participantIndex + "__" + idSuffix);
                $partElem.val($(this).val());
                $partElem.attr("readonly", "readonly");
            }
        });

        $(participantWrapper).find("input").valid();
    },

    AddParticipant: function(customerContact, callback) {
        var stop = false;
        if (customerContact) {
            if ($('*[customerContact="true"]').val() !== undefined) {
                var index = $('*[customerContact="true"]').find("*[id]").first().attr("id").match(/\d+/);
                BookingParticipants.LoadContactDataIntoParticipant(index);
                return;
            }
            $(".single-participant-wrapper").each(function(i, partWrapper) {
                var inputs = $(partWrapper).find("input[type=text]");
                var allEmpty = true;
                inputs.each(function(index, element) {
                    if ($(element).val() !== "") {
                        allEmpty = false;
                        return false;
                    }
                });
                if (allEmpty) {
                    stop = true;
                    BookingParticipants.LoadContactDataIntoParticipant(i);
                    return false;
                }
            });
        }

        if (stop) {
            return;
        }
        $(".single-participant-wrapper").find("#Participants_0__btnRemove").show();

        var targetDiv = $(this.AdditionalParticipantTarget);
        if (targetDiv.length > 0) {
            $.ajax({
                url: BookingParticipants.ParticipantsUrl,
                success: function(data) {
                    targetDiv.append(data);
                    BookingParticipants.ParticipantTemplate.ReplaceParticipantTemplate();
                    BookingParticipants.FixIndicies();
                    $.validator.unobtrusive.parseDynamicContent("#participantsWrapper");
                    FormPrice.InitFormPrice();
                    if (customerContact) {
                        addCustomerContact = false;

                        var index = $(".single-participant-wrapper").last().find("*[id]").first().attr("id")
                            .match(/\d+/);
                        BookingParticipants.LoadContactDataIntoParticipant(index);

                    }
                    if (callback) {
                        callback();
                    }
                }
            });
        }
    },

    FixIndicies: function() {
        var wrappers = $(".single-participant-wrapper");
        var count = wrappers.length;
        for (var i = 0; i < count; i++) {
            $(wrappers[i]).find("input").each(function(index, value) {
                if ($(value).attr("name").search(/\d+/, i) > -1) {
                    $(value).attr("name", $(value).attr("name").replace(/\d+/, i));
                } else {
                    $(value).attr("name", "Participants[" + i + "]." + $(value).attr("name"));
                }
            });
            $(wrappers[i]).find("*[id]").each(function(index, value) {
                if ($(value).attr("id").indexOf("Participants") === 0) {
                    $(value).attr("id", $(value).attr("id").replace(/\d+/, i));
                } else {
                    $(value).attr("id", "Participants_" + i + "__" + $(value).attr("id"));
                }
            });
            $(wrappers[i]).find("*[for]").each(function(index, value) {
                if ($(value).attr("for").indexOf("Participants") === 0) {
                    $(value).attr("for", $(value).attr("for").replace(/\d+/, i));
                } else {
                    $(value).attr("for", "Participants_" + i + "__" + $(value).attr("for"));
                }
            });
            $(wrappers[i]).find("*[data-valmsg-for]").each(function(index, value) {
                var newVal = "";
                if ($(value).data().valmsgFor.indexOf("Participants") === 0) {
                    newVal = $(value).data().valmsgFor.replace(/\d+/, i);
                } else {
                    newVal = "Participants[" + i + "]." + $(value).data().valmsgFor;
                }
                $(value).data("valmsgFor", newVal);
                $(value).attr("data-valmsg-for", newVal);
            });
        }
    },

    AddPrefix: function(element, index, attr, prefix, suffix) {
        var prevName = $(element).attr("name");
        if (prevName !== undefined && prevName !== null) {
            var temp = prevName.split(".");
            prevName = temp[temp.length - 1];
            $(element).attr("name", prefix + index + suffix + prevName);
        }
    },

    ParticipantTemplate: {
        PriceNameTemplate: "",
        SubEventTemplate: "",
        _regExpIndexReplace: new RegExp("ParticipantIndexPlaceholder", "g"),
        ReplaceParticipantTemplate: function() {
            $(".participantPriceNamePlaceholder")
                .replaceWith(BookingParticipants.ParticipantTemplate.PriceNameTemplate);
            $(".participantSubEventPlaceholder").replaceWith(BookingParticipants.ParticipantTemplate.SubEventTemplate);
        },
        RemoveDOMTemplates: function() {
            $(".participantSubEventPlaceholder").remove();
            $(".participantPriceNamePlaceholder").remove();
        }
    },

    DeleteParticipant: function(sender) {
        $(sender).closest(".single-participant-wrapper").remove();
        BookingParticipants.FixIndicies();
        $.validator.unobtrusive.parseDynamicContent("#participantsWrapper");
        FormPrice.InitFormPrice();
        this.HideDeleteButtonIfOnlySinglePart();
    },

    HideDeleteButtonIfOnlySinglePart: function() {
        if ($(".single-participant-wrapper").length <= 1) {
            $(".single-participant-wrapper").find("#Participants_0__btnRemove").hide();
        }
    },

    AddAsParticipant: function() {
        var allValid = true;
        $("#customerContactWrapper input").each(function() {
            allValid = $(this).valid() && allValid;
        });
        if (allValid) {
            addCustomerContact = true;
            BookingParticipants.AddParticipant(addCustomerContact, null);
        }
    }
};

function FormSubmit() {
    if (!$(this).valid()) {
        return false;
    } else {
        BookingParticipants.FixIndicies();
        $("#btnBook").hide();
        $("#loading").show();
        return true;
    }
}

function IndexBookingOnReady() {
    var priceNameTemplates = $(".ParticipantPriceNameTemplate");
    if (priceNameTemplates.length > 0) {
        BookingParticipants.ParticipantTemplate.PriceNameTemplate = $(priceNameTemplates[0]).html();
        BookingParticipants.ParticipantTemplate.ReplaceParticipantTemplate();
        priceNameTemplates.remove();
        BookingParticipants.ParticipantTemplate.RemoveDOMTemplates();
    }
    BookingParticipants.FixIndicies();
    BookingParticipants.HideDeleteButtonIfOnlySinglePart();
    FormPrice.InitFormPrice();
}
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


function getValue(valueHolder, ignoreContent) {
    var jqueryValueHolder = $(valueHolder);
    switch (jqueryValueHolder.data().type) {
        case "text":
        case "date":
            if (jqueryValueHolder.val().length > 0 || ignoreContent) {
                return jqueryValueHolder.data().value;
            } else
                return 0;
        case "checkbox":
        case "radio":
            if (jqueryValueHolder.is(":checked"))
                return jqueryValueHolder.data().value;
            else
                return 0;
        case "numerical":
            if (valueHolder.value == "")
                return 0;
            return parseSweFloat(jqueryValueHolder.data().value) * parseInt(valueHolder.value);
        case "select":
            var options = valueHolder.options;
            var selectedOption = options[options.selectedIndex];
            return parseSweFloat($(selectedOption).data().value);
        case "hidden":
            return parseSweFloat(jqueryValueHolder.data().value);
        default:
            return 0;
    }
}

function getQuantity(quantityHolderId) {
    var quantityHolder = $("#" + quantityHolderId);
    if (quantityHolder.val() == "")
        return 0;
    return parseSweFloat(quantityHolder.val());
}

Number.prototype.round = function (p) {
    p = p || 10;
    return parseSweFloat(this.toFixed(p));
};

function parseSweFloat(strNr) {
    if (typeof (strNr) === "string")
        return parseFloat(strNr.replace(',', '.'));
    return strNr;
};

FormPrice = {
    NatCurrShort: "",
    NoVat: "",

    Init: function (natCurrShort, noVat) {
        this.NatCurrShort = natCurrShort;
		this.NoVat = noVat;
	    FormPrice.InitFormPrice();
    },

    InitFormPrice: function () {
        var valueHolders = $(".formValue");
        var quantityHolders = $(".quantityHolder");
        var formValuesWithQuantity = $(".formValueWithQuantity");
        valueHolders.change(FormPrice.RecalculatePrice);
        quantityHolders.change(FormPrice.RecalculatePrice);
        formValuesWithQuantity.change(FormPrice.RecalculatePrice);
        FormPrice.RecalculatePrice();
    },

    RecalculatePrice: function () {
        var totalPrice = 0.0;
        var valueHolders = $(".formValue");
        for (var i = 0; i < valueHolders.length; i++) {
            var elPrice = getValue(valueHolders[i], false);
            totalPrice += parseSweFloat(elPrice);
        }
        var formValuesWithQuantity = $(".formValueWithQuantity");
        for (var i = 0; i < formValuesWithQuantity.length; i++) {
            var quantity = parseInt(getQuantity($(formValuesWithQuantity[i]).data().quantityid));
            var elValue = parseSweFloat(getValue(formValuesWithQuantity[i], true));
            totalPrice += quantity * elValue;
        }
        var priceString = totalPrice.round(2);
        var sign = FormPrice.NatCurrShort;
        if (priceString == 0) {
            $("#rowTotalPrice").hide();
            return;
        }
        $("#rowTotalPrice").show();
        $("#spnTotalPrice").text(priceString + " " + sign + " " + FormPrice.NoVat);
    }
}

function getOldParticipantCount() {
    var oldVal = $("#partqtyholder").val();
    if (oldVal === undefined) {
        return -1;
    }
    return parseInt(oldVal);
}
function CreateInterestReg(courseTemplateId) {
    var company = $("#CompanyName").val();
    var name = $("#Name").val();
    var email = $("#Email").val();
    var mobile = $("#Mobile").val();
    var partnr = $("#PartNr").val();
    var notes = $("#Notes").val();

	var valid = true;

    if (company.length < 1) {
        $("#CompanyName").parent().addClass("has-error");
        valid = false;
	}

    if (name.length < 1) {
        $("#Name").parent().addClass("has-error");
        valid = false;
	}

    if (email.length < 1) {
        $("#Email").parent().addClass("has-error");
        valid = false;
	}

    if (mobile.length < 1) {
        $("#Mobile").parent().addClass("has-error");
        valid = false;
	}

    if (partnr.length < 1) {
        $("#PartNr").parent().addClass("has-error");
        valid = false;
	}

    if (parseInt(partnr, 10) > 100) {
        $("#PartNr").parent().addClass("has-error");
        valid = false;
	}

    if (!Validation.ValidEmail(email)) {
        $("#Email").parent().addClass("has-error");
        valid = false;
    }

    if (valid === false) {
        $("#errorMessage").css("display", "block");
        return;
    }
}

function openPopup(id, titleName) {
	$("#schedule_" + id).dialog({
		title: titleName,
		height: "auto",
		modal: true
	});
}

function InterestRegSuccess(data) {
	if (data.success === true) {
        $("#interestRegistration").dialog("close");
		toastr.success(data.message);
    } else {
        EducationError.InterestRegFailed();
    }
}

function ShowInterestRegPopUp(courseTemplateId, titleName) {
	$("#interestRegistration").dialog({
		title: titleName,
		height: "auto",
		modal: true
	});
}

var EducationFilterObjects = {
    FilterObjectsUrl: "",
    Init: function(filterobjectsurl) {
        this.FilterObjectsUrl = filterobjectsurl;
    },
    FilterObjects: function(subjectID, categoryID) {
        $(".loading").show();
        if (subjectID === "" || subjectID === undefined)
            subjectID = null;
        if (categoryID === "" || categoryID === undefined)
            categoryID = null;
        $("#courseWrapper").load(this.FilterObjectsUrl + "?SubjectID=" + subjectID + "&CategoryID=" + categoryID,
            function() {
                $(".loading").hide();
            });
    }
};

var EducationError = {
    InterestRegistrationError: "",

    Init: function(interestRegistrationError) {
        this.InterestRegistrationError = interestRegistrationError;
    },
    InterestRegFailed: function() {
        toastr.error(EducationError.InterestRegistrationError);
    }
};

function showOrHideTable(courseTemplateId, hideCourses, moreCourses, moreCourse) {
	var table = document.getElementsByClassName(courseTemplateId);
	if (CheckForHiddenTables(table) === true) {
		for (var i = 0; i < table.length; i++) {
			table[i].style.display = "inline";
		}
		var item1 = document.getElementById("link" + courseTemplateId);
		item1.textContent = hideCourses;
	}
	else {
		for (var j = 0; j < table.length; j++) {
			if (j > 0) {
				table[j].style.display = "none";
			}
		}
		var item2 = document.getElementById("link" + courseTemplateId);
		if (table.length - 1 > 1) {
			item2.textContent = table.length - 1 + " " + moreCourses;
		}
		if (table.length - 1 === 1) {
			item2.textContent = table.length - 1 + " " + moreCourse;
		}

	}
}

function CheckForHiddenTables(tables) {
	var bool = false;
	for (var i = 0; i < tables.length; i++) {
		if (tables[i].style.display === "none") {
			bool = true;
			break;
		}
	}
	return bool;
}