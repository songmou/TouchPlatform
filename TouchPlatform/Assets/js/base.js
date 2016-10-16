var domain = "http://" + document.domain;
var domain_usb = "http://localhost:8080";

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


    $('.menu-block').load(domain + '/Master/Menu');
    $('.header-block').load(domain + '/Master/Top');

    //var htmlMenu = $.ajax({
    //    url: domain + '/Master/Menu',
    //    async: false
    //}).responseText;
    //$('.menu-block').html(htmlMenu);

    //var htmlHeader = $.ajax({
    //    url: domain + '/Master/Top',
    //    async: false
    //}).responseText;
    //$('.header-block').html(htmlHeader);
});


//公共方法 获取地址参数
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

//公共方法 绑定事件
function bindCheckbox() {
    $('.inbox-check').click(function () {
        var activeRow = $(this).parent().parent().parent();

        activeRow.toggleClass('active');
    });

    $('#inboxCollapse').click(function () {
        $('.inbox-menu-inner').slideToggle();
    });

    $('#chkAll').click(function () {
        if ($(this).prop('checked')) {
            $('.inbox-check').prop('checked', true);
            $('.inbox-check').parent().parent().parent().addClass('active');
        }
        else {
            $('.inbox-check').prop('checked', false);
            $('.inbox-check').parent().parent().parent().removeClass('active');
        }
    });
}

//扩展方法 格式化
$.format = function (source, params) {
    if (arguments.length == 1)
        return function () {
            var args = $.makeArray(arguments);
            args.unshift(source);
            return $.format.apply(this, args);
        };
    if (arguments.length > 2 && params.constructor != Array) {
        params = $.makeArray(arguments).slice(1);
    }
    if (params.constructor != Array) {
        params = [params];
    }
    $.each(params, function (i, n) {
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
    });
    return source;
};

(function ($) {

    var PagerPanel = function (that, options) {
        this.opts = $.extend({
            index: 1,
            size: 10,
            total: 0,
            pageCount: 1,
            callback: function () { },
            selector: "pagination",
            tags: "li",
        }, options);
        this.opts.pageCount = Math.floor(this.opts.total / this.opts.size) +
                ((this.opts.total != 0 && this.opts.total % this.opts.size == 0) ? 0 : 1);

        this.$el = $(that);

        this.init();
        this.bindclick();


        return this.opts;
    }

    PagerPanel.prototype = {
        init: function () {
            var opts = this.opts;

            var html = "";
            html += $.format('<ul class="{0}">', opts.selector);
            html += $.format('<{0}><a href="javascript:;" data-page="1">«</a></{0}>',
                opts.tags);

            for (i = opts.index - 3; i <= opts.index + 3; i++) {
                if (i > 0 && i <= opts.pageCount) {
                    html += $.format('<{0} class="{2}"><a href="javascript:;" data-page="{1}">{1}</a></{0}>',
                        opts.tags, i,
                        opts.index == i ? "active" : "");
                }
            }
            html += $.format('<{0}><a href="javascript:;" data-page="{1}">»</a></{0}>',
                opts.tags, opts.pageCount)
            html += '</ul>';

            html += $.format('<span class="pager-tip pull-right">{0}/{1} 页　{2} 条数据　</span>',
                opts.index, opts.pageCount, opts.total);

            this.$el.html(html);
        },
        bindclick: function () {
            var opts = this.opts;
            this.$el.find("ul li a[data-page]").click(function () {
                var current = $(this).data("page");
                if (opts.callback) opts.callback(current);
            });
        }
    };

    $.fn.PagerPanel = function (options, callback) {
        var plugin = new PagerPanel(this, options, callback);

    }

})(jQuery);

