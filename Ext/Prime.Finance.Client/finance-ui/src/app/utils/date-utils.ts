
export class DateUtils {
    public static unixToDate(msTimestamp): Date {
        return new Date(msTimestamp);
    }

    public static timeOfDate(date: Date): string {
        var hours = date.getHours();
        var minutes = "0" + date.getMinutes();
        var seconds = "0" + date.getSeconds();

        return hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);
    }
}
