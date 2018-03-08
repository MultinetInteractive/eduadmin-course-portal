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