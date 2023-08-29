function popupChooseRoom(roomName, capacity, startDate, endDate, startTime, endTime) {
    console.log("End Date: " + endDate);

    $(".modal-title").html("Meeting Room Reservation");

    $(".modal-body").html(
        `
        <form method="post" id="createMeetingForm" action="Booking/InsertMeeting">
        @Html.AntiForgeryToken()
            <h5>Meeting Detail</h5>
            <hr class="full-width-hr">
                <table class="table table-borderless">
                    <tr>
                        <td class="first-column">Meeting Summary</td>
                        <td class="mediator-column">:</td>
                        <td>
                            <input type="text" name="summary" class="second-column form-control"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="first-column desc">Description</td>
                        <td>:</td>
                        <td>
                            <textarea type="text" name="description" class="second-column form-control"/>
                            </textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="first-column desc">Attendee</td>
                        <td>:</td>
                        <td>
                            <textarea type="text" name="attendees" class="second-column form-control" placeholder="attendee1@example.com, attendee2@example.com"/>
                            </textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="first-column">Date</td>
                        <td class="mediator-column">:</td>
                        <td>
                            <input type="text" name="startDate" class="second-column form-control" value="${startDate}" disabled/>
                        </td>
                        <td>
                            <input type="text" name="endDate" class="second-column form-control" value="${endDate}" hidden/>
                        </td>
                    </tr>
                    <tr>
                        <td class="first-column">Time</td>
                        <td class="mediator-column">:</td>
                        <td class="row">
                                <div class="col">
                                    <input type="text" class="form-control" name="meetingStartTime" placeholder="hh:mm AM/PM"
                                        id="modalStartTimepicker" autocomplete="off" />
                                </div>
                                    -
                                <div class="col">
                                    <input type="text" class="form-control" name="meetingEndTime" placeholder="hh:mm AM/PM"
                                        id="modalEndTimepicker" autocomplete="off" />
                                </div>
                        </td>
                    </tr>              
                </table>   
            <h5>Room Detail</h5>
            <hr class="full-width-hr">
                <table class="table table-borderless">
                    <tr>
                        <td class="first-column">Room Name</td>
                        <td class="mediator-column">:</td>
                        <td>
                            <input type="text" name="roomName" class="second-column form-control" value="${roomName}" disabled/>
                        </td>
                    </tr>
                    <tr>
                        <td class="first-column">Room Capacity</td>
                        <td class="mediator-column">:</td>
                        <td>
                            <input type="text" name="roomCap" class="second-column form-control" value="${capacity}" disabled/>
                        </td>
                    </tr>              
                </table>
            <div style="text-align: right">
                <input type="submit" class="btn btn-success" id="submit-meeting" value="Create"/>
                <button type="button" data-bs-dismiss="modal" class="btn btn-primary">Cancel</button>
            </div>
        </form>
        `
    );

    // var midnight = new Date("January 13 2000 00:00");
    // var midnightHour = midnight.getHours();
    // var midnightMinute = midnight.getMinutes();

    var availStart = new Date("January 13 2000 " + startTime);
    console.log("Avail Start: " + availStart);

    if (availStart.getMinutes() > 30) {
        availStart.setHours(availStart.getHours() + 1);
        availStart.setMinutes(0);
    } else if (availStart.getMinutes() > 0) {
        availStart.setMinutes(30);
    }

    var availEnd = new Date("January 13 2000 " + endTime);
    console.log("Avail End: " + availEnd);

    $('#modalStartTimepicker').timepicker({
        timeFormat: 'H:i',
        minTime: availStart,
        maxTime: availEnd,
        dynamic: false,
        dropdown: true,
        scrollbar: true,
        step: 30,
        forceRoundTime: true
    });

    $('#modalEndTimepicker').timepicker({
        timeFormat: 'H:i',
        minTime: availStart,
        maxTime: availEnd,
        dynamic: false,
        dropdown: true,
        scrollbar: true,
        step: 30,
        forceRoundTime: true
    });

    var startTimeInput = $('#modalStartTimepicker').val();
    var endTimeInput = $('#modalEndTimepicker').val();

    $('#modalStartTimepicker').on("change", function () {
        startTimeInput = $("#modalStartTimepicker").val();
        $('#modalEndTimepicker').timepicker('option', 'minTime', startTimeInput);
        console.log("Start Time Input: " + startTimeInput);
    });

    $('#modalEndTimepicker').on("change", function () {
        endTimeInput = $("#modalEndTimepicker").val();
        console.log("End Time Input: " + endTimeInput);
    });

    $('#submit-meeting').on('click', function (event) {
        // console.log("Start Time: " + startTime);
        // console.log("Start Time Input: " + startTimeInput);
        // console.log(startTimeInput < startTime);
        // console.log("End Time Input: " + endTimeInput);
        // console.log("Midnight Hour: " + midnightHour);
        // console.log("End Time Hour: " + availEnd.getHours());
        // console.log(availEnd.getHours() == midnightHour);
        // event.preventDefault();

        if (startTimeInput < startTime || (startTimeInput > endTime && availEnd.getHours() != 0)) {
            alert("Invalid time! Please input start time within the range");
            event.preventDefault();
        } else if ((endTimeInput !== "00:00" && endTimeInput < startTime) || (endTimeInput > endTime && availEnd.getHours() != 0)) {
            alert("Invalid time! Please input end time within the range");
            event.preventDefault();
        }
    });

    $(".modal").modal("show");
}