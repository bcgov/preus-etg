var MathFunction = {
    truncate: function (number) {
        return this.sign(number) * Math.floor(Math.abs(number)) / 100;
    },

    round:function(number) {
        return this.sign(number) * Math.round(Math.abs(number));
    },

    sign: function(x) {
        return typeof x === 'number' ? (x ? (x < 0 ? -1 : 1) : (x === x ? 0 : NaN)) : NaN;
    }
}
module.exports = MathFunction;