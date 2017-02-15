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
//万恶的IE7 name与id 属性混淆的问题
if (/msie/i.test(navigator.userAgent)) //根据userAgent确定用户使用IE浏览器
{
    document.nativeGetElementById = document.getElementById;
    document.getElementById = function (id) {
        var elem = document.nativeGetElementById(id);
        if (elem) {
            //修改后的确认能得到id属性方法  
            if (elem.attributes['id'].value == id) {
                return elem;
            }
            else {
                //如果没有ID相同的,那么就遍历所有元素的集合找到id相同的元素  
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