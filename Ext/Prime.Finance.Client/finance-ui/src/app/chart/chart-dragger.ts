import { Observable, Subject } from "rxjs";

export class Point {
    constructor(public x, public y) { }

    public static fromPoint(p: Point) {
        return new Point(p.x, p.y);
    }
}

export class ChartDragger {
    private _isDragging: boolean = false;
    private _mousePos: Point = new Point(0, 0);
    private _dragStartPos: Point;

    private _dragStarted: Subject<Point> = new Subject<Point>();
    public dragStarted: Observable<Point> = this._dragStarted.asObservable();

    private _dragEnded: Subject<Point> = new Subject<Point>();
    public dragEnded: Observable<Point> = this._dragEnded.asObservable();

    private _dragMoved: Subject<Point> = new Subject<Point>();
    public dragMoved: Observable<Point> = this._dragMoved.asObservable();

    public dragStart() {
        if (this._isDragging === false) {
            this._isDragging = true;
            console.log(this._mousePos);
            this._dragStartPos = Point.fromPoint(this._mousePos);
            this._dragStarted.next(this._mousePos);
        }
    }

    public dragEnd() {
        if (this._isDragging == true) {
            this._isDragging = false;

            let endPoint = Point.fromPoint(this._mousePos);
            let pDispl = this.calcDragDisplacement(this._dragStartPos, endPoint);

            this._dragEnded.next(pDispl);

            this._dragStartPos = null;
        }
    }

    public mouseMove([x, y]) {
        this._mousePos.x = x;
        this._mousePos.y = y;

        if (this._isDragging) {
            let dp = this.calcDragDisplacement(this._dragStartPos, this._mousePos);
            this._dragMoved.next(dp);
        }
    }

    private calcDragDisplacement(startPoint: Point, endPoint: Point): Point {
        return new Point(endPoint.x - startPoint.x, endPoint.y - startPoint.y);
    }
}
