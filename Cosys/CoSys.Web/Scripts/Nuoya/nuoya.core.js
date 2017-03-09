(function ($) {
    var dialogTemplates = {
        dialog:
            '<div class="am-modal am-modal-alert" tabindex="-1">' +
                '<div class="am-modal-dialog">' +
                    '<div class="am-modal-bd">' +
                    '</div>' +
                '</div>' +
            '</div>',
        header: '<div class="am-modal-hd"></div>',
        footer: '<div class="am-modal-footer"></div>',
        closeButton: '<a href="javascript: void(0)" class="am-close am-close-spin" data-am-modal-close>&times;</a>'
    };
    var dataCache = {};  //palmv数据缓存对象
    $.Nuoya = {
        //数据请求
        action: function (url, param, callback, errorCallback, async) {
            jQuery.ajax({
                async: async,
                url: url + "?_token=" + $.Nuoya.getURLParam("_token"),
                type: "post",
                data: param,
                dataType: "json",
                success: function (json) {
                    var isSuccess;
                    if (json.Code == 0) {
                        isSuccess = true;
                    }
                    else {
                        isSuccess = false;
                    }

                    var work = function () {
                        if (json.Redirect) {
                            window.location.href = json.Redirect;
                        }
                        else {
                            if (isSuccess) {
                                if (callback)
                                    $.Nuoya.callFunction(callback, json.Result);
                            }
                            else {
                                $.Nuoya.alert(json.ErrorDesc);
                            }
                        }
                    }
                    if (json.Message) {
                        $.Nuoya.alert(json.Message, function () {
                            work();
                        });
                    }
                    else {
                        work();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                complete: function (XMLHttpRequest, textStatus) {
                }
            });
        },
        ajaxGet: function (url, param, callback) {
            $.get(url, param, function (data) {
                $.Nuoya.callFunction(callback, data);
            });
        },
        ajaxGetCache: function (url, param, callback) {
            var value = $.Nuoya.cacheFactory.get(url);
            if (value == null) {
                $.Nuoya.ajaxGet(url, param, function (data) {
                    $.Nuoya.cacheFactory.add(url, data);
                    $.Nuoya.callFunction(callback, data);
                });
            } else {
                $.Nuoya.callFunction(callback, value);
            }
        },
        deleteAction: function (url, param, callback) {
            $.Nuoya.confirm("是否确认删除,删除后将无法恢复？", function () {
                $.Nuoya.action(url, param, callback);
            })
        },
        alertAction: function (message, url, param, callback) {
            $.Nuoya.confirm(message, function () {
                $.Nuoya.action(url, param, callback);
            })
        },
        //数据缓存
        cacheFactory: {
            add: function (key, value) {
                dataCache[key] = value;
            },
            get: function (key) {
                return dataCache[key];
            }
        },
        //执行方法
        callFunction: function (arg, param, param2, param3) {
            if ($.isFunction(arg)) {
                return arg.call(undefined, param, param2, param3)
            }
        },
        //普通消息
        alert: function (message, callback) {
            $.Nuoya.dialog({
                message: message,
                btnExit: false,
                afterClose: function () {
                    $.Nuoya.callFunction(callback);
                },
                buttons: [{
                    label: "确定",
                    callback: function (e) {
                        e.hide();
                        $.Nuoya.callFunction(callback);
                    }
                }]
            });
        },
        //确认消息
        confirm: function (message, callback, cancelCallback) {
            $.Nuoya.dialog({
                message: message,
                btnExit: false,
                afterClose: function () {
                    $.Nuoya.callFunction(cancelCallback);
                },
                buttons: [{
                    className: "am-btn-primary",
                    label: "确定",
                    callback: function (e) {
                        e.hide();
                        $.Nuoya.callFunction(callback);
                    }
                }, {
                    className: "am-btn-danger",
                    label: "取消",
                    callback: function (e) {
                        e.hide();
                        $.Nuoya.callFunction(cancelCallback);
                    }
                }]
            });
        },
        ajaxDialog: function (options) {
            if (options.addIframe == undefined) {
                $.Nuoya.ajaxGetCache(options.ajaxUrl, null, function (html) {
                    options.message = html;
                    options.ajaxUrl = null;
                    var target = $.Nuoya.dialog(options);
                    $.Nuoya.callFunction(options.callback, target);
                });
            }
            else {
                var target = $.Nuoya.dialog(options);
                $.Nuoya.callFunction(options.callback, target);
            }
        },
        dialog: function (options) {
            var defaults = {
                title: null,
                message: null,
                buttons: [],
                btnExit: true,//取消按钮
                closeButton: true,//右上角关闭按钮
                container: "body",//父级容器
                afterClose: null,//关闭事件
                addIframe: false,
                closeOnConfirm: false,
                isForPhone:false
            };
            options = $.extend(true, defaults, options);

            var dialog = $(dialogTemplates.dialog);
         
            var innerDialog = dialog.find("am-modal-dialog");
            var body = dialog.find(".am-modal-bd");
            var buttons = options.buttons;
            var target = {
                dialog: dialog,
                hide: function () {
                    this.dialog.modal("close");
                }
            };

            if (!options.addIframe) {
                body.html(options.message);
            }
            else {
                body.html("<iframe id='alrtIframe' src='" + options.ajaxUrl + "' width='" + options.width + "' height='" + options.height + "' frameborder='no'  border='0'></iframe>");
            }

            if (options.title) {
                var header = $(dialogTemplates.header).append(options.title);
                body.before(header);
            }

            if (options.closeButton) {
                var closeButton = $(dialogTemplates.closeButton);
                if (options.title) {
                    dialog.find(".am-modal-hd").append(closeButton);
                } else {
                    dialog.find(".am-modal-bd").append(closeButton);
                }
            }

            if (options.btnExit) {
                buttons.push({ "label": "取消", "className": "am-btn-danger", "attrs": "data-am-modal-close", "callback": function (e) { e.hide(); } });
            }

            if (buttons != null) {
                var footer = $(dialogTemplates.footer);
                body.after(footer);
                $.each(buttons, function (index, button) {
                    var defaultButton = { className: "am-btn-primary", "attrs": "data-am-modal-confirm", label: null, callback: null, };
                    button = $.extend(true, defaultButton, button);
                    var btnElem = $('<span class="am-modal-btn ' + button.className + '" ' + button.attrs + '>' + button.label + '</span>');
                    $(btnElem).off('click.close.modal.amui');
                    btnElem.bind("click", function () {
                        $.Nuoya.callFunction(button.callback, target);
                    })
                    footer.append(btnElem);
                });
            }

            //隐藏后
            dialog.on("closed.modal.amui", function (e) {
                if (e.target === this) {
                    dialog.remove();
                }
            });
            //显示后
            dialog.on("opened.modal.amui", function () {
                dialog.find(".am-btn-primary:first").focus();
            });

            dialog.on("close.modal.amui", function (e) {
                $.Nuoya.callFunction(options.afterClose, target);
            });

            $(options.container).append(dialog);

            dialog.modal(options);
            return target;
        },
        //获取URl头部参数
        getURLParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) {
                return unescape(r[2]);
            }
            return "";
        },
        //由json序列化出来的时间格式进行转换
        jsonFormatDate: function (jsondate, format) {
            if (jsondate != null && jsondate != "") {
                format = format != null ? format : "yyyy-M-d hh:mm:ss";
                var date = new Date(parseInt(jsondate.replace("/Date(", "").replace(")/", ""), 10));
                var value = date.format(format);
                value = value.replace(" 00:00:00", "");
                return value;
            } else {
                return "";
            }
        },
        //由json序列化出来的时间格式进行转换
        jsonFormatDateNo: function (jsondate, format) {
            if (jsondate != null && jsondate != "") {
                format = format != null ? format : "yyyy-M-d hh:mm:ss";
                var date = new Date(parseInt(jsondate.replace("/Date(", "").replace(")/", ""), 10));
                var value = date.format(format);
                value = value.split(' ')[0].trim();
                return value;
            } else {
                return "";
            }
        },
        //设置字符串长度
        setStrLength: function (value, length) {
            if ($.Nuoya.getStrLength(value) > length) {
                var strlen = 0;
                var s = "";
                for (var i = 0; i < value.length; i++) {
                    if (value.charCodeAt(i) > 128) {
                        strlen += 2;
                    } else {
                        strlen++;
                    }
                    s += value.charAt(i);
                    if (strlen >= length) {
                        break;
                    }
                }
                return s + "...";
            }
            return value;
        },
        //获取字符长度
        getStrLength: function (str) {
            if (str != null) {
                var cArr = str.match(/[^\x00-\xff]/ig);
                return str.length + (cArr == null ? 0 : cArr.length);
            } else {
                return 0;
            }
        },
        resetModalPosition: function (modal) {
            $(modal).css("marginLeft", "-" + ($(modal).width() / 2) + "px");
            $(modal).css("marginTop", "-" + ($(modal).height() / 2) + "px");
        },
        setOperateHideByClass: function () {
            this.action("/Operate/GetNeedHideClass", { pageUrl: window.location.pathname }, function (list) {
                for (var i = 0; i < list.length; i++) {
                    var c = list[i];
                    $("." + c).hide();
                }
            });
        }
    }
})(jQuery);

