global.app.filter('postal', function () {
    return function (postal) {
        if (postal == null) return '';
        var list = postal;
        var chars = [];
        var lastChar = null;
        for (var i = 0; i < list.length && chars.length < 6; i++) {
            if (list[i].match(/^[A-Za-z]/) && (lastChar == null || lastChar == 'number')) {
                chars.push(list[i]);
                lastChar = 'character';
            }
            else if (list[i].match(/^\d/) && lastChar == 'character') {
                chars.push(list[i]);
                lastChar = 'number';
            }
        }
        return chars.join('').toUpperCase();
    };
});
