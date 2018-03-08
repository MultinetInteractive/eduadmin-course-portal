/// <reference path="HelperFunctions.js" />
/// <reference path="/Scripts/jquery-2.1.4.js" />
/// <reference path="/Scripts/formprice.js" />
/// <reference path="/Scripts/jquery.unobtrusive-ajax.js" />
/// <reference path="/Scripts/jquery.validate.js" />
/// <reference path="/Scripts/jquery.dynamic.content.validator.js" />
/// <reference path="/Content/jqueryui/jquery-ui.js" />

var Helper = {
	Insert: function (str, index, value) {
		return str.substr(0, index) + value + str.substr(index);
	},
	LuhnAlgorithm: function (number) {
		var calculatedNumbers = [];
		var sum = 0;
		for (var i = 0; i < number.length; i++) {
			var temp = i % 2 === 0 ? number[i] * 2 + "" : number[i];
			temp > 9 ? (calculatedNumbers.push(temp[0]), calculatedNumbers.push(temp[1])) : (calculatedNumbers.push(temp[0]));
		}
		sum = parseInt(calculatedNumbers.reduce(function (prev, cur) {
			return parseInt(prev) + parseInt(cur);
		}));
		return sum % 10 === 0;
	}
}

var Validation = {
    ValidateOrgNr: function (value) {
        //Luhn-algorithm
        var orgnr = value;
        orgnr = orgnr.replace("-", "");
        orgnr.trim();
        if (orgnr.length > 10) {
            return false;
        }

        else if (orgnr.length < 10 || orgnr.length > 12) {
            return false;
        }
        return Helper.LuhnAlgorithm(orgnr);

    },

    ValidatePersNr: function (value) {
        //Luhn-algorithm
        var prsnr = value;
        prsnr = prsnr.replace("-", "");
        prsnr.trim();
        if (prsnr.length > 10 && prsnr.length < 13) {
            prsnr = prsnr.substring(2, prsnr.length);
        }
        else if (prsnr.length < 10 || prsnr.length > 12) {
            return false;
        }

        var dateString = prsnr.substring(0, 6);
        dateString = Helper.Insert(dateString, 0, "19");
        dateString = Helper.Insert(dateString, 4, "-");
        dateString = Helper.Insert(dateString, 7, "-");
        if (!Validation.ValidDate(dateString)) {
            return false;
        }

        return Helper.LuhnAlgorithm(prsnr);
    },

    ValidDate: function (txtDate) {
        var currVal = txtDate;
        if (currVal == "")
            return false;

        var rxDatePattern = /^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/; //
        var dtArray = currVal.match(rxDatePattern); // is format OK?

        if (dtArray == null)
            return false;

        //Checks for mm/dd/yyyy format.
        dtMonth = dtArray[3];
        dtDay = dtArray[5];
        dtYear = dtArray[1];

        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    },

    ValidEmail: function (txtEmail) {
        var text = txtEmail;
        if (!text.indexOf("@")) {
            return false;
        }
        var regex = /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/i;
        var valid = regex.exec(text);
        return valid;
    },
    IsLetter: function (str) {
        return str.length === 1 && str.match(/[a-z]/i);
    },

    validateNotEmpty : function(value, element, params) {
        var parentClassName = $(element).attr("data-val-notempty-wrapperparentclass");
        var currentInputs = $(element).closest("." + parentClassName).find("input:visible");
        var numberOfInpus = currentInputs.length;
        var containsText = false;
        var valid = true;

        for (var i = 0; i < numberOfInpus; i++) {
            if (currentInputs[i].value != "") {
                containsText = true;
            }
        }

        if (value == "" && containsText) {
            valid = false;
        }

        return valid;
    },

    AddCustomJqueryValidation : function() {
        jQuery.validator.addMethod("validateCivRegNumber", function (value, element, params) {
            if (value === "" || value === null) {
                return true;
            }
            return Validation.ValidatePersNr(value);
        }, "");

        jQuery.validator.unobtrusive.adapters.add("civregnr", {}, function (options) {
            options.rules["validateCivRegNumber"] = true;
            options.messages["validateCivRegNumber"] = options.message;
        });

        jQuery.validator.addMethod("validateOrgNumber", function (value, element, params) {
            if (value === null || value === "") {
                return true;
            }
            return Validation.ValidateOrgNr(value) || Validation.ValidatePersNr(value);
        }, "");

        jQuery.validator.unobtrusive.adapters.add("orgnr", {}, function (options) {
            options.rules["validateOrgNumber"] = true;
            options.messages["validateOrgNumber"] = options.message;
        });

        jQuery.validator.addMethod("validateNotEmpty", function (value, element, params) {
            return Validation.validateNotEmpty(value, element, params);
        }, "");
        jQuery.validator.unobtrusive.adapters.add("notempty", {}, function (options) {
            options.rules["validateNotEmpty"] = true;
            options.messages["validateNotEmpty"] = options.message;
		});

		jQuery.validator.addMethod("enforcetrue", function (value, element, param) {
			return element.checked;
		});
		jQuery.validator.unobtrusive.adapters.addBool("enforcetrue");
    }
}

Validation.AddCustomJqueryValidation();