import { Point } from "../chart-dragger";
import { Subject, Observable } from "rxjs";
import * as d3 from "d3";
import { OhlcChartItem } from "./ohlc-chart-item";
import { OhlcDataRecord } from "../ohlc/ohlc-data-record";
import { RenderingCtx } from "./rendering-ctx";
import { ChartDimensions } from "./chart-dimensions";

export class SvgCore {
    private _svg;
    public dimensions: ChartDimensions;

    private _gMain;
    private _gAxisPrice;
    private _gAxisTime;

    private _pointer = { lineHorizontal: null, lineVertical: null };
    private _pointerPrice = {
        g: null,
        bg: null,
        ticker: null
    };
    private _pointerDate = {
        g: null,
        bg: null,
        ticker: null  
    };

    private _dateTimeFormat: any = d3.timeFormat("%d-%b-%y %H:%M:%S");

    private _chartItemsInView: OhlcChartItem[];

    public chartOffsetX: number = 0;
    public chartOffsetXInitialDiff: number = null;

    private _onSvgMouseMove: Subject<Point> = new Subject<Point>();
    public onSvgMouseMove: Observable<Point> = this._onSvgMouseMove.asObservable();

    private _onSvgMouseWheel: Subject<number> = new Subject<number>();
    public onSvgMouseWheel: Observable<number> = this._onSvgMouseWheel.asObservable();

    private _onSvgMouseDown: Subject<object> = new Subject<object>();
    public onSvgMouseDown: Observable<object> = this._onSvgMouseDown.asObservable();

    private _onSvgMouseUp: Subject<object> = new Subject<object>();
    public onSvgMouseUp: Observable<object> = this._onSvgMouseUp.asObservable();

    public initialize(svg, sizing) {
        this.dimensions = sizing;
        this._svg = svg.attr("height", this.dimensions.height);

        this.updateSvgWidth();
    }

    public registerSvgHandlers() {
        this._svg.on("mousemove", () => {
            let [x, y] = d3.mouse(d3.event.currentTarget);
            let p: Point = new Point(x, y);

            this._onSvgMouseMove.next(p); // May update text of crosshair ticker.
            this.svgMouseMoveHandler(p);
        });

        this._svg.on("mouseleave", () => {
            this.svgMouseLeaveHandler();
        });

        this._svg.on("mouseenter", () => {
            this.svgMouseEnterHandler();
        });

        this._svg.on("mousewheel", (e) => {
            let scrollValue = d3.event.wheelDeltaY;

            this._onSvgMouseWheel.next(scrollValue);
        });

        this._svg.on("mousedown", () => {
            this.pauseEvent();
            this._onSvgMouseDown.next(null);
        });

        this._svg.on("mouseup", () => {
            this._onSvgMouseUp.next(null);
        });
    }

    public createControls() {
        // Selection line.
        this._pointer.lineVertical = this._svg.append("line")
            .attr("x1", 0)
            .attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", this.dimensions.height)
            .attr("stroke-width", 1)
            .attr("stroke-dasharray", "5,5")
            .attr("stroke", "rgba(255, 255, 255, 0.5)")
            .attr("transform", "translate(-100, -100)");
        this._pointer.lineHorizontal = this._svg.append("line")
            .attr("x1", 0)
            .attr("x2", this.dimensions.width)
            .attr("y1", 0)
            .attr("y2", 0)
            .attr("stroke-width", 1)
            .attr("stroke-dasharray", "5,5")
            .attr("stroke", "rgba(255, 255, 255, 0.5)")
            .attr("transform", "translate(-100, -100)");

        // Rect to see SVG's boundaries.
        //this._svg.append("rect").attr("width", "100%").attr("height", "100%").attr("fill", "rgb(30, 30, 30)");

        // Price crosshair ticker.
        this._pointerPrice.g = this._svg
            .append("g")
            .attr("transform", "translate(-1000, 0)");
        this._pointerPrice.ticker = this._pointerPrice.g
            .append("text")
            .attr("y", 14)
            .attr("x", 2)
            .attr("color", "red")
            .text("");
        this._pointerPrice.bg = this._pointerPrice.g
            .append("rect")
            .attr("width", this._pointerPrice.ticker.node().getBBox().width + 5)
            .attr("height", 18)
            .attr("fill", "#385571");
        this._pointerPrice.ticker.raise();

        // Date crosshair ticker.
        this._pointerDate.g = this._svg
            .append("g")
            .attr("transform", "translate(-1000, 0)");
        this._pointerDate.ticker = this._pointerDate.g
            .append("text")
            .attr("y", 14)
            .attr("x", 2)
            .attr("color", "red")
            .text(6500);
        this._pointerDate.bg = this._pointerDate.g
            .append("rect")
            .attr("width", this._pointerDate.ticker.node().getBBox().width + 5)
            .attr("height", 18)
            .attr("fill", "#385571");
        this._pointerDate.ticker.raise();

        // Drawing area group.
        this._gMain = this._svg.append("g").attr("transform", `translate(${this.dimensions.margin.left}, ${this.dimensions.margin.top})`);
        this._gAxisPrice = this._svg.append("g");
        this._gAxisTime = this._svg.append("g");
    }

