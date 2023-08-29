function popupChooseRoom(roomName, capacity, startDate, endDate, startTime, endTime) {
    console.log("End Date: " + endDate);

    $(".modal-title").html("Meeting Room Reservation");

    $(".modal-body").html(
        `
        <form method="post" action="Booking/InsertEvent">
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
                        <td class="first-column">Date</td>
                        <td class="mediator-column">:</td>
                        <td>
                            <input type="text" name="startDate" class="second-column form-control" value="${startDate}" disabled/>
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
                            <input type="text" name="roomName" class="second-column form-control" value="${capacity}" disabled/>
                        </td>
                    </tr>              
                </table>
            <div style="text-align: right">
                <input type="submit" class="btn btn-success" value="Create"/>
                <button type="button" data-bs-dismiss="modal" class="btn btn-primary">Cancel</button>
            </div>
        </form>
        `
    );

    $('#modalStartTimepicker').timepicker({
        timeFormat: 'h:i A',
        minTime: startTime,
        maxTime: endTime,
        dynamic: false,
        dropdown: true,
        scrollbar: true
    });

    $('#modalEndTimepicker').timepicker({
        timeFormat: 'h:i A',
        minTime: startTime,
        maxTime: endTime,
        dynamic: false,
        dropdown: true,
        scrollbar: true
    });

    $(".modal").modal("show");
}