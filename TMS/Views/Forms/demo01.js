$(document).ready(function () {
    $("#MessageModal").hide();
    var MsgSuccess = '@(TempData["ErrorMessage"])';
    if (MsgSuccess != "" && MsgSuccess != null) {
        var color = '@(TempData["color"])';

        if (color == "red") {
            $('#TxtHeaderMessageModal').html('Error!');
            $('#divAlert').removeClass("alert-success");
            $('#divAlert').addClass("alert-danger");
        }
        else {

            $('#TxtHeaderMessageModal').html('Success!');
            $('#divAlert').removeClass("alert-danger");
            $('#divAlert').addClass("alert-success");
        }
        $('#TxtMessageModal').html(MsgSuccess);
        $("#MessageModal").show();
        $("#MessageModal").fadeTo(8000, 500).fadeOut(2000);
    }

});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}


function fnChangeDays() {

    var YearDL = document.getElementById("YearDL");

    var MonthDL = document.getElementById("MonthDL");

    var DayDL = document.getElementById("DayDL");

    var selectedDay = DayDL.selectedIndex;

    document.getElementById("DayDL").innerHTML = "";

    var txt = document.createElement('option');
    txt.text = "Day"
    txt.value = 0;

    DayDL.add(txt);


    if (MonthDL.selectedIndex == 1 || MonthDL.selectedIndex == 3 || MonthDL.selectedIndex == 5 ||
        MonthDL.selectedIndex == 7 || MonthDL.selectedIndex == 8 || MonthDL.selectedIndex == 10 ||
        MonthDL.selectedIndex == 12) {

        for (var i = 1; i <= 31; i++) {
            var option = document.createElement('option');
            option.text = option.value = i;
            DayDL.add(option);
        }
        if (selectedDay > 0)
            DayDL.selectedIndex = selectedDay;
        else
            DayDL.selectedIndex = 0;
    }

    else if (MonthDL.selectedIndex == 4 || MonthDL.selectedIndex == 6 || MonthDL.selectedIndex == 9 ||
        MonthDL.selectedIndex == 11) {
        for (var i = 1; i <= 30; i++) {
            var option = document.createElement('option');
            option.text = option.value = i;
            DayDL.add(option);
        }
        if (selectedDay > 0)
            DayDL.selectedIndex = selectedDay;
        else
            DayDL.selectedIndex = 0;
    }

    else if (MonthDL.selectedIndex == 2) {

        var ini = 1940;
        var curDate = new Date();
        var endDate = curDate.getFullYear() - 19;

        for (var j = ini; j <= endDate; j++) {
            ini = ini + 4;
            document.getElementById("DayDL").innerHTML = "";

            var txt = document.createElement('option');
            txt.text = "Day"
            txt.value = 0;
            DayDL.add(txt);

            if (YearDL.value == ini) {
                for (var i = 1; i <= 29; i++) {
                    var option = document.createElement('option');
                    option.text = option.value = i;
                    DayDL.add(option);
                }
                break;
            }

            else {
                for (var i = 1; i <= 28; i++) {
                    //alert("mmmm")
                    var option = document.createElement('option');
                    option.text = option.value = i;
                    DayDL.add(option);
                }
                //break;
            }
        }
        if (selectedDay > 0)
            DayDL.selectedIndex = selectedDay;
        else
            DayDL.selectedIndex = 0;
    }

}

function cbclick(e) {
    e = e || event;
    var cb = e.srcElement || e.target;
    if (cb.type !== 'checkbox') { return true; }
    var cbxs = document.getElementById('radiocb').getElementsByTagName('input'), i = cbxs.length;
    while (i--) {
        if (cbxs[i].type && cbxs[i].type == 'checkbox' && cbxs[i].id !== cb.id) {
            cbxs[i].checked = false;
        }
    }
}

function isBlnkSpc(evt, txt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode == 32 && txt.value.length == 0)
        return false;

    return true;
}

function OnSuccess(response) {
    var message = "OK";
    alert(message);
}
function OnFailure(response) {
    alert("Error occured.");
}
