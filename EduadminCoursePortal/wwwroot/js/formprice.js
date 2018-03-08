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