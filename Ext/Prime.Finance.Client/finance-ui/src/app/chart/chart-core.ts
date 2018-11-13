import { OhlcRecord } from "./ohlc/ohlc-record";
import * as d3 from "d3";
import { Observable, Subject } from "rxjs";
import { ChartDragger, Point } from "./chart-dragger";

class OhlcItem {
    ohlc: OhlcRecord;
    posX: number;
}

export class ChartCore {
    private _chartData: OhlcItem[];
    private _chartDataInView: OhlcItem[];

    private _chartDragger: ChartDragger = new ChartDragger();

    private svg;
    private gMain;
    private gLeftAxis;
    private selectionCrosshair = { horizontal: null, vertical: null };

    private _chartOffsetX: number = 0;

    private _onOhlcItemSelected: Subject<OhlcRecord> = new Subject();
    public onOhlcItemSelected: Observable<OhlcRecord> = this._onOhlcItemSelected.asObservable();

    private selectedOhlcRecord: OhlcRecord;

    constructor(selector: string) {
        this.initialize(selector);

        this._chartDragger.dragMoved.subscribe((dp) => {
            this.onSvgDragMoving(dp);
        });
        this._chartDragger.dragStarted.subscribe((p) => {
            this.onSvgDragStarted(p);
        });
        this._chartDragger.dragEnded.subscribe((dp) => {
            this.onSvgDragEnded(dp);
        });
    }

    private onSvgMouseMove([x, y]) {
        this.selectionCrosshair.vertical.attr("transform", `translate(${x}, 0)`);
        this.selectionCrosshair.horizontal.attr("transform", `translate(0, ${y})`);

        if (this._chartDataInView && this._chartDataInView.length > 0) {
            let chartOffset = -this._chartOffsetX + x;

            let dists = this._chartDataInView.map((x) => {
                return { dist: Math.abs(chartOffset - (x.posX + this._sizing.bars.width / 2)), item: x };
            });

            let min = dists[0].dist;
            let prevSelected = this.selectedOhlcRecord ? Object.assign({}, this.selectedOhlcRecord) : null;
            this.selectedOhlcRecord = dists[0].item.ohlc;
            dists.forEach(v => {
                if (v.dist < min) {
                    min = v.dist;
                    this.selectedOhlcRecord = v.item.ohlc;
                }
            });

            if (!prevSelected || prevSelected.time !== this.selectedOhlcRecord.time)
                this._onOhlcItemSelected.next(this.selectedOhlcRecord);
        }
    }

    private chartOffsetXInitialDiff: number = null;
    onSvgDragMoving(dp: Point): any {
        if (this.chartOffsetXInitialDiff === null) {
            this.chartOffsetXInitialDiff = this._chartOffsetX + dp.x;
            //console.log(`Offset: ${}`)
        }
        this.chartOffsetX = dp.x + this.chartOffsetXInitialDiff;
    }

    private onSvgMouseEnter(): any {
        this.selectionCrosshair.horizontal.style("opacity", 1);
        this.selectionCrosshair.vertical.style("opacity", 1);
    }
    private onSvgMouseLeave(): any {
        this.selectionCrosshair.horizontal.style("opacity", 0);
        this.selectionCrosshair.vertical.style("opacity", 0);
    }

    private onSvgDragStarted(p: Point): any {
        
    }

    private onSvgDragEnded(dp: Point): any {
        this.chartOffsetXInitialDiff = null;
    }

    private _sizing = {
        margin: {
            top: 20, right: 50, bottom: 10, left: 0
        },
        chartOffset: {
            max: 100,
            min: 0 // Initialized during runtime.
        },
        bars: {
            width: 10,
            gap: 3,
            maxWidth: 50,
            minWidth: 3
        },
        height: 450,
        width: 0
    };

    private setSvgWidth() {
        this._sizing.width = this.svg.node().clientWidth;
    }

    public updateSvgWidth() {
        this.setSvgWidth();
        this.render();
    }

    public moveLeft() {
        this.chartOffsetX += this._sizing.bars.width + this._sizing.bars.gap;
    }
    public moveRight() {
        this.chartOffsetX -= this._sizing.bars.width + this._sizing.bars.gap;
    }

    public set chartOffsetX(v: number) {
        this._chartOffsetX = v;
        this.render();
    }
    public get chartOffsetX(): number {
        return this._chartOffsetX;
    }

