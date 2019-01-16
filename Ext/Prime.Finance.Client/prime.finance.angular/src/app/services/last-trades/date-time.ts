import { DateUtils } from "src/app/utils/date-utils";

export class DateTime {
    constructor(public date: Date) {

    }

    public static fromUnixTimestamp(msTimestamp: number) {
        return new DateTime(DateUtils.unixToDate(msTimestamp));
    }

    get timeString(): string {
        return DateUtils.timeOfDate(this.date);
    }
}
