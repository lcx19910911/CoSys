Date.prototype.format = function (format) {
    format = format == null ? "yyyy-MM-dd hh:mm:ss" : format
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
//����IE7 name��id ���Ի���������
if (/msie/i.test(navigator.userAgent)) //����userAgentȷ���û�ʹ��IE�����
{
    document.nativeGetElementById = document.getElementById;
    document.getElementById = function (id) {
        var elem = document.nativeGetElementById(id);
        if (elem) {
            //�޸ĺ��ȷ���ܵõ�id���Է���  
            if (elem.attributes['id'].value == id) {
                return elem;
            }
            else {
                //���û��ID��ͬ��,��ô�ͱ�������Ԫ�صļ����ҵ�id��ͬ��Ԫ��  
                for (var i = 1; i < document.all[id].length; i++) {
                    if (document.all[id][i].attributes['id'].value == id) {
                        return document.all[id][i];
                    }
                }
            }
        }
        return null;
    };
}