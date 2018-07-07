
export class ActionThrottler {
    private timer = null;

    throttle(period: number, action: () => void) {
        clearTimeout(this.timer);
        this.timer = setTimeout(function() {
            action();
        }, period);
    }
}