    public set barWidth(v: number) {
        if (v > this._sizing.bars.maxWidth || v < this._sizing.bars.minWidth)
            return;

        let displacement = this.calcZoomOffsetDisplacement(v);

        this._sizing.bars.width = v;
        this.chartOffsetXInitialDiff = null;

        this.chartOffsetX -= displacement;
        this.render();
    }
    public get barWidth(): number {
        return this._sizing.bars.width;
    }

    public initialize(selector: string) {
        this.svg = d3.select(selector);

        this.setSvgWidth();

        // Rect to see SVG's boundaries.
        //this.svg.append("rect").attr("width", "100%").attr("height", "100%").attr("fill", "rgb(30, 30, 30)");

        // Drawing area rect.
        this.svg.on("mousemove", () => {
            let coords = d3.mouse(d3.event.currentTarget);
            this._chartDragger.mouseMove(coords);
            this.onSvgMouseMove(coords);
        });
        this.svg.on("mouseleave", () => {
            this.onSvgMouseLeave();
        });
        this.svg.on("mouseenter", () => {
            this.onSvgMouseEnter();
        });
        this.svg.on("mousewheel", (e) => {
            let scrollValue = d3.event.wheelDeltaY;
            this.barWidth += scrollValue / 100;
        });

        this.svg.on("mousedown", () => {
            this._chartDragger.dragStart();
        }).on("mouseup", () => {
            this._chartDragger.dragEnd();
        });

        // Selection line.
        this.selectionCrosshair.vertical = this.svg.append("line")
            .attr("x1", 0)
            .attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", this._sizing.height)
            .attr("stroke-width", 1)
            .attr("stroke-dasharray", "5,5")
            .attr("stroke", "rgba(255, 255, 255, 0.5)");
        this.selectionCrosshair.horizontal = this.svg.append("line")
            .attr("x1", 0)
            .attr("x2", this._sizing.width)
            .attr("y1", 0)
            .attr("y2", 0)
            .attr("stroke-width", 1)
            .attr("stroke-dasharray", "5,5")
            .attr("stroke", "rgba(255, 255, 255, 0.5)");

        // append("rect")
        //     .attr("width", this._sizing.width - (this._sizing.margin.left + this._sizing.margin.right))
        //     .attr("height", this._sizing.height - (this._sizing.margin.top + this._sizing.margin.bottom))
        //     .attr("transform", `translate(${this._sizing.margin.left}, ${this._sizing.margin.top})`)
        //     .attr("fill", "transparent")

        // Drawing area group.
        this.gMain = this.svg.append("g").attr("transform", `translate(${this._sizing.margin.left}, ${this._sizing.margin.top})`);
        this.gLeftAxis = this.svg.append("g");
    }

    public setData(data: OhlcRecord[]) {
        this._chartData = data.map((x, i) => {
            let r: OhlcItem = { ohlc: x, posX: this.getBarPosX(i) };
            return r;
        });

        this._chartOffsetX = -this._chartData[this._chartData.length - 50].posX;
        this._onOhlcItemSelected.next(this._chartData[this._chartData.length - 1].ohlc);
        //this.viewPort.x1 = this.getXbyIndex(data.length - 1);
    }

    private recalcItemsPosX(data: OhlcItem[]): OhlcItem[] {
        data.forEach((v, i) => {
            v.posX = this.getBarPosX(i);
        });

        let lastItem = data[data.length - 1];
        this._sizing.chartOffset.min = -lastItem.posX + this._sizing.width - 100;

        return data;
    }

    public getBarPosX(i: number): number {
        return i * (this._sizing.bars.width + this._sizing.bars.gap)
    }

    public getInView(data: OhlcItem[]): OhlcItem[] {
        let inView = data.filter((record, i) => {
            let startX = -(this._chartOffsetX);
            let endX = startX + this._sizing.width - (this._sizing.margin.right + this._sizing.bars.width) - this._sizing.margin.left;
            let currX = record.posX;

            let r = currX >= startX && currX <= endX;

            return r;
        });

        return inView;
    }

