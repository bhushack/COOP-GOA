var dscurl = "http://localhost:9000/";
var stack;
var taskRunning = false;
var stopTask = false;
var stackTotal = 0;
function DSCChecker(callback) {
    $.ajax({
        url: dscurl,
        success: function (html) {
            if (html == "<!DOCTYPE html><html><body>v1.5.1.0</body></html>") {  //Uncomment for DSC version check and updation.
                callback(true, "");
            }
            else {
                callback(true, "ReqNew");
            }
        },
        error: function () {
            callback(false, "");
        }
    });
}

function startTask() {
    if (stopTask) {
        return;
    }
    if (taskRunning) {
        setTimeout(startTask, 1000);
        return;
    }
    if (stack.length >= 0) {
        setTimeout(pickTask, 1000);
    }
    else {
        $(".btn-sign").removeAttr("disabled");
    }
    var per = Math.abs(((stackTotal - stack.length) / stackTotal) * 100);
    if (per == 0) {
        per = 5;
    }
    $(".sign-indic").attr("aria-valuenow", per);
    $(".sign-indic").attr("style", "width:" + per + "%");
    $(".sign-indic .sr-only").text(per + "% completed.");
}

function pickTask() {
    var index = 0;
    if (stack.length > 0) {
        index = stack.length - 1;
    }
    taskRunning = true;
    var apprefno = stack[index];
    $(".sign-indic-text").text("Signing " + stack[index]);
    stack.pop();
    CreatePayload(apprefno);
    startTask();
}

function CreatePayload(apprefno) {
    $(".sign-status").hide();
    $(".sign-status").removeClass("callout").removeClass("callout-success");
    var continuePin = "false";
    if (stack.length == 0) {
        continuePin = "true";
    }
    

    $.ajax({
        type: 'POST',
        method: 'POST',
        url: "Public?mode=CreatePayloadThirdParty",
        data: { ApplnRefNo: apprefno, Continue: continuePin },
        async: false,
        success: function (json) {          
            $(".sign-status").show();
            if (json.ErrorCode == "0" || json.ErrorCode == "" || json.ErrorCode == "null") {
                DSCSign(json.Result);
            }
            else {
                taskRunning = false;
                stopTask = true;
                $("#signModal").modal("hide");
                $(".sign-status").text(json.ErrorMessage);
                $(".sign-status").addClass("callout").addClass("callout-danger");
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            taskRunning = false;
            stopTask = true;
            $("#signModal").modal("hide");
            $(".sign-status").show();
            $(".sign-status").text("Could not Create payload. Please try again later.");
            $(".sign-status").addClass("callout").addClass("callout-danger");
        }
    });
}

function DSCSign(payload) {

    $(".sign-status").hide();
    $(".sign-status").removeClass("callout").removeClass("callout-success");
    $.ajax({
        type: 'POST',
        url: dscurl + "OpenESigner",
        data: payload,

        async: false,
        success: function (result) {
            $(".sign-status").show();
            var json = $.parseJSON(result);
            if (json.ErrorCode == "") {
                SavePayload(json.OutputData);
            }
            else {
                if (json.ErrorCode == "null") {
                    taskRunning = false;
                    stopTask = true;
                    $("#signModal").modal("hide");
                    $(".sign-status").text("ESIGN returned nothing.");
                    $(".sign-status").addClass("callout").addClass("callout-danger");
                }
                else {
                    taskRunning = false;
                    stopTask = true;
                    $("#signModal").modal("hide");
                    $(".sign-status").text(json.ErrorCode);
                    $(".sign-status").addClass("callout").addClass("callout-danger");
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            taskRunning = false;
            stopTask = true;
            $("#signModal").modal("hide");
            $(".sign-status").show();
            $(".sign-status").text("Could not DSC Sign. Please try again later.");
            $(".sign-status").addClass("callout").addClass("callout-danger");
        },
        timeout: 300000
    });
}
function SavePayload(output) {
    $(".sign-status").hide();
    $(".sign-status").removeClass("callout").removeClass("callout-success");
    $.ajax({
        type: 'POST',
        url: "Public.aspx?mode=SavePayloadThirdParty",
        data: { Payload: output },
        async: false,
        success: function (json) {
            taskRunning = false;
            $(".sign-status").show();
            if (stack.length == 0) {
                $("#signModal").modal("hide");
            }
            if (json.ErrorCode == "0" || json.ErrorCode == "" || json.ErrorCode == "null") {
                if (stack.length == 0) {
                    stopTask = true;
                    window.location.href = "Modal.aspx";
                    //window.location.href = getDomainUrl() + "/approval/listesign";
                }
            }
            else {
                stopTask = true;
                $(".sign-status").text(json.ErrorMessage);
                $(".sign-status").addClass("callout").addClass("callout-danger");
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            taskRunning = false;
            stopTask = true;
            $("#signModal").modal("hide");
            $(".sign-status").show();
            $(".sign-status").text("Could not save payload. Please try again later.");
            $(".sign-status").addClass("callout").addClass("callout-danger");
        }
    });
}