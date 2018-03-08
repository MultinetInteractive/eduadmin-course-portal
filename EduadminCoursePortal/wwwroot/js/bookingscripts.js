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