    // Calculates center of chart view (SVG view) in chart data coordinates (taking into account chart offset X).
    private calcZoomOffsetDisplacement(newWidth: number): number {
        let offsetAndWidth = -this._chartOffsetX + this._sizing.width;
        let itemsInViewOld = offsetAndWidth / (this._sizing.bars.width + this._sizing.bars.gap);
        let itemsInViewNew = offsetAndWidth / (newWidth + this._sizing.bars.gap);
        let rel = itemsInViewOld / itemsInViewNew;
        let partDiff = (rel * offsetAndWidth - offsetAndWidth) * ((offsetAndWidth - (this._sizing.width / 2)) / offsetAndWidth);
        return partDiff;
    }

    public render() {
        let self = this;
        let svg = this.svg;
        let allData = this.recalcItemsPosX(this._chartData);
        let sizing = this._sizing;
        let gMain = this.gMain;
        let gLeftAxis = this.gLeftAxis;

        this._chartDataInView = this.getInView(allData);
        let data = this._chartDataInView;

        let yScaleMin: number = d3.min(data, (d: OhlcItem) => { return d.ohlc.low; });
        let yScaleMax: number = d3.max(data, (d: OhlcItem) => { return d.ohlc.high; });

        // Scales.
        let yScaleRaw = d3.scaleLinear()
            .domain([yScaleMin, yScaleMax]) // // d3.extent(data, (x: OhlcRecord) => { return x.open; })
            .range([sizing.height - sizing.margin.bottom - sizing.margin.top, 0]);
        let yScale = function (v) {
            return yScaleRaw(v).toFixed(4);
        };

        // Clear everything.
        gMain.selectAll("*").remove();
        gLeftAxis.selectAll("*").remove();

        // Populate data.
        let svgData = gMain
            .selectAll("g")
            .data(data);

        // Add groups.
        let groups = svgData.enter().append("g")
            .attr("transform", (item: OhlcItem, i) => {
                return `translate(${(item.posX + self._chartOffsetX)}, ${yScale(item.ohlc.low)})`;
            })
            .attr("id", (item: OhlcItem, i) => {
                return "item-" + item.ohlc.time;
            });

        groups.append("line");
        groups.append("rect");

        // Axis.
        //let xAxis = d3.axisLeft(yScaleRaw);
        // svg.append("g").call(xAxis);

        //let gW = g.node().getBBox().width;

        // Line test.
        // function lineTest() {
        //     let line = d3.line()
        //         .x((d: OhlcRecord, i) => {
        //             return i * 10;
        //         })
        //         .y((da: OhlcRecord, i) => {
        //             return yScaleRaw(da.open);
        //         });
        //     svg.append("path")
        //         .attr("stroke-width", 1)
        //         .attr("stroke", "white")
        //         .attr("fill", "transparent")
        //         .attr("d", line(data));
        // }

        // Process lines.
        groups.selectAll("line")
            .attr("transform", (x: OhlcItem, i) => {
                return `translate(${(sizing.bars.width / 2)}, ${0})`;
            })
            .attr("x1", 0).attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", (x: OhlcItem, i) => {
                return (yScale(x.ohlc.high) - yScale(x.ohlc.low));
            })
            .attr("stroke-width", 1)
            .attr("stroke", (o: OhlcItem) => {
                return getColor(o.ohlc);
            });

        // Process rects.
        groups.selectAll("rect")
            .attr("width", sizing.bars.width)
            .attr("height", (o: OhlcItem) => {
                return Math.abs(yScale(o.ohlc.open) - yScale(o.ohlc.close));
            })
            .attr("fill", (o: OhlcItem) => {
                return getColor(o.ohlc);
            })
            .attr("x", 0)
            .attr("y", (x: OhlcItem, i) => {
                let openLow = yScale(x.ohlc.open) - yScale(x.ohlc.low);
                let closeLow = yScale(x.ohlc.close) - yScale(x.ohlc.low)
                return openLow < closeLow ? openLow : closeLow;
            });

        groups.exit().remove();

        // Zoom.
        // svg.call(d3.zoom()
        //     .scaleExtent([1, 10])
        //     //.translateExtent([[-allData[allData.length - 1].posX, 0], [allData[allData.length - 1].posX + 100, 0]])
        //     .on("zoom", () => {
        //         this.onTransformed();
        //     }));
        let rightAxis = d3.axisRight(yScaleRaw).ticks(10);
        gLeftAxis.attr("transform", `translate(${this._sizing.width - this._sizing.margin.right}, ${this._sizing.margin.top})`).call(rightAxis);

        function getColor(o: OhlcRecord): string {
            return o.open - o.close >= 0 ? "#d81571" : "#4caf0e";
        }
    }
}