    public render(data: OhlcChartItem[], ctx: RenderingCtx) {
        this._chartItemsInView = data;

        // Clear everything.
        this._gMain.selectAll("*").remove();
        this._gAxisPrice.selectAll("*").remove();
        this._gAxisTime.selectAll("*").remove();

        // Move crosshair.
        this._pointer.lineVertical.attr("y2", this.dimensions.height);
        this._pointer.lineHorizontal.attr("x2", this.dimensions.width);

        // ---------- Draw OHLC --------- //

        let yScaleRaw = ctx.yScaleRaw;
        let yScale = ctx.yScale;
        let xScaleRaw = ctx.xScaleRaw;

        // Populate data.
        let svgData = this._gMain
            .selectAll("g")
            .data(data);

        // Add groups.
        let groups = svgData.enter().append("g")
            .attr("transform", (item: OhlcChartItem, i) => {
                return `translate(${(item.posX + this.chartOffsetX)}, ${yScale(item.ohlc.low)})`;
            })
            .attr("id", (item: OhlcChartItem, i) => {
                return "item-" + item.ohlc.time;
            });

        groups.append("line");
        groups.append("rect");

        // Process lines.
        groups.selectAll("line")
            .attr("transform", (x: OhlcChartItem, i) => {
                return `translate(${(this.dimensions.bars.width / 2)}, ${0})`;
            })
            .attr("x1", 0).attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", (x: OhlcChartItem, i) => {
                return (yScale(x.ohlc.high) - yScale(x.ohlc.low));
            })
            .attr("stroke-width", 1)
            .attr("stroke", (o: OhlcChartItem) => {
                return getColor(o.ohlc);
            });


        // Process rects.
        groups.selectAll("rect")
            .attr("width", this.dimensions.bars.width)
            .attr("height", (o: OhlcChartItem) => {
                return Math.abs(yScale(o.ohlc.open) - yScale(o.ohlc.close));
            })
            .attr("fill", (o: OhlcChartItem) => {
                return getColor(o.ohlc);
            })
            .attr("x", 0)
            .attr("y", (x: OhlcChartItem, i) => {
                let openLow = yScale(x.ohlc.open) - yScale(x.ohlc.low);
                let closeLow = yScale(x.ohlc.close) - yScale(x.ohlc.low)
                return openLow < closeLow ? openLow : closeLow;
            });

        groups.exit().remove();

        // --------- Axes ----------- //
        
        let rightAxis = d3.axisRight(yScaleRaw).ticks(10);
        this._gAxisPrice.call(rightAxis).attr("transform", `translate(${this.dimensions.width - this.dimensions.margin.right}, ${this.dimensions.margin.top})`);

        let xAxis = d3.axisTop(xScaleRaw).ticks(5).tickFormat(this._dateTimeFormat);
        this._gAxisTime.attr('transform', 'translate(0, ' + (this.dimensions.height - this.dimensions.margin.bottom + 14) + ')').call(xAxis);

        // Put price ticker on top of the right axis.
        this._pointer.lineHorizontal.raise();
        this._pointer.lineVertical.raise();
        this._pointerPrice.g.raise();
        this._pointerDate.g.raise();

        function getColor(o: OhlcDataRecord): string {
            return o.open - o.close >= 0 ? "#d81571" : "#4caf0e";
        }
    }

    private pauseEvent() {
        let e = d3.event || window.event;
        if (e.stopPropagation) e.stopPropagation();
        if (e.preventDefault) e.preventDefault();
        e.cancelBubble = true;
        e.returnValue = false;
        return false;
    }

    private svgMouseMoveHandler(p: Point) {
        this._pointer.lineVertical.attr("transform", `translate(${p.x}, 0)`);
        this._pointer.lineHorizontal.attr("transform", `translate(0, ${p.y})`);

        // Price ticker.
        let bPrice = this._pointerPrice.ticker.node().getBBox();
        this._pointerPrice.g.attr("transform", `translate(${this.dimensions.width - 55}, ${p.y - bPrice.height / 2 - 2})`);
        this._pointerPrice.bg.attr("width", 55); // this._pointerPrice.ticker.node().getBBox().width + 5

        // Date ticker.
        let bDate = this._pointerDate.ticker.node().getBBox();
        this._pointerDate.g.attr("transform", `translate(${p.x - bDate.width / 2}, ${this.dimensions.height - bDate.height - 5})`);
        this._pointerDate.bg.attr("width", this._pointerDate.ticker.node().getBBox().width + 5);
    }

    public setCrosshairTickerPrice(price: number) {
        this._pointerPrice.ticker.text(price.toFixed(2));
    }

    public setCrosshairTickerDate(date: Date) {
        this._pointerDate.ticker.text(this._dateTimeFormat(date));
    }

    private svgMouseEnterHandler(): any {
        this._pointer.lineHorizontal.style("opacity", 1);
        this._pointer.lineVertical.style("opacity", 1);
        this._pointerPrice.g.style("opacity", 1);
        this._pointerDate.g.style("opacity", 1);
    }

    private svgMouseLeaveHandler(): any {
        this._pointer.lineHorizontal.style("opacity", 0);
        this._pointer.lineVertical.style("opacity", 0);
        this._pointerPrice.g.style("opacity", 0);
        this._pointerDate.g.style("opacity", 0);
    }

    public updateSvgWidth() {
        this.dimensions.width = this._svg.node().clientWidth;
    }

    public svgDragMovingHandler(p: Point) {
        if (this.chartOffsetXInitialDiff === null) {
            this.chartOffsetXInitialDiff = this.chartOffsetX + p.x;
            //console.log(`Offset: ${}`)
        }
        this.chartOffsetX = p.x + this.chartOffsetXInitialDiff;
    }

    public svgDragStartedHandler(dp: Point) {

    }

    public svgDragEndedHandler(dp: Point) {
        this.resetChartOffsetXInitialDiff();
    }

    public resetChartOffsetXInitialDiff() {
        this.chartOffsetXInitialDiff = null;
    }
}
