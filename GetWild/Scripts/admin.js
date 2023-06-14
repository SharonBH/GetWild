
//function($) {

//} 
//<script src="~/Scripts/bootstrap-multiselect.js"></script>

var pageIndex = 2;
var userType = 2;
var userActive = null;
var userFrozen = false;

function openInNewTab(url) {
    var win = window.open(url, '_blank');
    win.focus();
}
var loaddingdiv = '<div style=\"margin:10% 48%"><img src=\"/images/loading-red.gif\" /></div>';
//$('.datepicker').datepicker({ dateFormat: 'dd/mm/yy' });

//classes
//$('.btn-Participants').click(function (ve) {
//    var title = $(this).data('name');
//    var footer = $(this).data('footer');
//    var id = $(this).data('id');
//    $('#myModal .modal-title').html('רשומים לאימון: ' + title);
//    $('#myModal .modal-body').html(loaddingdiv);
//    //$('#myModal .modal-footer-inner').html('סה"כ מתאמנים רשומים: ' + footer);
//    $('#myModal .modal-body').load('/studioclass/GetClassEnrollment?id=' + id, function (response, status, xhr) {    //+ '&weeklyreport=true'
//        if (status != "success") {
//            $('#IMGUploadContainer').html('תקלה, אנא נסה מאוחר יותר' + response);
//        }
//    });
//    $('#myModal').modal('show', { backdrop: 'static' });
//    ve.stopPropagation();
//});

function classParticipants(link, e) {
    var title = link.data('name');
    var source = link.data('source');
    if (source == 'WeeklyReport') source = true;
    var id = link.data('id');
    $('#myModal .modal-title').html('רשומים לאימון: ' + title);
    $('#myModal .modal-body').html(loaddingdiv);
    //$('#myModal .modal-footer-inner').html('סה"כ מתאמנים רשומים: ' + footer);
    $('#myModal .modal-body').load('/studioclass/GetClassEnrollment?id=' + id + '&WeeklyReport=' + source, function (response, status, xhr) {    //+ '&weeklyreport=true'
        if (status != "success") {
            $('#IMGUploadContainer').html('תקלה, אנא נסה מאוחר יותר' + response);
        }
    });
    $('#myModal').modal('show', { backdrop: 'static' });
    e.stopPropagation();
}


$('#ClassesTable').on('click', '.btn-Participantsplc', function (ve) {
    //$('#myModal').modal('hide');
    //$('#ClassplacementsModal').modal('show');
    var id = $(this).data('id');
    window.location.replace("/studioclass/manageplacements?classid=" + id);
    //openInNewTab("/studioclass/manageplacements?classid=" +id);
    //$('#ClassplacementsModal').modal({ show: true });
    ve.stopPropagation();
});


