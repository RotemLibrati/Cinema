




function updateTextArea() {
    if($("input:checked")){
        $(".seat *").css({ "background-color": "red" });
    }
    if ($("input:checked").length == ($("#Numseats").val())) {
        $(".seatStructure *").prop("disabled", true);

        var allNameVals = [];
        var allNumberVals = [];
        var allSeatsVals = [];

        //Storing in Array
        allNameVals.push($("#Username").val());
        allNumberVals.push($("#Numseats").val());
        $('#seatsBlock :checked').each(function () {
            allSeatsVals.push($(this).val());
        });

        //Displaying 
        $('#nameDisplay').val(allNameVals);
        $('#NumberDisplay').val(allNumberVals);
        $('#seatsDisplay').val(allSeatsVals);
        var myVal = $("#seatsDisplay").data("MyValue");
    }
    else {
        alert("Please select " + ($("#Numseats").val()) + " seats")
    }
}


//function myFunction() {
//    alert($("input:checked").length);
    

//}



$(":checkbox").click(function () {
    if ($("input:checked").length == ($("#Numseats").val())) {
        $(":checkbox").prop('disabled', true);
        $(':checked').prop('disabled', false);
    }
    else {
        $(":checkbox").prop('disabled', true);
    }
});


