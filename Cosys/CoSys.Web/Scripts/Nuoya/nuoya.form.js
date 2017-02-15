(function ($) {
    $.Nuoya.form = function (formId) {
        var $form = $("#" + formId);


        //验证绑定
        var _validate = function (options) {
            //todo:自行修改消息容器
            var defaults = {
                errorPlacement: function (error, element) {
                    var errorText = $.trim($(error).text());
                    if (errorText != "") {
                        $(element).data("tipso", errorText);
                        $(element).tipso({
                            useTitle: false,
                            isMouseover: false,
                            onBeforeShow: function () {
                                var left = $(element).width() - $(element).data("plugin_tipso").tipso_bubble.width();
                                $(element).data("plugin_tipso").tipso_bubble.css("left", left + "px");
                            },
                            onHide: function () {
                                $(element).tipso('destroy');
                            }
                        });
                        $(element).tipso('show');
                    }
                },
                success: function (errorElement, element) {
                    if ($(element).data("plugin_tipso") != null) {
                        $(element).tipso('hide');
                    }
                },
                errorElement: "span",

            };
            options = $.extend(true, defaults, options);
            $form.validate(options);
        }



        //表单有效验证
        var _valid = function () {
            return $form.valid();
        }

        //表单有效验证
        var _focusInvalid = function (callback) {
            $form.data("validator").focusInvalid(callback);
        }

        //表单序列化
        var _serialize = function () {
            var map = {};
            $form.find("input[name]:not(input[type='file']),select[name],textarea[name]").each(function (index, item) {
                var $item = $(item);
                var name = $item.attr("name");
                var value = $item.val();
                if ($item.is("input[type='radio'],input[type='checkbox']")) {
                    if ($item.prop("checked")) {
                        if (map[name]) {
                            map[name] += "," + value;
                        } else {
                            map[name] = value;
                        }
                    }
                } else {
                    map[name] = value;
                }

            });
            return map;
        }

        var _fill = function (data) {
            $form.find("*[name]").each(function (index, item) {
                var $item = $(item);
                var datatype = $(item).attr("datatype");
                var dateformat = $(item).attr("dateformat");
                var value = "";
                try {
                    value = eval('data.' + $item.attr("name"));
                    if (datatype == "jsondate") {
                        value = $.Nuoya.jsonFormatDate(value, dateformat);
                    }
                    else if (datatype == "jsondateNo") {
                        value = $.Nuoya.jsonFormatDateNo(value, dateformat);
                    }
                } catch (e) {
                }
                if ($item.is("input") || $item.is("select") || $item.is("textarea")) {
                    switch ($item.attr("type")) {
                        case "radio":
                            $item.prop("checked", $item.val() == value);
                            break;
                        case "checkbox":
                            if ($.isArray(value)) {
                                var flag = false;
                                for (var i = 0; i < value.length; i++) {
                                    if ($item.val() == value[i]) {
                                        flag = true;
                                        break;
                                    }
                                }
                                $item.prop("checked", flag);
                            } else {
                                if ($item.val() == value) {
                                    $item.prop("checked", true);
                                }
                            }
                            break;
                        case "file": break;
                        default:
                            $item.val(value);
                    }

                } else {
                    $item.html(value);
                }

            });
        }

        var _dataLoad = function (options) {
            var defaults = {
                ajaxUrl: null,//数据请求地址
                params: null,//请求附带参数
                data: null,//当data!=null时ajaxUrl无效
                callback: null//提交完成后回调
            };
            options = $.extend(true, defaults, options);
            if (options.data != null) {
                _fill(options.data);
                $.Nuoya.callFunction(options.callback, options.data);
            } else {
                $.Nuoya.action(options.ajaxUrl, options.params, function (data) {
                    _fill(data);
                    $.Nuoya.callFunction(options.callback, data);
                })

            }
        }


        //表单提交
        var _submit = function (options) {
            var defaults = {
                ajaxUrl: null,//数据请求地址
                params: _serialize(),//请求附带参数                
                isValid: true,//是否验证有效性,在验证前需要绑定验证事件
                callback: null,//提交完成后回调
                beforeSubmit: null,
                afterValid: null
            };
            options = $.extend(true, defaults, options);

            if ((options.isValid && _valid()) || !options.isValid) {
                if (options.beforeSubmit === null || $.Nuoya.callFunction(options.beforeSubmit, options.params))
                    $.Nuoya.action(options.ajaxUrl, options.params, function (json) {
                        $.Nuoya.callFunction(options.callback, json);
                    });
            }
            else {
                _focusInvalid(options.afterValid);
            }
        };

        var _ajaxSubmit = function (options) {
            var defaults = {
                ajaxUrl: null,//数据请求地址
                params: _serialize(),//请求附带参数                
                isValid: true,//是否验证有效性,在验证前需要绑定验证事件
                callback: null,//提交完成后回调
                beforeSubmit: null,
                afterValid:null
            };
            options = $.extend(true, defaults, options);
            if ((options.isValid && _valid()) || !options.isValid) {
                if (options.beforeSubmit === null || $.Nuoya.callFunction(options.beforeSubmit, options.params)) {
                    $form.ajaxForm();
                    $form.ajaxSubmit({
                        data: options.params,
                        url: options.ajaxUrl,
                        type: "post",
                        datatype: "json",
                        success: function (json) {
                            if (json.Code == -100) {
                                $.Nuoya.alert("你没有该权限");
                                return false;
                            }
                            $.Nuoya.callFunction(options.callback, json);
                        }
                    });
                }
            }
            else {
                _focusInvalid(options.afterValid);
            }
        };
        var _init = function () {
            return {
                serialize: _serialize,//表单序列化
                validate: _validate,//开启验证
                valid: _valid,//验证返回结果
                submit: _submit,//提交
                ajaxSubmit: _ajaxSubmit,//ajax提交
                dataLoad: _dataLoad//数据加载
            };
        }
        return _init();
    }

    $.Nuoya.form.validator = {
        defaults: {
            onfocusout: function (element) {
                //if (!this.checkable(element) && (element.name in this.submitted || !this.optional(element))) {
                if (!this.checkable(element)) {
                    this.element(element);
                }
            },
            onkeyup: function (element, event) {
                // Avoid revalidate the field when pressing one of the following keys
                // Shift       => 16
                // Ctrl        => 17
                // Alt         => 18
                // Caps lock   => 20
                // End         => 35
                // Home        => 36
                // Left arrow  => 37
                // Up arrow    => 38
                // Right arrow => 39
                // Down arrow  => 40
                // Insert      => 45
                // Num lock    => 144
                // AltGr key   => 225
                var excludedKeys = [
                    16, 17, 18, 20, 35, 36, 37,
                    38, 39, 40, 45, 144, 225
                ];

                if (event.which === 9 && this.elementValue(element) === "" || $.inArray(event.keyCode, excludedKeys) !== -1) {
                    return;
                }
                this.element(element);
            }
        },
        messages: {
            required: "必选字段",
            remote: "请修正该字段",
            email: "请输入正确格式的电子邮件",
            url: "请输入合法的网址",
            date: "请输入合法的日期",
            dateISO: "请输入合法的日期 (ISO).",
            number: "请输入合法的数字",
            digits: "只能输入整数",
            creditcard: "请输入合法的信用卡号",
            equalTo: "请再次输入相同的值",
            accept: "请输入拥有合法后缀名的字符串",
            maxlength: jQuery.validator.format("请输入一个 长度最多是 {0} 的字符串"),
            minlength: jQuery.validator.format("请输入一个 长度最少是 {0} 的字符串"),
            rangelength: jQuery.validator.format("请输入 一个长度介于 {0} 和 {1} 之间的字符串"),
            range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
            max: jQuery.validator.format("请输入一个最大为{0} 的值"),
            min: jQuery.validator.format("请输入一个最小为{0} 的值"),
            //------------------------------------------------------------以上为jqeury.validate中内置验证------------------------------------------------------------
            mobile: "请输入正确的手机号码.",
            phone: "请输入正确的电话号码.",
            phoneMobile: "请输入正确的手机号码或电话号码.",
            qq: "请输入正确的qq号码.",
            decimalPositiveInteger: "请输入带小数的正整数.",
            noSpecialCaracters: "不允许包含特殊的字符.",
            onlyLetter: "只允许输入a-z的字符.",
            positiveInteger: "请输入正整数.",
            chinese: "请输入中文.",
            carsNumber: "请输入正确的车牌号.",
            idCard: "请输入正确的身份证号码.",
            ip: "请填写正确的IP地址.",
            idCardLength: "身份证号码长度错误.",
            idCardArea: "身份证号码地区编码错误.",
            idCardBirth: "身份证号码生日错误.",
            idCardCheckCode: "身份证号码校验位错误."
        },
        addMethod: function (name, method) {
            $.validator.addMethod(name, method, this.messages[name]);
        },
        addClassRules: function (name, rules) {
            $.validator.addClassRules(name, rules);
        },
        bindClassRules: function () {
            jQuery.validator.addClassRules("idCard", {
                required: true,
                idCardLength: true,
                idCardArea: true,
                idCardBirth: true,
                idCardCheckCode: true
            });
        },
        bindMethod: function () {
            this.addMethod("mobile", function (value, element) {
                return this.optional(element) || (/(13[0-9]{9})|(14[0-9]{9})|(15[0-9]{9})|(17[0-9]{9})|(18[0-9]{9})/.test(value));
            });
            this.addMethod("phone", function (value, element) {
                return this.optional(element) || (/^\d{3,4}-\d{7,8}(-\d{3,4})?$/.test(value));
            });
            this.addMethod("phoneMobile", function (value, element) {
                return this.optional(element) || (/(^\d{3,4}-\d{7,8}(-\d{3,4})?$)|(^(1[0-9]{10})$)|(^\d{10,12}$)|(^\d{3,4} \d{7,8}$)/.test(value));
            });
            this.addMethod("qq", function (value, element) {
                return this.optional(element) || (/^[0-9]+$/.test(value));
            });
            this.addMethod("decimal", function (value, element) {
                return this.optional(element) || (/^[0-9]+\.?[0-9]*$/.test(value));
            });
            this.addMethod("noSpecialCaracters", function (value, element) {
                return this.optional(element) || (/^[0-9a-zA-Z]+$/.test(value));
            });
            this.addMethod("onlyLetter", function (value, element) {
                return this.optional(element) || (/^[a-zA-Z\ \']+$/.test(value));
            });
            this.addMethod("positiveInteger", function (value, element) {
                return this.optional(element) || (/^[1-9]\d*$/.test(value));
            });
            this.addMethod("chinese", function (value, element) {
                return this.optional(element) || (/^[\u4e00-\u9fa5]*$/.test(value));
            });
            this.addMethod("carsNumber", function (value, element) {
                return this.optional(element) || (/^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{5}$/.test(value));
            });
            this.addMethod("ip", function (value, element) {
                return this.optional(element) || /^(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.)(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.){2}([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))$/.test(value);
            });
            this.addMethod("idCardLength", function (value, element) {
                var len = value.length;
                return this.optional(element) || (len == 15 || len == 18);
            });
            this.addMethod("idCardArea", function (value, element) {
                var area = {
                    11: "北京", 12: "天津", 13: "河北", 14: "山西",
                    15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江", 31: "上海",
                    32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西",
                    37: "山东", 41: "河南", 42: "湖北", 43: "湖南", 44: "广东",
                    45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州",
                    53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃", 63: "青海",
                    64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门",
                    91: "国外"
                }
                return this.optional(element) || area[parseInt(value.substr(0, 2))] != null;
            });

            this.addMethod("idCardBirth", function (value, element) {
                var len = value.length;
                if (len == 15) {
                    re = new RegExp(/^(\d{6})()?(\d{2})(\d{2})(\d{2})(\d{3})$/);
                } else {
                    re = new RegExp(/^(\d{6})()?(\d{4})(\d{2})(\d{2})(\d{3})([0-9xX])$/);
                }
                var a = value.match(re);
                var flag = false;
                if (a != null) {
                    if (len == 15) {
                        var DD = new Date("19" + a[3] + "/" + a[4] + "/" + a[5]);
                        flag = DD.getYear() == a[3] && (DD.getMonth() + 1) == a[4] && DD.getDate() == a[5];
                    }
                    else if (len == 18) {
                        var DD = new Date(a[3] + "/" + a[4] + "/" + a[5]);
                        flag = DD.getFullYear() == a[3] && (DD.getMonth() + 1) == a[4] && DD.getDate() == a[5];
                    }
                }
                return this.optional(element) || flag;
            });
            this.addMethod("idCardCheckCode", function (value, element) {
                var flag = false;
                var changeFivteenToEighteen = function (card) {
                    if (card.length == '15') {
                        var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
                        var arrCh = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
                        var cardTemp = 0, i;
                        card = card.substr(0, 6) + '19' + card.substr(6, card.length - 6);
                        for (i = 0; i < 17; i++) {
                            cardTemp += card.substr(i, 1) * arrInt[i];
                        }
                        card += arrCh[cardTemp % 11];
                        return card;
                    }
                    return card;
                };
                value = changeFivteenToEighteen(value);
                var len = value.length;
                if (len == 18) {
                    var idcard_array = value.split("");
                    S = (parseInt(idcard_array[0]) + parseInt(idcard_array[10])) * 7
                       + (parseInt(idcard_array[1]) + parseInt(idcard_array[11])) * 9
                       + (parseInt(idcard_array[2]) + parseInt(idcard_array[12])) * 10
                       + (parseInt(idcard_array[3]) + parseInt(idcard_array[13])) * 5
                       + (parseInt(idcard_array[4]) + parseInt(idcard_array[14])) * 8
                       + (parseInt(idcard_array[5]) + parseInt(idcard_array[15])) * 4
                       + (parseInt(idcard_array[6]) + parseInt(idcard_array[16])) * 2
                       + parseInt(idcard_array[7]) * 1
                       + parseInt(idcard_array[8]) * 6
                       + parseInt(idcard_array[9]) * 3;

                    Y = S % 11;
                    M = "F";
                    JYM = "10X98765432";
                    M = JYM.substr(Y, 1); //判断校验位
                    //检测ID的校验位
                    if (M == idcard_array[17]) {
                        flag = true;
                    }
                }
                return this.optional(element) || flag;
            });
        },
        init: function () {
            $.extend($.validator.messages, this.messages);
            $.extend($.validator.defaults, this.defaults);
            this.bindMethod();
            this.bindClassRules();
        }
    }
    $.Nuoya.form.validator.init();
})(jQuery);