//$('.btn-Waitlist').click(function (ve) {
$('#ClassesTable').on('click', '.btn-Waitlist', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var id = $(this).data('id');
    $('#myModal .modal-title').html('רשימת המתנה לאימון: ' + title);
    //$('#myModal .modal-footer-inner').html('סה"כ מתאמנים רשומים: ' + footer);
    $('#myModal .modal-body').load('/studioclass/GetClassWaitListEnrollment?id=' + id, function (response, status, xhr) {    //+ '&weeklyreport=true'
        if (status != "success") {
            $('#IMGUploadContainer').html('תקלה, אנא נסה מאוחר יותר' + response);
        }
    });
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-AddParticipants', function (ve) {
    //var title = $(this).data('name');
    var id = $(this).data('id');
    //$('#BoClassEnrollModal .modal-title').html('רישום לאימון: ' + title);
    //$('#BoClassEnrollModal').find('.btn-reg').data('id', id);
    //$('#BoClassEnrollModal').modal('show', { backdrop: 'static' });
    //var loaddingdiv = '<div style=\"margin:10% 48%"><img src=\"/images/loading-red.gif\" /></div>';

    $('#BoClassEnrollModal .modal-content').html(loaddingdiv).load('/studioclass/GetClassById?classId=' + id, function (response, status, xhr) {
        if (status != "success") {
            $('#BoClassEnrollModal .modal-body').html('תקלה, אנא נסה מאוחר יותר' + response);
        }
    });
    $('#BoClassEnrollModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

//$('a[data-confirm]').click(function (ev) {
//    debugger;
//    var href = $(this).data('href');
//    var source = $(this).data('source');
//    $('#dataConfirmModal').find('.btn-ok').attr('href', href);
//    $('#dataConfirmModal').find('.btn-ok').attr('source', source);
//    var msg = $(this).attr('data-message');
//    $('#dataConfirmModal').find('.modal-body').text(msg);
//    var title = $(this).attr('data-title');
//    $('#dataConfirmModal').find('.modal-header').text(title);

//    $('#dataConfirmModal').modal({ show: true });
//    ev.stopPropagation();
//});

function deleteUser(link, e) {
    var href = link.data('href');
    var source = link.data('souce');
    $('#dataConfirmModal').find('.btn-ok').attr('href', href);
    var msg = link.attr('data-message');
    $('#dataConfirmModal').find('.modal-body').text(msg);
    var title = link.attr('data-title');
    $('#dataConfirmModal').find('.modal-header').text(title);

    $('#dataConfirmModal').modal({ show: true });
    e.stopPropagation();
}

$('#btn_searchClassByDate').click(function () {
    var date = $('#SearchByDate').val();
    $('#ClassesTable').html(loaddingdiv).load('/studioclass/GetClassesbydate?date=' + date);
});

$('#btn_searchClassByWeek').click(function () {
    var date = $('#SearchByDate').val();
    //$('#ClassesTable').html(loaddingdiv).load('studioclass/GetClassesbydate?date='+date);
    $(location).attr('href', '/StudioClass/GetWeeklyByDate?date=' + date);
});

function smsSendParticipants(link, e) {
    window.document.location = "/Sms/SmsSender?smsType=" + link.data("smstype") + "&refid=" + link.data("refid");
    e.stopPropagation();
};

//$(".btn-smsSend").click(function (ev) {
//    window.document.location = "/Sms/SmsSender?smsType=" + $(this).data("smstype") + "&UserType=" + $('.ddlUserTypes').val();
//    ev.stopPropagation();
//});

$(".btn-smsSender").click(function (ev) {
    window.document.location = "/Sms/SmsSender?smsType=" + $(this).data("smstype") + "&usertype=0";
    ev.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-smsSendday', function (ve) {
    window.document.location = "/Sms/SmsSender?smsType=" + $(this).data("smstype") + "&refid=0&date=" + $(this).data("refdate").substring(0, 10);
    ev.stopPropagation();
});

$('#manageHeader').on('click', '.btn-smsSend', function (ve) {
    window.document.location = "/Sms/SmsSender?smsType=" + $(this).data("smstype") + "&UserType=" + $('.ddlUserTypes').val();
    ev.stopPropagation();
});


//users
//$(".newclickable-row").click(function () {
//    window.document.location = "/account/Manage?userid=" + $(this).data("id");
//});

//$(".nextclickable-row").click(function () {
//    window.document.location = '/studioclass/Create?id=' + $(this).data("id"); //+ '&withPlacements=' + $(this).data("withplacements");
//});


function processUser(btn,event) {
    var userid = btn.data('id');
    //var userid = $(this).data('id');
    //var btn = $(this);
    $.ajax({
        type: "POST",
        url: "/Account/TickUser",
        data: { UserId: userid },
        datatype: "text",
        success: function (data) {
            if (data.Response == 'Success') {
                btn.parent().html('<i class="glyphicon text-primary glyphicon-ok-sign"></i>');
            }
        }
    });
    event.stopPropagation();
};


$('#ClassesTable').on('click', '.clickable-row', function (e) {
    var source = $('#ClassesTable').data("source");
    window.document.location = '/studioclass/Create?id=' + $(this).data("id") + "&source=" + source;
});

$('#ClassTypeTable').on('click', '.clickable-row', function (e) {
    window.document.location = "/classtype/Create?id=" + $(this).data("id");
});


$('#UserListTable').on('click', '.Manageclick-row', function (e) {
    window.document.location = "/account/Manage?userid=" + $(this).data("id");
});

$('body').on('click', '.Createclick-row', function (e) {
    window.document.location = "/" + $(this).data("type") + "/Create?id=" + $(this).data("id");
});


$('#InstructorListTable').on('click', '.Instructorclick-row', function (e) {
    window.document.location = "/account/UpdateInstructor?userid=" + $(this).data("id");
});

//$(".clickable-row").click(function () {
//    window.document.location = "/UpdateInstructor?userid=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/studio/Create?id=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/classtype/Create?id=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/classtypedetails/Create?id=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/SubscriptionType/Create?id=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/tip/Create?id=" + $(this).data("id");
//});

//$(".clickable-row").click(function () {
//    window.document.location = "/account/Manage?userid=" + $(this).data("id");
//});

//$(".clickable-class").click(function () {
//    window.document.location = "/studioclass/Create?id=" + $(this).data("id") + "&WeeklyReport=true";
//});


$('.ddlUserTypes').change(function () {
    var ut = this.value;
    //pageIndex = 2;
    var frozen = $('#cbFrozen').prop('checked');
    getUsers(ut, true);
});

$('.ddlUserTypesTickets').change(function () {
    var ut = this.value;
    pageIndex = 2;
    getTicketsUsersByType(ut);
});

$('#btn-export-users').click(function (ve) {
    //var activated = $(this).data('activated');
    //var isLateCancel = $(this).data('isLateCancel');
    var ut = $('.ddlUserTypesTickets').val();
    document.location.href = '/export/ExportUserWithTicketSubscription?includefrozen=true' + '&ut=' + ut;
    ve.stopPropagation();
});



$('#cbFrozen').change(function () {
    var frozen = $(this).is(":checked");
    var ut = $('.ddlUserTypes').val();
    getUsers(ut, true);
});

function getUsersByType(type, incFrozen) {
    $.ajax({
        type: "POST",
        url: "/Account/GetUsersByType",
        data: { ut: type, frozen: incFrozen, pageno: 1 },
        datatype: "text",
        beforeSend: function () {
            $('#UserListTable').html(loaddingdiv);
        },
        success: function (data) {
            $('#UserListTable').html(data);
        }
    });
};


function getTicketsUsersByType(type) {
    $.ajax({
        type: "POST",
        url: "/Account/GetTicketsUsersByType",
        data: { ut: type },
        datatype: "text",
        beforeSend: function () {
            $('#UserListTable').html(loaddingdiv);
        },
        success: function (data) {
            $('#UserListTable').html(data);
        }
    });
};


function getUsers(type, active, frozen) {
    //pageIndex = 1;
    pageIndex = 2;
    userType = type;
    userActive = active;
    userFrozen = frozen;
    $.ajax({
        type: "POST",
        url: "/Account/GetUsersByTypeActiveFrozen",
        data: { ut: type, active: active, frozen: frozen },
        datatype: "text",
        beforeSend: function () {
            $('#UserListTable').html(loaddingdiv);
        },
        success: function (data) {
            $('#UserListTable').html(data);
        }
    });
};


function getUsersPage() {
    //debugger;
    //var type = $('.ddlUserTypes').val();
    //if (type == null) type = '2';
    //var incFrozen = false;
    $.ajax({
        type: 'POST',
        url: "/Account/GetUsersByTypeActiveFrozen",
        data: { ut: userType, frozen: userFrozen, active: userActive, pageno: pageIndex, loadmore: true },
        dataType: 'text',
        success: function (data) {
            if (data != "") {
                var i = pageIndex * 100;
                $('#loadMoreUsers').append(data);
                $('#RowCounter').text(i);
                pageIndex++;
            }
            else {
                $('#loadMoreUsers').append("לא נמצאו משתמשים נוספים.");
                pageIndex = -10;
            }
        },
        beforeSend: function () {
            $("#loadMoreUsersprogress").show();
        },
        complete: function () {
            $("#loadMoreUsersprogress").hide();
        }
    });
};

function GetData() {
    //debugger;
    var type = $('.ddlUserTypes').val();
    if (type == null) type = '2';
    var incFrozen = false;
    $.ajax({
        type: 'POST',
        url: "/Account/GetUsersByTypeActiveFrozen",
        data: { ut: type, frozen: incFrozen, active: true, pageno: pageIndex, loadmore: true },
        dataType: 'text',
        success: function (data) {
            if (data != "") {
                var i = pageIndex * 100;
                $('#loadMoreUsers').append(data);
                $('#RowCounter').text(i);
                pageIndex++;
            }
            else {
                $('#loadMoreUsers').append("לא נמצאו משתמשים נוספים.");
                pageIndex = -10;
            }
        },
        beforeSend: function () {
            $("#loadMoreUsersprogress").show();
        },
        complete: function () {
            $("#loadMoreUsersprogress").hide();
        }
    });
};

//$(function () {
$('#userddl').autocomplete({
    minLength: 3,
    source: function (request, response) {
        var url = $(this.element).data('url');
        var type = $(this.element).data('type');
        $.getJSON(url, { term: request.term, type: type }, function (data) {
            response(data);
        });
    },
    select: function (event, ui) {
        $(event.target).next('input[type=hidden]').val(ui.item.id);
        window.document.location = "Manage?userid=" + ui.item.id;
    },
    change: function (event, ui) {
        if (!ui.item) {
            $(event.target).val('').next('input[type=hidden]').val('');
        }
    }
});
//});

//class type details
//$('.btn-Classes').click(function (ve) {
//    var title = $(this).data('name');
//    var footer = $(this).data('footer');
//    var id = $(this).data('id');
//    var month = $(this).data('month');
//    $('#myModal .modal-title').html('מאמן: ' + title);
//    //$('#myModal .modal-footer-inner').html('סה"כ מתאמנים רשומים: ' + footer);
//    $('#myModal .modal-body').html(loaddingdiv);
//    $('#myModal .modal-body').load('/ClassTypeDetails/GetClassByTypesDetails?typeid=' + id + '&month=' + month, function (response, status, xhr) {
//        if (status != "success") {
//            $('#IMGUploadContainer').html('תקלה, אנא נסה מאוחר יותר' + response);
//        }
//    });
//    $('#myModal').modal('show', { backdrop: 'static' });
//    //$('#myModal').find('.btn-RemoveParticipants').data('wr', true);
//    ve.stopPropagation();
//});

//classes

$(".StudioClassChangeDate").click(function () {
    var weekno = $(this).data("weekno");
    //$("#linkbuttons>a.disabled").removeClass("btn-info disabled");
    //$("#day" + id).addClass("btn-info disabled");
    //$.ajax({
    //    type: "POST",
    //    url: "/Account/WeeklyReportList",
    //    data: { id: 0, weekno: weekno },
    //    datatype: "text",
    //    beforeSend: function () {
    //        $('#WekklyDayReport').html(loaddingdiv);
    //    },
    //    success: function (data) {
    //        $('#WekklyDayReport').html(data);
    //    }
    //});
    $(location).attr('href', '/StudioClass?weekno=' + weekno);
    //alert(weekno);
});

$('#ClassesTable').on('click', '.WklyCalanderChangeDate', function (ve) {
    var weekno = $(this).data("weekno");
    $.ajax({
        type: "POST",
        url: "/Report/GetWeeklyTable",
        data: { weekno: weekno },
        datatype: "text",
        beforeSend: function () {
            $('#ClassesTable').html(loaddingdiv);
        },
        success: function (data) {
            $('#ClassesTable').html(data);
        }
    });
});

$('#UserWeeklyReport').on('click', '.CalanderChangeDate', function (ve) {
    var weekno = $(this).data("weekno");
    $(location).attr('href', '/Account/WeeklyReport?weekno=' + weekno);
});



$('.timepicker').timepicker({
    stepMinute: 5,
    timeOnlyTitle: 'שעת תחילת האימון',
    timeText: '',
    hourText: 'שעה',
    minuteText: 'דקות',
    currentText: 'עכשיו',
    closeText: 'סגור',
    isRTL: 'true'
});

$("#classtypeddl").change(function () {
    $("#txtClassName").val($(this).find("option:selected").text());
    $.getJSON('/ClassTypeDetails/GetClassTypesDetailsByType?typeid=' + $(this).val(), function (result) {
        var ddl = $('#classtypedetailsddl');
        ddl.empty();
        $(result).each(function () {
            $(document.createElement('option'))
                .attr('value', this.Id)
                .text(this.Name)
                .appendTo(ddl);
        });
    });
    //$("#classtypedetailsddl")
});

$("#dailyslotddl").change(function () {
    //if ($("#dailyslotddl").find("option:selected").val() == '')
    //    $('#dailyslotddl').attr("data-val", "true");
    if ($(this).find("option:selected").val() == -1) {
        $('.othertime').removeClass("hidden");
        //$('.othertime').attr("data-val", "true");
        //$('.othertime').attr("data-val-required", "msfd");
    } else {
        $('.othertime').addClass("hidden");
        //$('.othertime').attr("data-val", "false");
    }
});

$("#btn_addclass").click(function () {
    //debugger;
    var r = $(".ipothertime").val();
    if ($("#dailyslotddl").find("option:selected").val() == -1 && $(".ipothertime").val() == '') {
        $(".othertime").removeClass('has-success');
        $(".othertime").addClass('has-error');
        return false;
    } else {
        $(".othertime").removeClass('has-error');
        $(".othertime").addClass('has-success');
    }
});

$.getScript('/Scripts/bootstrap-multiselect.js', function () {
    // script is now loaded and executed.
    // put your dependent JS here.
    $('#instructorMulti').multiselect({
        nonSelectedText: 'בחר מאמן',
        allSelectedText: 'כולם נבחרו'
    });
    $('#placementsMulti').multiselect({
        nonSelectedText: 'בחר סוגי מקום',
        allSelectedText: 'כולם נבחרו',
        onChange: function (option, checked, select) {
            var id = $(option).val();
            //debugger;
            //alert('Changed option ' + id + '.');
            if (checked) {
                $('.StudioPlacementId_' + id).removeClass('hidden');
                //$('#MaxParticipantstxt').prop('disabled', true);
                clacMaxParticipants();
            }
            else {
                $('.StudioPlacementId_' + id).addClass('hidden');
                var selectedOptions = $('#placementsMulti option:selected');
                clacMaxParticipants();
                if (selectedOptions.length == 0) { $('#MaxParticipantstxt').prop('disabled', false); }
            }
        }
    });
});

$(".placmentinput").on("change paste keyup", function () {
    //debugger;
    //var myArray = $(".placmentinput").map(function () {
    //    return parseInt($(this).val());
    //}).get();
    clacMaxParticipants();
    //var newval = $(this).val();
    //var maxval = $('#MaxParticipantstxt').val();
    //$('#MaxParticipantstxt').val(parseInt(newval) + parseInt(maxval));
});

function clacMaxParticipants() {
    //debugger;
    var total = 0;
    $(".placmentinput").map(function () {
        if ($(this).parent().parent().is(":not(.hidden, .StudioPlacementId_999)")) {
            total += parseInt($(this).val());
        }
    });
    $('#MaxParticipantstxt').val(total);
}

//SMS
$("#btn_SmsTester").click(function () {
    var isValid = $('#SmsTester').validate().form();
    if (!isValid) return false;
    $.ajax({
        type: "POST",
        url: "/Sms/SendTestMSG",
        data: $('#SmsTester').serialize(),
        datatype: "html",
        success: function (data) {
            $('#Resultmsg').html(data.Message);
        }
    });
});

$(".btn_MessageUpdate").click(function () {
    var isValid = $('#MessageUpdate-' + $(this).data("id")).validate().form();
    if (!isValid) return false;
    $.ajax({
        type: "POST",
        url: "/sms/EditMessage",
        data: $('#MessageUpdate-' + $(this).data("id")).serialize(),
        datatype: "html",
        success: function (data) {
            $('#myModal .modal-title').html('ניהול - SMS');
            $('#myModal .modal-body').html(data.Message);
            $('#myModal').modal('show', { backdrop: 'static' });
        }
    });
});

$(".btn_NumberUpdate").click(function () {
    var number = $("#SenderNumber").val();
    if (!number) return false;
    $.ajax({
        type: "POST",
        url: "/sms/UpdateSenderNumber",
        data: { SenderNumber: number },
        datatype: "html",
        success: function (data) {
            $('#myModal .modal-title').html('ניהול - SMS');
            $('#myModal .modal-body').html(data.Message);
            $('#myModal').modal('show', { backdrop: 'static' });
        }
    });
});


////enroll - outroll
//$("#btn_enrolltoClass").click(function () {
//    debugger;
//    $(this).attr("disabled", true);
//    //var userid = '@ViewBag.UserId';
//    var selectedplacement = $('#ClassPlacements').val();
//    if (!IsPlacements) selectedplacement = 0
//    var classid = $('select[name=ClasstoEnroll]').val();
//    //var posturl = 'AdminEnrollToClass';
//    //if (IsPlacements) posturl = 'AdminEnrollToClassPlacement'
//    if (classid != null && classid != '') {
//        $.ajax({
//            type: "POST",
//            url: "/StudioClass/AdminEnrollToClass",
//            data: { classId: classid, classAvailablePlacementId: selectedplacement, userId: userid, WeeklyReport: false, UserManage: true },
//            datatype: "text",
//            success: function (data) {
//                var result = data.Response;
//                if (result == 'Error') {
//                    $('#enroll-result').html(data.Message);
//                } else {
//                    $('#BoUserEnrollModal').modal('hide');
//                    $('body').removeClass('modal-open');
//                    $('.modal-backdrop').remove();
//                    $('#enrollmets-tab').html(data);

//                }
//            }
//        });
//    } else {
//        { $('#enroll-result').html('עליך לבחור אימון מהרשימה'); }
//    }
//});

//$("#btn_outrolltoClass").click(function () {
//    $(this).attr("disabled", true);
//    var id = $(this).data("id");
//    $.ajax({
//        type: "POST",
//        url: "/Gym/OutrollFromClass",
//        data: { classid: id },
//        datatype: "text",
//        success: function (data) {
//            var result = data.Response;
//            if (result == 'Error') {
//                $('#enroll-result').html(data.Message);
//            } else {
//                $('#CalanderDailyPanel').html(data);
//                $('#myModal').modal('hide');
//                $('#ConfirmEnrollMSG').html('השינויים נשמרו בהצלחה.');
//                $("#ConfirmEnrollModal").fadeTo(4000, 500).slideUp(500, function () {
//                    $("#ConfirmEnrollModal").alert('close');
//                });
//                //$('#RightNav').load("/Gym/GetRightNav");
//            }
//        }
//    });
//});

$('#ClassDDLByDate').change(function (e) {
    var date = $(this).val();
    //debugger;
    $.ajax({
        type: "POST",
        data: { date: date },
        url: '/Account/GetClassesDDLByDate',
        dataType: 'json',
        success: function (json) {
            var $el = $("#ClasstoEnroll");
            $el.empty(); // remove old options
            $el.append($("<option></option>")
                .attr("value", '').text('בחר אימון'));
            $(json).each(function () {
                $el.append($("<option></option>")
                    .attr("value", this.id).text(this.value));
            });
            //$("#btn_enrolltoClass").attr("disabled", false);
        }
    });
});

//$('#ClasstoEnroll').change(function (e) {
//    var classid = $(this).val();
//    //debugger;
//    $.ajax({
//        type: "POST",
//        data: { classid: classid },
//        url: '/Account/GetPlacementsByClassesDDL',
//        dataType: 'json',
//        success: function (json) {
//            if (json.length == 0) { }
//            else {
//                IsPlacements = true;
//                $('#placementsDiv').removeClass('hidden');
//                var $el = $("#ClassPlacements");
//                $el.empty(); // remove old options
//                $el.append($("<option></option>")
//                    .attr("value", '').text('בחר מיקום'));
//                $(json).each(function () {
//                    $el.append($("<option></option>")
//                        .attr("value", this.Id).text(this.DisplyName));
//                });
//            }
//            $("#btn_enrolltoClass").attr("disabled", false);
//        }
//    });
//});


//reports

$('#btn_searchUsersByDate').click(function () {
    var fromdate = $('#SearchFromDate').val();
    var todate = $('#SearchFromTo').val();
    $('#UsersGraph').html(loaddingdiv).load('/Report/GetGraph?fromdate=' + fromdate + '&todate=' + todate);

});

$('#btn_searchUsers').click(function () {
    var search = $('#searchbox').val();
    $.ajax({
        type: "POST",
        url: "/Account/GetUsersBySearch",
        data: { search: search },
        datatype: "text",
        beforeSend: function () {
            $('#UserListTable').html(loaddingdiv);
        },
        success: function (data) {
            $('#UserListTable').html(data);
        }
    });
});

$('.btn-processbox').click(function (ev) {
    var userid = $(this).data('id');
    var btn = $('#btn_UpdateReportUser');
    btn.attr('data-userid', userid);
    //var msg = $(this).attr('data-message');
    //$('#dataConfirmModal').find('.modal-body').text(msg);
    //var title = $(this).attr('data-title');
    //$('#dataConfirmModal').find('.modal-header').text(title);

    $('#UserReportModal').modal('show', { backdrop: 'static' });
    ev.stopPropagation();
});

$('.btn-SetComment').click(function (ev) {
    var enrollmentid = $(this).data('enrolmentid');
    var userid = $(this).data('userid');
    var classid = $(this).data('classid');
    var title = $(this).data('title');

    $.ajax({
        type: "POST",
        url: "/StudioClass/GetUserComment",
        data: { UserId: userid, EnrollmentId: enrollmentid, ClassId: classid  },
        datatype: "text",
        //beforeSend: function () {
        //    $('#ReportClasses').html(loaddingdiv);
        //},
        success: function (data) {
            $('#CommentPopupDiv').html(data);
            $('#CommentModal').modal('show', { backdrop: 'static' });
            $('#CommentModal .modal-title').html('הערה למתאמן: ' + title);
        }
    });
    ev.stopPropagation();
});

//$('#btn_UpdateComment').click(function () {
//    debugger;
//    var userid = $(this).data('userid');
//    var commentid = $(this).data('commentid');
//    var classid = $(this).data('classid');
//    var title = $(this).data('title');

//    var note = $('#UserComment').val();
//    $.ajax({
//        type: "POST",
//        url: "/StudioClass/SaveUserComment",
//        data: { UserId: userid, Note: note, Classid: classid, EnrollmentId: enrolmentid, CommentId: commentid },
//        datatype: "text",
//        success: function (data) {
//            $('#xxxxxx').html(data);
//            $('#CommentModal').modal('hide');
//        }
//    });
//});

$('#btn_UpdateReportUser').click(function () {
    var userid = $(this).data('userid');
    var note = $('#UserReportNote').val();
    $.ajax({
        type: "POST",
        url: "/Report/ProcessUser",
        data: { UserId: userid, Note: note },
        datatype: "text",
        beforeSend: function () {
            $('#ReportClasses').html(loaddingdiv);
        },
        success: function (data) {
            $('#ReportClasses').html(data);
            $('#UserReportModal').modal('hide');
        }
    });
});

$(".ReportChanged").click(function () {
    var id = $(this).data("id");
    $('#btn-export-weekly').data('id', id);
    var weekno = $(this).data("weekno");
    $("#linkbuttons>a.disabled").removeClass("btn-info disabled");
    $("#day" + id).addClass("btn-info disabled");
    var ut = $(".ddlUserTypesWeekly").val();
    getUsersforWeeklyReport(id, ut, weekno);
});

$(".WeeklyReportChangeDate").click(function () {
    var weekno = $(this).data("weekno");
    //$("#linkbuttons>a.disabled").removeClass("btn-info disabled");
    //$("#day" + id).addClass("btn-info disabled");
    //$.ajax({
    //    type: "POST",
    //    url: "/Account/WeeklyReportList",
    //    data: { id: 0, weekno: weekno },
    //    datatype: "text",
    //    beforeSend: function () {
    //        $('#WekklyDayReport').html(loaddingdiv);
    //    },
    //    success: function (data) {
    //        $('#WekklyDayReport').html(data);
    //    }
    //});
    $(location).attr('href', '/Account/WeeklyReport?weekno=' + weekno);
});
$(".btn-smsSendWR").click(function () {
    var id = $(".btn-info.disabled").data("id");
    var weekno = $(this).data("weekno");
    var url = "/Sms/SmsSender?smsType=" + $(this).data("smstype") + "&refid=" + id + "&date=&weekno=" + weekno;
    $(location).attr('href', url);
});
$('.ddlUserTypesWeekly').change(function () {
    var ut = this.value;
    var id = $('.ReportChanged.disabled').data("id");
    var weekno = $('.btn-smsSendWR').data("weekno");
    getUsersforWeeklyReport(id, ut, weekno);
});

$('#cbFrozenWeekly').change(function () {
    //var frozen = $(this).is(":checked");
    var id = $('.ReportChanged.disabled').data("id");
    var ut = $('.ddlUserTypesWeekly').val();
    var weekno = $('.btn-smsSendWR').data("weekno");
    getUsersforWeeklyReport(id, ut, weekno);
});

function getUsersforWeeklyReport(id, type, weekNo) {
    var frozen = $('#cbFrozenWeekly').prop('checked');
    $.ajax({
        type: "POST",
        url: "/Account/WeeklyReportList",
        data: {
            id: id, weekno: weekNo, ut: type, includeForzen: frozen
        },
        datatype: "text",
        beforeSend: function () {
            $('#WekklyDayReport').html(loaddingdiv);
        },
        success: function (data) {
            $('#WekklyDayReport').html(data);
        }
    });
};

$('#ClassesTable').on('click', '.btn-DailyParticipants', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var date = $(this).data('date');
    SetExportButton(date, null, false);
    $('#myModal .modal-title').html('רשומים לאימונים: ' + title);
    $('#myModal .modal-footer-inner').html('סה"כ מתאמנים רשומים: ' + footer);
    $.get('/studioclass/GetClassEnrollmentBydate?date=' + date.substring(0, 10), function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-DailyTrails', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var date = $(this).data('date');
    SetExportButton(date, 5, false);
    $('#myModal .modal-title').html('רשומים לאימונים (פוטנציאל): ' + title);
    $('#myModal .modal-footer-inner').html('סה"כ מתאמני פוטנציאל רשומים: ' + footer);
    $.get('/studioclass/GetClassEnrollmentBydate?date=' + date.substring(0, 10) + '&userrole=5&removeEmptyClasses=true', function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-DailyActivated', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var date = $(this).data('date');
    SetExportButton(date, null, true);
    $('#myModal .modal-title').html('רשומים לאימונים (חוזרים): ' + title);
    $('#myModal .modal-footer-inner').html('סה"כ מתאמנים חוזרים רשומים: ' + footer);
    $.get('/studioclass/GetActivatedEnrollmentBydate?date=' + date.substring(0, 10), function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-DailyComments', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var date = $(this).data('date');
    $('#myModal .modal-title').html('הערות: ' + title);
    $('#myModal .modal-footer-inner').html('' + footer);
    $.get('/studioclass/GetCommentsBydate?date=' + date.substring(0, 10), function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('#ClassesTable').on('click', '.btn-lateCancel', function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    var title = $(this).data('name');
    var footer = $(this).data('footer');
    var date = $(this).data('date');
    SetExportButton(date, null, false, true);
    $('#myModal .modal-title').html('ביטולים מאוחרים: ' + title);
    $('#myModal .modal-footer-inner').html('' + footer);
    $.get('/studioclass/GetLateCancelEnrollmentsBydate?date=' + date.substring(0, 10), function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

function SetExportButton(date, userRole, activated, isLateCancel) {
    //debugger;
    $('#btn-export-enrollments').data('date', date.substring(0, 10));
    $('#btn-export-enrollments').data('userRole', userRole);
    $('#btn-export-enrollments').data('activated', activated);
    $('#btn-export-enrollments').data('isLateCancel', isLateCancel);
    $('#btn-export-enrollments').show();
}

$('#btn-export-enrollments').click(function (ve) {
    //debugger;
    var date = $(this).data('date');
    var userRole = $(this).data('userRole');
    var activated = $(this).data('activated');
    var isLateCancel = $(this).data('isLateCancel');
    document.location.href = '/export/ExportEnrollmentBydate?date=' + date + '&userRole=' + userRole + '&activated=' + activated + '&isLateCancel=' + isLateCancel;
    ve.stopPropagation();
});

$('#btn-export-weekly').click(function (ve) {
    //debugger;
    var weekno = $(this).data('weekno');
    var id = $(this).data('id');
    var ut = $(".ddlUserTypesWeekly").val();
    var includeForzen = $('#cbFrozenWeekly').prop('checked');
    document.location.href = '/export/ExportWeeklyUserWithSubscription?id=' + id + '&weekno=' + weekno + '&ut=' + ut + '&includeForzen=' + includeForzen;
    ve.stopPropagation();
});

$('#myModal').on('hidden.bs.modal', function () {
    $('#btn-export-enrollments').hide();
    $('#myModal .modal-title').html('');
    $('#myModal .modal-footer-inner').html('');
});


$('.btn-WeeklyTrails').click(function (ve) {
    $('#myModal .modal-body').html(loaddingdiv);
    //var title = $(this).data('name');
    //var footer = $(this).data('footer');
    var week = $(this).data('weekno');
    $('#myModal .modal-title').html('רשומים לאימונים (פוטנציאל)');
    //$('#myModal .modal-title').html('רשומים לאימונים (פוטנציאל): ' + title);
    //$('#myModal .modal-footer-inner').html('סה"כ מתאמני פוטנציאל רשומים: ' + footer);
    $.get('/studioclass/GetClassEnrollmentByWeek?weekno=' + week + '&userrole=5&removeEmptyClasses=true', function (data) {
        $('#myModal .modal-body').html(data);
    });
    // $('#myModal .modal-body').load('/studioclass/GetClassEnrollmentBydate?date=' + date);
    //$('#myModal .modal-body').html('TEST 123');
    $('#myModal').modal('show', { backdrop: 'static' });
    ve.stopPropagation();
});

$('.btn_Unfreeze').click(function (ev) {
    var subscriptionId = $(this).data('id');
    $.ajax({
        type: "POST",
        url: "/Report/UpdateUnfreeze",
        data: { subscriptionId: subscriptionId },
        datatype: "text",
        beforeSend: function () {
            $('#FrozenReportList').html(loaddingdiv);
        },
        success: function (data) {
            $('#FrozenReportList').html(data);
        }
    });
    ev.stopPropagation();
});

$("#SMSSearchByDate").datepicker({
    //dateFormat: 'dd/mm/yy',
    onSelect: function (dateText) {
        $('#ReportClasses').html(loaddingdiv).load('/Report/GetSMSList?date=' + this.value);
    }
});

//user management

$("#btn_updateuser").click(function () {
    var isValid = $('#UserUpdate').validate().form();
    if (!isValid) return false;
    $.ajax({
        type: "POST",
        url: "/Account/UpdateUser",
        data: $('#UserUpdate').serialize(),
        datatype: "html",
        success: function (data) {
            $('#myModal .modal-title').html('ניהול משתמשים');
            $('#myModal .modal-body').html(data.Message);
            $('#myModal').modal('show', { backdrop: 'static' });
        }
    });
});

$("#btn_sendSMS").click(function () {
    var userId = $(this).data("userid");
    $.ajax({
        type: "POST",
        url: "/Account/SendWelcomeSMS",
        data: { userId: userId },
        datatype: "html",
        success: function (data) {
            $('#myModal .modal-title').html('ניהול משתמשים');
            $('#myModal .modal-body').html(data.Message);
            $('#myModal').modal('show', { backdrop: 'static' });
        }
    });
});

$("#btn_ChangePass").click(function () {
    $('#ResetPassModal').modal({ show: true });
});

$("#btn_ShowHealthTandC").click(function () {
    $('#HealthTandCPrintModal').modal({ show: true });
});

$('body').on('click', '#btn_updateprofile', function (ve) {
    var isValid = $('#UserProfile').validate().form();
    if (!isValid) return false;
    $.ajax({
        type: "POST",
        url: "/Account/UpdateProfile",
        data: $('#UserProfile').serialize(),
        datatype: "html",
        success: function (data) {
            $('#myModal .modal-title').html('ניהול משתמשים');
            $('#myModal .modal-body').html(data.Message);
            $('#myModal').modal('show', { backdrop: 'static' });
        }
    });
});

$("#btn_subscriptiondetails").click(function () {
    var id = $(this).data("id");
    $('#myTabbedModal #subdetails-tab').load("/Account/GetSubsicriptionDetails?subscriptionId=" + id, function (response, status, xhr) {
        if (status != "success") {
            $('#myTabbedModal #subdetails-tab').html('תקלה, אנא נסה מאוחר יותר' + response);
        }
    });
    $('#myTabbedModal #subfrozen-tab').load("/Account/GetFrozenSubsicriptionDetails?subscriptionId=" + id, function (response, status, xhr) {
        if (status != "success") {
            $('#myTabbedModal #subfrozen-tab').html('תקלה, אנא נסה מאוחר יותר' + response);
        }
    });
    $('#myTabbedModal').modal('show', { backdrop: 'static' });
});

//$(".btn-ProcessMarked").click(function () {
//    var id = $(this).data('compid');
//    $.ajax({
//        type: "POST",
//        url: "/Admin/ProcessMarked",//?subscriptionId=" + subid + "&userId=" + userid + "&toDate=" + todate,
//        data: { },
//        datatype: "text",
//        success: function (data) {
//            $('#dataConfirmModal').modal('hide');
//            $('.modal-backdrop').remove();
//            $('body').removeClass('modal-open');
//        }
//    });
//});

//$("#btn_subscriptionfreeze").click(function () {
//    $('#freezeModal').modal('show', { backdrop: 'static' });
//});

//$("#btn_updatesubscription").click(function () {
//    var isValid = $('#UserSubscription').validate().form();
//    if (!isValid) return false;
//    $.ajax({
//        type: "POST",
//        url: "/Account/UpdateSubscription",
//        data: $('#UserSubscription').serialize(),
//        datatype: "html",
//        success: function (data) {
//            if ((data.Response) == 'Error') {
//                $('#myModal .modal-title').html('ניהול משתמשים');
//                $('#myModal .modal-body').html(data.Message);
//                $('#myModal').modal('show', { backdrop: 'static' });
//            } else {
//                $('#subsription-tab').html(data);
//            }
//            //$('#subsription-tab').load('/Account/ManageSubscriptions');
//        }
//    });
//});

//$("#btn_Updatesubscriptiondexpire").click(function () {
//    var id = $(this).data("id");
//    var date = $('#DateExpire').val();
//    $.ajax({
//        type: "POST",
//        url: "/Account/UpdateSubscriptionExpire",
//        data: { subscriptionId: id, newExpireDate: date },
//        success: function (data) {
//            if ((data.Response) == 'Error') {
//                $('#myModal .modal-title').html('ניהול משתמשים');
//                $('#myModal .modal-body').html(data.Message);
//                $('#myModal').modal('show', { backdrop: 'static' });
//            } else {
//                $('.expiredate').html(date);
//                $('.expiredate').show();
//                $('.editexpiredate').hide();
//            }
//            //$('#subsription-tab').load('/Account/ManageSubscriptions');
//        }
//    });
//});

function getUserDetails(userId) {
    $.when($.ajax({
        type: "POST",
        url: "/Account/ManageProfile",
        data: {
            userId: userId
        },
        datatype: "text",
        beforeSend: function () {
            $('#profile-tab').html(loaddingdiv);
        },
        success: function (data) {
            $('#profile-tab').html(data);
        }
    }),
        //$.ajax({
        //    type: "POST",
        //    url: "/Account/ManageSubscriptions",
        //    data: {
        //        userId: userId
        //    },
        //    datatype: "text",
        //    beforeSend: function () {
        //        $('#subsription-tab').html(loaddingdiv);
        //    },
        //    success: function (data) {
        //        $('#subsription-tab').html(data);
        //    }
        //}),
        $.ajax({
            type: "POST",
            url: "/StudioClass/ManageUserEnrollment",
            data: {
                userId: userId
            },
            datatype: "text",
            beforeSend: function () {
                $('#enrollmets-tab').html(loaddingdiv);
            },
            success: function (data) {
                $('#enrollmets-tab').html(data);
            }
        }),
        $.ajax({
            type: "POST",
            url: "/StudioClass/ManageUserOldEnrollment",
            data: {
                userId: userId
            },
            datatype: "text",
            beforeSend: function () {
                $('#oldenrollmets-tab').html(loaddingdiv);
            },
            success: function (data) {
                $('#oldenrollmets-tab').html(data);
            }
        })
    );
}


