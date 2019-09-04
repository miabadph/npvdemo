var tbl = document.getElementById('tbl');
var cf = document.getElementById('cashFlowValue');
var low = document.getElementById('lowerBound');
var up = document.getElementById('upperBound');
var inc = document.getElementById('incrementBox');
var initial = document.getElementById('initialValue');
var cfItems = $('#cashFlowItems');
var resultOptions = $('#resultSelector');

var resultsRefreshing = false;
var isCalculating = false;

function calculate() {
	if (isCalculating) {
		return;
	}
	isCalculating = true;
    var lb = parseFloat(low.value);
    var ub = parseFloat(up.value);
    if (lb <= 0 || lb > ub) {
        alert("Enter a valid Lower Bound value.");
        low.value = "";
        return;
    }
    if (ub <= lb) {
        alert("Enter a valid Upper Bound value.");
        up.value = "";
        return;
    }
    //var cfStr = cf.value.replace(/ /g, '').split(",");
    var cfData = [];
    $("#cashFlowItems option").each(function () {
        cfData.push(parseFloat(this.value));
    });
	
	if(cfData.length < 1) {
		alert("No inputted cash flow/s.");
		return;
	}

    var inputData = {
        CashFlow: cfData,
        InitialValue: parseFloat(initial.value),
        LowerBound: lb,
        UpperBound: ub,
        Increment: parseFloat(inc.value)
    };

    var jsonInput = JSON.stringify(inputData);
    $.ajax({
        url: "/api/NPV/CalculateNPV",
		type: "POST",
		data: jsonInput,
        contentType:"application/json;charset=utf-8",
        success: function (res) {
            var opt = document.createElement('option');
            opt.value = res;
            opt.innerHTML = res;
            resultOptions.append(opt);
            resultOptions.val(res);
			GetResults(res);
			 ClearFields();
        },
        error: function () {
            console.log("Calculation Error");
			if (isCalculating) {
				isCalculating = false;
				 ClearFields();
			}
        }
    })

    //ClearFields();
}

function LoadResults() {
    $.ajax({
        url: "/api/NPV/GetAllCalculationId",
        type: "GET",
        success: function (data) {
            for (var item in data) {
                var opt = document.createElement('option');
                opt.value = data[item];
                opt.innerHTML = data[item];
                resultOptions.append(opt);
            }
        },
        error: function () {
            console.log("Error load results");
        }
    })
}

function GetResults(cid) {
    console.log("Getting results: " + cid);
    $.ajax({
        url: "/api/NPV/GetCalculationsById",
        type: "GET",
        dataType: "html",
        data: { id: cid },
        success: function (data) {
            var objectData = JSON.parse(data);
            for (var item in objectData) {
                addRow(objectData[item].cashFlow, objectData[item].rate, objectData[item].computedValue);
            }
			isCalculating = false;
        },
        error: function () {
            console.log("Calculation Error");
			isCalculating = false;
        }
    })
}

function getNPV(rate, initialCost, cashFlows) {
    var npv = initialCost;
    if (npv > 0) {
        npv = (npv * -1);
    }
    console.log(npv);
    var dRate = (rate / 100);
    for (var i = 0; i < cashFlows.length; i++) {
        npv += cashFlows[i] / Math.pow(1 + dRate, i + 1);
    }

    return npv;
}

function addCell(tr, val) {
    var td = document.createElement('td');

    td.innerHTML = val;

    tr.appendChild(td);
}


function addRow(val_1, val_2, val_3) {
    var tr = document.createElement('tr');

    addCell(tr, val_1);
    addCell(tr, val_2);
    addCell(tr, val_3);

    tbl.appendChild(tr);
}

function ClearFields() {
    cfItems.empty();
    low.value = "";
    up.value = "";
    inc.value = "";
    initial.value = "";
}

$("#clearResultsBtn").click(function () {
    $("#tbl tr:gt(0)").remove(); 
});

$('#calcBtn').click(calculate);

$('#displayBtn').click(function () {
    $("#tbl tr:gt(0)").remove();
    var selectedItem = resultOptions.find('option:selected');
    GetResults(selectedItem[0].value);
});

$('#addBtn').click(function () {
    var opt = document.createElement('option');
    var valueTest = /^[0-9]+(\.[0-9]{1,2})?$/;
    if (valueTest.test(cf.value)) {
        opt.value = cf.value;
        opt.innerHTML = cf.value;
        cfItems.append(opt);
        cf.value = "";
    } else {
        alert("Enter a valid amount.");
        cf.value = "";
    }
});

$('#removeBtn').click(function () {
    cfItems.find('option:selected').remove();
});

$('#clearBtn').click(function () {
    cfItems.empty();
})

$('#refreshResultsBtn').click(function () {
	if(resultsRefreshing) {
		return;
	}
	
	$.ajax({
        url: "/api/NPV/GetAllCalculationId",
        type: "GET",
        success: function (data) {
			resultOptions.empty();
            for (var item in data) {
                var opt = document.createElement('option');
                opt.value = data[item];
                opt.innerHTML = data[item];
                resultOptions.append(opt);
            }
			resultsRefreshing = false;
        },
        error: function () {
			resultsRefreshing = false;
            console.log("Error load results");
        }
    })
	resultsRefreshing = true;
});

LoadResults();