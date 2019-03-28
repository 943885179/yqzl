function getFromDepatement(OAdepatementeName,faqiren) {
        var caigouyibu = [//采购二部（基础档案维护之前只能这么判断）
        '刁西悦',
        '欧阳健联',
        '周晓琴',
        '林思敏',
        '王世东'];
    var caigouerbu = [
            '胡琼茹',
            '陈创彬',
            '黄佩雪',
            '陈丹宏',
            '李小倩',
            '刘楚倩',
            '窦梦飞',
            '张晓敏',
            '李婷婷',
            '何凤淇'];
       var caiwuguanlizhongxin = [
'梁献丹',
'林丽诗',
'高如珠',
'王少媚',
'刘婧',
'伍思蔚',
'吴建祥',
'周均源',
'张芬',
'尹淑贤',
'钟巧芳',
'卢晓娜',
'刘蓓',
'吴金玲',
'胡姗',
'陈晓晨',
'周莹',
'汤桂森',
'陈妹二'
    ];
    if ($.inArray(faqiren, caigouerbu)>=0) {//此人在采购二部
            return "采购二部";
    }
    if ($.inArray(faqiren, caigouyibu) >= 0) {//此人在采购一部
            return "采购一部";

    }
    if ($.inArray(faqiren, caiwuguanlizhongxin) >= 0) {//此人在财务管理中心
            return "财务管理中心";
    }
    return OAdepatementeName;
 }
