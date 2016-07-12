//var domain = "http://192.168.0.50:8092";
var domain = "http://localhost";

$(function () {
    //var html = '<div class="col-xs-6 col-xs-offset-3 text-left-sm">';
    //html += '<div class="input-group">'
    //html += '    <textarea rows="3"class="form-control code-doing"></textarea>'
    //html += '    <div class="input-group-btn">'
    //html += '        <button type="button" onclick="eval($(\'.code-doing\').val())" style="height:120%;" class="btn-doing btn btn-success no-shadow">'
    //html += '            <i class="fa fa-play" aria-hidden="true"></i> 执行'
    //html += '        </button>'
    //html += '    </div>'
    //html += '</div>'
    //html += '</div>';
    //$('body').append(html);


    //$('.menu-block').load(domain + '/Master/Menu');
    //$('.header-block').load(domain + '/Master/Top');
    var htmlMenu = $.ajax({
        url: domain + '/Master/Menu',
        async: false
    }).responseText;
    $('.menu-block').html(htmlMenu);

    var htmlHeader = $.ajax({
        url: domain + '/Master/Top',
        async: false
    }).responseText;
    $('.header-block').html(htmlHeader);
})


var queryString = function (query) {
    var search = window.location.search + '';
    if (search.charAt(0) != '?') {
        return undefined;
    }
    else {
        search = search.replace('?', '').split('&');
        for (var i = 0; i < search.length; i++) {
            if (search[i].split('=')[0] == query) {
                return decodeURI(search[i].split('=')[1]);
            }
        }
        return undefined;
    }
};