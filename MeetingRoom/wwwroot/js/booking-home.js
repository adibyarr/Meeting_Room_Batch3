$("#datepicker").datepicker({
    minDate: 0,
    showOtherMonths: true,
    selectOtherMonths: true
});

var startDate = new Date();

$("#datepicker2").datepicker({
    minDate: 0,
    showOtherMonths: true,
    selectOtherMonths: true
});

$('#timepicker').timepicker({
    timeFormat: 'H:i',
    dynamic: false,
    dropdown: true,
    scrollbar: true,
});

$('#timepicker2').timepicker({
    timeFormat: 'H:i',
    dynamic: false,
    dropdown: true,
    scrollbar: true,
});

var now = new Date();
var dateNow = now.getDate();
var monthNow = now.getMonth();
var yearNow = now.getFullYear();

var datepick = new Date();
var datepickDateNow = datepick.getDate();
var datepickMonthNow = datepick.getMonth();
var datepickYearNow = datepick.getFullYear();

var datepick2 = new Date();
var datepick2DateNow = datepick2.getDate();
var datepick2MonthNow = datepick2.getMonth();
var datepick2YearNow = datepick2.getFullYear();

var startDate = new Date();
$("#datepicker").on("change", function () {
    startDate = $("#datepicker").datepicker('getDate');
    $('#datepicker2').datepicker('option', 'minDate', startDate);
    datepick = $(this).datepicker('getDate');
    datepickDateNow = datepick.getDate();
    datepickMonthNow = datepick.getMonth();
    datepickYearNow = datepick.getFullYear();
    if (datepickDateNow == dateNow && datepickMonthNow == monthNow && datepickYearNow == yearNow) {
        $('#timepicker').timepicker('option', 'minTime', now);
    } else {
        $('#timepicker').timepicker('option', 'minTime', '0');
    }
});

$("#datepicker2").on("change", function () {
    datepick2 = $(this).datepicker('getDate');
    datepick2DateNow = datepick2.getDate();
    datepick2MonthNow = datepick2.getMonth();
    datepick2YearNow = datepick2.getFullYear();
});

var initTime;
$('#timepicker').on("change", function () {
    initTime = $("#timepicker").val() || "something";
    if (initTime != "something")
        $('#timepicker2').timepicker('option', 'minTime', initTime);
    console.log(initTime);
});