(function ($) {
    $.Nuoya.grid = function (options) {
        var defaults = {
            tableId: null,//表格id
            isHide: false,//表格id
            ajaxUrl: null,//数据请求地址
            search: null,//{ domainId: "搜索域", subId: "提交按钮" } 
            params: null,//每次请求必带的参数
            pageSize: 20,//每页显示条数
            pageIndex: 1,//默认页
            events: [],//事件
            callback: null,//数据加载完成后回调函数
            rowCallback: null,//表格行绘制的回调函数                
            drawCallback: null,//绘制完成回调
            isForPhone:false, //是否手机
        }

        var colspanTab = 0;//隐藏的字段td长度
        //权限数据
        var userOperateUrlArray = new Array();
        var options = $.extend(true, defaults, options);
        var tableObj = $("#" + options.tableId);
        var searchObj = options.search != null ? $("#" + options.search.domainId) : null;//搜索区域
        var searchSubObj = searchObj != null && options.search.subId != null ? $("#" + options.search.subId) : null;//搜索事件
        var opreateUrls = [];
        var columns = [];
        var lengthMenu = [];//菜单显示个数
        for (var i = 1; i <= 5; i++) {
            lengthMenu.push(i * options.pageSize);
        }

        //头部转换
        var _convertHead = function (column, td) {
            //全选
            if (column.type == "checkbox") {
                var checkAll = $("<input type='checkbox' headfieldname='" + column.dataname + "' />");
                checkAll.bind("click", function () {
                    var check = $(this);
                    tableObj.find("tr td[fieldname='" + check.attr("headfieldname") + "'] input[type='checkbox']").prop("checked", check.prop("checked"));
                });
                td.html(checkAll);
            }
            //排序
        }

        //事件转换
        var _convertEvent = function (item) {
            var eventList = $("<div class='eventList am-btn-group am-btn-group-xs'></div>");
            if ($.isArray(options.events)) {
                $.each(options.events, function (index, event) {
                    var eventSetting = { name: "", icon: "", className: "", click: null, formula: null, url: "" };
                    event = $.extend(true, eventSetting, event);
                    if (event.url == "" || userOperateUrlArray == null || (userOperateUrlArray != null && userOperateUrlArray.indexOf(event.url) > -1)) {
                        var operate = $('<button class="am-btn am-btn-default am-btn-xs ' + event.className + '"><span class="am-' + event.icon + '"></span> ' + event.name + '</button>');
                        operate.bind("click", function () {
                            $.Nuoya.callFunction(event.click, item);
                        })

                        if (event.formula != null) {
                            var eventBool = $.Nuoya.callFunction(event.formula, item);
                            if (eventBool) {
                                eventList.append(operate);
                            }
                        } else {
                            eventList.append(operate);
                        }
                    }
                });
            }
            return eventList;

        }

        var isHaveOperate = function (url) {

        }

        //获取字段
        var _evalColumnValue = function (dataname, item) {
            var columnValue = null;
            try {
                columnValue = eval("item." + dataname); //获取json某一个列的值
            } catch (e) {
                columnValue = "";
            }
            return columnValue;
        }

        //根据字段属性和过滤条件进行转换
        var _convertValue = function (column, columnValue) {
            if (column.dataType == "jsondate") {
                columnValue = $.Nuoya.jsonFormatDate(columnValue, column.dateformat);
            }
            if (column.dataType == "jsondateNo") {
                columnValue = $.Nuoya.jsonFormatDateNo(columnValue, column.dateformat);
            }
            if (!$.isEmptyObject(column.strLength)) {
                columnValue = $.Nuoya.setStrLength(columnValue, column.strLength); //设置字符长度                
            }
            if (!$.isEmptyObject(column.suffix)) {
                columnValue = columnValue + column.suffix;
            }
            return columnValue;
        }

        //获取搜索参数
        var _getSearchParams = function () {
            var seachParams = {};
            if (searchObj != null) {
                searchObj.find("input[type='text'],input[type='hidden'],select").each(function () {
                    var s = $(this);
                    var searchName = s.prop("name");
                    var searchValue = s.val();

                    if (!$.isEmptyObject(searchName) && !$.isEmptyObject(searchValue)) {
                        seachParams[searchName] = searchValue;
                    }
                });
            }
            return seachParams;
        }

        //获取提交参数
        var _getParams = function () {
            var params = { PageIndex: options.pageIndex, PageSize: options.pageSize };
            $.extend(true, params, options.params);
            $.extend(true, params, _getSearchParams());
            return params;
        }


        //创建表格内容
        var _createContent = function (data) {
            tableObj.find("tr:gt(0)").remove();
            if (data != null && data.length > 0 && $.isArray(data)) {
                $.each(data, function (index, item) {
                    var tr = $("<tr></tr>");
                    var isHaseHide = false;
                    var hideTr = $("<tr data-show='0' style='display:none;'><td></td><td></td></tr>");
                    var hideTrTable = $("<table class=\"am-table am-table-compact am-table-hover table-main\"></table>");
                    var hideTrThead = $("<thead><tr></tr></thead>");
                    hideTrThead.find("tr").append("<th></th><th></th>");
                    var hideTrTbody = $("<tbody><tr></tr></tbody>");
                    hideTrTbody.find("tr").append("<td></td><td></td>");
                    for (var i = 0; i < columns.length; i++) {
                        var td = $("<td ></td>");

                        var column = columns[i];
                        //获取实值
                        var columnValue = _evalColumnValue(column.dataname, item);
                        if (column.type == "checkbox" || column.type == "radio") {
                            td.attr("fieldname", column.dataname);
                            var inutType = $("<input />", {
                                type: column.type,
                                name: 'grid_' + column.dataname,
                                value: columnValue
                            }).data("dataObj", item);
                            td.append((index + 1) + ".");
                            td.append(inutType);
                        } else if (column.type == "eventlist") {
                            //创建操作项
                            td.append(_convertEvent(item));
                        } else if (column.type == "more") {

                            var more = $("<span class=\"am-icon-plus-square\">展开</span>");
                            more.bind("click", function () {
                                var obj = $(this).parents("tr").next();
                                var isOpen = $(obj).attr("data-show");
                                if (isOpen == "0") {
                                    $(obj).show();
                                    $(this).parents("tr").find("td:eq(1) span").removeClass("am-icon-plus-square").addClass("am-icon-minus").text("收起");
                                    $(obj).attr("data-show", 1);
                                }
                                else {
                                    $(obj).hide();
                                    $(this).parents("tr").find("td:eq(1) span").removeClass("am-icon-minus").addClass("am-icon-plus-square").text("展开");
                                    $(obj).attr("data-show", 0);
                                }
                            });
                            td.html(more);
                        } else {
                            if (column.render != null) {
                                td.append($.Nuoya.callFunction(eval(column.render), item));
                            } else {
                                td.html(_convertValue(column, columnValue));
                            }
                        }
                        if (options.isHide && column.isHide == 1) {
                            hideTrThead.find("tr").append("<th>" + column.trtext + "</th>");
                            hideTrTbody.find("tr").append(td);
                            isHaseHide = true;
                        }
                        else {
                            tr.append(td);
                        }
                        $.Nuoya.callFunction(options.rowCallback, item, tr);
                    }
                    tableObj.append(tr);
                    if (isHaseHide) {
                        hideTrTable.append(hideTrThead);
                        hideTrTable.append(hideTrTbody);
                        var hidTd = $("<td colspan=" + colspanTab + "></td>");
                        hidTd.append(hideTrTable);
                        hideTr.append(hidTd);
                        tableObj.append(hideTr);
                    }
                });
            }
            else {
                //tableObj.append("<tr><td class='grid_empty' colspan='" + columns.length + "'>暂无数据</td></tr>");
            }
        }

        //创建分页信息
        var _createPaginate = function (data) {
            var paginate = $("#paginate_" + options.tableId);
            if (paginate.length == 0) {
                paginate = $("<ul class='am-pagination paginate' id='paginate_" + options.tableId + "'></ul>");
                tableObj.parent().append(paginate);
            } else {
                paginate.html("");
            }

            var home_Page = $('<li><a href="#">首页</a></li>');//首页
            var previous_Page = $('<li><a href="#">上一页</a></li>');//上一页            
            var next_Page = $('<li><a href="#">下一页</a></li>');//下一页
            var end_Page = $('<li><a href="#">末页</a></li>');//末页
            var pageSizeMenu = $("<select data-am-selected='{dropUp: 1,btnWidth: 120px,btnSize: sm}'></select>");//每页显示条数

            for (var i = 0; i < lengthMenu.length; i++) {
                var option = $('<option value="' + lengthMenu[i] + '">' + lengthMenu[i] + '</option>');
                if (lengthMenu[i] == options.pageSize) {
                    option.prop("selected", true);
                }
                pageSizeMenu.append(option);
            }

            if (options.pageIndex > 1) {
                home_Page.bind("click", function () {
                    _reload({ pageIndex: 1 });
                });
                previous_Page.bind("click", function () {
                    _reload({ pageIndex: --options.pageIndex });
                });
            }

            if (!data.IsLastPage) {
                next_Page.bind("click", function () {
                    _reload({ pageIndex: ++options.pageIndex });
                });
                end_Page.bind("click", function () {
                    _reload({ pageIndex: data.PageCount });
                });
            }

            pageSizeMenu.bind("change", function () {
                _reload({ pageIndex: 1, pageSize: $(this).val() });
            });

            var start = options.pageIndex <= 2 ? 1 : options.pageIndex - 2;
            var pageNumber = '';//页码
            for (var i = start; i < start + 5; i++) {
                if (i <= data.PageCount) {
                    var num;
                    if (i == options.pageIndex) {
                        num = '<li class="am-active" num="' + i + '"><a href="#">' + i + '</a></li>';
                    }
                    else {
                        num = '<li num="' + i + '"><a href="#">' + i + '</a></li>';
                    }
                    pageNumber += num;
                }
            }
            pageNumber = $(pageNumber);
            $(pageNumber).filter("li[num]").each(function () {
                $(this).click(function (e) {
                    options.pageIndex = $(this).attr("num");
                    _dataload();
                });
            });

            var pageSizeMenuLi = $("<li id='selectCount' style='margin-left:10px;display:inline-block;position:relative;top:-3px;'></li>").append(pageSizeMenu);
            //lzq->以下是页面标准格式,需要的自行修改 2015-11-25 19:05
            paginate.append(home_Page).append(previous_Page).append(pageNumber).append(next_Page).append(end_Page).append($("<li>共" + data.PageCount + "页,本页" + data.List.length+ "条记录，每页显示</li>")).append(pageSizeMenuLi);
            $(pageSizeMenu).selected();
            if (options.isForPhone)
            {
                $(".am-selected.am-dropdown ").hide();
                
            }
        }

        //获取权限
        var getOperate = function () {

        }
        var _dataload = function () {

            $.Nuoya.action(options.ajaxUrl, _getParams(), function (json) {
                json.List = json.List == null ? [] : json.List;
                $.Nuoya.callFunction(options.callback, json);
                userOperateUrlArray = json.OperateList;
                operateUrlArray = json.OperateList;
                _createContent(json.List);//创建内容
                _createPaginate(json);//创建页码
                $.Nuoya.callFunction(options.drawCallback, json);
                //$.Nuoya.setOperateHideByClass();
                tableObj.find("thead input:checkbox").prop("checked", false);
            });
        }


        //获取选中节点对象
        var _getCheckNote = function (fieldName) {
            debugger
            fieldName = $.isEmptyObject(fieldName) ? "ID" : fieldName;
            return tableObj.find("tr td[fieldname='" + fieldName + "'] input:checked");
        }

        var _getCheckIds = function (fieldName) {
            var resultValue = [];
            _getCheckNote(fieldName).each(function () {
                if (!$.isEmptyObject($(this).val())) {
                    resultValue.push($(this).val());
                }
            });
            return resultValue;
        }

        var _getCheckItems = function (fieldName) {
            var resultValue = [];
            _getCheckNote(fieldName).each(function () {
                resultValue.push($(this).data("dataObj"));
            });
            return resultValue;
        }

        //搜索事件绑定
        var _searchEventBind = function () {
            if (searchObj != null) {
                //给普通文本框绑定回车事件
                searchObj.find("input[type='text']").bind("keyup", function (e) {
                    if (e.keyCode == 13) {
                        _reload({ pageIndex: 1 });
                    }
                });
                //修改只有一个文本框的时候刷新页面的BUG
                searchObj.find("input[type='text']").unbind("keypress").bind("keypress", function (e) {
                    if (e.keyCode == 13) {
                        return false;
                    }
                });
                //给查询按钮绑定搜索时间
                if (searchSubObj != null) {
                    searchSubObj.bind("click", function () {
                        _reload({ pageIndex: 1 });
                    });
                }
            }
        }

        //第一次加载遍历头部信息
        tableObj.find("tr:first th,td").each(function () {
            var td = $(this);
            var column = {
                dataname: td.attr("dataname"),
                trtext: td.text(),
                isHide: td.attr("isHide"),
                strLength: td.attr("strlength"), //字符长度                
                type: td.attr("type"), //操作类型[checkbox,radio,eventlist]
                dataType: td.attr("datatype"), //jsondate,默认string
                dateformat: td.attr("dateformat"), //时间格式 当DataType=jsondate 有效            
                render: td.attr("render"), //渲染
                suffix: td.attr("suffix"), //后缀    
            };
            if (column.isHide == "1") {
                colspanTab++;
            }
            columns.push(column);
            _convertHead(column, td);//头部进行处理
        });


        var _reload = function (newsetting) {
            if (newsetting != null) {
                $.extend(true, options, newsetting);
            }
            _dataload();
        }

        var _del = function (delOptions) {
            var ids = _getCheckIds();
            if (ids.length == 0) {
                $.Nuoya.alert("请选择您要删除的记录");
            } else {
                $.Nuoya.deleteAction(delOptions.ajaxUrl, { ids: ids.join(",") }, delOptions.callback);
            }
        }

        var _batchAction = function (options) {
            var ids = _getCheckIds();
            if (ids.length == 0) {
                $.Nuoya.alert("请选择您要操作的记录");
            } else {
                if (options.message != null) {
                    $.Nuoya.alertAction(options.message, options.ajaxUrl, $.extend({ ids: ids.join(",") }, options.params), options.callback);
                }
                else {
                    $.Nuoya.action(options.ajaxUrl, $.extend({ ids: ids.join(",") }, options.params), options.callback);
                }
            }
        }

        var _init = function () {
            _searchEventBind();
            _dataload();
            return {
                reload: _reload,
                getCheckIds: _getCheckIds,
                getCheckItems: _getCheckItems,
                getParams: _getParams,
                del: _del,
                batchAction: _batchAction,
                pageIndex: options.pageIndex,
                pageSize: options.pageSize
            };
        }
        return _init();
    }
})(jQuery);
