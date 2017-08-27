//ztree 选择框
(function ($) {
    $.extend($.fn, {
        ztreeSelect: function (valueInput, jsonUrl, selfValue, isMulitSelect, isFlag) {
            //选择框所依附的文本框
            console.log(isFlag);
            var dom = this;
            var name = $(dom).attr("name");
            var ztreeContentName = "ztreeContent_" + name;
            var ztreeName = "ztree_" + name;
            var html = '<div class="ztreeContent" name="' + ztreeContentName + '" style="z-index:1000;display:none; position: absolute;border: 1px solid #617775;background: #ffffff;"><ul name="' + ztreeName + '" class="ztree" style="margin-top:0; width:160px;"></ul></div>';
            var ztreeContent = $(html);
            var clearButtonHtml = '<button type="button" class="am-btn am-btn-danger am-btn-xs clearButton" name="btnClear_' + name + '">清空选择</button>';
            var clearButton = $(clearButtonHtml);
            $(ztreeContent).append(clearButton);
            $(dom).after(ztreeContent);
            var ztree = $(ztreeContent).find("[name='" + ztreeName + "']");
            var ztreeObj;
            var setting = {
                view: {
                    dblClickExpand: false
                },
                data: {
                    simpleData: {
                        enable: true
                    },
                    key: {
                        checked: "ischeck"
                    }
                },
                callback: {
                    onClick: onClick,
                    onCheck: onCheck
                },
                check: {
                    enable: isMulitSelect == true ? true : false
                }
            };
            function showMenu() {
                var offset = $(dom).position();
                if ($(ztreeContent).css("display") == "none") {
                    $(ztreeContent).css({ left: offset.left + "px", top: offset.top + dom.outerHeight() + "px", width: ($(dom).width() + 18) + "px" }).slideDown("fast");
                    $("body").bind("mousedown", onBodyDown);
                }
                else {
                    hideMenu();
                }
            }
            function hideMenu() {
                $(ztreeContent).fadeOut("fast");
                $("body").unbind("mousedown", onBodyDown);
            }
            $(dom).click(function () {
                showMenu();
            });
            function onBodyDown(event) {
                if (!($(event.target).attr("name") == ztreeName || $(event.target).attr("name") == ztreeContentName || $(event.target).parents("[name='" + ztreeContentName + "']").length > 0)) {
                    hideMenu();
                }
            }
            function onCheck(e, treeId, treeNode) {
                if (isFlag) {
                    var flag;
                    var nodes = ztreeObj.getCheckedNodes(true);
                    var selectText = "";
                    for (var i = 0; i < nodes.length; i++) {
                        if (nodes[i].value) {
                            if (flag == null) {
                                flag = parseInt(nodes[i].value);
                            }
                            else {
                                flag |= parseInt(nodes[i].value);
                            }
                        }
                        selectText += nodes[i].name
                        if (i != nodes.length - 1) {
                            selectText += ",";
                        }
                    }
                    $(dom).val("选中了 " + selectText);
                    $(valueInput).val(flag);
                }
                else {
                    if (isMulitSelect) {
                        var nodes = ztreeObj.getCheckedNodes(true);
                        $(dom).val("选中了" + nodes.length + "项");
                        var values = [];
                        for (var i = 0; i < nodes.length; i++) {
                            if (nodes[i].value) {
                                values.push(nodes[i].value);
                            }
                        }
                        $(valueInput).val(values.join(","));
                    }
                }
            }
            function onClick(e, treeId, treeNode) {
                nodes = ztreeObj.getSelectedNodes();
                if (!isFlag && !isMulitSelect) {
                    if (nodes && nodes.length > 0) {
                        if (selfValue != null && nodes[0].value == selfValue) {
                            $.Nuoya.alert("不能选取自身作为父级节点");
                        }
                        else {
                            $(dom).val(nodes[0].name);
                            $(valueInput).val(nodes[0].value);
                        }
                    }
                }
            }

            $(clearButton).click(function () {
                ztreeObj.checkAllNodes(false);
                $(dom).val("");
                $(valueInput).val("");
                var nodes = ztreeObj.getSelectedNodes();
                if (nodes) {
                    for (var i = 0; i < nodes.length; i++) {
                        ztreeObj.cancelSelectedNode(nodes[i]);
                    }
                }
            });
            $(document).ready(function () {
                $.Nuoya.action(jsonUrl, {}, function (json) {
                    var zNodes = json;
                    ztreeObj = $.fn.zTree.init($(ztree), setting, zNodes);


                    var valueNodes;
                    if (isFlag) {
                        var flag = parseInt(selfValue);
                        valueNodes = ztreeObj.getNodesByFilter(function (node) {
                            if ((parseInt(node.value) & flag) != 0)
                                return true;
                            else
                                return false;
                        });
                    }
                    else {
                        if ($(valueInput).val()) {
                            var values = $(valueInput).val().split(",");
                            valueNodes = ztreeObj.getNodesByFilter(function (node) {
                                for (var i = 0; i < values.length; i++) {
                                    if (node.value == values[i])
                                        return true;
                                }
                                return false;
                            });
                        }
                    }
                    if (isMulitSelect) {
                        if (valueNodes != null) {
                            $(dom).val("选中了" + valueNodes.length + "项");
                            for (var i = 0; i < valueNodes.length; i++) {
                                ztreeObj.checkNode(valueNodes[i]);
                            }
                        }
                    }
                    else {
                        if (valueNodes && valueNodes.length > 0) {
                            $(dom).val(valueNodes[0].name);
                            ztreeObj.selectNode(valueNodes[0]);
                        }
                    }
                });
            });
        }
    });
})(jQuery);