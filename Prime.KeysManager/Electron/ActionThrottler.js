var ActionThrottler = function() {
    let self = this;
    let timer = null;

    this.throttle = function(period, action) {
        clearTimeout(self.timer);
        self.timer = setTimeout(function() {
            action();
        }, period);
    }
}

module.exports = ActionThrottler;
